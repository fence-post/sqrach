using System;
using System.Collections;
using System.Collections.Generic;
using fp.lib.dbInfo;

namespace fp.lib.sqlparser
{
    public class SuggestionContextItem
    {
        public Token token = null;
        public Token aliasToken = null;
        public string type = "";
        public string name = "";
        public bool hasComma = false;
        public int pos = 0;
        public bool isLast = false;
        public bool isOn { get { return (type == "keyword" && name == "on"); } }
        public bool isJoin { get { return (type == "keyword" && name == "join"); } }
        public bool isBefore(SuggestionContextItem other)
        {
            return false;
        }

        public SuggestionContextItem(Token t, bool l)
        {
            token = t;
            if (t != null)
            {
                type = t.tokenType.ToString().ToLower();
                name = t.name;
                pos = t.startOffset;
                isLast = l;
                if (type == "keyword" && name.IsOneOf("and", "or", "not"))
                    type = "conjunction";
            }
        }
    }

    public class SuggestionContext
    {
        public bool suggestColumns = false;
        public bool suggestColumnsInSelectList = false;
        public bool suggestColumnsInSelectCandidateList = false;
        public bool suggestColumnsInFromTables = false;
        public bool suggestAllColumnsInQuery = false;
        public bool suggestAllColumns = false;
        public Type suggestColumnsDataType = null;
      
        public bool suggestAlias = false;
        public bool suggestOperators = false;
        public bool suggestOn = false;
        public bool suggestConjunctions = false;
        public bool suggestJoinExpression = false;
        public bool suggestTables = false;
        public bool suggestJoin = false;
        public bool suggestPrimaryKeywords = false;
        public bool suggestBookmarkedColumns = false;
        public bool suggestBookmarkedTables = false;

        public SuggestionContextItem last;
        public SuggestionContextItem lastOperator;
        public SuggestionContextItem lastConjunction;
        public SuggestionContextItem lastLiteral;
        public SuggestionContextItem lastColumn;
        public SuggestionContextItem lastKeyword;
        public SuggestionContextItem lastTable;
        public Token parent = null;
        public TokenList tokens = null;
        public SuggestionList suggestions = null;

        protected DbInfo Db { get { return Query.Db; } }

        SuggestionContextItem InitItem(TokenType typ, string[] names = null)
        {
            if (last.token.tokenType == typ)
                return last;
            else
                return new SuggestionContextItem(tokens.GetNearestTokenBefore(last.token, new[] { typ }, names), false);
        }

        public SuggestionContext(SuggestionList sug, Token p)
        {
            suggestions = sug;
            parent = p;
            suggestions.includeAliases = suggestions.enableAutoSuggestAliases;
            if (suggestions.includeAliases == false && suggestions.enableSuggestAliases)
                CheckForAlias();
        }

        void CheckForAlias()
        {
         
            bool aliasFound = false;
            if (parent.parentQuery.select != null)
            {
                foreach (Column c in parent.parentQuery.select.columns.tokens)
                    if (c.tableAlias != null)
                    {
                        aliasFound = true;
                        break;
                    }
            }
            if (aliasFound == false && parent.parentQuery.from != null)
            {
                foreach (Table t in parent.parentQuery.from.tables.tokens)
                    if (t.tableAlias != null)
                    {
                        aliasFound = true;
                        break;
                    }
            }
            suggestions.includeAliases = aliasFound;
        }

