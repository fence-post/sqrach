using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using fp.lib.forms;
using fp.lib.mysql;
using fp.lib;
using fp.lib.dbInfo;

namespace fp.sqratch
{
    public class QueryColumnInfo
    {
        public int maxLength;
        public int width;
        public string label;
        public Type dataType;
        public string sqlDataType;
        public HorizontalAlignment alignment;
        public bool fitData = false;
        public bool pinned = false;
        public int pinnedPosition = -1;
        public int originalPosition = -1;
        public DbColumn dbColumn = null;
        public int position = -1;

        public QueryColumnInfo(int pos, DbColumn dbColumn)
        {
            position = pos;
            position = pos;
            label = dbColumn.name;
            maxLength = label.Length;
            // dbColumn = Parser.GetDbColumnAtIndex(pos, label);
            sqlDataType = dbColumn.dataType.ToString().ToLower();
            dataType = dbColumn.dataType;
            width = defaultWidth;
            // the first column can only be left aligned due to limiation of control
            alignment = pos > 0 && isNumeric ? HorizontalAlignment.Right : HorizontalAlignment.Left;
        }

        public QueryColumnInfo(int pos, string l, string t)
        {
            position = pos;
            label = l;
            maxLength = label.Length;
            // dbColumn = Parser.GetDbColumnAtIndex(pos, label);
            sqlDataType = t.ToLower();
            dataType = QObject.GetTypeForSqlType(sqlDataType);

            width = defaultWidth;
            // the first column can only be left aligned due to limiation of control
            alignment = pos > 0 && isNumeric ? HorizontalAlignment.Right : HorizontalAlignment.Left;
        }

        public void UpdateMaxWidth(int n)
        {
            maxLength = Math.Max(maxLength, n);
        }

        private int _dataWidth = 0;

        public int dataWidth
        {
            get
            {
                if(_dataWidth == 0)
                    _dataWidth = FormsToolbox.GetTextWidth(maxLength + 4, UI.environmentFont);
                return _dataWidth;
            }
        }

        public int defaultWidth
        {
            get
            {
                int result = 100;
                switch (sqlDataType.ToLower())
                {
                    case "int":
                        result = FormsToolbox.GetTextWidth("0000 ", UI.environmentFont);
                        break;
                    case "float":
                    case "long":
                    case "double":
                        result = FormsToolbox.GetTextWidth("00000000 ", UI.environmentFont);
                        break;
                    case "datetime":
                        result = FormsToolbox.GetTextWidth("0/00/0000 00:00:00 AM ", UI.environmentFont);
                        break;
                }

                if(sqlDataType.ToLower() == "varchar")
                {
                    result = 150;
                    DbColumn col = dbColumn;
                    if (col != null && col.columnLength > 0)
                        result = Math.Min(result, FormsToolbox.GetTextWidth(col.columnLength + 4, UI.environmentFont));
                }

                return result;
            }
        }

        public bool isNumeric { get { return "int,long,double,float".Contains(sqlDataType); } }

        public int typeWidth
        {
            get
            {
                return 100;
            }
        }
    }
}
