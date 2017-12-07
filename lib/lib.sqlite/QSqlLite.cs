using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data.Common;
using fp.lib.sqlite;

namespace fp.lib.sqlite
{
    public class QSqlLite : QSqlBase
    {
        public static string databaseName = ""; // MyDatabase.sqlite
        SQLiteDataReader m_reader;
        SQLiteConnection m_db;

        public static void CreateDatabase(string fileName, bool onlyIfNotExist = true)
        {
            if(onlyIfNotExist && System.IO.File.Exists(fileName))
                return;

            DropDatabase(fileName);
            SQLiteConnection.CreateFile(fileName);
        }

        public static void DropDatabase(string fileName)
        {
            if(System.IO.File.Exists(fileName))
                System.IO.File.Delete(fileName);
        }

        protected override DbDataReader reader
        {
            get { return m_reader; }
        }

        public QSqlLite(string dbName = "")
        {
            if (dbName == "")
                dbName = databaseName;
            m_db = new SQLiteConnection("Data Source=" + dbName + ";Version=3;");
            m_db.Open();

        }

        ~QSqlLite()
        {
        }

        public override void OnDispose()
        {
            CloseReader();
            m_db.Close();

            // https://stackoverflow.com/questions/8511901/system-data-sqlite-close-not-releasing-database-file
            // also need to call this if you need to delete the file
            // GC.Collect();

        }

        private List<SQLiteCommand> commands = new List<SQLiteCommand>();

        private SQLiteCommand CreateCommand(string sSql)
        {
            SQLiteCommand myCommand = new SQLiteCommand(sSql, m_db);
            commands.Add(myCommand);
            return myCommand;
        }

        public int Execute(string sSql)
        {
            try
            {
                SQLiteCommand myCommand = CreateCommand(sSql);
                return Execute(myCommand);
            }
            catch (Exception error)
            {
                throw error;
            }
        }

        public int Execute(SQLiteCommand cmd)
        {
            CloseReader();
            cmd.ExecuteNonQuery();
            Open("SELECT last_insert_rowid()");
            if(GetRow())
                lastInsertId = GetInt(0);
            return lastInsertId;
        }

        static public int Exec(string sql, params object[] args)
        {
            AddArgsToQuery(false, ref sql, args);

            int nResult = 0;
            using (QSqlLite s = new QSqlLite())
            {
                nResult = s.Execute(sql);
            }

            return nResult;
        }


        static public int SqlInt(string sql, params object[] args)
        {
            AddArgsToQuery(false, ref sql, args);

            int nResult = 0;

            using (QSqlLite s = new QSqlLite())
            {
                s.Open(sql);
                if (s.GetRow())
                    nResult = s.GetInt(0);
            }

            return nResult;
        }

        static public string SqlString(string sSql)
        {
            string sResult = "";

            using (QSqlLite s = new QSqlLite())
            {
                s.Open(sSql);
                if (s.GetRow())
                    sResult = s.GetString(0);
            }
            
            return sResult;
        }

        protected override void OpenQuery(string sql)
        {
            CloseReader();

            try
            {
                SQLiteCommand myCommand = new SQLiteCommand(sql, m_db);
                Open(myCommand);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }


        public void Open(SQLiteCommand cmd)
        {
            try
            {
                m_reader = cmd.ExecuteReader();
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }

        }




    }
}
