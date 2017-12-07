using System;
using System.Collections.Generic;
using System.Linq;
using fp.lib.dbInfo;
using fp.lib;

namespace fp.lib.sqlparser
{
    public partial class Query : Token
    {
        string sql;
        public bool softMode = true;
        List<Query> queries = new List<Query>();
        
        public static Query Parse(DbInfo dbInfo, string expr, bool allowCrash)
        {
            Query.Db = dbInfo;
            Query query = new Query(0, expr);
            // Query.columnHints.Clear();
            Query.columnHints.AddRange(Query.Db.activeColumns);
     
            if (allowCrash)
                query.Parse();
            else
                query.TryParse();
            return query;
        }

        void TryParse()
        {
            try
            {
                Parse();
            }
            catch (Exception e)
            {
                AddError(e.Message, TokenStatus.Error);
            }
        }

        void Parse()
        {
            rootQuery = this;
            if (operators.Count == 0)
                Query.InitializeForParsing();

            allTables = new TableList();
            allColumns = new ColumnList();
            
            sql = expression + " ";
            
            if(parsedTokens.Count == 0)
                ParseTokens();
            FindSchemaTables();
            ParseSubqueries();
            BuildQueryStructure();
            BuildQuery();
        }

        void FindSchemaTables()
        {
            TokenList tokenList = rootQuery.parsedTokens.GetTokensWithin(this, new[] { TokenType.Identifier });
            foreach (Token t in tokenList.tokens)
            {
                if (t.charAfter == '.')
                {
                    Token tNext = rootQuery.parsedTokens.GetNextToken(t, TokenType.Identifier);
                    if(tNext != null && tNext.charBefore == '.' && Db.tables.ContainsKey(t.name + "." + tNext.name))
                    {
                        Table table = new Table(t.startOffset, t.name + "." + tNext.name);
                        rootQuery.parsedTokens.RemoveAt(t.startOffset);
                        rootQuery.parsedTokens.RemoveAt(tNext.startOffset);
                        rootQuery.parsedTokens.Add(table);
                    }
                }
            }
        }

        void ParseTokens()
        {
            ParseTokens tokenParser = new sqlparser.ParseTokens();
            tokenParser.Parse(sql);
            parsedTokens.AddRange(tokenParser.parsedTokens);
        }
        void AssignTokensToQueries(TokenType[] types)
        {         
            TokenList tokenList = rootQuery.parsedTokens.GetTokensWithin(this, types);
            foreach (Token t in tokenList.tokens)
            {
                if (t.parentQuery == null)
                {
                    tokens.Add(t);
                    t.parentQuery = this as Query;
                }
            }
        }

        void BuildTokens(bool buildKeywordStructure, bool buildTokenStructure, bool buildTokens, TokenType[] types)
        {
            TokenList tokenList = tokens.GetTokensWithin(this, types);

            if (buildKeywordStructure)
            {
                foreach (Token t in tokenList.tokens)
                {
                    if (select == null && t.tokenType == TokenType.Select)
                        select = t as Select;
                    if (from == null && t.tokenType == TokenType.From)
                        from = t as From;
                    if (where == null && t.tokenType == TokenType.Where)
                        where = t as Where;
                    if (order== null && t.tokenType == TokenType.Order)
                        order = t as OrderBy;
                    if (group == null && t.tokenType == TokenType.Group)
                        group = t as GroupBy;
                    if (limit == null && t.tokenType == TokenType.Limit)
                        limit = t as Limit;
                }

                foreach (Token t in tokenList.tokens)
                    t.UpdateLength();
            }
            if (buildTokenStructure)
            { 
                foreach (Token t in tokenList.tokens)
                    t.BuildStructure();
            }
            if (buildTokens)
            {
                foreach (Token t in tokenList.tokens)
                    t.Build();
            }
        }