        public SuggestionContext(SuggestionList sug, Token p, int pos, string textEntered)
        {
            suggestions = sug;
            parent = p;
            suggestions.includeAliases = suggestions.enableAutoSuggestAliases;
            if (suggestions.includeAliases == false && suggestions.enableSuggestAliases)
                CheckForAlias();

            tokens = p.GetTokensWithin();
            Token lastToken = tokens.GetTokenBeforeOrAtRandomOffset(pos);
            if (lastToken != null && lastToken.tokenType == TokenType.Identifier && textEntered.StartsWith(lastToken.name))
                lastToken = lastToken.GetPrevToken();
            if (lastToken != null)
            {
                last = new SuggestionContextItem(lastToken, true);
                lastKeyword = InitItem(TokenType.Keyword);
                lastLiteral = InitItem(TokenType.Literal);
                lastTable = InitItem(TokenType.Table);
                lastOperator = InitItem(TokenType.Operator);
                lastConjunction = InitItem(TokenType.Keyword, new[] { "and", "or", "not" });
                lastColumn = InitItem(TokenType.Column);

                if (lastTable.token != null)
                {
                    if (lastTable.isLast)
                        lastTable.hasComma = lastTable.token.charAfter == ',';
                    else
                    {
                        Table t = lastTable.token as Table;
                        if (t.tableAliasToken != null && t.tableAliasToken == last.token)
                        {
                            last = lastTable;
                            lastTable.aliasToken = t.tableAliasToken;
                            lastTable.hasComma = t.tableAliasToken.charAfter == ',';
                        }
                    }
                }
                if (last.type == "column" && last.token != null && last.token.charAfter == ',')
                    last.hasComma = true;
            }
        }

        #region add columns and tables
        
        public void AddColumnsInTable(Table t, bool primaryOnly = false)
        {
            if (t == null)
                return;
            if(t.dbTable != null)
                AddColumnsInTable(t.dbTable, t.tableAlias, primaryOnly);
            else if(t.subquery != null)
            {
                if (t.subquery.select != null)
                    foreach (Column c in t.subquery.select.columns.tokens)
                        AddColumn(c, t.tableAlias);
                
            }
        }
        public void AddColumn(Column c, string tableAlias = null)
        {
            if (tableAlias == null)
                tableAlias = c.tableAlias;
            if (suggestColumnsDataType == null || (c.dbColumn != null && c.dbColumn.dataType == suggestColumnsDataType))
            {
                if (c.dbColumn != null)
                    AddColumn(c.dbColumn, tableAlias);
                else
                {
                    Suggestion s = new Suggestion(TokenType.Column, c.columnName);                   
                    s.tableAlias = tableAlias;
                    suggestions.Add(s);
                }
            }
        }

        public void AddColumn(DbColumn column, string tableAlias = null)
        {
            if (suggestColumnsDataType == null || column.dataType == suggestColumnsDataType)
            {
                Suggestion s = new Suggestion(column);
                s.tableAlias = tableAlias;
                s.rightStuff = column.definition;
                s.middleStuff = T.AppendTo(column.table.schemaName.LimitToLength(10, "...", true), 
                    column.table.tableName.LimitToLength(20, "...", true), ".");
                suggestions.Add(s);
            }
        }

        public void AddColumnsInTable(DbTable t, string alias = null, bool primaryOnly = false)
        {
            foreach (DbColumn c in t.columns.Values)
                if(primaryOnly == false || c.primaryKey)
                    AddColumn(c, alias);
        }

        public void AddColumnsInTable(string table, string alias = null)
        {
            foreach (DbColumn c in Db.tables[table].columns.Values)
                AddColumn(c, alias);
        }
     
        public void AddColumnsInSelectList()
        {
            if (parent.parentQuery.select != null)
                foreach (Column c in parent.parentQuery.select.columns.tokens)
                    AddColumn(c);
        }

        public void AddColumnsInSelectCandidateList()
        {
            if (parent.parentQuery.select != null)
                foreach (DbTable t in parent.parentQuery.select.candidateTables.Keys)
                    AddColumnsInTable(t);
            }

        public void AddColumnsInFromTables()
        {
            if (parent.parentQuery.from != null)
                foreach (Table t in parent.parentQuery.from.tables.tokens)
                    AddColumnsInTable(t);
        }

