using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using fp.lib.dbInfo;

namespace fp.lib.sqlparser
{
    [DebuggerDisplay("count={tokens.Count} leftExtent={leftExtent} leftExtent={rightExtent}")]
    public class TokenList
    {
        public bool useIds = false;
        public HashSet<Token> tokens = new HashSet<Token>();
        protected SortedDictionary<int, Token> tokensByOffset = new SortedDictionary<int, Token>();
        protected SortedDictionary<string, Token> tokensById = new SortedDictionary<string, Token>();
        private int _leftExtent = -1;
        private int _rightExtent = -1;
        public int leftExtent  { get { if (_leftExtent == -1) UpdateExtents(); return _leftExtent; } }    // the first index of range
        public int rightExtent { get { if (_leftExtent == -1) UpdateExtents(); return _rightExtent; } }   // 1 + (last valid index in the range)

        public TokenList()
        {

        }

        public TokenList(Token t)
        {
            Add(t);
        }

        public Token GetTokenById(string id)
        {
            return tokensById.At(id);
        }

        public int Count { get { return tokens.Count; } }

        #region compare TokenLists
        public int Different(TokenList list, bool onlySignificantOnes = false, int max = 1)
        {
            TokenList result;
            return Difference(list, out result, onlySignificantOnes, max);
        }

        bool Significant(Token t)
        {
            if (t.tokenType == TokenType.Keyword)
                return true;
            if (t.tokenType == TokenType.Operator)
                return true;
            if (t.tokenType == TokenType.Table)
                return true;
            if(t.tokenType == TokenType.Identifier && Query.Db.tablesByColumnName.ContainsKey(t.name))
                return true;
            return false;
        }

        public int Difference(TokenList list, out TokenList result, bool onlySignificantOnes = false, int max = 0)
        {
            result = new TokenList();

            bool compareStartOffsets = false;

            int added = 0;
            int deleted = 0;
            int moved = 0;

            foreach (Token tNew in list.tokens)
            {
                Token tOld = GetTokenById(tNew.id);
                if (tOld == null)
                {
                    tNew.status = TokenStatus.Added;
                    result.Add(tNew);
                    added++;
                }
                if (max > 0 && result.Count >= max)
                    return result.Count;
            }
            foreach (Token tOld in tokens)
            {
                Token tNew = list.GetTokenById(tOld.id);
                if (tNew == null)
                {
                    tOld.status = TokenStatus.Removed;
                    result.AddIfNotExists(tOld);
                    deleted++;
                    if (max > 0 && result.Count >= max)
                        return result.Count;
                }
                else if(compareStartOffsets && tNew.startOffset != tOld.startOffset)
                {
                    tNew.status = TokenStatus.Moved;
                    result.AddIfNotExists(tNew);
                    moved++;
                    if (max > 0 && result.Count >= max)
                        return result.Count;
                }     
            }
            

            if(result.Count > 0)
                T.Debug("changed - added: " + added + " deleted: " + deleted + " moved:" + moved);

            return result.Count;
        }
        #endregion

        #region add and remove tokens

        public void MergeRange(TokenList list)
        {
            foreach (Token t in list.tokens)
                if (!tokensByOffset.ContainsKey(t.startOffset))
                    Add(t);
        }

        public void AddRange<T>(T list) where T : TokenList
        {

            // need to figure out how to make Token t be Column or Table instead of Token
            foreach (Token t in list.tokens)
                Add(t);
        }

        public void RemoveAt(int offset)
        {
            Token t = tokensByOffset[offset];
            if (rightExtent == t.rightExtent || leftExtent == t.startOffset)
                _leftExtent = -1; // invalidate extents
            tokensByOffset.Remove(offset);
            tokens.Remove(t);
        }

        void UpdateExtents()
        {
            _leftExtent = -1;
            _rightExtent = -1;
            foreach(Token t in tokens)
            {
                _leftExtent = _leftExtent == -1 ? t.startOffset : Math.Min(t.startOffset, _leftExtent);
                _rightExtent = Math.Max(_rightExtent, t.rightExtent);
            }
        }

