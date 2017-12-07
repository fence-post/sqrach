using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using fp.lib;
//using fp.lib.mysql;

namespace fp.lib.dbInfo
{  
    [DebuggerDisplay("{name} (table={table.name} type={dataType.Name})")]
    public class DbColumn : DbObject
    {
        public static AffinityMatrix affinityMatrix;
        public static HashSet<DbColumn> allColumns = new HashSet<DbColumn>();
        public static QDict<string, DbColumn> allWords = new QDict<string, DbColumn>();
        public static QDict<string, DbColumn> allWordStems = new QDict<string, DbColumn>();
        public static QDict<string, DbColumn> allNames = new QDict<string, DbColumn>();
        public static QDict<string, DbColumn> allNameStems = new QDict<string, DbColumn>();

        public bool active { get { return DbTable.dbInfo.activeColumns.Contains(this); } }

        [Persistent]
        public string tableName { get { return table.name; } set { } }
        [Persistent]
        public string columnName { get { return name; } set { } }
        [Persistent]
        public bool bookmarked = false;

        [Persistent]
        public double cardinality;
        [Persistent]
        public double fullness;
        [Persistent]
        public int uniqueValues;
        [Persistent]
        public int nonEmptyValues;
        [Persistent]
        public string columnType;

        public string name;
        public string definition;
        public bool primaryKey;
        public bool otherKey;
        public bool allowNulls;
        public string defaultValue;
        public DbTable table;
        public int columnLength;
        public Type dataType;

        public override string objectName { get => name; }
        bool isNumeric { get { return dataType.IsNumericType(); } }
        public bool likeIdentifier { get { return columnType == "identifier"; } }
       
        public QList<DbTableConstraintColumn> constraints = new QList<DbTableConstraintColumn>();
        public QList<DbTableConstraintColumn> references = new QList<DbTableConstraintColumn>();
        public Affinities affinities;

        // future use:
        double countVariance;
        double valueVariance;
        object _minValue;
        [Persistent]
        public string minValue { get { return _minValue == null ? null : _minValue.ToString(); } set { _minValue = value;  } }
        object _maxValue;
        [Persistent]
        public string maxValue { get { return _maxValue == null ? null : _maxValue.ToString(); } set { _maxValue = value; } }

        public string firstForeignKeyLabel
        {
            get {
                foreach(DbTableConstraintColumn c in constraints.Each())
                    if(c.constraint.isSloppyForeignKey || c.constraint.isForeignKey)
                        return c.referencedColumn.table.name;
                return "";
            }
        }

        #region columnInfo helpers

        string FormatPercent(string label, int x)
        {
            double result = 0;
            if (table.rowCount > 0)
                result = (Convert.ToDouble(x)) / Convert.ToDouble(table.rowCount) * Convert.ToDouble(100);
            return FormatColumnInfo(label, result.ToString("0.##") + "%");
        }

        string FormatColumnInfo(string label, object o)
        {
            string result = "";
            if(o != null)
            {
                if (o.GetType() == typeof(double))
                {

                    double val = Convert.ToDouble(o);
                    if (val != 0)
                        result = val.ToString("0.##");
                }
                else if(o.GetType() == typeof(int))
                {
                    int val = Convert.ToInt32(o);
                    result = val.ToString();
                }
                else
                {
                    result = o.ToString().LimitToLength(60);
                }
            }
            if(result != "")
                return "\n" + label + ": " + result;
            return "";
        }

        #endregion

        string _columnInfo = null;
        public string columnInfo
        {
            get
            {
                if (!DbTable.dbInfo.databaseStructureAnalyzed)
                    return "";
                if (_columnInfo == null && recentlyAnalyzed)
                {
                    string result = "";
                    result += FormatColumnInfo("cardinality", cardinality);
                    result += FormatColumnInfo("fullness", fullness);
                    result += FormatColumnInfo("min", minValue);
                    result += FormatColumnInfo("max", maxValue);
                    result += FormatPercent("empty", table.rowCount - nonEmptyValues);
                    result += FormatPercent("unique", uniqueValues);
                    _columnInfo = result;
                }
                return _columnInfo;
            }
        }

