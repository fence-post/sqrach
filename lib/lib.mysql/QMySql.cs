using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Collections;
using System.Diagnostics;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Data.Common;

namespace fp.lib.mysql
{
    
    public class QMySql : QSqlBase
    {
        public static string ConnectString = ""; // instance-specific connect string in constructor takes precidence over this
        public MySqlConnection m_db = null;
        MySqlDataReader m_reader = null;
        static int m_nCommandTimeout = 0;
       
        protected override DbDataReader reader
        {
            get { return m_reader; }
        }

        public int CommandTimeout
        {
            get { return m_nCommandTimeout; }
            set { m_nCommandTimeout = value; }
        }

        public QMySql()
        {
            m_db = new MySqlConnection(ConnectString);
            m_db.Open();
        }

        public QMySql(string instanceConnectString)
        {
            m_db = new MySqlConnection(instanceConnectString);
            m_db.Open();
        }

        public QMySql(string host, string db, string user, string pwd)
        {
            string instanceConnectString = "Server=" + host + ";Database=" + db + ";Uid=" + user + ";Pwd=" + pwd + ";";
            m_db = new MySqlConnection(instanceConnectString);
            m_db.Open();
        }

        public override void OnDispose()
        {
            
            if (m_db != null)
            {
                // m_db.CancelQuery(10);
                CloseReader();
                // Debug.Assert(m_db.State == ConnectionState.Closed);
                m_db.Close();
                m_db.Dispose();
                m_db = null;
            }
        }

        public override void CancelQuery()
        {
            m_db.CancelQuery(10);
        }

        protected override int ExecuteCommand(string sql)
        {
            try
            {
                MySqlCommand myCommand = new MySqlCommand(sql, m_db);
                return ExecuteCommand(myCommand);
            }
            catch (MySqlException error)
            {                
                throw error;
            }
        }

        public int ExecuteCommand(MySqlCommand cmd)
        {
            CloseReader();
            if (m_nCommandTimeout > 0)
                cmd.CommandTimeout = m_nCommandTimeout;
            cmd.ExecuteNonQuery();
            lastInsertId = Convert.ToInt32(cmd.LastInsertedId);
            return lastInsertId;
        }

        static public int Exec(string sSql, string connectString = null)
        {
            if (connectString == null)
                connectString = ConnectString;

            int nResult = 0;
            using (QMySql s = new QMySql(connectString))
            {
                nResult = s.Execute(sSql);
            }

            return nResult;
        }

        static public int SqlInt(string sql, params object[] args)
        {
            return SqlInt(false, sql, args);
        }

        static public int SqlInt(bool useRawValues, string sql, params object[] args)
        {
            int nResult = 0;

            AddArgsToQuery(useRawValues, ref sql, args);

            using (QMySql s = new QMySql())
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

            using (QMySql s = new QMySql())
            {
                s.Open(sSql);
                if (s.GetRow())
                 sResult = s.GetString(0);
            }
            
            return sResult;
        }

        protected override void OpenQuery(string sSql)
        {
            CloseReader();

            try
            {
                MySqlCommand myCommand = new MySqlCommand(sSql, m_db);
                OpenCommand(myCommand);
            }
            catch (SqlException e)
            {
                throw new ApplicationException(e.Message);
            }
        }

     
        public void OpenCommand(MySqlCommand cmd)
        {
            try
            {
                if (m_nCommandTimeout > 0)
                    cmd.CommandTimeout = m_nCommandTimeout;
                m_reader = cmd.ExecuteReader();
            }
            catch (SqlException e)
            {
                throw new ApplicationException(e.Message);
            }

        }

       



