using System;
using System.Collections.Generic;
using fp.lib.dbInfo;

namespace fp.lib.sqlparser
{
    public enum SuggestionType { Normal, Important, Suggested, Hidden }
    public class Suggestion
    {
        public SuggestionType suggestionType;
        public TokenType tokenType;
        public bool openImmediatelyAfter = false;
        public string tableAlias = null;
        public string expr = null;
        string _expr;
        public void PrepareExpr(bool includeAliases)
        {
            string result = _expr;
            if (tokenType == TokenType.Column)
            {
                string alias = "";
                if (includeAliases)
                {
                    if (tableAlias == null && dbColumn != null)
                        tableAlias = dbColumn.table.GetAlias(false);
                    alias = tableAlias;
                }
                string tableName = dbColumn == null ? _expr : dbColumn.name;
                result = T.AppendTo(alias, tableName, ".");
            }
            else if (tokenType == TokenType.Table)
            {
                string alias = "";
                if (includeAliases)
                    alias = dbTable.GetAlias(false);
                result = T.AppendTo(dbTable.name, alias, " ");
            }

            expr = result;
        }

        string _key = null;
        public string key
        {
            get
            {
                if(_key == null)
                {
                    string result = _expr;
                    if (result == null)
                        result = dbObject.objectName;
                    result = result.Trim();
                    if (middleStuff != null)
                        result += "." + middleStuff.Trim();
                    if (rightStuff != null)
                        result += "." + rightStuff.Trim();
                    _key = result;
                }
                return _key;
            }
        }

        public string rightStuff;
        public string middleStuff;
        DbObject dbObject;
        public DbColumn dbColumn { get { return dbObject as DbColumn; } set { dbObject = value; } }
        public DbTable dbTable
        {
            set { dbObject = value; }
            get
            {
                if (tokenType == TokenType.Table)
                    return dbObject as DbTable;
                else if (tokenType == TokenType.Column)
                    return dbColumn.table;
                return null;
            }
        }
        public int position;

        public Suggestion(DbColumn c)
        {
            tokenType = TokenType.Column;
            _expr = null;
            suggestionType = SuggestionType.Normal;
            dbColumn = c;
        }

        public Suggestion(DbTable t)
        {
            tokenType = TokenType.Table;
            _expr = null;
            suggestionType = SuggestionType.Normal;
            dbTable = t;
        }

        public Suggestion(TokenType typ, string text, SuggestionType t = SuggestionType.Normal)
        {
            tokenType = typ;
            _expr = text;
            suggestionType = t;
        }
    }

    public class SuggestionList
    {
        public bool enableSuggestColumns = true;
        public bool enableSuggestTables = true;
        public bool enableSuggestAliases = true;
        public bool enableAutoSuggestAliases = false;
        public bool enableSuggestKeywords = true;
        public bool enableSuggestJoins = true;

        public int wordStartPosition = 0;
        public string textEntered = "";
        public int lenEntered { get { return textEntered.Length; } }
        Dictionary<string, Suggestion> suggestions = new Dictionary<string,Suggestion>();
        public Query query = null;
        public Token keyword = null;
        public string keywordName = null;
        public SuggestionContext suggestionContext;
        public string charsAfter = " ";
        public bool queryMode { get { return wordStartPosition == -1; } }
        public bool includeAliases = false;

        public SuggestionList(Query q, Token k)
        {
            query = q;
            keyword = k;
        }

        public SuggestionList(Query q, string k)
        {
            keywordName = k;
            query = q;
            if (k == "select")
                keyword = q.select;
            else if (k == "where")
                keyword = q.where;
            else if (k == "order")
                keyword = q.order;
            else if (k == "group")
                keyword = q.group;
            else if (k == "from")
                keyword = q.from;

            if (keyword == null)
            {
                keyword = Keyword.CreateKeyword(0, k);
                keyword.parentQuery = q;
            }
        }

        public void Clear()
        {
            suggestions.Clear();
        }

        public int FindWordStartPosition(string text, int end)
        {
            int at = -1;
            string word = "";
            for(at = end; at > 0; at--)
            {
                char c = text[at - 1];
                if (Char.IsLetterOrDigit(c) || c == '.' || c == '_')
                    word += c;
                else
                    break;
            }
            return at;
        }

