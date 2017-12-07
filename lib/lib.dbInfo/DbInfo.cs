using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;

namespace fp.lib.dbInfo
{
    public class DbInfo : DbObject
    {
        const bool analyzeTableColumnAffinities = true;
        const bool analyzeColumnData = true;
        const bool rankTables = true;

        public const string PrimaryKeywords = "select from where group order limit";
        public const string SecondaryKeywords = "left right inner outer join by as having on and or not in union";
        public const string LogicKeywords = "like column desc distinct asc rowcount unique count top case if else then coalesce cast charindex ceiling coalesce contains convert floor isnull max min nullif";
        public const string DatatypeKeywords = "datetime int money double float decimal nchar ntext numeric smalldatetime smallint smallmoney text tinyint timestamp varbinary varchar";
        public const string Punctuation = ", ;";
        public const string Operators = "= <> < >";

        public static int dbId;
        public static HashSet<string> sqlKeywords = new HashSet<string>();
        public static bool analyzeColumnDataContext = false;
        public static bool findInferredRelationships = false;

        public virtual string databaseType { get { throw new NotImplementedException(); } }
        public HashSet<DbTable> activeTables = new HashSet<DbTable>();
        public HashSet<DbColumn> activeColumns = new HashSet<DbColumn>();

        public void ClearActiveObjects()
        {
            activeTables.Clear();
            activeColumns.Clear();
        }

        public QDict<string, DbTable> tablesByAlias = new QDict<string, DbTable>(QListSort.Descending);
        public QDict<string, DbTable> tablesByColumnName = new QDict<string, DbTable>(QListSort.Descending);
        public QDict<string, DbTable> tables = new QDict<string, DbTable>(QListSort.Descending);
        public override string objectName { get { return databaseName; } }
        public string databaseName { get; set; }

        Dictionary<string, string> proposedAliases = new Dictionary<string, string>();

        public DbInfo()
        {
            DbObject.dbInfo = this;
            if(sqlKeywords.Count == 0)
            {
                sqlKeywords.AddRange(DbInfo.PrimaryKeywords.Split(' '));
                sqlKeywords.AddRange(DbInfo.SecondaryKeywords.Split(' '));
                sqlKeywords.AddRange(DbInfo.LogicKeywords.Split(' '));
                sqlKeywords.AddRange(DbInfo.DatatypeKeywords.Split(' '));
            }
        }

        #region alias and object usage

        public void AliasUsed(string alias, string table)
        {
            DbTable t = tables[table];
            tablesByAlias.AddIfNotExists(alias, t);
            tablesByAlias.SetPosition(alias, t.AliasUsed(alias));
            activeTables.AddIfNotExists(t);
        }

        public void TableUsed(string name)
        {
            activeTables.AddIfNotExists(tables[name]);
            tables.SetPosition(name, tables[name].Used());
        }

        public void ColumnUsed(DbColumn col)
        {
            activeColumns.AddIfNotExists(col);
            col.table.columns.SetPosition(col.name, col.Used());
        }

        public string GetTableNameByAlias(string alias, string def = "")
        {
            return tablesByAlias.ContainsKey(alias) ? tablesByAlias[alias].name : def;
        }

        public IEnumerable<string> GetPossibleTablesForAlias(string alias)
        {
            HashSet<string> result = new HashSet<string>();
            foreach (string t in proposedAliases.Keys)
                if (proposedAliases[t] == alias)
                    result.AddIfNotExists(t);

            if(result.Count == 0)
            {
                foreach(DbTable t in tables.Values)
                {
                    if (t.name.StartsWith(alias) && t.GetAlias(false) == alias)
                        result.AddIfNotExists(t.name);

                }
            }

            return result;
        }

        bool AliasUsedByAnotherActiveTable(string alias, string table)
        {
            if (sqlKeywords.Contains(alias))
                return true;

            foreach (DbTable t in activeTables)
                if (t.name != table && t.lastAlias == alias)
                    return true;
            
            return false;
        }

