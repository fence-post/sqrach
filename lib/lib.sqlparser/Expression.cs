using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using fp.lib.dbInfo;

namespace fp.lib.sqlparser
{
    public class Expression : Token
    {
        TokenList tokens = new TokenList();
        public Expression(int offset, string expr) : base(TokenType.Expression, offset, expr)
        {
            
        }

        public Expression(int offset, Token parent, TokenList tok) : base(TokenType.Expression, offset, "")
        {
            parentQuery = parent.parentQuery;
            parentToken = parent;
            tokens.AddRange(tok);
            SetExpression(rootQuery.expression.Substring(tokens.leftExtent, tokens.rightExtent - (tokens.leftExtent)));
        }

        public override TokenList GetChildren()
        {
            return tokens;
        }


    }
}
