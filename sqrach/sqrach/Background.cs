using fp.lib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace fp.sqratch
{
    public enum BackgroundStatus { None, Loading, QueryQueued, ExecutingQuery, ShuttingDown, Stopped };
    public class Background : BackgroundWorker
    {
        public static BackgroundStatus status = BackgroundStatus.Loading;
        protected Query queuedQuery = null;

        /*
    public enum BackgroundStatus { None, Loading, QueryCancelled, QueryQueued, ExecutingQuery, ReadingRows, QueryDone, ShuttingDown, Stopped };
        public string expr;
        public int tabId;
        public int queryId;
        public bool busy { get { return !(status == BackgroundStatus.QueryCancelled || status == BackgroundStatus.QueryDone || status == BackgroundStatus.None); } }
        public bool queryReadyToRun { get { return (T.ne(expr) && status == BackgroundStatus.QueryQueued); } }
        */
        public bool busy { get { return (status != BackgroundStatus.None); } }

        public Background()
        {          
            WorkerSupportsCancellation = false;
            WorkerReportsProgress = true;
        }
        
        public void CancelQuery()
        {
            if (queuedQuery != null)
                queuedQuery.queryCancelled = true;
            // status = BackgroundStatus.QueryCancelled;           
        }

        public void Shutdown()
        {
            CancelQuery();
            status = BackgroundStatus.ShuttingDown;
        }

        /*

        bool Notify()
        {
            if (status == BackgroundStatus.QueryCancelled || status == BackgroundStatus.ShuttingDown)
                return false;

            ReportProgress(tabId);
            return true;
        }
        public void Reset()
        {
            status = BackgroundStatus.None;
            tabId = 0;
            queryId = 0;
            expr = "";
            lock (A.columns)
            {
                A.columns.Clear();
            }
            lock(A.rows)
            { 
                A.rows.Clear();
            }
        }
        */
        public void QueueQuery(Query q)
        {
            if (busy)
                return;

                if (S.Get("BlockWritableQueries", false))
            {
                string sql = q.query.ToLower().Trim();
                if (sql.StartsWith("update ") || sql.StartsWith("insert ") || sql.StartsWith("replace ") || sql.StartsWith("delete "))
                {
                    A.AddToLog("Writable queries are blocked", true, MsgStatus.Warning);
                    return;
                }
            }

                
            queuedQuery = q;
            status = BackgroundStatus.QueryQueued;
        }

        public void RunQuery()
        {
            if (status != BackgroundStatus.QueryQueued)
                throw new InvalidOperationException();

            status = BackgroundStatus.ExecutingQuery;
            if (Settings.Get("QuietExecutionErrors", true))
                queuedQuery.TryRun();
            else
                queuedQuery.Run();
            status = BackgroundStatus.None;
            // App.StopProgress();
        }
    }
}



/*
protected void TryRun()
{
    try
    {
        Run();
    }
    catch(Exception e)
    {
        A.QueryDone(Status.Failed);
        Settings.SaveQueryFailed(queryId);
        for (int i = 0; e != null && i < 5; i++)
        {
            string message = e.Message;
            message = message.Replace("You have an error in your SQL syntax; check the manual that corresponds to your MariaDB server version for the right syntax to use near ", "SQL Error: ");
            A.AddToLog(message, false, MsgStatus.Error);
            e = e.InnerException;
        }
        Reset();
    }
}

protected void Run()
{ 
    if (!Notify())
        return;

    using (QSqlBase s = A.NewQuery())
    {
        int expected = 0;
        try
        {
            s.Open("select count(*) from (" + expr + ") t0101");
            if(s.GetRow())
                expected = s.GetInt(0);
        }
        catch(Exception e)
        {

        }

        bool first = true;
        A.QueryExecutionStart();
        s.Open(expr);
        A.QueryExecutionComplete(expected);
        while (s.GetRow())
        {
            if (first)
            {
                lock(A.columns)
                {
                    for (int i = 0; i < s.FieldCount; i++)
                        A.columns.Add(new ColumnInfo(i, s.GetColumnName(i), s.GetColumnType(i)));                        
                }

                first = false;
                status = BackgroundStatus.ReadingRows;
                if (!Notify())
                    break;
            }
            List<string> row = new List<string>();
            for (int i = 0; i < s.FieldCount; i++)
            {
                string txt = s.GetString(i);
                lock(A.columns)
                {
                    A.columns[i].UpdateMaxWidth(txt.Length);
                }
                row.Add(txt);
            }

            if (A.QueryAddRow(row))
            {
                if (!Notify())
                    break;
            }
        }
    }
    A.QueryDone(status == BackgroundStatus.QueryCancelled ? Status.Cancelled : Status.Ready);
    status = BackgroundStatus.QueryDone;
    Notify();
}
*/
