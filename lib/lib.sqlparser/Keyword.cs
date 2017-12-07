using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using fp.lib.dbInfo;

namespace fp.lib.sqlparser
{
    public enum KeywordType { Primary, Secondary, Logic, DataType }

    public class Keyword : Token
    {      
        public static Keyword CreateKeyword(int offset, string word)
        {
            Keyword k = null;
            if (word == "select")
                k = new Select(offset, word);
            if (word == "from")
                k = new From(offset, word);
            if (word == "where")
                k = new Where(offset, word);
            if (word == "order")
                k = new OrderBy(offset, word);
            if (word == "group")
                k = new GroupBy(offset, word);
            if (word == "limit")
                k = new Limit(offset, word);
            else if (secondaryKeywords.Contains(word))
                k = new Keyword(offset, KeywordType.Secondary, word);
            else if (logicKeywords.Contains(word))
                k = new Keyword(offset, KeywordType.Logic, word);
            else if (datatypeKeywords.Contains(word))
                k = new Keyword(offset, KeywordType.DataType, word);

            return k;
        }

        public KeywordType keywordType;
        public ColumnList columns = new ColumnList();

        public Keyword(int offset, KeywordType typ, string expr) : base(TokenType.Keyword, offset, expr)
        {
            keywordType = typ;         
        }

        public Keyword(TokenType typ, KeywordType ktyp, int offset, string expr) : base(typ, offset, expr)
        {
            keywordType = ktyp;
        }

        public override string shortName
        {
            get
            {
                return isPrimaryKeyword ? tokenType.ToString().ToLower() : name;
            }

        }

        protected void ClaimColumns(ColumnList list)
        {
            foreach (Column c in GetTokensWithin(new[] { TokenType.Column }).tokens)
                list.Add(c);
        }    
    }

   
    public class OrderBy : Keyword
    {
        Keyword by = null;


        protected override TokenType[] terminators { get { return new[] { TokenType.Limit }; } }
        protected override bool terminateAtEndOfQueryIfNoTerminatorTokensFound { get { return true; } }

        public OrderBy(int offset, string expr) : base(TokenType.Order, KeywordType.Primary, offset, expr)
        {
           //  terminateAtEndOfQueryIfNoTerminatorTokensFound = true;
           //  terminators = new[] { TokenType.Limit };
        }

        public override void GetSuggestions(SuggestionList s)
        {
            SuggestionContext sc = s.suggestionContext;

            if(s.enableSuggestColumns)
            {
                if (parentQuery.select != null)
                    sc.AddColumnsInSelectList();
                if (s.textEntered.Length > 0)
                    sc.AddColumnsInFromTables();
            }

            if (columns.Count > 0 && s.enableSuggestKeywords)
            {
                if(sc.last.hasComma == false)
                {
                    s.AddKeyword("desc");
                    s.AddKeyword("asc");
                }
                    
                sc.SuggestPrimaryKeywords();
            }
        }

        public override void Build()
        {
            // list of columns
            // each column can be a select statement -- that is handled within the column object
            // numbers point to column index
            // only available in root query
            // each column can have asc or desc keywords

            ClaimColumns(columns);
            by = GetNextToken(TokenType.Keyword, this, "by") as Keyword;
        }

        public override TokenList GetChildren()
        {
            TokenList result = new TokenList(by);
            result.AddRange(columns);
            return result;
        }
    }

    public class GroupBy : Keyword
    {
        public Keyword by = null;


        protected override TokenType[] terminators { get { return new[] { TokenType.Order, TokenType.Limit }; } }
        protected override bool terminateAtEndOfQueryIfNoTerminatorTokensFound { get { return true; } }

        public GroupBy(int offset, string expr) : base(TokenType.Group, KeywordType.Primary, offset, expr)
        {
          //   terminateAtEndOfQueryIfNoTerminatorTokensFound = true;
           //  terminators = new[] { TokenType.Order, TokenType.Limit };
        }
        
        public override void Build()
        {
            ClaimColumns(columns);
            by = GetNextToken(TokenType.Keyword, this, "by") as Keyword;
        }

        public override TokenList GetChildren()
        {
            TokenList result = new TokenList(by);
            result.AddRange(columns);
            return result;
        }
    }

    public class Limit : Keyword
    {
        public Literal limitValue;

        protected override bool terminateAtEndOfQueryIfNoTerminatorTokensFound { get { return true; } }

        public Limit(int offset, string expr) : base(TokenType.Limit, KeywordType.Primary, offset, expr)
        {
        //     terminateAtEndOfQueryIfNoTerminatorTokensFound = true;
        }
       
        public override void Build()
        {
            // requires one integer literal
            parentQuery.limit = this;
            limitValue = GetNextToken(TokenType.Literal, this) as Literal;
        }

        public override TokenList GetChildren()
        {
            return new TokenList(limitValue);
        }
    }
}

/*
public class InPredicate : Keyword
{
    public Expression inExpression = null;
    public Query subQuery = null;

    public InPredicate(int offset, string expr) : base(TokenType.InPredicate, KeywordType.Secondary, offset, expr)
    {

    }

    public override void Build()
    {
        Token t = parentQuery.tokens.GetTokenAfter(this, TokenType.Any);
        if (t.tokenType == TokenType.Query)
            subQuery = t as Query;
        else if (t.tokenType == TokenType.Expression)
            inExpression = t as Expression;

    }

    public override TokenList GetChildren()
    {
        TokenList result = new TokenList(subQuery);
        result.Add(inExpression);
        return result;
    }
}
*/
