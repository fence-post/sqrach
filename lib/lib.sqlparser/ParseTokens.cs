using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fp.lib;
using fp.lib.dbInfo;

namespace fp.lib.sqlparser
{
    public class ParseTokens
    {
        public TokenList parsedTokens = new TokenList();
        public TokenType endsWith = TokenType.Any;
        

        private const string numberChars = "01234567890-.";
        private const string numberStartChars = "01234567890-";
        private const string operatorChars = "<>=";
        private const string punctuationChars = ",;";
        protected DbInfo Db { get { return Query.Db; } }

        string sql;
        

        public ParseTokens()
        {
            
        }

        public void Parse(string txt)
        {
            lock(parsedTokens)
            {
                parsedTokens.Clear();
                parsedTokens.useIds = true;
                endsWith = TokenType.Any;
                sql = txt.TrimEnd();
                Parse();
            }
        }

        private void Parse()
        { 
            string debugAt = "";
            string word = "";
            bool inNumber = false;
            bool inString = false;
            bool inIdentifier = false;
            bool inOperator = false;
            int wordStart = -1;
            for (int at = 0; at < sql.Length; at++)
            {
                char c = sql[at];
                debugAt = sql.Substring(at);
                if(inString && c == '\'' && at < sql.Length - 1 && sql[at+1] == '\'' )
                {
                    word += '\'';
                    at++;
                    continue;
                }
                if (c == '\'' && inString)
                {
                    AddParsedToken(new Literal(wordStart, word + c));
                    word = "";
                    inString = false;
                    continue;
                }
                else if (inNumber && !numberChars.Contains(c))
                {
                    AddParsedToken(new Literal(wordStart, word));
                    inNumber = false;
                }
                else if (inOperator && !operatorChars.Contains(c))
                {
                    AddParsedToken(new Operator(wordStart, word));
                    inOperator = false;
                }
                else if (inIdentifier && !IsIdentifierChar(at))
                {
                    if (Query.keywords.Contains(word))
                        AddParsedToken(Keyword.CreateKeyword(wordStart, word));
                    else if (Db.tables.ContainsKey(word))
                        AddParsedToken(new Table(wordStart, word));
                    else
                        AddParsedToken(new Identifier(wordStart, word));
                    inIdentifier = false;
                }
                if (inOperator == false && inIdentifier == false && inString == false && inNumber == false)
                {
                    wordStart = at;
                    word = "";
                    if (numberStartChars.Contains(c))
                        inNumber = true;
                    else if (operatorChars.Contains(c))
                        inOperator = true;
                    else if (IsIdentifierChar(at))
                        inIdentifier = true;
                    else if (c == '\'')
                        inString = true;
                    else if (punctuationChars.Contains(c))
                        AddParsedToken(new Punctuation(at, c));
                }
                if (inOperator || inIdentifier || inString || inNumber)
                    word += c;
            }

            if (inString)
                AddParsedToken(new Literal(wordStart, word));
            else if(inNumber)
                AddParsedToken(new Literal(wordStart, word));
            if (inOperator)
                AddParsedToken(new Operator(wordStart, word));
            if (inIdentifier)
            {
                if (Query.keywords.Contains(word))
                    AddParsedToken(Keyword.CreateKeyword(wordStart, word));
                else if (Db.tables.Keys.Contains(word))
                    AddParsedToken(new Table(wordStart, word));
                else
                    AddParsedToken(new Identifier(wordStart, word));
            }
                
        }

        void AddParsedToken(Token t)
        {
            // t.id = t.tokenType.ToString() + parsedTokens.Count.ToString(); 
            t.charBefore = GetCharAt(t.startOffset - 1);
            t.charAfter = GetCharAt(t.startOffset + t.expressionLength);
            parsedTokens.Add(t);
        }

        protected bool IdentifierCharsOnEdges(int at, string word, string otherExcludedChars = "")
        {
            if (IsIdentifierChar(at - 1) || IsIdentifierChar(at + word.Length))
                return true;
            if (IsCharIn(at - 1, otherExcludedChars) || IsCharIn(at + word.Length, otherExcludedChars))
                return true;
            return false;
        }

        protected bool IsIdentifierChar(int pos)
        {
            return (pos >= 0 && pos < sql.Length) && (char.IsLetterOrDigit(sql[pos]) || sql[pos] == '_');
        }

        protected bool IsCharIn(int pos, string charList)
        {
            return (pos >= 0 && pos < sql.Length) && (charList.Contains(sql[pos]));
        }

        protected bool IsWhitespaceChar(int pos)
        {
            return (pos >= 0 && pos < sql.Length) && char.IsWhiteSpace(sql[pos]);
        }

        protected char GetCharAt(int pos)
        {
            return (pos >= 0 && pos < sql.Length) ? sql[pos] : char.MinValue;
        }


    }
}
