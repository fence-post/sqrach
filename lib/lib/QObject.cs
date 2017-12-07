using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;

namespace fp.lib
{
    /*
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class PrimaryKeyAttribute : Attribute
    {
    }
    */

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class PersistentAttribute : Attribute
    {
        bool persistent = true;
        bool primaryKey = false;
        public bool isPrimaryKey { get { return primaryKey; } set { primaryKey = value; } }
        public bool isPersistent { get { return persistent; } set { persistent = value; } }
        // todo specify the column name and default value
    }

    public class QObject
    {
        public static void PopulateFromRow(QSqlBase s, object o,
            BindingFlags flags = BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance)
        {
            bool getFields, getProperties, declaredOnly;
            GetFlags(ref flags, out getFields, out getProperties, out declaredOnly);

            if (getProperties)
            {
                foreach (var prop in o.GetType().GetProperties(flags))
                {
                    bool hasPersistentAttribute = false;
                    if (prop.IsDefined(typeof(PersistentAttribute), true))
                    {
                        if (!prop.GetCustomAttribute<PersistentAttribute>(true).isPersistent)
                            continue;
                        hasPersistentAttribute = true;
                    }
                    if (declaredOnly == false || hasPersistentAttribute)
                    {
                        int i = s.MapColumnNameToIndex(prop.Name);
                        if (i >= 0)
                        {
                            object val = s.GetForType(prop.PropertyType, i);
                            if(val != null)
                                prop.SetValue(o, val);
                        }
                    }
                }
            }
            if (getFields)
            {
                foreach (var prop in o.GetType().GetFields(flags))
                {
                    bool hasPersistentAttribute = false;
                    if (prop.IsDefined(typeof(PersistentAttribute), true))
                    {
                        if (!prop.GetCustomAttribute<PersistentAttribute>(true).isPersistent)
                            continue;
                        hasPersistentAttribute = true;
                    }

                    if (declaredOnly == false || hasPersistentAttribute)
                    {
                        int i = s.MapColumnNameToIndex(prop.Name);
                        if (i >= 0)
                        {
                            object val = s.GetForType(prop.FieldType, i);
                            if (val != null)
                                prop.SetValue(o, val);
                        }
                    }
                }
            }

        }

        public static string RenderInsert(object o, string tableName = null, bool includeDelete = false,
            BindingFlags flags = BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance)
        {
            bool translateColumnNames = false;
      
            bool getFields, getProperties, declaredOnly;
            GetFlags(ref flags, out getFields, out getProperties, out declaredOnly);
      
            Type t = o.GetType();
            if(tableName == null)
                tableName = t.Name.ToLower();

            string columns = "";
            string values = "";
            string delete = "";

            if(getProperties)
            {
                foreach (PropertyInfo pi in t.GetProperties(flags))
                {
                    string columnName = translateColumnNames ? T.CamelCaseToDbCase(pi.Name) : pi.Name;
                    bool hasPersistentAttribute = false;
                    bool hasPrimaryKeyAttribute = false;
                    if (pi.IsDefined(typeof(PersistentAttribute), true))
                    {
                        PersistentAttribute attr = pi.GetCustomAttribute<PersistentAttribute>(true);
                        if (!attr.isPersistent)
                            continue;
                        if (attr.isPrimaryKey)
                            hasPrimaryKeyAttribute = true;
                        hasPersistentAttribute = true;
                    }

                    if (declaredOnly == false || hasPersistentAttribute)
                    {
                        string val = GetValueForSql(pi.PropertyType, pi.GetValue(o));
                        if(val != null)
                        {
                            if (hasPrimaryKeyAttribute)
                                delete = T.AppendTo(delete, columnName + "=" + val, " and ");
                            columns = columns.AppendTo(columnName, ",");
                            values = values.AppendTo(val, ",");
                        }
                    }
                }
            }

            if (getFields)
            {
                foreach (FieldInfo fi in t.GetFields(flags))
                {
                    string columnName = translateColumnNames ? T.CamelCaseToDbCase(fi.Name) : fi.Name;
                    bool hasPersistentAttribute = false;
                    bool hasPrimaryKeyAttribute = false;
                    if (fi.IsDefined(typeof(PersistentAttribute), true))
                    {
                        PersistentAttribute attr = fi.GetCustomAttribute<PersistentAttribute>(true);
                        if (!attr.isPersistent)
                            continue;
                        if (attr.isPrimaryKey)
                            hasPrimaryKeyAttribute = true;
                        hasPersistentAttribute = true;
                    }

                    if (declaredOnly == false || hasPersistentAttribute)
                    {
                        string val = GetValueForSql(fi.FieldType, fi.GetValue(o));
                        if(val != null)
                        {
                            if (hasPrimaryKeyAttribute)
                                delete = T.AppendTo(delete, columnName + "=" + val, " and ");
                            columns = columns.AppendTo(columnName, ",");
                            values = values.AppendTo(val, ",");
                        }
                    }
                }
            }

            string sql = "";
            if (includeDelete && delete != "")
                sql += "delete from " + tableName + " where " + delete + ";";
            sql += "insert into " + tableName + " (" + columns + ") values (" + values + ");";

            return sql;

        }

