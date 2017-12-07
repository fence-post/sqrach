using fp.lib.dbInfo;

namespace fp.lib.sqlparser
{
    public class Table : Token
    {
        public Identifier tableAliasToken = null;
        public string tableAlias { get { return tableAliasToken == null ? null : tableAliasToken.name; } }
        public string tableName { get { return dbTable == null ? null : dbTable.name; } }
        public Query subquery = null;
        public Keyword asToken = null;
        public TokenList joinKeywords = new TokenList();
        public DbTable dbTable;
        public string joinType;
        public Table joinTable;
        public Expression joinExpression;

        public Table(int offset, string tableName) : base(TokenType.Table, offset, tableName)
        {
            dbTable = Db.tables[tableName];
        }

        public Table(Identifier alias, Keyword a, Query q) : base(TokenType.Table, q.startOffset, q.expression)
        {
            tableAliasToken = alias;
            asToken = a;
            subquery = q;
            q.parentToken = this;
        }

        public void AddJoin(string typ, Table table, TokenList keywords, TokenList expr)
        {
            joinType = typ;
            joinTable = table;
            joinKeywords.AddRange(keywords);
            joinExpression = new Expression(expr.leftExtent, this, expr);
        }

        protected override string GetDebugText()
        {
            return tokenType.ToString() + " " + T.AppendTo(name, tableAlias, " ");
        }
        
        public override TokenList GetChildren()
        {
            TokenList children = new TokenList(asToken);
            children.Add(subquery);
            children.AddRange(joinKeywords);
            children.Add(joinExpression);
            return children;
        }
    }
}
