using System;
using System.Collections.Generic;
using System.Windows.Forms;
using fp.lib;
using fp.lib.forms;

namespace fp.sqratch
{
    public class QueryListView : DarkListView
    {
        public int columnWidthsDirty = -1;
        public bool dirty = false;
        bool updating = false;
            
        public QueryListView()
        {
            OwnerDraw = true;
            DrawItem += new DrawListViewItemEventHandler(OnDrawItem);
            DrawColumnHeader += new DrawListViewColumnHeaderEventHandler(OnDrawColumnHeader);
     
            // Columns.Add("");
            Location = new System.Drawing.Point(46, 111);
            Name = "queryHistoryList";
            ShowItemToolTips = true;
            Size = new System.Drawing.Size(533, 500);
            TabIndex = 0;
            UseCompatibleStateImageBehavior = false;
            FullRowSelect = true;
            View = System.Windows.Forms.View.Details;
            Resize += new System.EventHandler(OnResize);
            ColumnWidthChanged += new ColumnWidthChangedEventHandler(OnColumnWidthChanged);

        }

        public void UpdateUIPreferences()
        {
            BackColor = UI.passiveBackColor;
            ForeColor = UI.passiveForeColor;
            Font = UI.environmentFont;
            Invalidate();
        }

        void OnResize(object sender, EventArgs e)
        {
            if (A.uiLoaded == false || updating)
                return;
            columnWidthsDirty = 0;
        }

        public void OnColumnWidthChanged(object sender, ColumnWidthChangedEventArgs args)
        {
            if (A.uiLoaded == false || updating)
                return;
            columnWidthsDirty = args.ColumnIndex;
        }

        public void UpdateColumnWidths()
        {

            if (updating)
                return;

            if (Columns.Count == 0)
                return;

            // ExtendLastColumn();
            // return;

            updating = true;
            int space = Width;
            // space = T.MinMax(10, space / 2, space);
            Columns[columnWidthsDirty].Width = T.MinMax(10, space / 3, Columns[columnWidthsDirty].Width);
            int spaceAvailable = space - Columns[columnWidthsDirty].Width;
            spaceAvailable -= (Margin.Left + Margin.Right + 2);
            int spaceChange = spaceAvailable / 3; //  (ClientRectangle.Width - (Columns[0].Width + Columns[1].Width + Columns[2].Width + Columns[3].Width)) / 3;
            SetColWidth(0, spaceChange, ref space);
            SetColWidth(1, spaceChange, ref space);
            SetColWidth(2, spaceChange, ref space);
            SetColWidth(3, spaceChange, ref space);
     
            /*
            Columns[0].Width = spacePerCol;
            Columns[1].Width = spacePerCol;
            Columns[2].Width = spacePerCol;
            Columns[3].Width = spacePerCol;
            */

            columnWidthsDirty = -1;
            updating = false;
        }

        void SetColWidth(int col, int dx, ref int spaceLeft)
        {
            int width = Columns[col].Width;
           // int max = Math.Min(spaceLeft, width);
            width = dx;
            // width = T.MinMax(0, max, width);
            if(columnWidthsDirty != col)
                Columns[col].Width = width;
            spaceLeft -= Columns[col].Width;
        }

        public void DeleteSelected()
        {
            foreach (int i in SelectedIndices)
                Settings.DeleteQueryVersion(Convert.ToInt32(Items[i].Tag));
            dirty = true;
        }

        public void UpdateQueryHistory(bool all = false)
        {
            all = true;

            if (A.loading)
                return;
            if (updating)
                return;

            if(Columns.Count == 0)
            {
                Columns.Add("When");
                Columns.Add("Name");
                Columns.Add("Rows", 80, HorizontalAlignment.Right);
                Columns.Add("Time");
     //           Columns.Add("");
            }

            dirty = false;
            updating = true;
            List<QueryHistory> history = S.GetQueryHistory();

            BeginUpdate();
            Items.Clear();
            foreach(QueryHistory item in history)
            { 
                string queryTime = item.ms == 0 ? "" : TimeSpan.FromMilliseconds(item.ms).ToString("mm':'ss':'ff");
                ListViewItem rowItem = this.AddRow(item.whenChanged.ToString("MM/dd HH:mm"), 
                    T.Coalesce(item.label, "untitled"), item.rows.ToString(), queryTime);
                rowItem.Tag = item.queryVersionId;
                rowItem.ToolTipText = item.expr;
                if (item.failed)
                    rowItem.ForeColor = UI.activeRed;
                if (!item.visible)
                        rowItem.ForeColor = rowItem.ForeColor == UI.activeRed ? UI.passiveRed : UI.passivePassiveForeColor;
            }
            columnWidthsDirty = 0;
            updating = false;
            EndUpdate();
        }
    }
}
