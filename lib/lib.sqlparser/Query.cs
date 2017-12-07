
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using fp.lib.dbInfo;

namespace fp.lib.sqlparser
{
    [DebuggerDisplay("{tokenType} {startOffset} {rightExtent} {expression}")]
    [DebuggerTypeProxy(typeof(QueryDebugView))]
    public partial class Query : Token
    {
        internal class QueryDebugView
        {
            public List<Token> tokens { get { return query.tokens.tokens.ToList(); } }
            public List<Query> queries { get { return query.queries; } }
            public Select select { get { return query.select; } }
            public From from { get { return query.from; } }
            public Where where { get { return query.where; } }
            private Query query;
            public QueryDebugView(Query token)
            {
                this.query = token;
            }
        }

        public static HashSet<DbColumn> columnHints = new HashSet<DbColumn>();

        public List<string> errors = new List<string>();

        public Select select;
        public From from;
        public Where where;
        public GroupBy group;
        public OrderBy order;
        public Limit limit;

        public TableList allTables = null;
        public ColumnList allColumns = null;
        public TokenList tokens = new TokenList();

        public Query(int offset, string expr) : base(TokenType.Query, offset, expr)
        {
        }

       
        public Query GetQueryAtOffset(int offset)
        {
            foreach (Query q in queries)
            {
                Query result = q.GetQueryAtOffset(offset);
                if (result != null)
                    return result;
            }
            if (offset >= startOffset && offset <= rightExtent)
                return this;
            return null;
        }

        public override TokenList GetChildren()
        {
            TokenList children = new TokenList();

            children.Add(select);
            children.Add(from);
            children.Add(where);
            children.Add(group);
            children.Add(order);
            children.Add(limit);

            if (children.Count == 0)
                foreach (Query q in queries)
                    children.Add(q);


            return children;
        }

        public class TokenListVisitor : IVisitor
        {
            public TokenList tokens = null;
            public TokenType[] types = null;
            public TokenListVisitor(TokenType t, TokenList list = null)
            {
                tokens = list == null ? new TokenList() : list;
                types = new[] { t };
            }

            public void Visit(Token token)
            {
                if (types.Contains(token.tokenType))
                    tokens.Add(token);
            }
        }

        public Column GetSelectColumn(string columnName, string tableAlias = null)
        {
            foreach (Column c in select.columns.tokens)
                if ((tableAlias == null || c.tableAlias == tableAlias) && c.columnName == columnName)
                    return c;
            return null;
        }

        public ColumnList GetAllColumns()
        {
            ColumnList list = new ColumnList();
            TokenListVisitor v = new TokenListVisitor(TokenType.Column, list);
            Accept(v);
            return list;
        }

        public TokenList GetAllTokens(TokenType t)
        {
            TokenListVisitor v = new TokenListVisitor(t);
            Accept(v);
            return v.tokens;
        }
    }
}
