using fp.lib;
using fp.lib.mysql;
using fp.lib.sqlparser;
using System;
using System.Collections.Generic;
using System.Linq;
using fp.lib.dbInfo;

namespace fp.sqratch
{
    public static class Parser
    {
        public delegate void OnParsed();
        public static OnParsed onParsed;
        public static lib.sqlparser.Query query = null;
        public static Token currentToken = null;
        public static Keyword currentKeyword = null;
        public static lib.sqlparser.Query currentQuery = null;
        public static bool parseFailed { get { return query != null && query.status == TokenStatus.Error; } }
        public static bool parseSucceeded { get { return query != null && (query.status == TokenStatus.Built || query.status == TokenStatus.Warning); } }
        public static bool parseHadWarnings { get { return query != null && query.status == TokenStatus.Warning; } }
        public static bool suggestionsReady = false;
        public static Dictionary<string, SuggestionList> querySuggestions = new Dictionary<string, SuggestionList>();
        public static bool suggestionsNeedUpdating { get { return query != null && suggestionsDirty; } }

        static int currentPosition = -1;
        static int lastParse = 0;
        static string queryExpression = "";
        static bool queryQueued = false;
        static bool suggestionsDirty = false;

        static void UpdateSuggestions(string keyword)
        {
            SuggestionList list = Parser.GetSuggestions(keyword);
            if(list != null)
                querySuggestions.Add(keyword, list);
        }

        public static void UpdateSuggestions()
        {
            if(query != null)
            {
                lock(query)
                {
                    querySuggestions.Clear();
                    UpdateSuggestions("select");
                    UpdateSuggestions("from");
                    UpdateSuggestions("where");
                    UpdateSuggestions("group");
                    UpdateSuggestions("order");
                    suggestionsDirty = false;
                    suggestionsReady = true;
                }
            }
        }

        public static bool readyToRun
        {
            get
            {
                return (S.Get("EnableParser", true) && Environment.TickCount - lastParse > 300 && queryExpression != "" && queryQueued);
            }
        }

     
        public static bool OnIdle(string sql, int pos)
        {
            if (Environment.TickCount - lastParse > 1000 && sql != queryExpression)
            {
                TextChanged(sql, pos, true);
                return true;
            }
            return false;
        }

        static bool SignificantCharChange(string sql, int pos)
        {
            if(pos > 1 && pos <= sql.Length)
            {
                char c = sql[pos - 1];
                char cPrev = sql[pos - 2];
                if (char.IsWhiteSpace(c) && char.IsWhiteSpace(cPrev))
                    return false;
                if ((char.IsLetterOrDigit(c) || "._".Contains(c)) && (char.IsLetterOrDigit(cPrev) || "._".Contains(cPrev)))
                    return false;
                if ("<>=".Contains(c) && "<>=".Contains(cPrev))
                    return false;
                T.Debug("char change: " + cPrev.ToString() + "->" + c.ToString());
            }
            
            return true;
        }

        public static void TextChanged(string sql, int pos, bool parseEvenIfsame)
        {
            if(sql == "")
            {
                Clear();
                return;
            }
            
            bool significantCharChange = SignificantCharChange(sql, pos);
            bool significantChar = pos > 0 && pos <= sql.Length && ")(".Contains(sql[pos - 1]);
            currentPosition = pos;

            if(sql != queryExpression && (significantCharChange || significantChar || parseEvenIfsame))
            {
                queryExpression = sql;
                queryQueued = true;
            }
            
            if (queryQueued)
            {
                Parser.Parse(S.Get("crashOnParserErrors", S.Get("QuietParserErrors", true)));
            }
        }
        
        public static void Clear()
        {
            fp.lib.sqlparser.Query.columnHints.Clear();
            query = null;
            queryExpression = "";
            queryQueued = false;
            currentKeyword = null;
            currentQuery = null;
            if(A.db != null)
                A.db.ClearActiveObjects();
            lastParse = 0;          
        }

