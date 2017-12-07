using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using fp.lib.dbInfo;

namespace fp.lib.sqlparser
{
    public enum TokenType { Any, Query, Operator, Literal, Identifier, Expression, InPredicate, Punctuation, Column, Table, Keyword, Select, From, Where, Order, Group, Limit };
    public enum TokenStatus { New, Warning, Error, Built, Added, Removed, Moved }

    [DebuggerDisplay("{tokenType} {startOffset} {rightExtent} {expression}")]
    [DebuggerTypeProxy(typeof(TokenDebugView))]
    public class Token
    {
        internal class TokenDebugView
        {
            public Query parentQuery { get { return token.parentQuery; } }
            private Token token;
            public TokenDebugView(Token token)
            {
                this.token = token;
            }
        }

        public static DbInfo Db;

     
        public static HashSet<string> operators = new HashSet<string>();
        public static HashSet<string> keywords = new HashSet<string>();
        public static HashSet<string> secondaryKeywords = new HashSet<string>();
        public static HashSet<string> datatypeKeywords = new HashSet<string>();
        public static HashSet<string> logicKeywords = new HashSet<string>();
        public static HashSet<string> punctuation = new HashSet<string>();

        public static void InitializeForParsing()
        {
            keywords.Clear();
            keywords.AddRange(DbInfo.PrimaryKeywords.Split(' '));
            keywords.AddRange(DbInfo.SecondaryKeywords.Split(' '));
            keywords.AddRange(DbInfo.LogicKeywords.Split(' '));
            keywords.AddRange(DbInfo.DatatypeKeywords.Split(' '));
            secondaryKeywords.Clear();
            secondaryKeywords.AddRange(DbInfo.SecondaryKeywords.Split(' '));
            logicKeywords.Clear();
            logicKeywords.AddRange(DbInfo.LogicKeywords.Split(' '));
            datatypeKeywords.Clear();
            datatypeKeywords.AddRange(DbInfo.DatatypeKeywords.Split(' '));
            operators.Clear();
            operators.AddRange(DbInfo.Operators.Split(' '));
            punctuation.Clear();
            punctuation.AddRange(DbInfo.Punctuation.Split(' '));

        }

        public class SetParentVisitor : IParentChildVisitor
        {
            public void Visit(Token parent, TokenList children)
            {
                foreach (Token t in children.tokens)
                    t.parentToken = parent;
            }
        }

        public int GetLine()
        {
            int lines = 0;
            for(int at = 0; at < startOffset; at++)
            {
                at = Query.rootQuery.expression.IndexOf('\n', at);
                if (at < 0)
                    break;
                if (at >= 0 && at < startOffset)
                    lines++;
            }

            return lines + 1;
        }


        public bool isPrimaryKeyword
        {
            get
            {
                Keyword k = this as Keyword;
                return (k != null) ? k.keywordType == KeywordType.Primary : false;
            }
        }

        public virtual void GetSuggestions(SuggestionList result)
        {
            
        }

            public Keyword GetParentPrimaryKeyword()
        {
            Keyword k = null;
            if (tokenType == TokenType.Select || tokenType == TokenType.From || tokenType == TokenType.Where || tokenType == TokenType.Group || tokenType == TokenType.Order || tokenType == TokenType.Limit)
                return this as Keyword;
            if(parentToken == null)
            {
                T.Debug("parentToken null for " + debugText);
            }
            k = null;
            if(parentQuery != null)
                k = parentQuery.tokens.GetNearestTokenBefore(this,
                                new[] { TokenType.Select, TokenType.From, TokenType.Where, TokenType.Group,
                                TokenType.Order, TokenType.Limit }) as Keyword;
            return k;
        }

        public bool TokenWithin(Token t, bool includeParent = false)
        {
            return (includeParent || t != this) &&  t.startOffset >= startOffset && t.startOffset + t.expressionLength <= rightExtent;
            
        }

        public TokenList GetTokensWithin(TokenType[] types = null)
        {
            return parentQuery.tokens.GetTokensWithin(this, types);
        }

        public Token GetPrevToken(TokenType type = TokenType.Any, Token within = null, string name = null)
        {
            return parentQuery.tokens.GetPrevToken(this, type, within, name);
        }

        public Token GetNextToken(TokenType type = TokenType.Any, Token within = null, string name = null)
        {
            return parentQuery.tokens.GetNextToken(this, type, within, name);
        }
     