        public string GetAliasForTable(string table, bool makeUsed = true)
        {
            string result = null;

            if (result == null)
            {
                DbTable dbTable = tables[table];
                if (dbTable.lastAlias != null && !AliasUsedByAnotherActiveTable(dbTable.lastAlias, table))
                    result = dbTable.lastAlias;
                else
                {
                    foreach (string a in dbTable.aliases.Keys)
                        if (!AliasUsedByAnotherActiveTable(a, table))
                        {
                            result = a;
                            break;
                        }
                }
            }

            if (result == null)
            {

                if (proposedAliases.ContainsKey(table))
                {
                    if (AliasUsedByAnotherActiveTable(proposedAliases[table], table))
                        proposedAliases.Remove(table);
                    else
                        result = proposedAliases[table];
                }
            }
            if (result == null)
            {
                string txt = T.CamelCaseToDbCase(table);
                txt = txt.Replace("__", "_");
                string[] tokens = txt.Split('_');
                for (int i = 1; i < 5; i++)
                {
                    string alias = "";
                    foreach (string word in tokens)
                        alias += word.Left(i);
                    alias = alias.ToLower();
                    if (!AliasUsedByAnotherActiveTable(alias, table))
                    {
                        proposedAliases.Add(table, alias);
                        result = alias;
                        break;
                    }
                }

            }

            T.Assert(result != "");
            T.Assert(result != null);
            if (makeUsed)
            {
                AliasUsed(result, table);
            }
            return result;
        }
        
        #endregion

        public string MakeFrom(List<string> tableList, bool includeAlias)
        {
            Dictionary<string, string> joins = new Dictionary<string, string>();
            
            foreach(string table in tableList)
            {
                string bestDestination = null;
                string bestJoin = null;
                foreach (string toTable in tableList)
                {
                    if (table != toTable && joins.ContainsKey(toTable) == false)
                    {
                        string join = tables[table].RenderJoin(toTable, includeAlias);
                        if (join != "" && (bestJoin == null || bestJoin.CountOccurrances('.') > join.CountOccurrances('.')))
                        {
                            bestDestination = toTable;
                            bestJoin = join;
                        }
                    }
                }
                if (bestJoin != null)
                    joins.Add(bestDestination, bestJoin);
            }

            string result = "";
            foreach (string table in tableList)
            {
                string tableAndAlias = table + (includeAlias ? " " + GetAliasForTable(table) : "");

                if (result.Contains(" " + tableAndAlias + " ") || result.Contains(" " + tableAndAlias + ","))
                    continue;
                if (result == "")
                    result = " " + tableAndAlias + " ";
                else
                {
                    if (joins.ContainsKey(table))
                        result = result.TrimEnd() + " " + joins[table];
                    else
                        result = result.TrimEnd() + ", " + tableAndAlias;
                }
            }
            
            return result.Trim();
        }
        
        public virtual int SqlInt(string sql)
        {
            throw new NotImplementedException();
        }

        public virtual int SqlInt(bool useRawValues, string sql, params object[] args)
        {
            throw new NotImplementedException();
        }
        
        public virtual QSqlBase GetSql()
        {
            throw new NotImplementedException();
        }
        
        public virtual void LoadStructure()
        {
            throw new NotImplementedException();
        }

        public virtual void FindExplicitRelationships()
        {
           
        }
        
        public int GetExpectedRowsInQuery(string sql)
        {
            int result = 0;
            try
            {
                using (QSqlBase s = GetSql())
                {
                    s.Open("select count(*) from (" + sql + ") t1010ee");
                    if (s.GetRow())
                        result = s.GetInt(0);
                }
            }
            catch (Exception ex)
            {

            }

            return result;
        }

        public IEnumerable<string> TableNamesSortedByUsage(IEnumerable<string> list)
        {
            Dictionary<string, int> tablePositions = new Dictionary<string, int>();
            foreach (string tableName in list)
                tablePositions.Add(tableName, tables[tableName].usage);

            return tablePositions.KeysSortedByValue();
        }

            public void SortObjects()
        {
            Dictionary<string, int> tablePositions = new Dictionary<string, int>();
            foreach (string tableName in tables.Keys)
            {
                tablePositions.Add(tableName, tables[tableName].usage);
                Dictionary<string, int> columnPositions = new Dictionary<string, int>();
                foreach (string column in tables[tableName].columns.Keys)
                    columnPositions.Add(column, (tables[tableName].columns[column].primaryKey ? 500 : 0) +
                        tables[tableName].columns[column].usage);
                lock (tables[tableName].columns)
                {
                    tables[tableName].columns.SetPositions(columnPositions);
                }
            }
            tables.SetPositions(tablePositions);
        }

        #region analysis 

        List<DbTable> tablesToBeAnalyzed = new List<DbTable>();
        List<DbColumn> columnsToBeAnalyzed = new List<DbColumn>();

        public bool databaseStructureAnalyzed = false;