        static void GetFlags(ref BindingFlags flags, out bool getFields, out bool getProperties, out bool declaredOnly)
        {
            getFields = (flags & BindingFlags.GetField) != 0;
            getProperties = (flags & BindingFlags.GetProperty) != 0;
            declaredOnly = (flags & BindingFlags.DeclaredOnly) != 0;

            flags = flags & ~BindingFlags.GetField;
            flags = flags & ~BindingFlags.GetProperty;
            flags = flags & ~BindingFlags.DeclaredOnly;
        }


        static string GetColumnDefinitionForField(FieldInfo fi, bool notNull)
        {
            return GetColumnDefinitionForType(fi.FieldType, notNull);
        }

        static string GetColumnDefinitionForProperty(PropertyInfo pi, bool notNull)
        {
            return GetColumnDefinitionForType(pi.PropertyType, notNull);
        }

        static string GetColumnDefinitionForType(Type t, bool notNull)
        {
            string n = notNull ? " not null " : "";
            if (t == typeof(string))
                return "varchar(100)" + n;
            if (t == typeof(int))
                return "int" + n;
            if (t == typeof(bool))
                return "bit" + n;
            if (t == typeof(DateTime))
                return "datetime" + n;

            throw new ArgumentException();
        }

        public static string GetZeroSqlValueForType(Type t)
        {
            if (t == typeof(string))
                return GetValueForSql(t, default(string));
            if (t == typeof(bool))
                return GetValueForSql(t, default(bool));
            if (t == typeof(DateTime))
                return GetValueForSql(t, default(DateTime));
            if (t.IsNumericType())
                return GetValueForSql(t, 0);

            throw new ArgumentException();

        }
        
        public static Type GetTypeFromName(string nam)
        {
            switch (nam.ToLower())
            {
                case "string":
                    return typeof(string);
                case "bool":
                case "boolean":
                    return typeof(bool);
                default:
                    return GetTypeForSqlType(nam);

            }
        }

        public static Type GetTypeForSqlType(string def)
        {
            switch(def.RemoveAfter("(").ToLower())
            {
                case "varchar":
                case "mediumtext":
                case "char":
                case "text":
                case "xml":
                case "nvarchar":
                case "nchar":
                case "ntext":
                case "enum":
                case "set":
                    return typeof(string);
                case "double":
                    return typeof(double);
                case "long":
                case "bigint":
                    return typeof(long);
                case "decimal":
                    return typeof(decimal);
                case "int":
                case "integer":
                case "year":
                case "mediumint":
                case "tinyint":
                case "smallint":
                case "blob":
                    return typeof(int);
                case "date":
                case "timestamp":
                case "datetime":
                    return typeof(DateTime);

                case "bit":
                    return typeof(bool);

                default:
                    return typeof(string);
            }

            throw new ArgumentException();
        }

        public static string GetValueForJson(Type t, object val)
        {
            if (t == typeof(string))
                return "\"" + (val == null ? "" : val.ToString()) + "\"";
            if (t == typeof(bool))
                return (val != null && Convert.ToBoolean(val)) ? "true" : "false";
            if (t == typeof(DateTime))
                return "\"" + Convert.ToDateTime(val).ToString("yyyy-MM-dd HH:mm:ss") + "\"";
            else if(t.IsNumericType())
                return val == null ? "" : val.ToString();

            return null;
        }

        public static string GetValueForCSharp(Type t, object val)
        {
            if (t == typeof(string))
                return "\"" + (val == null ? "" : val.ToString()) + "\"";
            else if (t == typeof(bool))
                return (val != null && Convert.ToBoolean(val)) ? "True" : "False";
            else if (t == typeof(DateTime))
            {
                if (val == null || val.ToString() == "")
                    return null;

                DateTime d = Convert.ToDateTime(val);
                string s = d.Year + "," + d.Month + "," + d.Day;
                 return "new DateTime(" + s + ")";
            }
            else if (t.IsNumericType())
                return val == null ? "null" : val.ToString();
                
            return null;
        }

        public static string GetValueForSql(Type t, object val)
        { 
            if (t == typeof(string))
                return "'" + (val == null ? "" : QSqlBase.Normalize(val.ToString())) + "'";
            else if (t == typeof(bool))
                return (val != null && Convert.ToBoolean(val)) ? "1" : "0";
            else if (t == typeof(DateTime))
                return (val == null || val.ToString() == "") ? null : Convert.ToDateTime(val).ToString("yyyy-MM-dd HH:mm:ss");
            else if (t.IsNumericType())
                return val == null ? "null" : val.ToString();
            return null;
        }
    }
}