        /*
        protected virtual string GetId()
        {
            string result = "";
            result = result.AppendTo(tokenType.ToString(), ".");
            result = result.AppendTo(name.GetHashCode().ToString(), ".");

            return result;
        }

        private string _id = "";
        
        public string id
        {
            set { _id = value;  }
            get {
                if(_id == "")
                {
                    T.Assert(false);
                    string result = "";
                    if (parentToken != null)
                        result = result.AppendTo(parentToken.GetId(), ".");
                    else if (parentQuery != null)
                        result = result.AppendTo(parentQuery.GetId(), ".");
                    result = result.AppendTo(GetId(), ".");
                    _id = result;
                }              

                return _id;
            }
        }
        */
        public TokenStatus status = TokenStatus.New;
        private string _expression;
        public string expression { get { return _expression; } }
        protected void SetExpression(string s)
        {
            _expression = s;
        }
        protected TokenList parsedTokens = new TokenList();
        public Token parentToken = null;
        public Query parentQuery = null;

        public string id
        {
            get { return "t" + startOffset + tokenType.ToString(); }
        }
        public TokenType tokenType;
        public int startOffset;
        public int expressionLength { get { return expression.Length; } }
        public int rightExtent { get { return startOffset + expressionLength; } }
        public bool isRootQuery { get { return parentQuery == null; } }
        public string name { get { return expression; } }
        public static Query rootQuery;
        public char charBefore;
        public char charAfter;

        

        public Token(TokenType t, int offset, string expr)
        {
            tokenType = t;
            startOffset = offset;
            _expression = expr;
        }

        public virtual string shortName {
        get
            {
                return name;
            }

        }

        protected virtual TokenType[] terminators { get { return null; } }
        protected virtual bool terminateAtEndOfQueryIfNoTerminatorTokensFound { get { return false; } }

        public int GetInsertOffset()
        {
            T.Assert(tokenType != TokenType.Select);
            if (terminators != null && parentQuery.select != null)
            {
                Token terminator = parentQuery.tokens.GetNearestTokenAfter(parentQuery.select, terminators);
                if (terminator != null)
                    return terminator.startOffset;
                if (terminateAtEndOfQueryIfNoTerminatorTokensFound)
                    return parentQuery.rightExtent;
            }

            return -1;

        }

        public void UpdateLength()
        {
            int len = expressionLength;
            if(terminators != null)
            {
                Token terminator = parentQuery.tokens.GetNearestTokenAfter(this, terminators);
                if (terminator != null)
                    len = terminator.startOffset - startOffset;
                else if (terminateAtEndOfQueryIfNoTerminatorTokensFound)
                    len = parentQuery.rightExtent - (startOffset); 
            }
            len = Math.Min(len, rootQuery.expressionLength - startOffset);
            _expression = rootQuery.expression.Substring(startOffset, len);
            
        }

        public TableList GetTablesInScope(bool includeparents)
        {
            TableList list = new TableList();
            if(tokenType == TokenType.Query)
            {
                Query q = this as Query;
                if (q.from != null)
                    list.AddTables(q.from.tables);
            }

            if (includeparents && parentQuery != null)
                list.AddTables(parentQuery.GetTablesInScope(includeparents));

            return list;
        }

        public virtual void Build()
        {

        }

        public virtual void BuildStructure()
        {

        }

        public bool AddError(string msg, TokenStatus stat = TokenStatus.Warning)
        {
            if (stat == TokenStatus.Error)
                status = TokenStatus.Error;
            else if (status != TokenStatus.Error && stat == TokenStatus.Warning)
                status = TokenStatus.Warning;
            rootQuery.errors.Add(msg);
            rootQuery.status = status;

            return status == TokenStatus.Error;
        }

        public int depth
        {
            get
            {
                if (parentToken != null)
                    return parentToken.depth + 1;
                else return 0;
            }
        }

        public string debugText
        {
            get
            {
                string text = GetDebugText();
                text = text.Replace("\r\n", " ");
                text = text.Replace("\n", " ");
                text = text.Replace("\r", " ");
                text = text.Replace("\t", " ");
                text = text.Replace("  ", " ");
                return text.Trim();
                
            }
        }

        protected virtual string GetDebugText()
        {
            return tokenType.ToString() + " " + expression;
        }

      

        public virtual List<Token> children
        {
            get
            {
                TokenList tokenList = GetChildren();
                return tokenList.tokens.ToList();
            }
             
        }

        public virtual TokenList GetChildren()
        {
            return new TokenList();
        }

        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
            foreach (Token t in children)
                t.Accept(visitor);
        }

        public void Accept(IParentChildVisitor visitor)
        {
            visitor.Visit(this, GetChildren());
            foreach (Token t in children)
                t.Accept(visitor);
        }
    }

    public class Punctuation : Token
    {
        public Punctuation(int offset, char c) : base(TokenType.Punctuation, offset, c.ToString())
        {

        }
    }

    public class Identifier : Token
    {
        public Identifier(int offset, string expr) : base(TokenType.Identifier, offset, expr)
        {

        }
    }

    public class Literal : Token
    {
        public Literal(int offset, string expr) : base(TokenType.Literal, offset, expr)
        {
            
        }
    }

    public class Operator : Token
    {
        public Operator(int offset, string expr) : base(TokenType.Operator, offset, expr)
        {

        }
    }
}   