        void BuildQuery()
        {
            foreach (Query q in queries)
                q.BuildQuery();
           
            AssignTokensToQueries(new[] { TokenType.Query });
            AssignTokensToQueries(null);
            BuildTokens(true, false, false, new[] { TokenType.From, TokenType.Select, TokenType.Where, TokenType.Group, TokenType.Order, TokenType.Limit });
            BuildTokens(false, true, false, new[] { TokenType.From });
            BuildTokens(false, true, false, new[] { TokenType.Select });
            BuildTokens(false, true, false, new[] { TokenType.Where, TokenType.Group, TokenType.Order, TokenType.Limit });
            FindColumns(false);
            //AssignTokensToQueries(new[] { TokenType.Column });
            BuildTokens(false, false, true, new[] { TokenType.From, TokenType.Select, TokenType.Where, TokenType.Group, TokenType.Order, TokenType.Limit });
            SetParents();
        }

        void BuildQueryStructure()
        {
            if(softMode && startOffset + expressionLength > rootQuery.sql.Length)
            {
                // fix for adding ) at end to parse
                int len = rootQuery.sql.Length - startOffset;
                SetExpression(rootQuery.sql.Substring(startOffset, len));
            }
                
            foreach(Query q in queries)
            {
                q.parentQuery = this;
                tokens.Add(q);
                q.BuildQueryStructure();
            }
        }

        void SetParents()
        {
            SetParentVisitor v = new SetParentVisitor();
            Accept(v);
        }
        
        void FindColumns(bool includeParents)
        {
            TableList tablesInScope = GetTablesInScope(includeParents);
            TokenList tokenList = tokens.GetTokensWithin(this, new[] { TokenType.Identifier });
            foreach (Token t in tokenList.tokens)
            {
                DbColumn dbColumn = null;
                Column column = null;
                Table table = null;
                string alias = null;
                Identifier tAlias = t.GetPrevToken(TokenType.Identifier) as Identifier;
                if (tAlias != null && tAlias.charAfter == '.' && t.charBefore == '.')
                    alias = tAlias.name;
                if(alias != null)
                    table = tablesInScope.GetTableForAlias(alias, t.name);

                /*
                if(alias != null)
                {
                    foreach (string tableName in Db.GetPossibleTablesForAlias(alias))
                    {
                        if(Db.tables[tableName].columns.ContainsKey(t.name))
                    }
                        sc.AddColumnsInTable(t, alias);

                }
  
                if (table == null && select != null)
                {
                    foreach (DbTable dbTable in select.candidateTables.Keys)
                    {
                        table = tablesInScope.GetTableForDbTableColumn(dbTable, t.name);
                        if (table != null)
                            break;
                    }
                }
                              */

                if (table == null)
                {
                    foreach(Table tabl in tablesInScope.tokens)
                    {
                        if(tabl.dbTable != null && tabl.dbTable.columns.ContainsKey(t.name))
                        {
                            if(alias == null || tabl.dbTable.aliases.ContainsKey(alias))
                            {
                                table = tabl;
                                break;
                            }
                        }
                    }
                }
                if (table != null)
                {
                    if (table.dbTable != null)
                        dbColumn = table.dbTable.columns[t.name];
                    else if(table.subquery != null)
                        column = table.subquery.GetSelectColumn(t.name, alias);
                    AddColumnToQuery(new Column(t as Identifier, tAlias, table, dbColumn));
                }
            }            
        }

       
        void AddColumnToQuery(Column c)
        {
            Token t = tokens.GetTokenAtOffset(c.startOffset);
            T.Assert(t != null && t.tokenType == TokenType.Identifier);
            if (t != null && t.tokenType == TokenType.Identifier)
            {
                tokens.RemoveAt(c.startOffset);
                tokens.Add(c);
                c.parentQuery = this;
                rootQuery.allColumns.AddIfNotExists(c);
            }
        }

        int FixBrackets()
        {
            Stack<int> brackets = new Stack<int>();

            int openBrackets = 0;
            int closeBrackets = 0;
            for (int i = 0; i < sqlForParsing.Length; ++i)
            {
                char ch = sqlForParsing[i];
                if (ch == '(')
                    openBrackets++;
                else if (ch == ')')
                    closeBrackets++;
            }

            if(softMode)
            {
                if (openBrackets > closeBrackets)
                    sqlForParsing += new string(')', openBrackets - closeBrackets);
            }
            return openBrackets - closeBrackets;
        }
      
