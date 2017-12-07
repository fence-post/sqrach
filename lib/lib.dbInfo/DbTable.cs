using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity.Design;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using fp.lib;
//using fp.lib.mysql;

namespace fp.lib.dbInfo
{
    public class DbTable : DbObject
    {
        public static AffinityMatrix affinityMatrix;

        public bool active { get { return dbInfo.activeTables.Contains(this); } }

        public string tableName;
        public string schemaName;
        public string tableType;
        [Persistent]
        public string name { get { return T.AppendTo(schemaName, tableName, "."); } set { } }
        [Persistent]
        public bool bookmarked;
        public QDict<string, DbColumn> columns = new QDict<string, DbColumn>(QListSort.Descending);
        public QDict<string, DbAlias> aliases = new QDict<string, DbAlias>(QListSort.Descending);
        [Persistent]
        public bool showAllColumns;
        public override string objectName { get { return name; } }
        public QDict<string, DbTableConstraint> constraints = new QDict<string, DbTableConstraint>();
        public QList<DbTableConstraint> references = new QList<DbTableConstraint>();
        public DbTablePaths tablePaths;
        public string lastAlias { get { return aliases.Count > 0 ? aliases[0].alias : null; } }
        public double importance = 0;
        public int tableRows = -1; // this comes from information_schema, but it's not accurate
        int _rowCount = -1;

        [Persistent]
        public int rowCount { get { return _rowCount == -1 ? tableRows : _rowCount; } set { _rowCount = value; } }
        public int avgRowLength;
        public int dataLength;
        public int indexLength;
        public Affinities affinities;
        DbColumn labelColumn;
        string topColumns = "";

        public Dictionary<string, List<DbColumn>> queryColumns = new Dictionary<string, List<DbColumn>>();
      
        public DbTable(string schema, string nam, string typ)
        {
            schemaName = schema;
            tableName = nam;
            tableType = typ;
        }
     
        public int AliasUsed(string alias)
        {
            DbAlias a = aliases.ContainsKey(alias) ? aliases[alias] : null;
            if(a == null)
            {
                a = new DbAlias(alias, this);
                aliases.Add(alias, a);
            }
            int pos = a.Used();
            aliases.SetPosition(alias, pos);

            return pos;
        }

        public string GetAlias(bool markUsed)
        {
            return dbInfo.GetAliasForTable(this.name, markUsed);
        }

        public bool HasBookmarkedOrActiveColumns(bool checkBookmarked = true, bool checkActive = true)
        {
            foreach (DbColumn c in columns.Values)
                if ((checkBookmarked && c.bookmarked) || (checkActive && c.active))
                    return true;
            return false;
        }

        public DbTableConstraint GetForeignKey(string refTable, string refColumn)
        {
            foreach (DbTableConstraint c in constraints.Values)
            {
                if (c.referencedTable != null && c.referencedTable.objectName == refTable)
                {
                    foreach (DbTableConstraintColumn col in c.constraintColumns.Values)
                        if (col.referencedColumn.objectName == refColumn)
                            return c;
                }
            }

            return null;
        }

        public string RenderJoinCols(string otherTable, bool includeAlias)
        {
            string result = "";
            if (tablePaths != null && tablePaths.paths.ContainsKey(otherTable))
                result = tablePaths.paths[otherTable].RenderJoinCols(includeAlias);
            return result;
        }

        public string RenderJoin(string otherTable, bool includeAlias)
        {
            string result = "";
            if (tablePaths != null && tablePaths.paths.ContainsKey(otherTable))
                tablePaths.paths[otherTable].RenderJoin(ref result, includeAlias);
            return result;
        }

        public IEnumerable<string> accessibleTableNames
        {
            get
            {
                List<string> result = new List<string>();
                if (tablePaths != null)
                    result.AddRange(dbInfo.TableNamesSortedByUsage(tablePaths.accessibleTableNames));
                return result;
            }
        }

        public DbTableConstraint GetOrAddConstraint(string nam, string typ)
        {
            return constraints.ContainsKey(nam) ? constraints[nam] : constraints.Add(nam, new DbTableConstraint(nam, typ, this));
        }

        public DbTableConstraint GetPrimaryKeyConstraint()
        {
            foreach (DbTableConstraint constraint in constraints.Values)
                if (constraint.type == "PRIMARY KEY")
                    return constraint;
   
            return null;
        }

        public DbColumn GetSinglePrimaryKeyColumn()
        {
            foreach (DbTableConstraint constraint in constraints.Values)
                if (constraint.type == "PRIMARY KEY" && constraint.constraintColumns.Count == 1)
                    return constraint.constraintColumns[0].dbColumn;

            return null;
        }

