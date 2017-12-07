using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Threading.Tasks;
using fp.lib.mysql;
using fp.lib.mssql;
using fp.lib.dbInfo;
using fp.lib;

namespace fp.sqratch
{
    public enum Status { Ready, Initializing, Parsing, Executing, LoadingRows, Closing, Failed, Cancelled }

    public static class A
    {
        public static DbInfo db = null;
        public static bool loading = true;
        public static bool uiLoaded = false;
        public static string appTitle { get { return form.Text; } set { form.Text = value; } }
        public static object mutex = new object();
        public static List<Msg> messages = new List<Msg>();
        public static main form = null;
        public static bool logActive = false;
        public static string currentStatusText { get { return _currentStatus.ToString(); } }
        public static Status currentStatus { get { return _currentStatus; } }
        public static int dbId { get { return db == null ? 0 : DbInfo.dbId; } }
        public static bool ready { get { return dbId > 0 && uiLoaded; } }
        static Status _currentStatus;

        public static bool Initialize(main f)
        {
            SetStatus(Status.Initializing);
            AddToLog("initializing...");
            form = f;
            Settings.Initialize();
            if (S.initSettings.databaseId > 0 && S.initSettings.loadDatabase)
            {
                if (!OpenDatabase(S.initSettings.databaseId))
                    return false;
            }
            return true;
         }

        public static void CloseDatabase()
        {
            A.appTitle = "Sqrach Pad";
            QMySql.ConnectString = "";
            db = null;
        }

      
        public static bool OpenDatabase(int id)
        {
            try
            {
                ConnectSettings c = S.GetConnection(id);
                if (c.type == "MySql")
                {
                    DbInfoMySql dbInfo = new DbInfoMySql();
                    dbInfo.Connect(c.host, c.database, c.user, c.password);
                    S.initSettings.databaseId = id;
                    db = dbInfo;
                }
                else if (c.type == "Sql Server")
                {
                    DbInfoMsSql dbInfo = new DbInfoMsSql();
                    dbInfo.Connect(c.host, c.database, c.user, c.password);
                    S.initSettings.databaseId = id;
                    db = dbInfo;
                }
                else if (c.type == "SQLite")
                {
                    DbInfoSqLite dbInfo = new DbInfoSqLite();
                    dbInfo.Connect(c.database);
                    S.initSettings.databaseId = id;
                    db = dbInfo;
                }
                if (db == null)
                    throw new Exception("unknown error");
                DbInfo.dbId = id;
                A.appTitle = "Sqrach Pad - " + c.host + " / " + db.databaseName;
                return true;
            }
            catch(Exception e)
            {
                OkBox(form, "Could not open database\r\n" + e.Message, MessageBoxIcon.Error);
            }
            
            return false;
        }

        public static void SetStatus(Status status)
        {
            lock (mutex)
            {
                _currentStatus = status;
            }
        }

    
        public static int progressMax = 0;
        public static int progressValue = 0;

        public static void SetProgress(int at = 0, int max = 0, bool redrawNow = false)
        {
            lock(mutex)
            {
                progressMax = max;
                progressValue = at;
                if (redrawNow)
                    form.UpdateProgress(true);
            }
        }

        public static void AddToLog(string txt, bool includeWhen = true, MsgStatus s = MsgStatus.Normal)
        {
            lock (messages)
            {
                messages.Add(new Msg(txt, includeWhen, s));
            }
        }

      

        public static String Pluralize(this string str)
        {
            return PluralizationService.CreateService(new CultureInfo("en-US")).Pluralize(str);
        }

        public static bool OkCancelBox(this Form f, string msg, MessageBoxIcon icon = MessageBoxIcon.Question)
        {
            return DialogResult.OK == MessageBox.Show(f, msg, appTitle, MessageBoxButtons.OKCancel, icon);
        }

        public static bool YesNoBox(this Form f, string msg, MessageBoxIcon icon = MessageBoxIcon.Question)
        {
            return DialogResult.Yes == MessageBox.Show(f, msg, appTitle, MessageBoxButtons.YesNo, icon);
        }

        public static DialogResult YesNoCancelBox(this Form f, string msg, MessageBoxIcon icon = MessageBoxIcon.Question)
        {
            return MessageBox.Show(f, msg, appTitle, MessageBoxButtons.YesNoCancel, icon);
        }

        public static void OkBox(this Form f, string msg, MessageBoxIcon icon = MessageBoxIcon.Information)
        {
            MessageBox.Show(f, msg, appTitle, MessageBoxButtons.OK, icon);
        }
    }

    public enum MsgStatus { Normal, Warning, Error }
    public class Msg
    {
        public bool showLog = false;
        public string msg;
        public DateTime when;
        public bool includeWhen;
        public MsgStatus status;
        public Msg(string m, bool i, MsgStatus s = MsgStatus.Normal)
        {
            msg = m;
            includeWhen = i;
            when = DateTime.Now;
            status = s;
            if (s == MsgStatus.Error && S.Get("ShowLogOnError", true))
                showLog = true;
        }

    }


}
