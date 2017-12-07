using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using fp.lib;
using fp.lib.mysql;
using System.Diagnostics;
using fp.lib.sqlparser;
using fp.lib.sqlite;
using System.Text;
using fp.lib.forms;


namespace fp.sqratch
{
    public class ResultsListView : DarkListView
    {
        public int rowUnderMouse = -1;
        public int colUnderMouse = -1;
        public bool lastColumnDirty = false;

        Query query { get {
                return mainForm.selectedQuery; } }
        public bool resultsReady = false;
        main mainForm;
        bool loading = false;

        public ResultsListView(main f)
        {
            mainForm = f;
            headerFont = UI.environmentFont;
            OwnerDraw = true;
            DrawItem += new DrawListViewItemEventHandler(OnDrawItem);
            DrawColumnHeader += new DrawListViewColumnHeaderEventHandler(OnDrawColumnHeader);
            Resize += new EventHandler(OnResize);
            ColumnWidthChanged += new ColumnWidthChangedEventHandler(OnColumnWidthChanged);
            Dock = System.Windows.Forms.DockStyle.Fill;
            VirtualMode = true;
            VirtualListSize = 0;
            BorderStyle = System.Windows.Forms.BorderStyle.None;
            FullRowSelect = true;
            HideSelection = false;
            Location = new System.Drawing.Point(0, 0);
            Name = "results";
            ShowItemToolTips = true;
            Size = new System.Drawing.Size(200, 200);
            UseCompatibleStateImageBehavior = false;
            View = System.Windows.Forms.View.Details;
            ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.OnColumnClick);
            RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(OnRetrieveVirtualItem);
        }

        public void UpdateUIPreferences()
        {
            BackColor = UI.passiveBackColor;
            ForeColor = UI.passiveForeColor;
            Font = headerFont = UI.environmentFont;
            Invalidate();
        }

        public void CopyColumnAs(string mode)
        {
            if (query != null && colUnderMouse >= 0 && colUnderMouse < query.columns.Count)
            {
                RenderResults r = new RenderResults();
                r.AddColumn(query.columns[colUnderMouse]);
                r.format = mode;
                r.betweenRows = ",";
                List<string> row = new List<string>();
                foreach (int i in SelectedIndices)
                {
                    row.Clear();
                    row.Add(query.rows[i][colUnderMouse]);
                    r.WriteRow(row);
                }

                Clipboard.SetText(r.results);
            }
        }

        public void CopySelectedRows()
        {
            if (query != null)
            {
                RenderResults r = new RenderResults();
                for (int i = 0; i < query.columns.Count; i++)
                {
                    r.AddColumn(query.columns[i]);
                }
                r.betweenColumns = ",";
                r.betweenRows = "CRLF";
                List<string> row = new List<string>();
                foreach (int i in SelectedIndices)
                {
                    A.SetProgress(i, SelectedIndices.Count, true);

                    row.Clear();
                    for (int j = 0; j < query.columns.Count; j++)
                        row.Add(query.rows[i][j]);
                    r.WriteRow(row);
                }
                A.SetProgress();

                Clipboard.SetText(r.results);
            }
        }


        public void FixForScroll()
        {
            return;
            /*
            if (Columns.Count == 0)
                return;

            int rightmostColumn = Columns.Count - 1;
            int totalSizeUsed = 0;
            int dataSizeUsed = 0;

            foreach (ColumnHeader col in Columns)
            {
                totalSizeUsed += col.Width;
                if (col.DisplayIndex != rightmostColumn)
                    dataSizeUsed += col.Width;
            }

            if(totalSizeUsed >= dataSizeUsed + (SystemInformation.VerticalScrollBarWidth) && ClientRectangle.Width >= dataSizeUsed)
                Columns[rightmostColumn].Width = Math.Max(0, ClientRectangle.Width - dataSizeUsed);
            lastColumnDirty = false;
            */
        }

