using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fp.lib;
using fp.lib.dbInfo;

namespace fp.lib.sqlparser
{
    public interface IVisitor
    {
        void Visit(Token token);
    }

    public interface IParentChildVisitor
    {
        void Visit(Token parent, TokenList children);
    }

    public class DebugVisitor : IVisitor
    {
        public string debugText = "";
        public string queryPositionText = "";
        public void Visit(Token token)
        {
            string line = new string(' ', token.depth + 1) + token.debugText;
            debugText += line.LimitToLength(60) +  "\r\n";

            if(token.tokenType == TokenType.Query)
            {
                line = new string(' ', token.startOffset) + token.expression;
                queryPositionText += line.LimitToLength(80) + "\r\n";
            }
        }
    }
}