        public void AddIfNotExists(Token t)
        {
            if(!tokensByOffset.ContainsKey(t.startOffset))
                _Add(t);
        }

        public int Add(Token t)
        {
            // T.Assert(GetType().Name == "TokenList");
            return _Add(t);
        }
       
        protected int _Add(Token t)
        { 
            if (t != null)
            {
                tokensByOffset.Add(t.startOffset, t);
                if (useIds)
                    tokensById.Add(t.id, t);
                tokens.Add(t);
            }
            return tokens.Count;
        }

        protected void _Clear()
        {
            tokensById.Clear();
            tokensByOffset.Clear();
            tokens.Clear();
            _leftExtent = -1;
        }

        public void Clear()
        {
            _Clear();
        }

        #endregion

        #region find tokens in list

        public string GetNames(string conj = " ")
        {
            string result = "";
            foreach (int offset in tokensByOffset.Keys)
                result = result.AppendTo(tokensByOffset[offset].name, conj);
            return result;
        }

        public void GetObjectNameToken(Token tableOrColumn, out Keyword asToken, out Identifier nameToken, Token within = null)
        {
            asToken = null;
            nameToken = null;
            Token t = GetNextToken(tableOrColumn, TokenType.Any, within);
            if (t != null && t.tokenType == TokenType.Keyword && t.name == "as")
            {
                asToken = t as Keyword;
                t = GetNextToken(t, TokenType.Any, within);
            }
            if (t != null && t.tokenType == TokenType.Identifier && t.charAfter != '.' && t.charBefore != '.')
                nameToken = t as Identifier;
        }

        public IEnumerable<int> GetPrevOffsets(Token t, int leftBoundary = -1, int maxToReturn = 200)
        {
            // returns offsets in descending order

            List<int> offsets = tokensByOffset.Keys.ToList();
            int i = offsets.IndexOf(t.startOffset);
            int returned = 0;
            while(--i >= 0 && ++returned < maxToReturn && (leftBoundary < 0 || offsets[i] >= leftBoundary))
                yield return offsets[i];
        }

        public IEnumerable<int> GetNextOffsets(Token t, int rightBoundary = -1, int maxToReturn = 200)
        {
            List<int> offsets = tokensByOffset.Keys.ToList();
            int i = offsets.IndexOf(t.startOffset);
            int returned = 0;
            while (++i < offsets.Count && ++returned < maxToReturn && (rightBoundary < 0 || offsets[i] < rightBoundary))
                yield return offsets[i];
        }

        public Token this[int i]
        {
            get
            {
                return GetTokenAtOffset(tokensByOffset.Keys.ToList().ElementAt(i));
            }
        }

        public Token GetTokenAtOffset(int n)
        {
            return tokensByOffset.Keys.Contains(n) ? tokensByOffset[n] : null;
        }
    
        public Token GetTokenBeforeOrAtRandomOffset(int pos, TokenType type = TokenType.Any, Token within = null, string name = null)
        {
            // returns nearest token that matches the condition

            foreach (int offset in tokensByOffset.Keys.Reverse())
            {
                if (offset <= pos)
                {
                    Token t = tokensByOffset[offset];
                    if ((type == TokenType.Any || t.tokenType == type) && (within == null || within.TokenWithin(t)) && (name == null || name == t.name))
                        return t; ;
                }
            }

            return null;
        }
        
        public TokenList GetTokensWithin(Token tParent, int start, int end, TokenType[] types = null)
        {
            TokenList result = new TokenList();
            foreach (int offset in tokensByOffset.Keys)
            {
                Token t = tokensByOffset[offset];
                if (t.startOffset >= start && t.rightExtent <= end && tParent.TokenWithin(t, false) && (types == null || types.Contains(t.tokenType)))
                    result.Add(t);
            }
            return result;
        }

