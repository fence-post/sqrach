using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fp.lib.dbInfo;

namespace fp.lib.sqlparser
{
    public class ColumnParts
    {
        public int start;
        public int end;
        public Identifier t;
        public DbTable dbTable = null;
        public Table table = null;
        public Identifier tTableAlias;
        public Keyword tAs;
        public Identifier tColumnAlias;
        public string alias = null;

        public ColumnParts(int s, int e)
        {
            start = s;
            end = e;
        }
    }

    public class SelectColumnParser
    {
        Select select;
        TokenList tokens;
        QDict<DbTable, Token> candidateTables;
        List<ColumnParts> strays = new List<ColumnParts>();

        public SelectColumnParser(Select s, QDict<DbTable, Token> c)
        {
            select = s;
            candidateTables = c;
            tokens = select.GetTokensWithin(null);
        }
        
        public void Parse()
        {
            ParseQueries();

            TokenList commas = tokens.GetTokensWithin(select, new[] { TokenType.Punctuation }, ",");
            //if (commas.Count < 1)
            //    return;
            int start = select.expression.IndexOf("select ") + 7;
            for(int i = 0; i <= commas.Count; i++)
            {
                int end = i >= commas.Count ? select.rightExtent : commas[i].startOffset;
                Parse(new ColumnParts(start, end));
                start = end + 1;
            }
     
            HandleStrays();
        }

        public void Parse(ColumnParts parts)
        {
            TokenList elements = tokens.GetTokensWithin(select, parts.start, parts.end);
            if (elements.Count <= 0 || elements.Count > 4)
                return;
            else if (elements.Count == 1)
            {
                parts.t = elements[0] as Identifier;
            }
            else if (elements.Count == 2)
            {
                if(elements[0].charAfter == '.' && elements[1].charBefore == '.')
                {
                    parts.tTableAlias = elements[0] as Identifier;
                    parts.t = elements[1] as Identifier;
                }
                else
                    parts.t = elements[0] as Identifier;
            }
            else if (elements.Count == 3)
            {
                parts.tTableAlias = elements[0] as Identifier;
                parts.t = elements[1] as Identifier;
                parts.tColumnAlias = elements[2] as Identifier;
            }
            else if (elements.Count == 4)
            {
                parts.tTableAlias = elements[0] as Identifier;
                parts.t = elements[1] as Identifier;
                parts.tAs = elements[2] as Keyword;
                parts.tColumnAlias = elements[3] as Identifier;
            }
            else
            {
                throw new ArgumentException();
            }
            if (parts.t == null)
                return;

            if(elements.Count == 1)
            {
                if(parts.t.charAfter == '.')
                {
                    if(Query.rootQuery.expression.Substring(parts.t.rightExtent).StartsWith(".*"))
                    {
                        Table t = select.parentQuery.from.tables.GetTableForAlias(parts.t.name);
                        if(t != null)
                        {
                            AddColumn(new Column(parts.t, t));
                            return;
                        }
                    }
                }
            }

            
            if (parts.tTableAlias != null && parts.tTableAlias.charAfter == '.' && parts.t.charBefore == '.' && select.parentQuery.from != null)
                parts.alias = parts.tTableAlias.name;
            if (parts.alias != null && select.parentQuery.from != null)
                parts.table = select.parentQuery.from.tables.GetTableForAlias(parts.alias, parts.t.name);
            if (parts.table == null && parts.t != null && select.parentQuery.from != null)
                parts.table = select.parentQuery.from.tables.GetTableForColumnName(parts.t.name, parts.alias);
            if (parts.table == null)
                parts.dbTable = GetDbTable(parts);

            if (parts.table != null)
                AddColumn(new Column(parts.tAs, parts.tColumnAlias, parts.t, parts.tTableAlias, parts.table));
            else if (parts.dbTable != null)
                AddColumn(new Column(parts.tAs, parts.tColumnAlias, parts.t, parts.tTableAlias, parts.dbTable, parts.alias));
            else
                strays.Add(parts);
        }

        DbTable GetDbTable(ColumnParts parts)
        {
            foreach (DbColumn c in Query.columnHints)
            {
                if (c.name == parts.t.name)
                {
                    if (parts.alias == null)
                        return c.table;
                    if (c.table.aliases.ContainsKey(parts.alias))
                        return c.table;
                    candidateTables.AddIfNotExists(c.table, parts.t);
                }
            }

            DbTable dbTable = null;
            int ct = Query.Db.tablesByColumnName.ValueCount(parts.t.name);
            if (ct == 1)
                dbTable = Query.Db.tablesByColumnName[parts.t.name];
            foreach (DbTable tmp in Query.Db.tablesByColumnName.Each(parts.t.name))
                candidateTables.AddIfNotExists(tmp, parts.t);
            if (dbTable == null && parts.alias == null)
            {
                foreach (DbColumn c in Query.Db.activeColumns)
                    if (c.name == parts.t.name)
                        return c.table;
            }

            if (dbTable == null && parts.alias != null)
            {
                ct = Query.Db.tablesByAlias.ValueCount(parts.alias);
                if (ct == 1 && Query.Db.tablesByAlias[parts.alias].columns.ContainsKey(parts.t.name))
                    dbTable = Query.Db.tablesByAlias[parts.alias];
                foreach (DbTable tmp in Query.Db.tablesByAlias.Each(parts.alias))
                    if (tmp.columns.ContainsKey(parts.t.name))
                        candidateTables.AddIfNotExists(tmp, parts.t);
            }
            /*
            if(dbTable == null)
            {
                ct = 0;
                match = null;
                foreach(DbTable t in Query.Db.activeTables)
                {
                    if (t.columns.ContainsKey(parts.t.name))
                    {
                        match = t.columns[parts.t.name];
                        ct++;
                    }
                }
                if(ct == 1)
                    dbTable = match.table;
            }
            */

            return dbTable;

        }

        void HandleStrays()
        {
            if (strays.Count > 0)
            {

                foreach (DbTable table in candidateTables.GetEnumerator(null, QListSort.None))
                    candidateTables.SetPosition(table, candidateTables.ValueCount(table));
                foreach (ColumnParts parts in strays)
                {
                    parts.dbTable = candidateTables.FindFirstKeyWithValue(parts.t, null);
                    if (parts.dbTable != null)
                    {
                        AddColumn(new Column(parts.tAs, parts.tColumnAlias, parts.t, parts.tTableAlias, parts.dbTable, parts.alias));
                    }
                }
            }
        }

        void AddColumn(Column c)
        {
            Token t = select.parentQuery.tokens.GetTokenAtOffset(c.startOffset);
            
            if (t != null && t.tokenType == TokenType.Identifier)
            {
                c.parentQuery = select.parentQuery;
                c.parentToken = select;
                select.parentQuery.select.columns.Add(c);
                select.parentQuery.tokens.RemoveAt(c.startOffset);
                select.parentQuery.tokens.Add(c);
                Query.rootQuery.allColumns.AddIfNotExists(c);
            }
        }

        void ParseQueries()
        {
            TokenList tokenList = select.GetTokensWithin(new[] { TokenType.Query });
            foreach (Token token in tokenList.tokens)
            {
                Keyword asToken = null;
                Identifier nameToken = null;
                select.parentQuery.tokens.GetObjectNameToken(token, out asToken, out nameToken, select);
                if (nameToken != null)
                    select.columns.Add(new Column(nameToken, asToken, token as Query));
            }
        }
    }
}