        public DbColumn(DbTable t, string n, string d, int charLen, bool p, bool nu, string def)
        {
            table = t;
            name = n;
            otherKey = false;
            primaryKey = p;
            allowNulls = nu;
            defaultValue = def;
            definition = d;
            dataType = QObject.GetTypeForSqlType(definition);
            columnLength = charLen;
        }
    
        public DbColumn(DbTable t, string n, string d, bool p, bool nu, string def)
        {
            table = t;
            name = n;
            definition = d;
            dataType = QObject.GetTypeForSqlType(definition);
            primaryKey = p;
            otherKey = false;
            allowNulls = nu;
            defaultValue = def;
            int at = definition.IndexOf("(");
            if (at > 0)
            {
                int close = definition.IndexOf(",");
                if (close < 0)
                    close = definition.IndexOf(")");
                string lengthString = definition.Substring(at + 1, close - (1 + at));
                columnLength = lengthString.ToInt(0);
            }
        }

        public override int FindAffinities()
        {
            affinities = new Affinities();
            affinityMatrix.GetAffinities(this, objectNameWords, ref affinities, dataType);

            string possibleColumnTypes = "";
            if (dataType == typeof(Int32))
                possibleColumnTypes = possibleColumnTypes.AppendTo("identifier");
            if (dataType.IsNumericType())
                possibleColumnTypes = possibleColumnTypes.AppendTo("quantity");
            if (dataType == typeof(DateTime))
                possibleColumnTypes = possibleColumnTypes.AppendTo("when");
            if (dataType == typeof(string))
            {
                possibleColumnTypes = possibleColumnTypes.AppendTo("location");
                possibleColumnTypes = possibleColumnTypes.AppendTo("description");
                possibleColumnTypes = possibleColumnTypes.AppendTo("label");
            }
            affinities.SummarizeAffinities("important", "descriptive,payment,value,order,need,status,product,sale,manufacturer,customer,person,buyer,client,need,purchase", true);
            columnType = affinities.GetMaxAffinity(possibleColumnTypes);

            return affinities.Count;
        }

        public override void Dump()
        {
            Log("  " + objectName + " - " + dataType.Name + ":");
            Log("   columnType", columnType, true);
            Log("   min", minValue, true);
            Log("   max", maxValue, true);
            Log("   cardinality", cardinality, true);
            Log("   uniqueValues", uniqueValues, true);
            Log("   countVariance", countVariance, true);
            Log("   valueVariance", valueVariance, true);
            // Log(affinities.Dump("   all affinities"));
        }
        
        public override void Analyze()
        {
            base.Analyze();

            foreach (string st in objectNameStems)
                allWordStems.Add(st, this);
            foreach (string word in objectNameWords)
                allWords.Add(word, this);
            allNames.Add(objectName.ToLower(), this);
            allNameStems.Add(stem.ToLower(), this);
            allColumns.Add(this);
        }

        public override string AnalyzeData()
        {
            string result = "";
            try
            {
                TryAnalyzeData();
                result = table.name + "." + name + " data analyzed";
            }
            catch(Exception e)
            {
                result = "error analyzing data for " + table.name + "." + name + " " + e.Message;
            }
            Analyzed();
            _columnInfo = null;

            return result;
        }

        void TryAnalyzeData()
        {          
            if (table.rowCount > 10000)
                return;

            string sql = " from @1 t where t.@2 is not null";
            if (dataType == typeof(string) || dataType.IsNumericType())
                sql += " and t.@2 <> " + QObject.GetZeroSqlValueForType(dataType);
            nonEmptyValues = dbInfo.SqlInt(true, "select count(*)" + sql, table.objectName, objectName);
            uniqueValues = dbInfo.SqlInt(true, "select count(distinct(t.@2))" + sql, table.objectName, objectName);
            cardinality = T.Normalize(uniqueValues, 0, nonEmptyValues);
            fullness = T.Normalize(nonEmptyValues, 0, table.rowCount);

            if (dataType.IsNumericType())
            {
                AnalyzeData(objectName, false);
            }
            else if(dataType == typeof(DateTime))
            {
                AnalyzeData("to_days(" + objectName + ")", true);
            }
            else if(dataType == typeof(string) && columnLength < 100)
            {
                AnalyzeStringData();
            }
            affinities.Add("groupy", 1 - cardinality);
            affinities.Add("ordery", cardinality);
            affinities.Add("fullness", fullness);
            affinities.SummarizeAffinities("selecty", "descriptive,important,fullness", true);           
        }

