using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fp.lib.dbInfo;

namespace fp.lib.sqlparser
{
    public class SqlBuilder
    {
        QDict<int, string> inserts = new QDict<int, string>();
        QDict<int, Token> deletes = new QDict<int, Token>();
        public Query query; 
        public SqlBuilder(Query q)
        {
            query = q;
        }

        public string Render()
        {
            string result = "";
            for(int i = 0; i <= Query.rootQuery.expressionLength; i++)
            {
                int max = 0;
                foreach (Token t in deletes.Each(i))
                    max = Math.Max(max, t.rightExtent + (t.charAfter == ',' ? 1 : 0));
                if (max > i)
                    i = max;
                foreach (String insert in inserts.Each(i))
                    result += insert;
                if(i < Query.rootQuery.expression.Length)
                    result += Query.rootQuery.expression[i];
            }

            return result.Trim();
        }

        public void Delete(Token t)
        {
            deletes.Add(t.startOffset, t);
            foreach (Token child in t.children)
                Delete(child);
        }

        public void InsertBefore(Token t, string sql)
        {
            int at = t.tokenType == TokenType.Query ? t.startOffset - 1 : t.startOffset;
            inserts.Add(at, sql);

        }

        public void AddColumn(Keyword k, DbColumn c, bool includeAlias)
        {
            int insertPos = -1;
            string sql = "";
            if(insertPos == -1)
            {
                if (k.tokenType != TokenType.Select && k.startOffset == 0)
                {
                    sql = " " + k.shortName.ToLower();
                    if (k.tokenType == TokenType.Group || k.tokenType == TokenType.Order)
                        sql += " by";
                    sql += " ";
                    insertPos = k.GetInsertOffset();
                }
                else
                {
                    insertPos = k.rightExtent;
                }
            }

            string alias = AddTable(c.table, includeAlias);
            sql += T.AppendTo(alias, c.name, ".");
            if(k.tokenType == TokenType.Where)
            {
                sql = sql + "=" + QObject.GetZeroSqlValueForType(c.dataType);
                if (k.columns.Count > 0)
                    sql = " and " + sql;
            }
            else 
            {
                if (k.columns.Count > 0)
                {
                    sql = ", " + sql;
                   // insertPos--;
                }
                    
            }
            while (Char.IsWhiteSpace(Query.rootQuery.expression[insertPos - 1]))
                insertPos--;
            inserts.Add(insertPos, sql);
        }

        public string AddTable(DbTable t, bool includeAlias)
        {
            string alias = "";
            if(includeAlias)
                alias = t.GetAlias(true);
            int insertPos = 0;
            if (query.from == null)
            {
                Keyword k = Keyword.CreateKeyword(0, "from");
                k.parentQuery = query;
                insertPos = k.GetInsertOffset();
                inserts.Add(insertPos, ("from " + t.name + " " + alias).Trim());
            }
            else
            {
                if (query.from.tables.GetTableByName(t.name) == null)
                {
                    insertPos = query.from.rightExtent;
                    while (Char.IsWhiteSpace(Query.rootQuery.expression[insertPos - 1]))
                        insertPos--;
                    string bestJoin = null;
                    foreach (Table table in query.from.tables.tokens)
                    {
                        string join = table.dbTable.RenderJoin(t.name, includeAlias);
                        if (join != "" && (bestJoin == null || bestJoin.CountOccurrances('.') > join.CountOccurrances('.')))
                            bestJoin = join;
                    }
                    if (bestJoin != null)
                        inserts.Add(insertPos, bestJoin);
                    
                }
            }

            return alias;
        }
        
        public void InsertAfter(Token t, string sql)
        {
            inserts.Add(t.rightExtent, sql);

        }

        public void Append(Token t, string sql)
        {
            inserts.Add(t.rightExtent, sql);

        }

        public string Format()
        {
            FormatVisitor v = new FormatVisitor(this);
            query.Accept(v);
            string sql = Render();
            string[] lines = sql.Split('\n');
            sql = "";
            foreach(string line in lines)
            {
                if (line.Trim() != "")
                    sql += line + '\n';
            }
            return sql;
        }

        public class FormatVisitor : IVisitor
        {
            SqlBuilder builder;
            
            public FormatVisitor(SqlBuilder b)
            {
                builder = b;
            }

            string ExpressionAt(Token t)
            {
                return Query.rootQuery.expression.Substring(t.startOffset);
            }

            public void Visit(Token token)
            {
                if (token.tokenType == TokenType.Query && token != Query.rootQuery && token.parentToken != null)
                {
                    builder.InsertBefore(token, "\n" + new string(' ', token.depth - 2));
                    builder.Append(token, "\n" + new string(' ', token.depth - 2));
                }
                else if(token.isPrimaryKeyword)
                    builder.InsertBefore(token, "\n" + new string(' ', token.depth - 1));
                else if (token.tokenType == TokenType.Keyword && ExpressionAt(token).StartsWith("inner join"))
                    builder.InsertBefore(token, "\n" + new string(' ', token.depth - 1));
            }
        }
    }
}
