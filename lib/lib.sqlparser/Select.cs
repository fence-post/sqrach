using System;
using System.Collections.Generic;
using fp.lib.dbInfo;

namespace fp.lib.sqlparser
{
    public class Select : Keyword
    {

        protected override TokenType[] terminators { get { return new[] { TokenType.From }; } }
        protected override bool terminateAtEndOfQueryIfNoTerminatorTokensFound { get { return rootQuery.softMode; } }

        public Select(int offset, string expr) : base(TokenType.Select, KeywordType.Primary, offset, expr)
        {
        }

        public override void GetSuggestions(SuggestionList s)
        {
            SuggestionContext sc = s.suggestionContext;
            if (s.enableSuggestKeywords && sc.last != null && ((sc.last.type == "column" && sc.last.hasComma == false) 
                    || (sc.last.type == "identifier" && sc.lastColumn != null && sc.lastColumn.hasComma == false)))
            {
                sc.SuggestPrimaryKeywords();
            }
            else if(s.enableSuggestColumns)
            {
                if (columns.Count > 0 || candidateTables.Count > 0)
                    sc.suggestPrimaryKeywords = true;
                if (s.textEntered.Contains("."))
                {
                    s.includeAliases = s.enableSuggestAliases;
                    string alias = s.textEntered.Substring(0, s.textEntered.IndexOf("."));
                    string table = columns.GetTableNameForTableAlias(alias);
                    if (table == null)
                        table = Db.GetTableNameByAlias(alias, null);
                    if (table != null)
                    {
                        sc.AddColumnsInTable(table, alias);
                    }
                    foreach(string t in Db.GetPossibleTablesForAlias(alias))
                        sc.AddColumnsInTable(t, alias);
                }
                sc.AddColumnsInFromTables();
                sc.AddColumnsInSelectCandidateList();
                sc.AddAllColumns();
            }


        }

        public QDict<DbTable, Token> candidateTables = new QDict<DbTable, Token>(QListSort.Descending);
        public List<Token> strayIdentifiers = new List<Token>();

        public override void BuildStructure()
        {
            SelectColumnParser p = new SelectColumnParser(this, candidateTables);
            p.Parse();
        }
        
        public override TokenList GetChildren()
        {
            return columns;
        }
    }
}