        public TokenList GetTokensWithin(Token tParent, TokenType[] types = null, string name = null)
        {
            TokenList result = new TokenList();
            foreach (int offset in tokensByOffset.Keys)
            {
                Token t = tokensByOffset[offset];
                if (tParent.TokenWithin(t, false) && (types == null || types.Contains(t.tokenType)) && (name == null || t.name == name))
                    result.Add(t);
            }
            return result;
        }


        public TokenList GetNextTokens(Token tOrigin, TokenType[] types = null, string[] names = null, int rightBoundary = -1, int maxToReturn = -1)
        {
            TokenList result = new TokenList();
            foreach (int offset in GetNextOffsets(tOrigin, rightBoundary))
            {
                Token t = tokensByOffset[offset];
                T.Assert(t.startOffset > tOrigin.startOffset && (rightBoundary == -1 || t.startOffset < rightBoundary));
                if ((types == null || types.Contains(t.tokenType)) && (names == null || names.Contains(t.name)))
                    result.Add(t);
                if (maxToReturn > 0 && result.Count >= maxToReturn)
                    break;
            }
            return result;
        }

        public TokenList GetPrevTokens(Token tOrigin, TokenType[] types = null, string[] names = null, int leftBoundary = -1, int maxToReturn = -1)
        {
            // returns tokens in order of descending offset position

            TokenList result = new TokenList();
            foreach (int offset in GetPrevOffsets(tOrigin, leftBoundary))
            {
                Token t = tokensByOffset[offset];
                T.Assert(t.startOffset < tOrigin.startOffset && (leftBoundary == -1 || t.startOffset >= leftBoundary));
                if ((types == null || types.Contains(t.tokenType)) && (names == null || names.Contains(t.name)))
                    result.Add(t);
                if (maxToReturn > 0 && result.Count >= maxToReturn)
                    break;
            }
            return result;
        }

        public Token GetNearestTokenBefore(Token tOrigin, TokenType[] types = null, string[] names = null)
        {
            // returns nearest token that matches the condition
            TokenList list = GetPrevTokens(tOrigin, types, names, -1, 1);
            T.Assert(list.Count <= 1);
            return (list.Count == 1 ? list.tokens.First() : null);

        }
        public Token GetNearestTokenBefore(Token tOrigin, TokenType type = TokenType.Any, string name = null)
        {
            return GetNearestTokenBefore(tOrigin, type == TokenType.Any ? null : new[] { type }, name == null ? null : new[] { name });
        }

        public Token GetNearestTokenAfter(Token tOrigin, TokenType[] types = null, string[] names = null)
        {
            // returns nearest token that matches the condition
            TokenList list = GetNextTokens(tOrigin, types, names, -1, 1);
            T.Assert(list.Count <= 1);
            return (list.Count == 1 ? list.tokens.First() : null);
        }


        public Token GetNearestTokenAfter(Token tOrigin, TokenType type, string name = null)
        {
            // returns nearest token that matches the condition
            return GetNearestTokenAfter(tOrigin, type == TokenType.Any ? null : new[] { type }, name == null ? null : new[] { name });
        }

        public Token GetPrevToken(Token token, TokenType type = TokenType.Any, Token within = null, string name = null)
        {
            // returns the first neighbor token or null if it doesn't match the condition
            //Token t = GetNearestTokenBefore(token.startOffset);
            Token t = GetNearestTokenBefore(token, type);
            if (t != null && (type == TokenType.Any || t.tokenType == type) && (within == null || within.TokenWithin(t)) && (name == null || name == t.name))
                return t;
            return null;
        }

        public Token GetNextToken(Token token, TokenType type = TokenType.Any, Token within = null, string name = null)
        {
            // returns the first neighbor token or null if it doesn't match the condition
            Token t = GetNearestTokenAfter(token);
            if (t != null && (type == TokenType.Any || t.tokenType == type) && (within == null || within.TokenWithin(t)) && (name == null || name == t.name))
                return t;
            return null;
        }

