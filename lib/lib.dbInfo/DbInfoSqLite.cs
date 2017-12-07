using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fp.lib;
using fp.lib.sqlite;

namespace fp.lib.dbInfo
{
    public class DbInfoSqLite : DbInfo
    {
        string file;

        public override string databaseType { get { return "SqLite"; } }

        public DbInfoSqLite()
        {
        }

        public override int SqlInt(string sql)
        {
            return QSqlLite.SqlInt(sql);
        }

        public override QSqlBase GetSql()
        {
            return new QSqlLite(file);
        }

        public void Connect(string filePath)
        {
            using (QSqlLite s = new QSqlLite(filePath))
            {
                s.Open("SELECT name FROM sqlite_master WHERE type='table'");
                s.GetRow();
            }
            file = filePath;
            databaseName = T.GetFileNameFromFilePath(filePath, true);
        }

        public override void AnalyzeDatabaseStructure()
        {

        }

        public override bool AnalyzeDatabaseData(out string msg)
        {
            msg = null;
            return false;
        }

        public override void LoadStructure()
        {
            tables.Clear();

            using (QSqlLite s = new QSqlLite(file))
            {
                s.Open("SELECT name FROM sqlite_master WHERE type='table'");
                while (s.GetRow())
                {
                    DbTable t = new DbTable("", s[0], "BASE TABLE");
                    tables.Add(t.name, t);
                }

                foreach(DbTable t in tables.Values)
                {
                    s.Open("PRAGMA table_info(@1)", t.name);
                    while (s.GetRow())
                    {
                        string s0 = s[0];
                        string s1 = s[1];
                        string s2 = s[2];
                        string s3 = s[3];
                        string s4 = s[4];
                        string s5 = s[5];
//                        string s6 = s[6];
                       
                        DbColumn c = new DbColumn(t, s[1], s[2], 
                            s.GetBool(5),
                            !s.GetBool(3), s[4]);
                        t.columns.Add(c.name, c);
                        tablesByColumnName.Add(c.name, t);
                    }
                }



            }
        }
    }
}