        public int FindInferredRelationships()
        {
            DbColumn primaryKeyColumn = GetSinglePrimaryKeyColumn();
            if (primaryKeyColumn == null)
                return 0;

            QList<DbColumn> references = new QList<DbColumn>(QListSort.Descending);
            foreach (DbColumn c in DbColumn.allNames.Each(name.ToLower()))
                references.Add(c, 10);
            foreach (DbColumn c in DbColumn.allNameStems.Each(name.Stem().ToLower()))
                references.Add(c, 7);
            foreach (DbColumn c in DbColumn.allWords.Each(name.ToLower()))
                if (c.likeIdentifier)
                    references.Add(c, 4 - c.objectNameWords.Count);
            foreach (DbColumn c in DbColumn.allWordStems.Each(name.Stem().ToLower()))
                if (c.likeIdentifier)
                    references.Add(c, 3 - c.objectNameWords.Count);

            /*
            if (!primaryKeyColumn.objectName.In("id,code,name,label"))
            {
                if (primaryKeyColumn.dataType == typeof(int) || primaryKeyColumn.dataType == typeof(string))
                {
                    foreach (DbColumn c in DbColumn.allNames.Each(primaryKeyColumn.objectName.ToLower()))
                        references.Add(c, 5);
                }
            }
            */


            // todo: should these fields have zero empty values?

            int ct = 0;
            
            List<string> tablesAdded = new List<string>();
            foreach (DbColumn c in references.Each())
            {
                if (c != primaryKeyColumn && !tablesAdded.Contains(c.objectName))
                {
                    if (c.dataType == primaryKeyColumn.dataType && c.columnLength == primaryKeyColumn.columnLength)
                    {
                        if (c.table.GetForeignKey(objectName, primaryKeyColumn.objectName) == null)
                        {
                            DbTableConstraint constraint = c.table.GetOrAddConstraint("inferred" + objectName, "inferred");
                            constraint.AddInferredRelationship(c.objectName, this, primaryKeyColumn);
                            tablesAdded.Add(c.table.name);
                            ct++;

                        }
                    }
                }
            }

            return ct;
        }

        #region analyze

        public override void Analyze()
        {
            base.Analyze();
            foreach (DbColumn c in columns.Values)
                c.Analyze();
        }

        void MakeColumnsNeedAnalysis()
        {
            foreach (DbColumn c in columns.Values)
                c.lastAnalyzed = 0;

        }

        public override string AnalyzeData()
        {
            int ct = dbInfo.SqlInt("select count(*) from " + objectName);
            if (rowCount != ct)
            {
                MakeColumnsNeedAnalysis();
                rowCount = ct;
            }
            Analyzed();

            return name + " data analyzed";
        }

        public void FindTablePaths()
        {
            tablePaths = new DbTablePaths(this);
        }

        public void AnalyzeAffinities()
        {
            Dictionary<DbColumn, double> label = new Dictionary<DbColumn, double>();
            Dictionary<DbColumn, double> identifier = new Dictionary<DbColumn, double>();
            Dictionary<DbColumn, double> descriptive = new Dictionary<DbColumn, double>();
            Dictionary<DbColumn, double> when = new Dictionary<DbColumn, double>();
            Dictionary<DbColumn, double> category = new Dictionary<DbColumn, double>();
            Dictionary<DbColumn, double> location = new Dictionary<DbColumn, double>();
            Dictionary<DbColumn, double> quantity = new Dictionary<DbColumn, double>();
            Dictionary<DbColumn, double> selecty = new Dictionary<DbColumn, double>();
            Dictionary<DbColumn, double> groupy = new Dictionary<DbColumn, double>();
            Dictionary<DbColumn, double> ordery = new Dictionary<DbColumn, double>();

            if(name == "categories")
            {
                int nn = 0;
            }

            HashSet<DbTable> tables = new HashSet<DbTable>();
           // tables.AddRange(tablePaths.accessibleTables);
            tables.AddIfNotExists(this);

            foreach (DbTable t in tables)
            {
                t.GetTopColumnsByAffinity("label", label, this);
                t.GetTopColumnsByAffinity("identifier", identifier, this);
                t.GetTopColumnsByAffinity("descriptive", descriptive, this);
                t.GetTopColumnsByAffinity("when", when, this);
                t.GetTopColumnsByAffinity("category", category, this);
                t.GetTopColumnsByAffinity("location", location, this);
                t.GetTopColumnsByAffinity("selecty", selecty, this);
                t.GetTopColumnsByAffinity("groupy", groupy, this);
                t.GetTopColumnsByAffinity("ordery", ordery, this);
                t.GetTopColumnsByAffinity("quantity", quantity, this);
            }

            queryColumns.Add("label", GetTopColumns("label", label));
            queryColumns.Add("identifier", GetTopColumns("identifier", identifier));
            queryColumns.Add("descriptive", GetTopColumns("descriptive", descriptive));
            queryColumns.Add("when", GetTopColumns("when", when));
            queryColumns.Add("category", GetTopColumns("category", category));
            queryColumns.Add("location", GetTopColumns("location", location));
            queryColumns.Add("selecty", GetTopColumns("selecty", selecty));
            queryColumns.Add("groupy", GetTopColumns("groupy", groupy));
            queryColumns.Add("ordery", GetTopColumns("ordery", ordery));
            queryColumns.Add("quantity", GetTopColumns("quantity", quantity));
        }
  
