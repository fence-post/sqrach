using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fp.lib;
using fp.lib.mssql;

namespace fp.lib.dbInfo
{
    public class DbInfoMsSql : DbInfo
    {
        public string host;
        public string user;
        public string password;

        public override string databaseType { get { return "MsSql"; } }

        public override QSqlBase GetSql()
        {
            return new QMsSql();
        }

        public override int SqlInt(string sql)
        {
            return QMsSql.SqlInt(sql);
        }

        public override int SqlInt(bool useRawValues, string sql, params object[] args)
        {
            return QMsSql.SqlInt(useRawValues, sql, args);
        }

        public void Connect(string h, string n, string u, string p)
        {
            host = h;
            user = u;
            password = p;
            QMsSql.ConnectString = "Server="+ h + ";Initial Catalog=" + n + ";User Id=" + u + ";Password=" + p + ";";
            QMsSql.SqlInt("select count(*) FROM INFORMATION_SCHEMA.TABLES");
            databaseName = n;
        }
      
        public override void LoadStructure()
        {
            tables.Clear();
     
            using (QMsSql s = new QMsSql())
            {
                s.Open(@"
SELECT TABLE_SCHEMA, TABLE_NAME, TABLE_TYPE 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_CATALOG=@1 order by 1,2", databaseName);
                while (s.GetRow())
                {
                    DbTable t = new DbTable(s[0], s[1], s[2]);
                    tables.Add(t.name, t);
                }
                s.Open(@"
SELECT c.COLUMN_NAME, c.COLUMN_DEFAULT,c.IS_NULLABLE,
c.DATA_TYPE,c.CHARACTER_MAXIMUM_LENGTH, 
(select count(*) from INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE cc
INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc 
	on tc.CONSTRAINT_TYPE='PRIMARY KEY' and tc.TABLE_NAME=cc.TABLE_NAME 
	and tc.CONSTRAINT_NAME=cc.CONSTRAINT_NAME
where cc.TABLE_NAME=c.TABLE_NAME and CC.COLUMN_NAME=c.COLUMN_NAME) as pri,
c.table_schema, c.TABLE_NAME
from 
INFORMATION_SCHEMA.COLUMNS c where c.table_catalog=@1 order by c.ORDINAL_POSITION", databaseName);
                while (s.GetRow())
                {
                    string tableName = T.AppendTo(s[6], s[7], ".");
                    if (tables.ContainsKey(tableName))
                    {
                        // mssql allows views with columns that are multipart identifers - not supported here
                        if (s[0].Contains('.'))
                        {
                            tables.Remove(tableName);
                            continue;
                        }

                        DbTable t = tables[tableName];
                        DbColumn c = new DbColumn(t, s[0], s[3], s.GetInt(4), s.GetInt(5) > 0, s[2] == "YES", s[1]);
                        t.columns.Add(c.name, c);
                        tablesByColumnName.Add(c.name, t);
                    }
                }
            } 
        }

        public override void FindExplicitRelationships()
        {
            /*
            using (QMsSql s = new QMsSql())
            {
                s.Open(@"
SELECT  
     KCU1.CONSTRAINT_NAME AS FK_CONSTRAINT_NAME 
    ,KCU1.TABLE_schema AS FK_TABLE_schema 
    ,KCU1.TABLE_NAME AS FK_TABLE_NAME 
    ,KCU1.COLUMN_NAME AS FK_COLUMN_NAME 
    ,KCU1.ORDINAL_POSITION AS FK_ORDINAL_POSITION 
    ,KCU2.CONSTRAINT_NAME AS REFERENCED_CONSTRAINT_NAME 
    ,KCU2.TABLE_schema AS REFERENCED_TABLE_schema 
    ,KCU2.TABLE_NAME AS REFERENCED_TABLE_NAME 
    ,KCU2.COLUMN_NAME AS REFERENCED_COLUMN_NAME 
    ,KCU2.ORDINAL_POSITION AS REFERENCED_ORDINAL_POSITION 
FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS AS RC 

INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS KCU1 
    ON KCU1.CONSTRAINT_CATALOG = RC.CONSTRAINT_CATALOG  
    AND KCU1.CONSTRAINT_SCHEMA = RC.CONSTRAINT_SCHEMA 
    AND KCU1.CONSTRAINT_NAME = RC.CONSTRAINT_NAME 

INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS KCU2 
    ON KCU2.CONSTRAINT_CATALOG = RC.UNIQUE_CONSTRAINT_CATALOG  
    AND KCU2.CONSTRAINT_SCHEMA = RC.UNIQUE_CONSTRAINT_SCHEMA 
    AND KCU2.CONSTRAINT_NAME = RC.UNIQUE_CONSTRAINT_NAME 
    AND KCU2.ORDINAL_POSITION = KCU1.ORDINAL_POSITION 
where rc.constraint_catalog=@1
", databaseName);
                while (s.GetRow())
                {
                    string table = T.AppendTo(s[1], s[2], ".");
                    string referencedTable = T.AppendTo(s[6], s[7], ".");
                    if(tables.ContainsKey(table) && tables.ContainsKey(referencedTable))
                    {
                        relationships.AddRelationship(new Relationship
                        {
                            type = RelationshipType.ParentChild,
                            parentTable = tables[referencedTable],
                            parentColumn = tables[referencedTable].columns[s[8]],
                            childTable = tables[table],
                            childColumn = tables[table].columns[s[3]],
                            inferred = false
                        });
                    }
                }
            }
            */
        }
    }
}
