using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using fp.lib;
using fp.lib.dbInfo;

namespace fp.lib.sqlparser
{
    public class From : Keyword
    {
        public TableList tables = new TableList();

        protected override TokenType[] terminators { get { return new[] { TokenType.Where, TokenType.Group, TokenType.Order, TokenType.Limit }; } }
        protected override bool terminateAtEndOfQueryIfNoTerminatorTokensFound { get { return true; } }

        public From(int offset, string expr) : base(TokenType.From, KeywordType.Primary, offset, expr)
        {

        }

        public override void GetSuggestions(SuggestionList s)
        {
            if (parentQuery.select == null)
                return;

            SuggestionContext sc = s.suggestionContext;

            Table rightTable = null;
            Table leftTable = null;
            
            if(sc.last == null)
            {
                sc.suggestTables = true;
            }
            else
            {
                rightTable = sc.lastTable.token != null ? sc.lastTable.token as Table : null;
                leftTable = rightTable == null ? null : sc.tokens.GetNearestTokenBefore(rightTable, TokenType.Table) as Table;
               
                if (rightTable == null)
                {
                    sc.suggestTables = true;
                }
                else if(sc.last.type.IsOneOf("conjunction"))
                {
                    sc.suggestColumns = true;
                }
                else if(sc.last.type == "operator")
                {
                    sc.suggestColumns = true;
                    if(sc.lastColumn.token != null)
                    {
                        Column c = sc.lastColumn.token as Column;
                        sc.suggestColumnsDataType = c.dbColumn.dataType;
                    }
                }
                else if(sc.last.type.IsOneOf("identifier", "column", "literal"))
                {
                    sc.suggestOperators = sc.lastOperator.pos < sc.lastConjunction.pos;
                    sc.suggestPrimaryKeywords = sc.suggestConjunctions = sc.lastOperator.pos == 0 || sc.lastOperator.pos > sc.lastConjunction.pos;
                    sc.suggestJoin = sc.lastOperator.pos > sc.lastKeyword.pos;
                }
                else if(sc.last.type == "table")
                {
                    sc.suggestPrimaryKeywords = (sc.lastKeyword.isJoin == false);
                    if (sc.last.hasComma)
                        sc.suggestTables = true;
                    else
                    {
                        sc.suggestAlias = true;
                        sc.suggestJoin = sc.lastKeyword.isJoin == false;
                        sc.suggestJoinExpression = sc.suggestOn = !sc.suggestJoin;
                    }
                        
                }
                else if (sc.last.isOn)
                {
                    sc.suggestColumns = true;
                    sc.suggestJoinExpression = true;
                }
                else if(sc.last.isJoin)
                {
                    sc.suggestTables = true;
                }
            }

            if (sc.suggestJoinExpression && s.enableSuggestJoins && rightTable != null && leftTable != null)
            {
                string join = leftTable.dbTable.RenderJoinCols(rightTable.name, s.includeAliases);
                if(join != null)
                {
                    if (sc.suggestOn)
                        join = "on " + join;
                    s.Add(new Suggestion(TokenType.Expression, join));
                }
            }

            if (sc.suggestJoin && s.enableSuggestKeywords)
                s.AddKeyword("inner join", true);
            if (sc.suggestOn && s.enableSuggestKeywords)
                s.AddKeyword("on", true);

            if (sc.suggestAlias && s.enableSuggestAliases && rightTable != null && rightTable.dbTable != null)
            {
                foreach (string alias in rightTable.dbTable.aliases.Keys)
                    s.Add(new Suggestion(TokenType.Identifier, alias));
                s.Add(new Suggestion(TokenType.Identifier, Db.GetAliasForTable(rightTable.dbTable.name, false)));
            }
            
            if (sc.suggestColumns && s.enableSuggestColumns)
            {
                if(rightTable != null && rightTable.dbTable != null)
                {
                    sc.AddColumnsInTable(leftTable, true);
                    sc.AddColumnsInTable(rightTable, true);
                    sc.AddColumnsInTable(leftTable);
                    sc.AddColumnsInTable(rightTable);
                    sc.AddColumnsInFromTables();
                }
            }

            if (sc.suggestTables && s.enableSuggestTables)
            {
                QList<string> names = new QList<string>();
                QList<string> list = new QList<string>();
                foreach (Column c in parentQuery.select.columns.tokens)
                {
                    if (c.table != null && c.table.dbTable != null && (c.columnType == Column.ColumnType.Db))
                        list.AddIfNotExists(c.table.dbTable.name);
                    sc.AddTable(c.tableName);
                }
                foreach (DbColumn c in Query.columnHints)
                    sc.AddTable(c.table.name);

                if (rightTable != null && rightTable.dbTable != null)
                {
                    /*
                    if (rightTable.tableAlias != null)
                        aliasFound = true;
                    // need to sort these tables by most recently used
                    names.MergeRange(Db.relationships.GetRelatedTables(rightTable.dbTable.name));
                    foreach (string name in names.Each())
                        sc.AddTable(name, aliasFound ? Db.GetAliasForTable(name, true) : "");
                    names.Clear();
                    */
                }

                if (s.textEntered.Trim() != "")
                {
                    foreach (Column c in parentQuery.select.columns.tokens)
                        if ((c.tableAlias == null || c.columnType == Column.ColumnType.Proposed)
                            && c.tableName != null && Db.tables.ContainsKey(c.tableName))
                        {
                            list.AddIfNotExists(c.tableName);
                            names.AddIfNotExists(c.tableName);
                        }

                    foreach (string srcTable in list.Each())
                        names.MergeRange(Db.TableNamesSortedByUsage(Db.tables[srcTable].accessibleTableNames));
                    names.MergeRange(Db.tables.Keys);
                    foreach (string name in names.Each())
                        sc.AddTable(name);
                }
            }
        }