        public virtual void AnalyzeDatabaseStructure()
        {
            DbTable.affinityMatrix = new AffinityMatrix();
            string words = System.IO.File.ReadAllText("affinities.txt");
            DbTable.affinityMatrix.AddWords(words);
            DbColumn.affinityMatrix = new AffinityMatrix();
            DbColumn.affinityMatrix.AddWords(words);
            DbColumn.allColumns.Clear();
            DbColumn.allWords.Clear();
            DbColumn.allWordStems.Clear();
            DbColumn.allNames.Clear();
            DbColumn.allNameStems.Clear();

            base.Analyze();

            AnalyzeStructure();

            foreach (DbTable table in tables.Values)
                table.Analyze();

            foreach (DbColumn c in DbColumn.allColumns.ToList())
                c.FindAffinities();

            if(findInferredRelationships)
            {
                foreach (DbTable t in tables.Values)
                    t.FindInferredRelationships();
            }

            foreach (DbTable t in tables.Values)
                t.FindTablePaths();

            tablesToBeAnalyzed.AddRange(tables.Values);
            databaseStructureAnalyzed = true;
        }

        public override void Dump()
        {
            foreach (DbTable t in tables.Values)
                t.Dump();
        }

        public virtual bool AnalyzeDatabaseData(out string msg)
        {
            msg = null;
            if (tablesToBeAnalyzed.Count > 0)
            {
                DbTable t = tablesToBeAnalyzed.First();
                tablesToBeAnalyzed.RemoveAt(0);
                msg = t.AnalyzeData();
                return true;
            }
            else if (columnsToBeAnalyzed.Count > 0)
            {
                DbColumn c = columnsToBeAnalyzed.First();
                columnsToBeAnalyzed.RemoveAt(0);
                msg = c.AnalyzeData();
                return true;
            }
            else
            {
                foreach (DbColumn c in DbColumn.allColumns)
                    if (c.lastAnalyzed == 0)
                        columnsToBeAnalyzed.Insert(0, c);

                if (columnsToBeAnalyzed.Count > 0)
                    return true;

                foreach (DbTable t in tables.Values)
                {
                    if (t.lastAnalyzed == 0)
                        tablesToBeAnalyzed.Insert(0, t);
                    else if (!t.recentlyAnalyzed)
                        tablesToBeAnalyzed.Add(t);
                }
                if (tablesToBeAnalyzed.Count > 0)
                    return true;

                foreach (DbColumn c in DbColumn.allColumns)
                    if (!c.recentlyAnalyzed)
                        columnsToBeAnalyzed.Add(c);

                if (columnsToBeAnalyzed.Count > 0)
                    return true;
            }

            return false;
        }

        public virtual void AnalyzeSummarize()
        {
            if (analyzeTableColumnAffinities)
            {
                foreach (DbTable t in tables.Values)
                {
                    t.FindAffinities();
                    t.AnalyzeAffinities();
                }
            }

            if (rankTables)
            {
                QDict<string, DbTable> rankList = new QDict<string, DbTable>(QListSort.Descending);

                QList<string> list = new QList<string>(QListSort.Descending);
                foreach (DbTable t in tables.Values)
                {
                    rankList.Add(t.name, t);
                    t.affinities.Add("rowCount", t.rowCount);
                    if (t.references.Count > 0)
                        t.affinities.Add("references", t.references.Count);
                }

                RankTablesByAffinity("references", 5, 7);
                RankTablesByAffinity("important", 5, 3);
                RankTablesByAffinity("descriptive", 5, 3);
                RankTablesByAffinity("label", 5, 3);
                //RankTablesByAffinity("quantity", 5, 2);
                RankTablesByAffinity("rowCount", 5, 2);
                foreach (DbTable t in tables.Values)
                    rankList.SetPosition(t.objectName, Convert.ToInt32(1000 * t.importance));

                Log("table rank");
                foreach(string nam in rankList.Keys)
                {
                    Log(" " + nam);
                }
            }

          
            
        }

        void RankTablesByAffinity(string affinityTableName, int ct, double weight)
        {
            QList<string> top = GetTop(affinityTableName, ct);
            int rank = ct;
            foreach (string t in top.Each())
                tables[t].importance += T.Normalize(rank--, 0, ct, weight);
        }

        int GetTableReferenceCount()
        {
            int result = 0;
            foreach (DbTable t in tables.Values)
                result += t.references.Count;
            return result;
        }

        QList<string> GetTop(string table, int ct)
        {
            QList<string> list = new QList<string>(QListSort.Descending);
            foreach (DbTable t in tables.Values)
            {
                list.Add(t.objectName, t.affinities.Get(table));
            }
            LogTop(table, list, ct);
            return list;
        }

        public static void LogTop(string table, QList<string> list, int ct)
        {
            Log("top " + table);

            int i = 0;
            foreach (string t in list.Each())
            {

                Log(" " + t + " " + list.GetPosition(t));
                i++;
                if (i >= ct)
                    break;
            }

        }

        protected virtual void AnalyzeStructure()
        {
        }

    }
    #endregion

}