        /*
         * 
         * 
        static public List<string> SqlList(string sql)
        {
            return SqlList(sql, "@0");
        }

        static public List<string> SqlList(string sql, string template)
        {
            List<string> result = new List<string>();
            using (QMySql s = new QMySql())
            {
                s.Open(sql);
                while (s.GetRow())
                {
                    string row = template;
                    for (int i = 0; i < s.FieldCount; i++)
                    {
                        row = row.Replace("@" + i.ToString(), s.GetString(i));
                    }
                    result.Add(row);
                }
            }

            return result;
        }


        public static string DumpTable(string tableName)
        {
            string result = "";
            result += "set identity insert [" + tableName + "] on\r\n";
            using (QMySql s = new QMySql())
            {
                s.Open("select * from " + tableName);
                while (s.GetRow())
                {
                    string fields = "";
                    string values = "";
                    for (int i = 0; i < s.FieldCount; i++)
                    {
                        fields += ",[" + s.GetColumnName(i) + "]";
                        values += "," + "'" + QMySql.Normalize(s.GetString(i)) + "'";
                    }
                    if (fields.Length > 0) fields = fields.Substring(1);
                    if (values.Length > 0) values = values.Substring(1);
                    result += "insert into [" + tableName + "] (" + fields + ") values (" + values + ")\r\n";
                }
            }
            result += "set identity_insert [" + tableName + "] off\r\n";

            return result;
        }

         public static DbType GetTypeForObject(object o)
        {

            DbType result = DbType.Int32;
            if (o != null)
            {
                Type t = o.GetType();
                if (t.Name == "String")
                    result = DbType.String;

                // todo: fill this in from examples in QSqlObject GetDbTypeForMember
            }

            return result;
        }

        public static DataSet CreateDataset(string sSql)
        {
            QMySql sql = new QMySql();
            DataSet ds = sql.CreateDatasetNonStatic(sSql);
            sql.Dispose();
            return ds;
        }

        public DataSet CreateDatasetNonStatic(string sSql)
        {
            MySqlCommand cmd = new MySqlCommand(sSql, m_db);
            using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
            {
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds;
            }
        }

        public static DataSet CreatePagingDataset(int nStart, int nRecords, string sSql)
        {
            QMySql sql = new QMySql();
            DataSet ds = sql.CreatePagingDatasetNonStatic(nStart, nRecords, sSql);
            sql.Dispose();
            return ds;
        }

        public DataSet CreatePagingDatasetNonStatic(int nStart, int nRecords, string sSql)
        {
            MySqlCommand cmd = new MySqlCommand(sSql, m_db);
            using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
            {
                DataSet ds = new DataSet();
                da.Fill(ds, nStart, nRecords, "Query");
                return ds;
            }
        }

        public MySqlCommand CreateCommandFromSql(string sql)
        {
            MySqlCommand myCommand = new MySqlCommand(sql, m_db);
            if (m_nCommandTimeout > 0)
                myCommand.CommandTimeout = m_nCommandTimeout;

            return myCommand;
        }

        public void AddInParameter(MySqlCommand cmd, string name, object value, DbType type = DbType.String)
        {
            // 2. define parameters used in command object
            SqlParameter param = new SqlParameter();
            param.ParameterName = name;
            param.Value = value;
            param.DbType = type;

            // 3. add new parameter to command object
            cmd.Parameters.Add(param);
        }
        
        

        static public int Execute(string sql, object arg1, object arg2 = null, object arg3 = null)
        {
            DbType arg1Type = GetTypeForObject(arg1);
            DbType arg2Type = GetTypeForObject(arg2);
            DbType arg3Type = GetTypeForObject(arg3);

            return Execute(sql, arg1, arg1Type, arg2, arg2Type, arg3, arg3Type);
        }

        static public int Execute(string sql,
           object arg1, DbType arg1Type,
           object arg2 = null, DbType arg2Type = DbType.Int32,
           object arg3 = null, DbType arg3Type = DbType.Int32)
        {

            throw new ApplicationException("are args implemented in Msql?");

            // make it figure out type by looking at the object's type
            // remove the dbtype args

            int result = 0;

            using (QMySql s = new QMySql())
            {
                MySqlCommand cmd = s.CreateCommandFromSql(sql);
                if (arg1 != null)
                    s.AddInParameter(cmd, "arg1", arg1, arg1Type);
                if (arg2 != null)
                    s.AddInParameter(cmd, "arg2", arg2, arg2Type);
                if (arg3 != null)
                    s.AddInParameter(cmd, "arg3", arg3, arg3Type);
                result = s.ExecuteNonStatic(cmd);
            }

            return result;
        }
        */
        /*
        protected string GetConnectString()
        {

            string txt = GlobalConnectString;
            if (txt == null || txt == "")
            {
                txt = System.Configuration.ConfigurationSettings.AppSettings["connectString"];
            }

            if (txt == null || txt == "")
            {
                throw new ApplicationException("QMySql:GetConnectString() - you need to configure the connect string for this machine.");
            }

            return txt;
        }
        */
    }
}