        static string sqlForParsing;
        static Dictionary<int, bool> unions = new Dictionary<int, bool>();
           
        void ParseSubqueries()
        {
            unions.Clear();
            sqlForParsing = sql;

            if (FixBrackets() != 0)
            {
                if (softMode)
                    AddError("mismatched brackets");
                else
                    throw new Exception("mismatched brackets");
            }

            for(int at = 0; (at = sqlForParsing.IndexOf("union", at)) > 0; at++)
                unions.Add(at, false);

            FindUnions(this, startOffset, rightExtent);
            DoParseSubqueries(sqlForParsing.Length - 1);         
        }

        void DoParseSubqueries(int right)
        {
            int ct = 0;
            int at = startOffset;
            while (true)
            {
                if (ct++ > 1000)
                    throw new ApplicationException("could not parse subqueries");
                at = sqlForParsing.IndexOf('(', at);
                if (at < 0 || at > right)
                    break;
                    
                int brackets = 1;
                int open = at + 1;
                int close = -1;
                at = open;
                while (brackets > 0)
                {
                    if (ct++ > 1000)
                        throw new ApplicationException("could not parse subqueries");
                    close = sqlForParsing.IndexOf(')', at);
                    int nextOpen = sqlForParsing.IndexOf('(', at);
                    if(nextOpen > 0 && nextOpen < close)
                    {
                        at = nextOpen + 1;
                        brackets++;
                    }
                    else if (close > 0)
                    {
                        at = close + 1;
                        brackets--;
                    }
                    else
                    {
                        throw new ApplicationException("could not parse subqueries");
                    }
                }
                AddQuery(open, close);

            }
            
        }

        private void AddQuery(int open, int close)
        {
            Query q = new Query(open, sqlForParsing.Substring(open, close - open));
            q.DoParseSubqueries(close);
            queries.Add(q);
            FindUnions(q, open, close);
        }

        void FindUnions(Query q, int open, int close)
        {
            string expr = sqlForParsing.Substring(open, close - open);
            int at = open;
            while (true)
            {
                at = sqlForParsing.IndexOf("union", at + 1);
                if (at < 0 || at > close)
                    break;
                if (IdentifierCharsOnEdges(at, "union"))
                    continue;
                if (!unions[at])
                {
                    unions[at] = true;
                    expr = sqlForParsing.Substring(open, at - open);
                    Query qUnion = new Query(open, expr);
                    q.queries.Add(qUnion);
                    at = open = at + 5;
                }
            }
            if(q.queries.Count > 0)
            {
                expr = sqlForParsing.Substring(open, close - open);
                Query qUnion = new Query(open, expr);
                q.queries.Add(qUnion);               
            }
        }
        protected bool IdentifierCharsOnEdges(int at, string word, string otherExcludedChars = "")
        {
            if (IsIdentifierChar(at - 1) || IsIdentifierChar(at + word.Length))
                return true;
            if (IsCharIn(at - 1, otherExcludedChars) || IsCharIn(at + word.Length, otherExcludedChars))
                return true;
            return false;
        }

        protected bool IsIdentifierChar(int pos)
        {
            return (pos >= 0 && pos < sql.Length) && (char.IsLetterOrDigit(sql[pos]) || sql[pos] == '_');
        }

        protected bool IsCharIn(int pos, string charList)
        {
            return (pos >= 0 && pos < sql.Length) && (charList.Contains(sql[pos]));
        }

        protected bool IsWhitespaceChar(int pos)
        {
            return (pos >= 0 && pos < sql.Length) && char.IsWhiteSpace(sql[pos]);
        }

        protected char GetCharAt(int pos)
        {
            return (pos >= 0 && pos < sql.Length) ? sql[pos] : char.MinValue;
        }
        



    }
}