        public void FindLabel()
        {
            DbColumn col = null;
            double maxAffinity = -1;

            foreach (DbTableConstraint constraint in constraints.Values)
            {
                for (int i = 0; i < constraint.constraintColumns.Count; i++)
                {
                    if (constraint.constraintColumns[i].nonUnique == false && constraint.constraintColumns[i].dbColumn.dataType == typeof(string))
                    {
                        DbColumn colInfo = constraint.constraintColumns[i].dbColumn;
                        double aff = colInfo.affinities.Get("label");
                        if (aff > maxAffinity)
                        {
                            col = colInfo;
                            maxAffinity = aff;
                        }
                    }
                }
            }
            if (col == null && queryColumns["label"].Count > 0)
                col = queryColumns["label"][0];
            if (col != null)
                labelColumn = col;
        }

        public override int FindAffinities()
        {
            affinities = new Affinities();
            foreach (DbColumn c in columns.Values)
            {
                affinityMatrix.GetAffinities(this, c.objectNameWords, ref affinities, c.dataType, 1);
            }
            affinityMatrix.GetAffinities(this, objectNameWords, ref affinities, null, 50);
            affinities.SummarizeAffinities("important", "descriptive,payment,value,order,need,status,product,sale,manufacturer,customer,person,buyer,client,need,purchase", true);

            return affinities.Count;
        }

        public void GetTopColumnsByAffinity(string affinityTable, Dictionary<DbColumn, double> result, DbTable callingTable)
        {
            // double affinitiesMax = affinities.Normalize();
            double max = 0;
            int distance = 0;
            string path = "";
            if (callingTable != this)
            {
                path = callingTable.tablePaths.GetPathLabel(this.objectName);
                distance = Math.Min(3, 1 + path.CountOccurrances('.'));
            }
            foreach (DbColumn c in columns.Values)
                max = Math.Max(max, c.affinities.Get(affinityTable));

            if (max > 0)
            {
                foreach (DbColumn c in columns.Values)
                {
                    if (c.fullness < 1)
                        continue;
                    double val = c.affinities.Get(affinityTable); //  T.Normalize(c.affinities.Get(affinityTable), 0, max, 10);
                    if (val > 0)
                    {
                        val = T.Normalize(val, 0, max, 10);
                        val -= T.Normalize(distance, 0, 2, 9);
                        result.Add(c, val);
                    }
                }
            }
        }
        
        List<DbColumn> GetTopColumns(string label, Dictionary<DbColumn, double> input)
        {
            List<DbColumn> output = new List<DbColumn>();
            int ct = 10;
            string txt = "";
            for(int i = 0; i < ct && i < input.Keys.Count; i++)
            {
                DbColumn c = input.KeysSortedByValue().ElementAt(i);
             //   string path = c.tableInfo != this ? (c.tableInfo.tablePaths.GetPathLabel(this.objectName) + ".") : "";
                string path = c.table != this ? (tablePaths.GetPathLabel(c.table.objectName) + ".") : "";
                txt += "  " + path + c.objectName + " " + input[c] + "\r\n";
                output.Add(c);
            }
            if(txt != "")
            {
                txt = " top " + label + " columns:\r\n" + txt;
                topColumns += txt;
            }

            return output;
        }

        public void FindQueries()
        {
            
        }

        /*
        List<string> suggestedQueries = new List<string>();

        string RenderQuery(Dictionary<DbColumn, double> list, int ct)
        {
            HashSet<string> tables = new HashSet<string>();
            string cols = RenderColumns(list, tables, ct);
            string from = tablePaths.RenderFrom(tables.ToList());
            return "select " + cols + " " + from;
        }
        
        string RenderColumns(Dictionary<DbColumn, double> list, HashSet<string> tables, int ct)
        {
            string result = "";
            int i = 0;
            foreach (DbColumn c in list.KeysSortedByValue())
            {
                string col = c.objectName;
                if (c.table != this)
                {
                    col = T.AppendTo(c.table.objectName, col, ".");
                    tables.AddIfNotExists(c.table.objectName);
                }
                result = T.AppendTo(result, col, ", ");
                i++;
                if (i > ct)
                    break;
            }

            return result;

        }
        */
        #endregion

        #region dump functions

        public override void Dump()
        {
            Log("");
            Log(objectName + " " + dbInfo.GetAliasForTable(objectName, true));
            foreach (string s in tablePaths.accessibleTableNames)
                Log(" " + s + " " + dbInfo.GetAliasForTable(s, true) + " " + RenderJoin(s, true));
            Log(" accessible tables: \n  " + tablePaths.accessiblePathNames.Join("\n  "));
            // Log(" label column: " + labelColumn.objectName);
            Log(topColumns);
            /*
            Log(" columns");
            foreach (DbColumn c in columns.Values)
                c.Dump();
                */

        }

        #endregion


    }


}
