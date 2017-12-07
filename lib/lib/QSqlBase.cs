using System;
using System.Collections.Generic;
using System.Reflection;
using System.Data.Common;

namespace fp.lib
{
    public class QSqlBase : IDisposable
    {
        public int lastInsertId = 0;
        public dynamic d;
        bool disposed = false;
        Dictionary<string, int> mapFieldNameToIndex = null;

        protected virtual DbDataReader reader
        {
            get { throw new NotImplementedException(); }
        }

        ~QSqlBase()
        {
            if (!disposed)
            {
                // You need to call Dispose() or else you will run into a connection pooling error
                // because connections can not be freed automatically in the destructor.

               T.Debug("Dispose() was not called.");
            }
        }

        public void Dispose()
        {
            OnDispose();
            disposed = true;
        }

        public virtual void OnDispose()
        {

        }

        public int FieldCount
        {
            get { return reader.FieldCount; }
        }

        public int GetScalar(int def, string sql, params object[] args)
        {
            int result = def;
            Open(sql, args);
            if (GetRow())
                result = GetInt(0);
            return result;
        }

        public void Open(bool useRaw, string sql, params object[] args)
        {
            AddArgsToQuery(useRaw, ref sql, args);
            OpenQuery(sql.Trim());
        }

        public void Open(string sql, params object[] args)
        {
            Open(false, sql, args);
        }

        public int Execute(string sql, params object[] args)
        {
            AddArgsToQuery(false, ref sql, args);
            return ExecuteCommand(sql);
        }

        protected virtual int ExecuteCommand(string sql)
        {
            throw new NotImplementedException();
        }
      
        protected static void AddArgsToQuery(bool raw, ref string sql, params object[] args)
        {

            if (args.Length > 0)
            {
                Template t = new Template(sql, true);
                for (int i = 0; i < args.Length; i++)
                    t.SetToken((i + 1).ToString(), raw ? args[i].ToString() : QObject.GetValueForSql(args[i].GetType(), args[i]));
                sql = t.Render();
            }

        }

        protected virtual void OpenQuery(string sSql)
        {
            throw new NotImplementedException();
        }
        
        public dynamic GetRowData()
        {
            if (GetRow(true))
                return d;
            return null;
        }

        /*
        public void PopulatePropertiesFromRow(object o, BindingFlags flags = BindingFlags.Public | BindingFlags.Instance)
        {
            foreach (var prop in o.GetType().GetProperties(flags))
            {
                int i = MapColumnNameToIndex(prop.Name);
                if (i >= 0)
                {

                    object val;
                    switch (prop.PropertyType.Name)
                    {
                        case "String":
                            val = GetString(i);
                            break;
                        case "Int32":
                            val = GetInt(i);
                            break;
                        case "Boolean":
                            val = GetBool(i);
                            break;
                        default:
                            throw new Exception();
                    }

                    prop.SetValue(o, val);
                }
            }
        }

        
        public void PopulateFieldsFromRow(object o, BindingFlags flags = BindingFlags.Public | BindingFlags.Instance)
        {
            foreach (var prop in o.GetType().GetFields(flags))
            {
                int i = MapColumnNameToIndex(prop.Name);
                if (i >= 0)
                {

                    object val;
                    switch (prop.FieldType.Name)
                    {
                        case "String":
                            val = GetString(i);
                            break;
                        case "Int32":
                            val = GetInt(i);
                            break;
                        case "Boolean":
                            val = GetBool(i);
                            break;
                        default:
                            throw new Exception();
                    }
                    
                    prop.SetValue(o, val);
                }
            }
        }
        */
        public bool GetRow(bool loadColumnsAsProperties = false)
        {
            if (reader.Read())
            {
                if (loadColumnsAsProperties)
                {
                    d = new System.Dynamic.ExpandoObject();
                    var dictionary = (IDictionary<string, object>)d;
                    for (int i = 0; i < reader.FieldCount; i++)
                        dictionary.Add(reader.GetName(i), reader.GetValue(i));

                }
                return true;
            }

            return false;
        }

       
        public string this[int i]
        {
            get { return GetString(i); }
        }

        public string this[string nam]
        {
            get
            {
                return GetString(nam);
            }
        }

        public bool IsNull(int nColumn)
        {
            return reader.IsDBNull(nColumn);
        }

        public virtual void CancelQuery()
        {
        }

        protected void CloseReader()
        {
            mapFieldNameToIndex = null;

            if (reader != null)
            {
                reader.Close();
                // reader = null;
            }
        }

        public int MapColumnNameToIndex(string nam)
        {
            if (mapFieldNameToIndex == null)
            {
                mapFieldNameToIndex = new Dictionary<string, int>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    mapFieldNameToIndex.Add(reader.GetName(i), i);
                }
            }
            return mapFieldNameToIndex.ContainsKey(nam) ? mapFieldNameToIndex[nam] : -1;
        }

        public string GetString(string nam)
        {
            return GetString(MapColumnNameToIndex(nam));
        }

        public string GetString(int nColumn)
        {
            if (GetColumnType(nColumn) == "varchar" && IsNull(nColumn) == false)
                return reader.GetString(nColumn);
            else
                return reader.GetValue(nColumn).ToString();
        }
        public bool GetBool(string nam)
        {
            return GetBool(MapColumnNameToIndex(nam));
        }

        public bool GetBool(int n)
        {
            return GetInt(n) != 0;
        }

        public int GetInt(string nam)
        {
            return GetInt(MapColumnNameToIndex(nam));
        }

        public double GetDouble(int nColumn)
        {
            Type t = QObject.GetTypeForSqlType(GetColumnType(nColumn).ToLower());
            if (!t.IsNumericType())
                throw new ArgumentException();
            if (IsNull(nColumn))
                return 0;
            return reader.GetDouble(nColumn);
            
        }

        public int GetInt(int nColumn)
        {
            if (IsNull(nColumn))
                return 0;
            else if (GetColumnType(nColumn) == "int")
                return reader.GetInt32(nColumn);
            else if (GetColumnType(nColumn) == "bit")
                return (reader.GetValue(nColumn).ToString().ToLower() == "true") ? 1 : 0;
            else
            {
                string sValue = reader.GetValue(nColumn).ToString();
                return Convert.ToInt32(sValue);
            }
        }

        public object GetObject(int nColumn)
        {
            return reader.GetValue(nColumn);
        }

        public object GetForType(Type t, int i)
        {
            switch (t.Name)
            {
                case "String":
                    return GetString(i);
                case "Int32":
                    return GetInt(i);
                case "Boolean":
                    return GetBool(i);
                case "DateTime":
                    return Convert.ToDateTime(GetString(i));
            }

            return null;
        }

        public string GetColumnType(int nCol)
        {
            return reader.GetDataTypeName(nCol);
        }


        public string GetColumnName(int nCol)
        {
            return reader.GetName(nCol);
        }

     
        public static string n(string t)
        {
            return Normalize(t);
        }

        public static string Normalize(string sText)
        {
            sText = sText.Replace("\"", "\'");
            sText = sText.Replace("\'", "\'\'");

            return sText;
        }
    }
}