        #endregion

    }

    #region TableList class

    public class TableList : TokenList
    {
        Dictionary<string, Table> mapAliasToTable = new Dictionary<string, Table>();
        Dictionary<string, Table> mapNameToTable = new Dictionary<string, Table>();
        Dictionary<DbTable, Table> mapDbTableToTable = new Dictionary<DbTable, Table>();

        public new Table this[int i]
        {
            get
            {
                return i >= 0 && i < tokensByOffset.Count ? tokensByOffset[tokensByOffset.Keys.ToList()[i]] as Table : null;
            }
        }

        public new void AddIfNotExists(Token tok)
        {
            Table table = tok as Table;
            if (!mapNameToTable.ContainsKey(table.dbTable.name))
                Add(tok);

        }

        public new int Add(Token tok)
        {
            Table t = tok as Table;
            if (t.tableAlias != null)
                mapAliasToTable.AddIfNotExists(t.tableAlias, t);
            if (t.dbTable != null)
                mapDbTableToTable.AddIfNotExists(t.dbTable, t);
            if(t.tableName != null)
                mapNameToTable.AddIfNotExists(t.tableName, t);
            return _Add(t);            
        }

        public new void Clear()
        {
            mapAliasToTable.Clear();
            mapNameToTable.Clear();
            _Clear();
        }

        public void AddTables(TokenList list)
        {
            foreach (Table t in list.tokens)
                Add(t);
        }

        public bool Contains(DbTable t)
        {
            return mapDbTableToTable.ContainsKey(t);
        }

        public Table GetTableByName(string n)
        {
            return mapNameToTable.ContainsKey(n) ? mapNameToTable[n] : null;
        }

        public Table GetTableForColumnName(string columnName, string alias = null)
        {
            Table roughMatch = null;
            foreach(DbTable dbTable in Query.Db.tablesByColumnName.Each(columnName))
            {
                Table table = mapDbTableToTable.At(dbTable);
                if (table != null)
                {
                    roughMatch = table;
                    if (alias != null && dbTable.aliases.ContainsKey(alias))
                        return table;
                }
            }
            return roughMatch;
        }

        public Table GetTableForDbTableColumn(DbTable t, string columnName = null)
        {
            Table table = mapDbTableToTable.At(t);
            if(table != null && (columnName == null || t.columns.ContainsKey(columnName)))
                return table;
            return null;
        }

        public Table GetTableForAlias(string alias, string columnName = null)
        {
            Table table = mapAliasToTable.At(alias);
            if (table == null)
                table = mapNameToTable.At(alias);
            if (table == null)
                return null;

            if (columnName == null)
                return table;
            if (table.dbTable != null)
            {
                if (table.dbTable.columns.ContainsKey(columnName))
                    return table;
            }
            else if(table.subquery != null && table.subquery.select != null)
            {
                foreach (Column c in table.subquery.select.columns.tokens)
                    if (c.columnAlias == columnName)
                        return table;
            }
            return null;
        }

    }
    #endregion

    #region ColumnList class

    public class ColumnList : TokenList
    {
        public HashSet<string> tableAliases = new HashSet<string>();
        
        public string GetTableNameForTableAlias(string alias)
        {
            QList <Column> list = new QList<Column>(QListSort.Descending);
            foreach (Column c in tokens)
                if (alias == c.columnAlias)
                    list.Add(c, c.significance);

            return list.Count > 0 ? list[0].tableName : null;
        }

        public Column GetColumnForDbColumn(DbColumn dbColumn)
        {
            foreach (Column c in tokens)
                if (c.dbColumn == dbColumn)
                    return c;

            return null;
        }

        public int Add(Column c)
        {
            tableAliases.AddIfNotExists(c.tableAlias);
            return _Add(c as Column);
        }

        public new void Clear()
        {
            tableAliases.Clear();
            _Clear();
        }
    }

    #endregion

}