        public void AddTablesByAffinity()
        {
            HashSet<DbTable> list = new HashSet<DbTable>();
            if(parent.parentQuery.select != null)
            {
                foreach(Column c in parent.parentQuery.select.columns.tokens)
                {
                    if(c.dbColumn != null)
                    {
                        list.AddIfNotExists(c.dbColumn.table);
                    }
                }
            }
            List<DbTable> result = new List<DbTable>();
            if (parent.parentQuery.from != null)
            {
                foreach (Table t in parent.parentQuery.from.tables.tokens)
                    if (t.dbTable != null && t.dbTable.tablePaths != null)
                    {
                        foreach(DbTable table in t.dbTable.tablePaths.accessibleTables)
                        {
                            list.AddIfNotExists(table);
                        }
                    }
                foreach(DbTable t in list)
                {
                    if (parent.parentQuery.from.tables.GetTableByName(t.name) == null)
                        result.Add(t);
                }

            }
            else
            {
                result.AddRange(list);
            }
            foreach (DbTable t in list)
            {
                AddTable(t.name);
            }

        }

        public void AddColumnsByAffinity()
        {
            if (parent.tokenType == TokenType.From)
            {
                AddTablesByAffinity();
                return;
            }

            if (parent.parentQuery.from != null)
            {
                HashSet<string> columnNamesAdded = new HashSet<string>();
                Keyword keyword = parent as Keyword;
                ColumnList columnsAlready = keyword == null ? null : keyword.columns;
                string affinityTable = keyword.shortName.ToLower() + "y";

                foreach (Table t in parent.parentQuery.from.tables.tokens)
                {
                    if (t.dbTable != null && t.dbTable.queryColumns.ContainsKey(affinityTable))
                    {
                        foreach (DbColumn c in t.dbTable.queryColumns[affinityTable])
                        {
                            if (columnNamesAdded.Contains(c.name) == false && (columnsAlready == null || columnsAlready.GetColumnForDbColumn(c) == null))
                            {
                                AddColumn(c);
                                columnNamesAdded.Add(c.name);
                            }

                        }
                    }
                }
                foreach (Table t in parent.parentQuery.from.tables.tokens)
                {
                    if (t.dbTable != null)
                    {
                        foreach (DbColumn c in t.dbTable.columns.Values)
                        {
                            if (columnNamesAdded.Contains(c.name) == false && (columnsAlready == null || columnsAlready.GetColumnForDbColumn(c) == null))
                            {
                                AddColumn(c);
                                columnNamesAdded.Add(c.name);
                            }

                        }
                    }
                }
            }
        }

        public void AddAllColumnsInQuery()
        {
            QList<Column> list = new QList<Column>(QListSort.Descending);
            foreach (Column c in Query.rootQuery.allColumns.tokens)
                list.Add(c, c.significance);
            foreach(Column c in list.Each())
                AddColumn(c);
        }

        public void AddBookmarkedColumns()
        {
            foreach (DbTable table in Db.tables.Values)
            {
                if (table.bookmarked)
                    foreach (DbColumn column in table.columns.Values)
                        AddColumn(column);
            }
        }

        public void AddAllColumns()
        {
            foreach (DbTable table in Db.tables.Values)
            {
                foreach (DbColumn column in table.columns.Values)
                    AddColumn(column);
            }
        }

        public void AddTable(string name)
        {
            T.Assert(parent == parent.parentQuery.from);
            if (name != "" && parent.parentQuery.from.tables.GetTableByName(name) == null)
                suggestions.Add(new Suggestion(Db.tables[name]));
        }
      
        #endregion

        public void SuggestPrimaryKeywords()
        {
            if (suggestions.lenEntered > 0)
            {
                if (parent.parentQuery.from == null)
                    suggestions.AddKeyword("from", true);
                if (parent.parentQuery.where == null)
                    suggestions.AddKeyword("where");
                if (parent.parentQuery.group == null)
                    suggestions.AddKeyword("group by");
                if (parent.parentQuery.order == null)
                    suggestions.AddKeyword("order by", true);
                if (parent.parentQuery.group == null)
                    suggestions.AddKeyword("limit");
            }
        }


        public void SuggestOperators()
        {
            //suggestions.AddKeywords("=", "<", ">", "<>");
        }

        public void SuggestConjunctions()
        {
            if (suggestions.lenEntered > 0)
            {
                suggestions.AddKeyword("and");
                suggestions.AddKeyword("or");
                suggestions.AddKeyword("not");

            }
        }
    }

}