        public int currentPos;
        public int BuildSuggestionList(string text, int pos)
        {
            currentPos = pos;
            wordStartPosition = FindWordStartPosition(text, pos);
            if (wordStartPosition < 0) // || wordStartPosition == pos) 
                return 0;
            textEntered = text.Substring(wordStartPosition, pos - wordStartPosition);
            if (keyword != null)
            {
                suggestionContext = new SuggestionContext(this, keyword, wordStartPosition, textEntered);
                keyword.GetSuggestions(this);
            }
            if(Query.rootQuery.expressionLength < 5 && suggestions.Count == 0 && query != null && query.select == null)
                AddKeyword("select");
                
            return suggestions.Count;
        }

        public int BuildSuggestionList()
        {
            currentPos = -1;
            wordStartPosition = -1;
            textEntered = "";
            suggestionContext = new SuggestionContext(this, keyword);
            suggestionContext.AddColumnsByAffinity();
            return suggestions.Count;
        }

        /*
        public IEnumerable<Suggestion> GetSuggestions(bool sortAlphabetically, TokenType tokenType = TokenType.Any, SuggestionType suggestionType = SuggestionType.Normal)
        {
            if (sortAlphabetically)
            {
                SortedList<string, Suggestion> eek = new SortedList<string, Suggestion>();
                foreach (string k in suggestions.Keys)
                {
                    Suggestion s = suggestions[k];
                    eek.Add(s.GetExpr(includeAliases) + eek.Count, s);
                }
                foreach (var k in eek.Keys)
                {
                    Suggestion s = eek[k];
                    if ((tokenType == TokenType.Any || s.tokenType == tokenType) && (suggestionType == SuggestionType.Normal || s.suggestionType == suggestionType))
                        yield return s;
                }
            }
            else
            {
                SortedList<int, Suggestion> eek = new SortedList<int, Suggestion>();
                foreach (string k in suggestions.Keys)
                {
                    Suggestion s = suggestions[k];
                    eek.Add(s.position, s);
                }
                foreach (int k in eek.Keys)
                {
                    Suggestion s = eek[k];
                    if ((tokenType == TokenType.Any || s.tokenType == tokenType) && (suggestionType == SuggestionType.Normal || s.suggestionType == suggestionType))
                        yield return s;
                }
            }
        }
        */


        public IEnumerable<Suggestion> GetSuggestions(bool sortedAlphabetically, string filter)
        {
            SortedList<string, Suggestion> list = new SortedList<string, Suggestion>();
            foreach (Suggestion s in suggestions.Values)
            {
                // s.rightStuff = "pos:" + s.position.ToString() + "]";
                s.PrepareExpr(includeAliases);
                if (filter == "" || s.expr.Contains(filter))
                    list.Add(s.expr + list.Count, s);
            }
            foreach (string k in list.Keys)
                yield return list[k];

            /*
            if (sortedAlphabetically)
            {
                SortedList<string, Suggestion> list = new SortedList<string, Suggestion>();
                foreach (Suggestion s in suggestions.Values)
                {
                    s.rightStuff = "pos:" + s.position.ToString() + "]";
                    s.PrepareExpr(includeAliases);
                    if (filter == "" || s.expr.Contains(filter))
                        list.Add(s.expr + list.Count, s);
                }
                foreach (string k in list.Keys)
                    yield return list[k];
            }
            else
            {
                SortedList<int, Suggestion> list = new SortedList<int, Suggestion>();
                foreach (Suggestion s in suggestions.Values)
                {
                    s.PrepareExpr(includeAliases);
                    if (filter == "" || s.expr.Contains(filter))
                        list.Add(s.position, s);
                }
                foreach (int k in list.Keys)
                    yield return list[k];
            }
            */
        }

        public int Count { get{return suggestions.Count;} }

        public void Add(Suggestion s)
        {
            s.position = suggestions.Count;
            if (!suggestions.ContainsKey(s.key))
                suggestions.Add(s.key, s);
        }

        public void AddKeyword(string expr, bool openAfter = false)
        {
            Suggestion s = new Suggestion(TokenType.Keyword, expr);
            s.openImmediatelyAfter = openAfter;
            Add(s);
        }
    }
}
