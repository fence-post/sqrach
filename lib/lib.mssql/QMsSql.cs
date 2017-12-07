using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Collections;
using System.Diagnostics;
using System.Configuration;
using System.Data.Common;
using fp.lib;

namespace fp.lib.mssql
{
    
    public class QMsSql : QSqlBase
    {
        public static string ConnectString = ""; // instance-specific connect string in constructor takes precidence over this
        public SqlConnection m_db = null;
        SqlDataReader m_reader = null;
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

        public QMsSql()
        {
            m_db = new SqlConnection(ConnectString);
            m_db.Open();
        }

        public QMsSql(string instanceConnectString)
        {
            m_db = new SqlConnection(instanceConnectString);
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

     
        public int Execute(string sSql)
        {
            try
            {
                SqlCommand myCommand = new SqlCommand(sSql, m_db);
                return Execute(myCommand);
            }
            catch (SqlException error)
            {                
                throw error;
            }
        }

        public int Execute(SqlCommand cmd)
        {
            CloseReader();
            if (m_nCommandTimeout > 0)
                cmd.CommandTimeout = m_nCommandTimeout;
            cmd.ExecuteNonQuery();
            lastInsertId = 0; // Convert.ToInt32(cmd.LastInsertedId);
            return lastInsertId;
        }

        static public int Exec(string sSql)
        {
            int nResult = 0;
            using (QMsSql s = new QMsSql())
            {
                nResult = s.Execute(sSql);
            }

            return nResult;
        }

        static public int SqlInt(bool useRawValues, string sql, params object[] args)
        {
            int nResult = 0;

            AddArgsToQuery(useRawValues, ref sql, args);

            using (QMsSql s = new QMsSql())
            {
                s.Open(sql);
                if (s.GetRow())
                    nResult = s.GetInt(0);
            }

            return nResult;
        }

        static public int SqlInt(string sSql)
        {
            int nResult = 0;

            using (QMsSql s = new QMsSql())
            {
                s.Open(sSql);
                if (s.GetRow())
                    nResult = s.GetInt(0);
            }
            
            return nResult;
        }

        static public string SqlString(string sSql)
        {
            string sResult = "";

            using (QMsSql s = new QMsSql())
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
                SqlCommand myCommand = new SqlCommand(sSql, m_db);
                Open(myCommand);
            }
            catch (SqlException e)
            {
                throw new ApplicationException(e.Message);
            }
        }

        public void Open(SqlCommand cmd)
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
       
    }
}