        public static void Parse(bool allowCrash)
        {
            lastParse = Environment.TickCount;
            queryQueued = false;
            A.AddToLog("parsing...");

            lib.sqlparser.Query tryQuery = lib.sqlparser.Query.Parse(A.db, queryExpression, allowCrash);
            if(tryQuery != null)
            {
                if (tryQuery.status == TokenStatus.Error || tryQuery.status == TokenStatus.Warning)
                {
                    foreach (string msg in tryQuery.errors)
                        A.AddToLog(msg, false, tryQuery.status == TokenStatus.Error ? MsgStatus.Error : MsgStatus.Warning);
                }
                if(tryQuery.status != TokenStatus.Error)
                {
                    OnParsedQuery(tryQuery);
                    A.AddToLog("succeeded");
                    onParsed();
                }
            }
        }

        public static void OnParsedQuery(lib.sqlparser.Query q)
        {
            if (query == null)
                query = q;
            lock (query)
            {
                currentKeyword = null;
                currentToken = null;
                currentQuery = null;
                query = q;
                currentPosition = Math.Min(currentPosition, query.rightExtent);
                if (currentPosition > 0)
                {
                    currentQuery = query.GetQueryAtOffset(currentPosition);
                    if (currentQuery != null)
                    {
                        currentToken = currentQuery.tokens.GetTokenBeforeOrAtRandomOffset(currentPosition);
                        if (currentToken != null)
                        {
                            currentKeyword = currentToken.GetParentPrimaryKeyword();                            
                        }                    
                    }
                }

                A.db.ClearActiveObjects();
                foreach (Table table in query.allTables.tokens)
                {
                    if (table.tableAlias != null && S.Get("RememberAliases", true))
                        A.db.AliasUsed(table.tableAlias, table.name);
                    A.db.TableUsed(table.name);
                }

                foreach (Column column in query.allColumns.tokens)
                {
                    
                    if(column.dbColumn != null)
                        A.db.ColumnUsed(column.dbColumn);
                }

                A.db.SortObjects();

                if(S.Get("QuerySuggestions", true))
                    suggestionsDirty = true;

                T.Debug("currentQuery: " + (currentQuery == null ? "" : currentQuery.debugText));
                T.Debug("currentToken: " + (currentToken == null ? "" : currentToken.debugText));
                T.Debug("currentKeyword: " + (currentKeyword == null ? "" : currentKeyword.debugText));
          }

        }

        public static void MakeSuggestionsDirty()
        {
            if (S.Get("QuerySuggestions", true))
                suggestionsDirty = true;
        }

        public static SuggestionList GetSuggestions(string keyword)
        {
            fp.lib.sqlparser.Query q = T.Coalesce(Parser.currentQuery, query);
            if (q != null)
            {
                lock (query)
                {
                    SuggestionList suggestions = new SuggestionList(q, keyword);
                    if (suggestions.BuildSuggestionList() > 0)
                        return suggestions;
                }
            }

            return null;
        }

        public static SuggestionList GetSuggestions(string text, int position)
        {
            if (Parser.currentQuery != null)
            {
                lock(query)
                {
                    SuggestionList suggestions = new SuggestionList(Parser.currentQuery, Parser.currentKeyword);
                    suggestions.enableSuggestColumns = S.Get("AutocompleteColumns", true);
                    suggestions.enableSuggestTables = S.Get("AutocompleteTables", true);
                    suggestions.enableSuggestAliases = S.Get("AutocompleteAliases", true);
                    suggestions.enableAutoSuggestAliases = S.Get("AutocompleteInsertAliases", false);
                    suggestions.enableSuggestKeywords = S.Get("AutocompleteKeywords", true);
                    suggestions.enableSuggestJoins = S.Get("AutocompleteJoins", true);
                    if (suggestions.BuildSuggestionList(text, position) > 0)
                        return suggestions;
                }
            }

            return null;
        }

        public static bool Accept(IVisitor v)
        {
            if (query != null && query.status != TokenStatus.Error)
            {
                lock(query)
                {
                    query.Accept(v);
                }
                return true;
            }
            return false;
        }

        public enum InsertPosition { Select, From, Where, GroupBy, OrderBy, UpdateColumns, UpdateValues }

        public static bool CanInsert(InsertPosition pos, List<DbTable> tables, List<DbColumn> columns)
        {
            return false;
        }
    }
}
