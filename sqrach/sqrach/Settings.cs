using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using fp.lib;
using fp.lib.sqlite;
using System.IO;
using fp.lib.dbInfo;

namespace fp.sqratch
{
    public class S : Settings { }
   
    public class InitSettings
    {
        public int databaseId = 0;
        public bool loadDatabase = false;
        public bool maximized = true;
        public string screen = "";
        public int width = 0;
        public int height = 0;
        public int horzSplitterPos = 0;
        public int vertSplitterPos = 0;
        public int lastLoadTime = 0;
        public int avgLoadTime = 0;
        public bool dark = true;
        public bool shutDownClean = false;

        [Persistent(isPersistent = false)]
        public string initError;
    }

    public class ConnectSettings
    {
        public int databaseId;
        public string database;
        public string type;
        public string host;
        public string user;
        public string password;
    }

    public class QueryInfo
    {
        public int queryId;
        public int queryVersionId;
        public string tabLabel;
        public string expr;
        public bool visible;
    }

    public class QueryHistory
    {
        public int id;
        public int queryVersionId;
        public string label;
        public string expr;
        public bool visible;
        public DateTime whenChanged;
        public bool failed;
        public int rows;
        public int ms;
    }

    public class Settings
    {
        public const string defaultConnectionFile = "default.txt";
        public static InitSettings initSettings;
        public static bool dirty = false;
        protected static Dictionary<string, string> settings = new Dictionary<string, string>();

        #region settings getters and setters

        public static bool Toggle(string id, bool def)
        {
            bool val = !Settings.Get(id, def);
            Set(id, val);
            return val;
        }

        public static bool Set(string id, bool val)
        {
            Set(id, val ? "true" : "false");
            return val;
        }

        public static int Set(string id, int val)
        {
            Set(id, val.ToString());
            return val;
        }

        public static string Set(string id, string val)
        {
            if (!A.loading)
            {
                lock(settings)
                {
                    if (settings.ContainsKey(id))
                        settings[id] = val;
                    else
                        settings.Add(id, val);
                    dirty = true;
                }
            }

            return val;
        }

        public static int Get(string id, int def)
        {
            return Get(id, def.ToString()).ToInt32();
        }

        public static bool Get(string id, bool def)
        {
            return Get(id, def ? "true" : "false") == "true";
        }

        public static string Get(string id, string def = "")
        {
            return settings.At(id, def);
            /*
            if (settings.ContainsKey(id))
                return settings[id];
            return def;
            */
        }

        #endregion

        #region initialize

        public static void Initialize()
        {

            initSettings = new InitSettings();
            try
            {
                QSqlLite.databaseName = "sqrach.sqlite";
                if (!File.Exists(QSqlLite.databaseName))
                {
                    CreateSettingsDatabase();
                }

                using (QSqlLite s = new QSqlLite())
                {
                    s.Open("select * from init_settings");
                    if (s.GetRow(true))
                    {
                        QObject.PopulateFromRow(s, initSettings);
                        if (initSettings.shutDownClean == false)
                        {
                            initSettings.databaseId = 0;
                            initSettings.loadDatabase = false;
                        }
                        initSettings.shutDownClean = false;
                    }
                }
            }
            catch (Exception e)
            {
                initSettings.databaseId = 0;
                initSettings.loadDatabase = false;
                initSettings.initError = e.Message;
            }
        }

