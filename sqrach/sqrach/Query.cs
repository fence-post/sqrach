using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fp.lib;
using fp.lib.sqlite;

namespace fp.sqratch
{
    public class Query
    {
        public static ResultsListView resultsList;
        public List<QueryColumnInfo> columns = new List<QueryColumnInfo>();
        public List<List<string>> rows = new List<List<string>>();
        public string executionTime;
        public string loadingTime;
        public int tabId;
        public int prevQueryId = -1;
        public int nextQueryId = -1;
        public bool canGoPrev { get { return prevQueryId > 0; } }
        public bool canGoNext { get { return nextQueryId > 0; } }
        public string label = "";
        public bool dirty = false;
        public bool queryCancelled = false;

        Stopwatch watch = new Stopwatch();
        int expectedRows = 0;
        string expr;
        int _currentQueryId;
        int queryMs = 0;
        const int queryUpdateFreq = 100000;

        public Query(QueryInfo info)
        {
            tabId = info.queryId;
            label = info.tabLabel;
            currentQueryVersionId = info.queryVersionId;

        }

        public Query(int tid, string l = "", int queryId = 0)
        {
            tabId = tid;
            label = l;
            currentQueryVersionId = queryId;

        }

        public string query
        {
            get
            {
                return expr;
            }
            set
            {
                if (expr != value)
                {
                    dirty = true;
                    expr = value;

                }
            }
        }

        public void Save(bool executing = false)
        {
            dirty = false;
            if(expr != null)
                currentQueryVersionId = Settings.SaveQuery(tabId, currentQueryVersionId, expr, executing);
        }

        public int currentQueryVersionId
        {
            set
            {
                prevQueryId = 0;
                nextQueryId = 0;
                _currentQueryId = value;
                if (_currentQueryId > 0)
                {
                    // todo put in model
                    prevQueryId = QSqlLite.SqlInt("select id from queries where tab_id=" + tabId + " and id < " + _currentQueryId + " order by id desc limit 1");
                    nextQueryId = QSqlLite.SqlInt("select id from queries where tab_id=" + tabId + " and id > " + _currentQueryId + " order by id limit 1");
                    expr = QSqlLite.SqlString("select expr from queries where id=" + _currentQueryId);
                }
            }
            get { return _currentQueryId; }

        }

        public void ClearResults()
        {
            rows.Clear();
            columns.Clear();
            expectedRows = 0;
            columnGetDataErrors.Clear();
        }
        
        public void TryRun()
        {
            try
            {
                Run();
            }
            catch (Exception e)
            {
                A.SetStatus(Status.Failed);
                Settings.SaveQueryFailed(_currentQueryId);
                for (int i = 0; e != null && i < 5; i++)
                {
                    string message = e.Message;
                    message = message.Replace("You have an error in your SQL syntax; check the manual that corresponds to your MariaDB server version for the right syntax to use near ", "SQL Error: ");
                    A.AddToLog(message, false, MsgStatus.Error);
                    e = e.InnerException;
                }
            }
        }

        HashSet<int> columnGetDataErrors = new HashSet<int>();

        string GetColumnValue(QSqlBase s, int i)
        {
            string result = null;
            try
            {
                if(!s.IsNull(i))
                    result = s.GetString(i);
            }
            catch(Exception e)
            {
                if(!columnGetDataErrors.Contains(i))
                {
                    A.AddToLog("error getting value for column " + i + " - " + e.Message, true, MsgStatus.Warning);
                    A.AddToLog("further errors for column " + i + " will be suppressed", false, MsgStatus.Warning);
                    columnGetDataErrors.Add(i);
                }
                
            }
            return result;
        }

        public void Run()
        {
            A.AddToLog("running query...");
            A.SetStatus(Status.Executing);
            columnGetDataErrors.Clear();
            rows.Clear();
            columns.Clear();
            queryCancelled = false;
            expectedRows = A.db.GetExpectedRowsInQuery(expr);
            
            using (QSqlBase s = A.db.GetSql())
            {
                if(!queryCancelled)
                {
                    watch.Reset();
                    watch.Start();

                    s.Open(expr);

                    TimeSpan span = watch.Elapsed;
                    queryMs = Convert.ToInt32(span.TotalMilliseconds);
                    executionTime = span.ToString("ss':'ff");
                    watch.Reset();
                    watch.Start();
                    bool first = true;
                    A.SetStatus(Status.LoadingRows);

                    while (queryCancelled == false && s.GetRow())
                    {
                        if (first)
                        {
                            lock (columns)
                            {
                                for (int i = 0; i < s.FieldCount; i++)
                                    columns.Add(new QueryColumnInfo(i, s.GetColumnName(i), s.GetColumnType(i)));
                            }

                            first = false;
                        }
                        List<string> row = new List<string>();
                        for (int i = 0; i < s.FieldCount; i++)
                        {
                            string txt = GetColumnValue(s, i);
                            if(txt != null)
                            {
                                lock (columns)
                                {
                                    columns[i].UpdateMaxWidth(txt.Length);
                                }
                            }
                            row.Add(txt);
                        }

                        bool notify = false;
                        lock (rows)
                        {
                            if (rows.Count > 10000000)
                                throw new ApplicationException("too many rows");
                            rows.Add(row);
                            notify = rows.Count == 100 || (rows.Count % queryUpdateFreq == 0);
                            A.SetProgress(rows.Count, expectedRows);
                        }

                        if (notify)
                            resultsList.resultsReady = true;
                    }
                }
            }
            if (queryCancelled)
            {
                A.SetStatus(Status.Cancelled);
            }
            else
            {
                resultsList.resultsReady = true;
                A.SetStatus(Status.Ready);
                TimeSpan span = watch.Elapsed;
                loadingTime = span.ToString("ss':'ff");
                lock (rows)
                {
                    Settings.SaveQueryDiagnostics(_currentQueryId, true, rows.Count, queryMs);
                    A.AddToLog("execution time: " + executionTime);
                    A.AddToLog("loading time: " + loadingTime);
                    A.AddToLog("rows: " + rows.Count);
                }
            }
        }
    }
}

/*
 
        public void StartQuery(int queryId)
        {
            A.AddToLog("running query...");
            A.SetStatus(Status.Executing);
        }

        public void QueryExecutionStart()
        {
            watch.Reset();
            watch.Start();
        }

        public void QueryExecutionComplete(int expected)
        {
            expectedRows = expected;
            TimeSpan span = watch.Elapsed;
            queryMs = Convert.ToInt32(span.TotalMilliseconds);
            executionTime = span.ToString("ss':'ff");
            watch.Reset();
            watch.Start();
        }

   
        bool AddRow(List<string> row)
        {
            lock (rows)
            {
                rows.Add(row);
                return rows.Count == 100 || (rows.Count % queryUpdateFreq == 0);
            }
        }

        public void QueryDone(Status s)
        {
            if (s == Status.Ready)
            {
                lock (rows)
                {
                    Settings.SaveQueryDiagnostics(_currentQueryId, true, rows.Count, queryMs);
                }
            }
        }
*/