        public void UpdateLastColumn()
        {
            
            ExtendLastColumn();
            lastColumnDirty = false;
        }

        public void OnResize(object sender, EventArgs args)
        {
            if (loading)
                return;
            if (Columns.Count == 0 || query == null || Columns.Count != query.columns.Count + 1)
                return;
            lastColumnDirty = true;
        }

        
        public void OnColumnWidthChanged(object sender, ColumnWidthChangedEventArgs args)
        {
            if (loading)
                return;
            if (Columns.Count == 0 || query == null || Columns.Count != query.columns.Count + 1)
                return;
            lastColumnDirty = true;
            // ExtendLastColumn();
        }

        public void SelectAll()
        {
            using (new Wait())
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    Items[i].Selected = true;
                    A.SetProgress(i, Items.Count, true);
                }
                A.SetProgress();
            }
        }

        public void InvertSelection()
        {

            HashSet<int> idx = new HashSet<int>();
            int ct = 0;
            int max = SelectedIndices.Count + Items.Count;
            foreach (int i in SelectedIndices)
            {
                A.SetProgress(++ct, max, true);
                idx.Add(i);
            }
            for (int i = 0; i <  Items.Count; i++)
            {
                A.SetProgress(++ct, max, true);
                Items[i].Selected = !idx.Contains(i);
            }
            A.SetProgress();
        }

        public void SelectNone()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                Items[i].Selected = false;
               A.SetProgress(i, Items.Count, true);
            }
            A.SetProgress();

        }

        public string GetColData(int col, string lineSeparator, bool selectedOnly)
        {
            StringBuilder sb = new StringBuilder();
            if (selectedOnly)
            {
                foreach (int i in SelectedIndices)
                    sb.Append(query.rows[i][col] + lineSeparator);
            }
            else
            {

                foreach (List<string> row in query.rows)
                    sb.Append(row[col] + lineSeparator);
            }

            return sb.ToString();
        }

        /*
        public string GetSelectedRowData(string colSeparator, string rowSeparator)
        {
            StringBuilder sb = new StringBuilder();
            int ct = 0;
            foreach (int i in SelectedIndices)
            {
                A.SetProgress(++ct, SelectedIndices.Count, true);
                List<string> row = query.rows[i];
                foreach (string col in row)
                    sb.Append(col + colSeparator);
                sb.Append(rowSeparator);
            }
            A.SetProgress();
            return sb.ToString();
        }
        */

        void OnColumnClick(object sender, ColumnClickEventArgs e)
        {
            this.GetMousePosition();
            
            if (query != null && colUnderMouse >= 0 && (query.columns[colUnderMouse].fitData || ColumnDataTruncated(colUnderMouse)))
                FitDataInColumn(colUnderMouse, true);
        }

        public bool ColumnDataTruncated(int col)
        {
            if (query == null)
                return false;

            int dataWidth = FormsToolbox.GetTextWidth(Math.Min(100, query.columns[col].maxLength), Font);
            return (dataWidth > Columns[col].Width);
        }

        public void FitDataInColumn(int col, bool toggle = false)
        {
            if (query != null && col > 0 && col < query.columns.Count)
            {
                QueryColumnInfo columnInfo = query.columns[col];
                columnInfo.fitData = (toggle && columnInfo.fitData) ? false : true;
                Columns[col].Width = columnInfo.fitData ? Math.Max(columnInfo.defaultWidth, FormsToolbox.GetTextWidth(Math.Min(100, columnInfo.maxLength), Font)) : columnInfo.width;
                Invalidate(true);
            }
        }

        public Point GetMousePosition()
        {
            ListViewHitTestInfo info = HitTest(PointToClient(Cursor.Position));
            int i = info.SubItem == null ? 0 : Convert.ToInt32(info.SubItem.Tag);
            rowUnderMouse = info.Item.Index;
            colUnderMouse = (i >= 0 && query != null && i < query.columns.Count) ? i : -1;
            return new Point(colUnderMouse, rowUnderMouse);
        }

        public int GetPinnedColumns()
        {
            lock (query.columns)
            {
                int result = 0;
                foreach (QueryColumnInfo i in query.columns)
                    if (i.pinned)
                        result++;

                return result;
            }
        }

        public void TogglePinColumnToLeft(int col)
        {
            int pinnedCount = GetPinnedColumns();
            QueryColumnInfo info = query.columns[col];
            info.pinned = !info.pinned;
            int pos = 0;
            for (int i = 0; i < Columns.Count - 1; i++)
                if (query.columns[i].pinned)
                    Columns[i].DisplayIndex = pos++;
            for (int i = 0; i < Columns.Count - 1; i++)
                if (!query.columns[i].pinned)
                    Columns[i].DisplayIndex = pos++;
            Invalidate();
            Update();
        }

        public void ClearResults()
        {
            Items.Clear();
            Columns.Clear();
            VirtualListSize = 0;
        }

        void ExpandAvailableSpaceToFitData()
        {
            int avaliableSpace = ClientRectangle.Width - (SystemInformation.VerticalScrollBarWidth + SystemInformation.VerticalScrollBarWidth);
            int usedSpace = 0;
            int truncatedColumns = 0;
            foreach (QueryColumnInfo info in query.columns)
            {
                usedSpace += info.width;
                if (info.dataWidth > info.width)
                    truncatedColumns++;
            }

            if (avaliableSpace > usedSpace)
            {
                avaliableSpace -= usedSpace;
                foreach (QueryColumnInfo info in query.columns)
                {
                    if (info.dataWidth > info.width)
                    {
                        int max = truncatedColumns > 0 ? (avaliableSpace / truncatedColumns) : avaliableSpace;
                        int toAdd = Math.Min(max, info.dataWidth - info.width);
                        toAdd = Math.Min(toAdd, avaliableSpace);
                        info.width += toAdd;
                        avaliableSpace -= toAdd;
                        truncatedColumns--;
                        Columns[info.position].Width = info.width;
                    }
                    if (avaliableSpace <= 0)
                        break;
                }
            }
        }

        public void LoadResults()
        {
            resultsReady = false;
            // App.StartProgress(App.expectedRows);
            loading = true;
            bool firstTime = false;
            BeginUpdate();
            if (Columns.Count == 0)
            {
                firstTime = true;
                foreach (QueryColumnInfo i in query.columns)
                {
                    ColumnHeader h = new ColumnHeader();
                    h.Name = "column" + i;
                    h.Text = i.label;
                    h.Width = i.width;
                    h.TextAlign = i.alignment;
                    Columns.Add(h);
                }
                Columns.Add("");
            }

            if (VirtualListSize < query.rows.Count)
                VirtualListSize = query.rows.Count;
            if (firstTime)
            {
                ExpandAvailableSpaceToFitData();
                ExtendLastColumn();
                lastColumnDirty = true;
                // Focus();
            }
            Invalidate();
            EndUpdate();
            loading = false;
        }

        void OnRetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            ListViewItem itm = null;
            itm = new ListViewItem("");
            for (int i = 1; i < Columns.Count; i++)
                itm.SubItems.Add("");
            itm.SubItems.Add("");

            if (Columns.Count > 0 && (A.currentStatus == Status.LoadingRows || A.currentStatus == Status.Ready))
            {
                if (e.ItemIndex < query.rows.Count)
                {
                    itm = new ListViewItem(query.rows[e.ItemIndex][0]);
                    for (int i = 1; i < query.columns.Count; i++)
                    {
                        ListViewItem.ListViewSubItem subItem = itm.SubItems.Add(query.rows[e.ItemIndex][i]);
                        subItem.Tag = i;
                    }
                    itm.SubItems.Add("");
                }
            }
            if (itm != null)
            {
                e.Item = itm;
            }
        }
    }
}
