using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fp.lib.dbInfo;

namespace fp.lib.sqlparser
{
    public class Where : Keyword
    {
        public Expression whereExpression;

        protected override TokenType[] terminators { get { return new[] { TokenType.Group, TokenType.Order, TokenType.Limit }; } }
        protected override bool terminateAtEndOfQueryIfNoTerminatorTokensFound { get { return true; } }

        public Where(int offset, string expr) : base(TokenType.Where, KeywordType.Primary, offset, expr)
        {
            // terminateAtEndOfQueryIfNoTerminatorTokensFound = true;
            // terminators = new[] { TokenType.Group, TokenType.Order, TokenType.Limit };
        }

        public override void GetSuggestions(SuggestionList s)
        {
            SuggestionContext sc = s.suggestionContext;

            if (sc.last == null)
            {
                sc.suggestColumns = true;
            }
            else
            {
                if (sc.last.type.IsOneOf("conjunction"))
                {
                    sc.suggestColumns = true;
                }
                else if (sc.last.type == "operator")
                {
                    sc.suggestColumns = true;
                    if (sc.lastColumn.token != null)
                    {
                        Column c = sc.lastColumn.token as Column;
                        sc.suggestColumnsDataType = c.dbColumn.dataType;
                    }
                }
                else if (sc.last.type.IsOneOf("column", "literal"))
                {
                    sc.suggestOperators = sc.lastOperator.pos < sc.lastConjunction.pos;
                    sc.suggestPrimaryKeywords = sc.suggestConjunctions = sc.lastOperator.pos == 0 || sc.lastOperator.pos > sc.lastConjunction.pos;
                }
                else
                {
                    sc.suggestColumns = true;
                }
            }
            
            if (sc.suggestColumns && s.enableSuggestColumns)
            {
                sc.AddAllColumnsInQuery();
                sc.AddColumnsInFromTables();
            }
            if (s.enableSuggestKeywords)
            {
                if (sc.suggestConjunctions)
                    sc.SuggestConjunctions();
                if (sc.suggestOperators)
                    sc.SuggestOperators();
                if (sc.suggestPrimaryKeywords)
                    sc.SuggestPrimaryKeywords();
            }
        }

        public override void Build()
        {
            ClaimColumns(columns);
            TokenList tokens = GetTokensWithin();
            if (tokens.Count > 0)
                whereExpression = new Expression(startOffset + 5, this, tokens);
        }

        public override TokenList GetChildren()
        {
            return new TokenList(whereExpression);
        }
    }

}
