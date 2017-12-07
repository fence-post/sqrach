using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using fp.lib;

namespace fp.sqratch
{
    public class RenderResults : IDisposable
    {
        public string beforeColumn = "";
        public string afterColumn = "";
        public string betweenColumns = "";
        public string beforeRow = "";
        public string afterRow = "";
        public string betweenRows = "";
        public string rowTemplate = "";
        public string columnTemplate = "";
        public bool removeManySpaces = false;
        public bool includeNulls = false;
        public bool removeNewLines = true;
        public bool removeNonAscii = true;
        public bool removeTime = false;
        public bool putAllColumnsInQuotes = false;
        public bool putStringColumnsInDoubleQuotes = false;
        public bool putStringColumnsInSingleQuotes = false;
        public bool includeHeaders = false;
        public bool padStrings = false;
        public int maxBufferSize = 10000;
        public bool escapeWhenNecessary = false;
        public int rowsWritten = 0;
        public string filePath;
        public string format;
        public string results { get { return sb.ToString(); } }

        bool usingMemory { get { return filePath == ""; } }
        List<QueryColumnInfo> columns = new List<QueryColumnInfo>();
        StringBuilder sb;
        bool initialized = false;
        string quoteChar;

        string _beforeColumn = "";
        string _afterColumn = "";
        string _betweenColumns = "";
        string _beforeRow = "";
        string _afterRow = "";
        string _betweenRows = "";
        string _runningBeforeRow;

        public RenderResults(string filePath = "")
        {
            this.filePath = filePath;
        }

        public void AddColumn(QueryColumnInfo i)
        {
            columns.Add(i);
        }
 
        void Initialize()
        {
            // formatCombo.Initialize("SQL Insert", "C# Object", "C# Array", "PHP Array", "JSON Array", "JSON Object", "Python List");

            if (format == "JSON Array")
            {
                beforeRow = "[";
                afterRow = "]";
            }
            else if (format == "JSON Object")
            {
                beforeRow = "{";
                afterRow = "}";
            }
            else if (format == "SQL Insert")
            {
                beforeRow = "(";
                afterRow = ")";
            }
            else if (format == "C# Object" || format == "C# Array")
            {
                beforeRow = "{";
                afterRow = "}";
            }
            quoteChar = putStringColumnsInSingleQuotes ? "\'" : "\"";

            _beforeColumn = ReplaceMacros(beforeColumn);
            _afterColumn = ReplaceMacros(afterColumn);
            _betweenColumns = ReplaceMacros(betweenColumns);
            _beforeRow = ReplaceMacros(beforeRow);
            _afterRow = ReplaceMacros(afterRow);
            _betweenRows = ReplaceMacros(betweenRows);
            _runningBeforeRow = "";
            sb = new StringBuilder();
            if (File.Exists(filePath))
                File.Delete(filePath);
            initialized = true;
        }

        public void WriteHeader()
        {
            if (!initialized)
                Initialize();

            List<string> row = new List<string>();
            foreach(QueryColumnInfo c in columns)
                row.Add(c.label);
        
            WriteRow(row);
        }

        
        public void WriteRow(List<string> row)
        {
            if (!initialized)
                Initialize();
            string rowData = "";
            rowData += _beforeRow;
            string _runningBeforeColumn = "";
            for (int i = 0; i < columns.Count; i++)
            {
                rowData += _runningBeforeColumn + _beforeColumn + RenderValue(row[i], columns[i]) + _afterColumn;
                _runningBeforeColumn = _betweenColumns;
            }
            rowData += _afterRow;

            if (rowTemplate != "")
            {
                Template t = new Template(rowTemplate);
                t.SetToken("ROW", rowData);
                rowData = t.Render();
            }

            sb.Append(_runningBeforeRow);
            sb.Append(rowData);
            
            if(sb.Length > maxBufferSize && !usingMemory)
            {
                Flush();
            }
            _runningBeforeRow = _betweenRows;
            rowsWritten++;
        }

        public void Dispose()
        {
            Flush();
        }

        public void Flush()
        {
            if (sb.Length > 0 && !usingMemory)
            {
                File.AppendAllText(filePath, sb.ToString());
                sb.Clear();
            }
        }
        
        string RenderValue(string s, QueryColumnInfo info)
        {

            bool putInQuotes = false;
            if (includeNulls && s == null)
            {
                s = "null";
            }
            else
            {
                if (s == null)
                    s = "";

                if (removeTime && info.dataType == typeof(DateTime))
                {
                    int at = s.IndexOf(" ");
                    if (at > 0)
                        s = s.Substring(0, at);
                }
                if (removeNonAscii)
                {
                    s = Regex.Replace(s, @"[^\u0000-\u007F]+", string.Empty);
                }

                if (removeNewLines)
                {
                    s = s.Replace("\r\n", " ");
                    s = s.Replace("\n", " ");
                    s = s.Replace("\r", " ");
                }
                if (removeManySpaces)
                {
                    s = s.Replace("  ", " ");
                }
                if (escapeWhenNecessary)
                {
                    // https://www.csvreader.com/csv_format.php
                    s = s.Replace("\"", "\"\"");
                    if (s.Contains(","))
                        putInQuotes = true;
                    if (s.Contains("\n"))
                        putInQuotes = true;

                }
                if (info.dataType == typeof(string))
                {
                    if (putStringColumnsInDoubleQuotes)
                        putInQuotes = true;
                    if (putStringColumnsInDoubleQuotes)
                        putInQuotes = true;
                }
                if (padStrings)
                    s = s.PadRight(info.maxLength);
                if (putInQuotes)
                {

                    s = quoteChar + s + quoteChar;
                }
            }
            
            if (format.IsOneOf("JSON Array", "JSON Object"))
            {
                s = QObject.GetValueForJson(info.dataType, s);
                if (format == "JSON Object")
                    s = "\"" + info.label + "\":" + s; 
            }
            else if (format == "SQL Insert")
            {
                s = QObject.GetValueForSql(info.dataType, s);
            }
            else if (format == "C# Object" || format == "C# Array")
            {
                s = QObject.GetValueForCSharp(info.dataType, s);
                if (format == "C# Object")
                    s = info.label + " = " + s;
            }
            
    
            if(columnTemplate != "")
            {
                Template t = new Template(columnTemplate);
                t.SetToken("COLUMN", s);
                s = t.Render();
            }

            return s;
        }

        string ReplaceMacros(string s)
        {
            s = s.Replace("CRLF", "\r\n");
            s = s.Replace("TAB", "\t");
            return s;
        }
    }
}
