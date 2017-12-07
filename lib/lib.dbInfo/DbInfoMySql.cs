using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fp.lib;
using fp.lib.mysql;

namespace fp.lib.dbInfo
{
    public class DbInfoMySql : DbInfo
    {
        public override string databaseType { get { return "MySql"; } }

        public override int SqlInt(string sql)
        {
            return QMySql.SqlInt(sql);
        }

        public override int SqlInt(bool useRawValues, string sql, params object[] args)
        {
            return QMySql.SqlInt(useRawValues, sql, args);
        }

        public override QSqlBase GetSql()
        {
            return new QMySql();
        }
        
        public DbInfoMySql()
        {
        }

        public void Connect(string host, string name, string user, string password)
        {
            QMySql.ConnectString = "Server=" + host + ";Database=" + name + ";Uid=" + user + ";Pwd=" + password + ";Allow User Variables=True;";
            QMySql.SqlInt("select count(*) FROM INFORMATION_SCHEMA.TABLES");
            databaseName = name;
        }

        public override void LoadStructure()
        {
            tables.Clear();

            using (QMySql s = new QMySql())
            {
                s.Open("select table_name, table_type from information_schema.tables where table_type in ('BASE TABLE','VIEW') and table_schema='" + databaseName + "'");
                while (s.GetRow())
                {
                    DbTable t = new DbTable("", s[0], s[1]);
                    tables.Add(t.name, t);
                    using (QMySql sFields = new QMySql())
                    {
                        sFields.Open("describe " + t.name);
                        // List<string> tableFieldNames = new List<string>();
                        while (sFields.GetRow())
                        {
                            DbColumn c = new DbColumn(t, sFields[0], sFields[1],
                                sFields[3] == "PRI",
                                sFields[2] == "YES", sFields[4]);
                            t.columns.Add(c.name, c);
                            tablesByColumnName.Add(c.name, t);
                        }
                    }
                }
            }
        }

        protected override void AnalyzeStructure()
        {
            using (QSqlBase s = GetSql())
            {
                s.Open(@"
select table_name,row_format,table_rows,avg_row_length,data_length,index_length 
from information_schema.tables 
where table_type='BASE TABLE' and table_schema='" + databaseName + "'");
                while (s.GetRow())
                {
                    DbTable dbTable = tables[s["table_name"]];
                    dbTable.tableRows = s.GetInt(2);
                    dbTable.avgRowLength = s.GetInt(3);
                    dbTable.dataLength = s.GetInt(4);
                    dbTable.indexLength = s.GetInt(5);
                    //                    tables.Add(dbTable.name, table);
                }

                s.Open(@"
select c.table_name,c.constraint_name,c.constraint_type,s.non_unique,s.seq_in_index,s.column_name,s.cardinality,s.index_type
from information_schema.table_constraints c
inner join information_schema.statistics s on s.table_name=c.table_name and c.table_schema=s.table_schema
and s.index_name=c.constraint_name
where c.table_schema = '" + databaseName + "' order by c.table_name, s.seq_in_index");
                while (s.GetRow())
                {
                    DbTableConstraint constraint = tables[s["table_name"]].GetOrAddConstraint(s["constraint_name"], s["constraint_type"]);
                    constraint.AddColumn(constraint.dbTable.columns[s["column_name"]], s.GetInt("seq_in_index"), s.GetBool("non_unique"), s.GetInt("cardinality"));
                }

                s.Open(@"
select constraint_name,table_name,column_name,ordinal_position,position_in_unique_constraint,
referenced_table_name,referenced_column_name
from information_schema.KEY_COLUMN_USAGE 
where table_schema= '" + databaseName + "'");

                while (s.GetRow())
                {
                    DbTableConstraint constraint = tables[s["table_name"]].GetOrAddConstraint(s["constraint_name"], "FOREIGN_KEY");
                    DbTable refTable = s["referenced_table_name"] != "" ? tables[s["referenced_table_name"]] : null;
                    if (refTable != null)
                    {
                        constraint.AddReference(s["column_name"], refTable, s["referenced_column_name"], s.GetInt("ordinal_position"),
                          s.GetInt("position_in_unique_constraint"));

                    }
                }
            }
        }

        /*
    public override void FindExplicitRelationships()
    {
        using (QMySql s = new QMySql())
        {
            s.Open(@"
SELECT TABLE_NAME, COLUMN_NAME, REFERENCED_TABLE_NAME, REFERENCED_COLUMN_NAME 
FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE 
WHERE REFERENCED_TABLE_NAME is not null and REFERENCED_COLUMN_NAME is not null and table_schema= '" + databaseName + "'");
            while (s.GetRow())
            {
                relationships.AddRelationship(new Relationship
                {
                    type = RelationshipType.ParentChild,
                    parentTable = tables[s[2]],
                    parentColumn = tables[s[2]].columns[s[3]],
                    childTable = tables[s[0]],
                    childColumn = tables[s[0]].columns[s[1]],
                    inferred = false
                });
            }
        }
    }
        */

    }
}