        public override void Build()
        {
            TokenList fromTokens = GetTokensWithin();
            for (int i = 0; i < tables.tokens.Count - 1; i++)
            {
                Table leftTable = tables[i] as Table;
                Table rightTable = tables[i + 1] as Table;
                TokenList joinTokens = fromTokens.GetNextTokens(leftTable, new[] { TokenType.Keyword }, null, rightTable.startOffset);
                string joinType = joinTokens.GetNames();
                Token onToken = fromTokens.GetNearestTokenAfter(rightTable, TokenType.Keyword, "on");
                if (joinType.Contains("join") && onToken != null)
                {
                    joinTokens.Add(onToken);
                    TokenList joinExpression = fromTokens.GetNextTokens(onToken,
                        new[] { TokenType.Literal, TokenType.Operator, TokenType.Column }, null, rightExtent);
                    if (joinExpression.Count > 0)
                        leftTable.AddJoin(joinType, rightTable, joinTokens, joinExpression);
                }
            }
        }

        public override void BuildStructure()
        {
            TokenList tokenList = GetTokensWithin(new[] { TokenType.Table, TokenType.Query });
            foreach (Token tTableOrQuery in tokenList.tokens)
            {
                Keyword asToken = null;
                Identifier nameToken = null;
                parentQuery.tokens.GetObjectNameToken(tTableOrQuery, out asToken, out nameToken, this);
                if (tTableOrQuery.tokenType == TokenType.Query)
                {
                    if (nameToken != null)
                        tables.Add(new Table(nameToken, asToken, tTableOrQuery as Query));
                }
                else
                {
                    if (nameToken == null || tables.GetTableForAlias(nameToken.name) == null)
                    {
                        Table table = tTableOrQuery as Table;
                        table.tableAliasToken = nameToken;
                        table.asToken = asToken;
                        tables.Add(table);
                        rootQuery.allTables.AddIfNotExists(table);
                    }
                }
            }
        }
        public override TokenList GetChildren()
        {
            return tables;
        }
    }
}