        public void AnalyzeStringData()
        {
            using (QSqlBase s = dbInfo.GetSql())
            {
                s.Open(true, "select min(t.@1), max(t.@1) from @2 t where (t.@1 is not null) and t.@1 <> ''", objectName, table.objectName);
                if (s.GetRow())
                {
                    _minValue = s.GetString(0);
                    _maxValue = s.GetString(1);
                }
                if (_maxValue == null || _minValue == null)
                    return;

                string sql = @"
select stddev(ct), stddev(rnk) from
(    
    select count(t.@1) as ct, t.@1, percent_rank as rnk from
    (
        SELECT o.@1, PERCENT_RANK() OVER w AS 'percent_rank' 
        FROM @2 o WINDOW w AS(ORDER BY o.@1)
    ) t
    group by t.@1
) t2
";
                s.Open(true, sql, objectName, table.objectName);
                if(s.GetRow())
                {
                    countVariance = s.GetDouble(0);
                    valueVariance = s.GetDouble(1);
                }
            }
        }

        public List<double> valueVariances = new List<double>();
        public List<double> countVariances = new List<double>();

        public void AnalyzeData(string colExpr, bool round)
        {

            using (QSqlBase s = dbInfo.GetSql())
            {
                s.Open(true, "select min(@1), max(@1) from @2 where (@1 is not null) and @1 <> 0", colExpr, table.objectName);
                if (s.GetRow())
                {
                    _minValue = s.GetDouble(0);
                    _maxValue = s.GetDouble(1);
                }
                if (_maxValue == null || Convert.ToDouble(_maxValue) == 0)
                    return;

                int partitionCount = 10;
                if (table.rowCount >= 100)
                    partitionCount = 100;

                double difference = Convert.ToDouble(_maxValue) - Convert.ToDouble(_minValue);
                if (difference == 0)
                    return;

                double stepSize = table.rowCount / partitionCount;
                if (round)
                    stepSize = Math.Floor(stepSize);
                string sql = "";
                double bottom = Convert.ToDouble(_minValue);
                double top = bottom + stepSize;
                for (int i = 0; i < partitionCount; i++, bottom += stepSize, top += stepSize)
                    sql = sql.AppendTo("select count(*) ct, stddev(@1) dev from @2 where @1>=" + bottom + " and @1<" + top, " union ");

                string tmpSql = sql;

                sql = "select stddev(ct), stddev(dev) from (" + sql + ") t";
                s.Open(true, sql, colExpr, table.objectName);
                if (s.GetRow())
                {
                    countVariance = s.GetDouble(0);
                    valueVariance = s.GetDouble(1);
                }

                if (!DbInfo.analyzeColumnDataContext)
                    return;

                DbTableConstraint constraint = table.GetPrimaryKeyConstraint();
                if (constraint == null)
                    return;

                string primaryKeyColumns = "";
                foreach (DbTableConstraintColumn colConstraint in constraint.constraintColumns.Values)
                    primaryKeyColumns = primaryKeyColumns.AppendTo(colConstraint.dbColumn.name, ",");

                int groupSize = table.rowCount / partitionCount;
                sql = @"
with baseTable as
(
SELECT @rank:= @rank + 1 AS rank, @1
FROM @2,
(SELECT @rank:=0) r
ORDER BY @3
)
select grp, max(@1), min(@1), avg(@1) as avg, stddev(@1), count(*) - count(@1) as nullValues from
(
select rank, floor(rank / @4) as grp, @1 from baseTable
) t
group by grp
";
                double maxCt = 0;
                double maxVal = 0;
                List<double> valList = new List<double>();
                List<double> ctList = new List<double>();
                s.Open(true, sql, colExpr, table.objectName, primaryKeyColumns, groupSize);
                while (s.GetRow())
                {
                    double ct = s.GetDouble(4);
                    double val = s.GetDouble(3);
                    maxCt = Math.Max(maxCt, ct);
                    maxVal = Math.Max(maxVal, val);
                    ctList.Add(ct);
                    valList.Add(val);
                }

                for (int i = 0; i < valList.Count; i++)
                {
                    countVariances.Add(T.Normalize(ctList[i], 0, maxCt));
                    valueVariances.Add(T.Normalize(valList[i], 0, maxVal));
                }

            }
        }
    
    }
}