        public static void CreateSettingsDatabase()
        {
            File.Delete(QSqlLite.databaseName);
            QSqlLite.CreateDatabase(QSqlLite.databaseName);
            using (QSqlLite s = new QSqlLite())
            {

                #region create structure
                s.Execute(@"
create table init_settings (
loadDatabase int not null default 0,
databaseId int not null,
maximized integer default 1 not null,
screen text not null default '' ,
width integer not null default 0,
height integer not null default 0,
horzSplitterPos integer not null default 0,
vertSplitterPos integer not null default 0,
dark integer not null default 1,
shutDownClean not null default 0,
lastLoadTime integer not null default 0,
avgLoadTime integer not null default 0
)");

                s.Execute(@"
create table databases (
databaseId int not null primary key,
database text not null,
host text not null,
type text not null,
user text not null,
password text not null,
unique (database,host)
)");

                s.Execute(@"
CREATE TABLE tables (
databaseId int not null,
name text not null,
lastAnalyzed integer not null default 0,
lastUsed integer not null default 0,
useCount integer not null default 0,
rowCount integer not null default 0,
bookmarked integer not null default 0,
showAllColumns integer not null default 0
)");
                s.Execute(@"
CREATE TABLE table_columns (
databaseId int not null,
tableName text not null, 
columnName text not null,
columnType text not null,
bookmarked integer not null default 0,
lastAnalyzed integer not null default 0,
lastUsed integer not null default 0,
useCount integer not null default 0,
cardinality real not null default 0,
fullness real not null default 0,
uniqueValues integer not null default 0,
nonEmptyValues integer not null default 0,
minValue text not null,
maxValue text not null
)");
                s.Execute(@"
CREATE TABLE table_aliases (
databaseId int not null,
tableName text not null, 
alias text not null,
lastAnalyzed integer not null default 0,
lastUsed integer not null default 0,
useCount integer not null default 0
)");


                s.Execute(@"
CREATE TABLE settings (
name text not null unique,
value text not null)");

                s.Execute(@"
CREATE TABLE tabs (
databaseId int not null,
id integer  NOT NULL PRIMARY KEY,
pos integer  NOT NULL DEFAULT 0,
name text  NOT NULL,
visible integer  NOT NULL DEFAULT 1)");

                s.Execute(@"
 CREATE TABLE queries (
id integer NOT NULL PRIMARY KEY,
tab_id integer NOT NULL,
failed integer NOT NULL default 0,
expr text DEFAULT NULL,
rows integer null,
ms text null,
    when_changed text NOT NULL
    )  ");
                #endregion
            }
        }

        #endregion

        #region query methods

        public static List<QueryHistory> GetQueryHistory()
        {
            List<QueryHistory> result = new List<QueryHistory>();
            string sql = "t.databaseId=@1";
            if (S.Get("ShowClosedQueries", false))
                sql = T.AppendTo(sql, "t.visible<>0", " and ");
            if (sql != "")
                sql = " where " + sql;
            sql = @"
select q.id, q.tab_id, q.expr, q.when_changed, t.visible, t.name, q.failed, q.rows, q.ms
from queries q 
inner join tabs t on t.id=q.tab_id " + sql + " order by q.when_changed desc limit 100";
            using (QSqlLite s = new QSqlLite())
            {
                s.Open(sql, A.dbId);
                while (s.GetRow())
                {
                    string z = s.GetString(6);
                    result.Add(new QueryHistory
                    {
                        queryVersionId = s.GetInt(0),
                        id = s.GetInt(1),
                        expr = s.GetString(2),
                        whenChanged = Convert.ToDouble(s.GetString(3)).ToDateTime(),
                        visible = s.GetBool(4),
                        label = s.GetString(5),
                        failed = s.GetBool(6),
                        rows = s.GetInt(7),
                        ms = (s.GetString(8) == "" ? 0 : Convert.ToInt32(s.GetString(8)))
                    });
                }
            }

            return result;
        }

        public static List<QueryInfo> GetQueryInfo(IEnumerable<int> ids)
        {
            List<QueryInfo> result = new List<QueryInfo>();
            string idList = ids.Join();
            if (idList != "")
            {
                using (QSqlLite s = new QSqlLite())
                {
                    // this order will put the newest versions at the bottom of the results
                    // so if multiple version are opened, it will end up with the latest version anyways
                    s.Open(@"
select t.id, t.name, q.expr, t.visible, q.id 
from queries q 
inner join tabs t on t.id=q.tab_id 
where q.id in (" + idList + ") order by q.id");
                    while (s.GetRow())
                        result.Add(new QueryInfo { queryVersionId=s.GetInt(4), visible=s.GetBool(3), queryId = s.GetInt(0), tabLabel = s.GetString(1), expr = s.GetString(2) });

                }
            }
            
            return result;
        }

        public static int GetQueries(List<Query> result)
        {
            if (A.dbId == 0)
                return 0;
            QSqlLite.Exec("delete from queries where expr=''");
            QSqlLite.Exec("delete from tabs where databaseId=@1 and id not in (select tab_id from queries)", A.dbId);
            using (QSqlLite s = new QSqlLite())
            {
                s.Open("select t.id, t.name, (select q.id from queries q where q.tab_id=t.id order by q.id desc limit 1) from tabs t where t.databaseId=@1 and t.visible<>0 order by t.pos", A.dbId);
                while (s.GetRow())
                {
                    result.Add(new Query(s.GetInt(0), s[1], s.GetInt(2)));
                }
            }

            return result.Count;
        }

        public static void DeleteQueryVersion(int queryId)
        {
            QSqlLite.Exec("delete from queries where id=" + queryId);
        }
        public static void SaveQueryFailed(int queryId)
        {
            QSqlLite.Exec("update queries set failed=1 where id=" + queryId);
        }

        public static void SaveQueryDiagnostics(int queryId, bool succeeded, int rows, long ms)
        {
            QSqlLite.Exec("update queries set ms=" + ms.ToString() + ",rows=" + rows + " where id=" + queryId);
        }

        public static int SaveQuery(int tabId, int currentQueryId, string expr, bool executing)
        {
            // string now = "'" + DateTime.Now.ToString("u") + "'";
            double now = DateTime.Now.ToEpoch();
            expr = QSqlLite.n(expr);
            bool justUpdate = (currentQueryId != 0 && 0 != QSqlLite.SqlInt("select count(*) from queries where id=" + currentQueryId + " and expr='" + expr + "'"));
            string sql;
            if (justUpdate)
            {
                sql = "update queries set expr='" + expr + "',when_changed=" + now;
                sql += " where id = " + currentQueryId;
                QSqlLite.Exec(sql);
            }
            else
            {
                sql = "insert into queries (tab_id, expr,when_changed) values (" + tabId + ",'" + expr + "'," + now + ")";
                currentQueryId = QSqlLite.Exec(sql);
            }

            return currentQueryId;
        }

        /*
        public static IEnumerable<string> GetQueries()
        {
            List<string> result = new List<string>();
            using (QSqlLite s = new QSqlLite())
            {
                s.Open("select expr from queries limit 10");
                while (s.GetRow())
                    yield return s.GetString(0);
            }
        }
        */

        #endregion

        #region connections

        public static void Load()
        {
            BindingFlags flags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.Instance;
            settings.Clear();
            using (QSqlLite s = new QSqlLite())
            {

                s.Open("select name,value from settings");
                while (s.GetRow())
                    settings.Add(s[0], s[1]);
                if (A.dbId > 0)
                {
                    s.Open("select * from tables where databaseId=@1", A.dbId);
                    while (s.GetRow())
                        QObject.PopulateFromRow(s, A.db.tables[s["name"]], flags);
                    s.Open("select * from table_columns where databaseId=@1", A.dbId);
                    while (s.GetRow())
                        QObject.PopulateFromRow(s, A.db.tables[s["tableName"]].columns[s["columnName"]], flags);
                    s.Open("select * from table_aliases where databaseId=@1", A.dbId);
                    while (s.GetRow())
                    {
                        DbAlias a = new DbAlias();
                        QObject.PopulateFromRow(s, a, flags);
                        A.db.tables[a.table.name].aliases.Add(a.alias, a);
                        A.db.tablesByAlias.AddIfNotExists(a.alias, a.table);
                    }
                }
            }
        }

        public static List<KeyValuePair<string, int>> GetConnections(out int selectedIndex)
        {
            selectedIndex = -1;
            List<KeyValuePair<string, int>> result = new List<KeyValuePair<string, int>>();
            using (QSqlLite s = new QSqlLite())
            {
                s.Open("select databaseId, host, database, type from databases order by database, host");
                while (s.GetRow())
                {
                    string nam = s[2];
                    if (s[3] == "SQLite")
                        nam = T.GetFileNameFromFilePath(nam, true);
                    result.Add(new KeyValuePair<string, int>(s[1] + " / " + nam, s.GetInt(0)));
                    if (s.GetInt(0) == initSettings.databaseId)
                        selectedIndex = result.Count - 1;
                }
            }
            return result;
        }

        public static ConnectSettings GetConnection(int id)
        {
            using (QSqlLite s = new QSqlLite())
            {
                s.Open("select * from databases where databaseId=" + id);
                if (s.GetRow())
                {
                    ConnectSettings result = new ConnectSettings();
                    QObject.PopulateFromRow(s, result);
                    return result;
                }
            }

            return null;
        }

        public static void DeleteConnection(int id)
        {
            QSqlLite.Exec("delete from databases where databaseId=@1;delete from tables where databaseId=@1;delete from table_aliases where databaseId=@1;delete from table_columns where databaseId=@1;delete from tabs where databaseId=@1", id);
        }

        public static int SaveConnection(string type, string host, string database, string userId,
            string password, bool savePassword, bool makeDef)
        {

            int id = QSqlLite.SqlInt(@"select databaseId from databases where type=@3 and host=@2 and database=@1;
", database, host, type, userId, password);
            if (id == 0)
            {
                id = QSqlLite.Exec(@"
insert into databases (databaseId, type, host, database, user, password) 
values ((select 1 + coalesce(max(databaseId),0) from databases), @3, @2, @1, @4, @5);
select databaseId from databases where type=@3 and host=@2 and database=@1;
", database, host, type, userId, password);
            }
            else
            {
                QSqlLite.Exec("update databases set password=@1 where databaseId=@2", password, id);
            }
            initSettings.loadDatabase = makeDef;
            return id;
        }

        #endregion

        #region settings save and load

        public static void ClearSettings()
        {
            settings.Clear();
            initSettings = new InitSettings();
            QSqlLite.Exec("delete from settings;delete from tables;delete from table_columns;delete from table_aliases;");
        }
        
        public static void SaveSettings(bool shuttingDown)
        {

            lock(settings)
            {
                A.AddToLog("saving settings");
                
                string sql = "delete from settings;";
                foreach (string key in settings.Keys)
                    sql += "insert into settings (name,value) values ('" + key + "', '" + QSqlLite.n(settings[key]) + "');";
                QSqlLite.Exec(sql);
                dirty = false;
                BindingFlags flags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetField | BindingFlags.GetProperty;

                if(A.dbId > 0)
                {
                    sql = "delete from tables;delete from table_columns;delete from table_aliases;";
                    foreach (DbTable table in A.db.tables.Values)
                    {
                        if (table.hasSettings)
                        {
                            sql += QObject.RenderInsert(table, "tables", false, flags);
                            foreach (DbColumn column in table.columns.Values)
                            {
                                if (column.hasSettings)
                                {
                                    sql += QObject.RenderInsert(column, "table_columns", false, flags);
                                }
                            }
                            foreach(DbAlias a in table.aliases.Values)
                            {
                                if(a.hasSettings)
                                {
                                    sql += QObject.RenderInsert(a, "table_aliases", false, flags);
                                }
                            }
                          
                        }
                    }
                    QSqlLite.Exec(sql);
                    if(shuttingDown)
                    {
                        initSettings.shutDownClean = true;
                        sql = "delete from init_settings;" + QObject.RenderInsert(initSettings, "init_settings", false);
                        QSqlLite.Exec(sql);
                    }
                }
            }
           
        }

        #endregion

        #region tabs

        public static void SetTabPositions(Dictionary<int,int> positions)
        {
            string sql = "";
            foreach(int tabId in positions.Keys)
                sql += "update tabs set pos=" + positions[tabId] + " where id=" + tabId + ";";
            if (sql != "")
                QSqlLite.Exec(sql);
        }

        public static int AddQuery(string name = "", string expr = "")
        {
            int pos = 1 + QSqlLite.SqlInt("select max(pos) from tabs where visible<>0 and databaseId=@1", A.dbId);
            //int result = QSqlLite.Exec("update tabs set pos=pos+1;insert into tabs (name) values ('" + QSqlLite.n(name) + "');");
            int result = QSqlLite.Exec("update tabs set pos=pos+1 where databaseId=@1;insert into tabs (name, pos,databaseId) values (@2,@3,@1);", A.dbId, name, pos);
            if (expr != "")
                SaveQuery(result, 0, expr, false);
            return result;
        }

        public static void SetQueryName(int id, string name)
        {
            QSqlLite.Exec("update tabs set name='" + QSqlLite.n(name) + "' where id=" + id);
        }

        public static void CloseQuery(int id, bool makeOpen = false)
        {
            QSqlLite.Exec("update tabs set visible=0 where id=" + id);
        }

        public static void OpenQuery(int id, string sql = "")
        {
            QSqlLite.Exec("update tabs set visible=1 where id=" + id);
            if(sql != "")
                SaveQuery(id, 0, sql, false);
        }

        public static int tabCount
        {
            get { return QSqlLite.SqlInt("select id from tabs where visible and databaseId=@1", A.dbId); }
        }
        
        #endregion

    }
}
