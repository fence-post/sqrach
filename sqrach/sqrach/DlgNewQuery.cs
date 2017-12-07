using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using fp.lib;
using fp.lib.dbInfo;
using fp.lib.forms;

namespace fp.sqratch
{
    
    public partial class DlgNewQuery : Form
    {
        #region list item helper classes 

        class ListDbTable
        {
            public static bool includeAlias;
            public string tableAlias { get { return includeAlias ? A.db.GetAliasForTable(dbTable.name, true) : ""; } }
            // public string tableAlias { get { return ""; } }
            public string DisplayMember { get { return T.AppendTo(dbTable.name, tableAlias, " "); } }
            public DbTable ValueMember { get { return dbTable; } }
            public DbTable dbTable;
            
            public ListDbTable(DbTable t)
            {
                dbTable = t;
            }
        }

        class ListDbColumn
        {
            public static bool includeAlias;
            public string DisplayMember { get { return T.AppendTo(tableAlias, column.name, "."); } }
            public string tableAlias { get { return includeAlias ? A.db.GetAliasForTable(column.table.name, true) : ""; } }
            //public string tableAlias { get { return ""; } }
            public DbColumn ValueMember { get { return column; } }
            public DbColumn column;
            public DbTable table;

            public ListDbColumn(DbTable t, DbColumn c)
            {
                table = t;
                column = c;
            }
        }
        #endregion

        public bool includeWorkingObjects = false;
        public string sql;
        public HashSet<DbTable> workingTables = new HashSet<DbTable>();
        public HashSet<DbColumn> workingColumns = new HashSet<DbColumn>();
        bool loaded = false;
        bool listsLoaded = false;
        ScintillaNET.Scintilla sqlEditor;
        List<ListDbTable> tables = new List<ListDbTable>();
        List<ListDbColumn> columns = new List<ListDbColumn>();
        string queryType { get { return queryTypeCombo.SelectedItem.ToString(); } }
        Dictionary<string, ListBox> columnListBoxes = new Dictionary<string, ListBox>();
        Dictionary<string, TabPage> columnTabPages = new Dictionary<string, TabPage>();
        // ObjectSortMode tableSortMode = ObjectSortMode.WhenUsed;

        public DlgNewQuery(List<DbTable> tables, List<DbColumn> columns)
        {
            if (tables != null)
            {
                workingTables.AddRange(tables);
                if (columns != null)
                    workingColumns.AddRange(columns);
            }

            InitializeComponent();
            queryTypeCombo.Items.AddRange(new string[] { "Select", "Insert", "Replace Into", "Update", "Delete" });
            queryTypeCombo.SelectedIndex = 0;
            sqlEditor = new ScintillaNET.Scintilla();
            sqlEditor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            sqlEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            sqlEditor.Location = new System.Drawing.Point(0, 0);
            sqlEditor.Name = "sqlEditor";
            sqlEditor.Size = new System.Drawing.Size(300, 300);
            sqlEditor.TabIndex = 18;
            sqlEditor.WrapMode = ScintillaNET.WrapMode.Word;
            previewFrame.Controls.Add(sqlEditor);

            UI.InitializeEditor(sqlEditor);
            Font = SystemFonts.MessageBoxFont;
            loaded = true;

            showAllColumns.Visible = workingColumns.Count > 0;
            showAllColumns.Checked = S.Get("NewQueryShowAllColumns", false);
            includeAllCheckbox.Visible = workingTables.Count > 0;
            includeAllCheckbox.Checked = S.Get("NewQueryIncludeWorkingObjects", true);

            bool selectAll = columnListBoxes.ContainsKey("select") && workingColumns.Count > 0 && workingTables.Count > 0 && workingTables.Count < 3;
            UpdateTableList();
            UpdateColumnTabs();
            if (selectAll)
                tablesList.SelectAll();
            UpdateColumnLists();
            if(selectAll)
                columnListBoxes["select"].SelectAll();

            listsLoaded = true;
           // UpdateEditor();

        }
        
        void UpdateTableList()
        {
            tables.Clear();
            foreach (string tableName in A.db.tables.Keys)
            {
                DbTable table = A.db.tables[tableName];
                if (workingTables.Count > 0 && !workingTables.Contains(table))
                    continue;
                tables.Add(new ListDbTable(table));
            }
            tablesList.SetDataSource(tables, "DisplayMember", "ValueMember");
            includeAllCheckbox.Checked = workingTables.Count > 0;
        }

        #region column tabs
        
        void UpdateColumnLists()
        {
            if (!loaded)
                return;

            bool showAllColumns = workingColumns.Count == 0 || S.Get("NewQueryShowAllColumns", false);
            columns.Clear();
            foreach (int i in tablesList.SelectedIndices)
            {
                ListDbTable table = tablesList.Items[i] as ListDbTable;
                foreach (DbColumn column in table.dbTable.columns.Values)
                {
                    if (showAllColumns == false && !workingColumns.Contains(column))
                        continue;
                    columns.Add(new ListDbColumn(table.dbTable, column));
                }
            }
            foreach (ListBox list in columnListBoxes.Values)
                list.SetDataSource(columns, "DisplayMember", "ValueMember");
        }

        void AddColumnTabPage(string title)
        {
            string name = title.ToLower().Replace(" ", "");
            ListBox columnListBox = new ListBox();
            columnListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            columnListBox.FormattingEnabled = true;
            columnListBox.ItemHeight = 31;
            columnListBox.Location = new System.Drawing.Point(0, 0);
            columnListBox.Name = name + "ColumnList";
            columnListBox.Size = new System.Drawing.Size(100, 100);
            columnListBox.TabIndex = 0;
            columnListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            columnListBox.DisplayMember = "DisplayMember";
            columnListBox.ValueMember = "ValueMember";
            columnListBox.DataSource = columns;
            columnListBox.SelectedIndex = -1;
            columnListBox.IntegralHeight = false;
            columnListBox.BorderStyle = BorderStyle.None;

            columnListBox.SelectedIndexChanged += new System.EventHandler(columnList_SelectedIndexChanged);
            columnListBoxes.Add(name, columnListBox);

            TabPage page = new TabPage();
            page.SuspendLayout();
            page.Controls.Add(columnListBox);
            page.Location = new System.Drawing.Point(10, 48);
            page.Name = name + "TabPage";
            page.Size = new System.Drawing.Size(columnsTabControl.ClientRectangle.Width, columnsTabControl.ClientRectangle.Height);
            page.TabIndex = 0;
            page.Text = title;
            page.UseVisualStyleBackColor = true;
            columnTabPages.Add(name, page);
            columnsTabControl.TabPages.Add(page);
            page.ResumeLayout();
        }

        void UpdateColumnTabs()
        {
            columnsTabControl.SuspendLayout();
            columnsTabControl.TabPages.Clear();
            columnListBoxes.Clear();
            columnTabPages.Clear();

            innerJoin.Visible = false;
            insertIgnore.Visible = false;

            innerJoin.Checked = S.Get("NewQueryInnerJoin", false);
            insertIgnore.Checked = S.Get("NewQueryInsertIgnore", false);
            parameterize.Checked = S.Get("NewQueryParameterize", false);
            includeAliases.Checked = false;

            if (queryType == "Select")
            {
                innerJoin.Visible = tablesList.Items.Count > 1;
                includeAliases.Checked = true;
                AddColumnTabPage("Select");
                AddColumnTabPage("Where");
                AddColumnTabPage("Group by");
                AddColumnTabPage("Order by");
                tablesList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            }
            else if (queryType == "Insert")
            {
                insertIgnore.Visible = true;
                AddColumnTabPage("Columns");
                tablesList.SelectionMode = System.Windows.Forms.SelectionMode.One;
            }
            else if (queryType == "Replace")
            {
                AddColumnTabPage("Columns");
                AddColumnTabPage("Where");
                tablesList.SelectionMode = System.Windows.Forms.SelectionMode.One;
            }
            else if (queryType == "Update")
            {
                AddColumnTabPage("Columns");
                AddColumnTabPage("Where");
                tablesList.SelectionMode = System.Windows.Forms.SelectionMode.One;
            }
            else if (queryType == "Delete")
            {
                AddColumnTabPage("Where");
                tablesList.SelectionMode = System.Windows.Forms.SelectionMode.One;
            }
            columnsTabControl.ResumeLayout();
            ListDbColumn.includeAlias = ListDbTable.includeAlias = includeAliases.Checked;
            tablesList.Update();
        }

        #endregion

        #region create sql

        enum GetSelectedColumnsMode { None, Equals, Insert, Update }

        string GetSelectedColumns(string title, string conj, string def = "", GetSelectedColumnsMode mode = GetSelectedColumnsMode.None)
        {
            bool paramize = S.Get("NewQueryParameterize", false);
            string keyword = title.ToLower();
            if (keyword == title) // all lower case chars
                title = "";
            string name = keyword.Replace(" ", "");
            string result = "";
            string values = "";
            if(columnListBoxes.ContainsKey(name))
            {
                ListBox lb = columnListBoxes[name];
                foreach (int i in lb.SelectedIndices)
                {
                    ListDbColumn item = lb.Items[i] as ListDbColumn;
                    string text = item.DisplayMember;
                    string value = (paramize ? "@" + item.column.name : (item.column.dataType == typeof(string) ? "''" : "0"));
                    if (mode == GetSelectedColumnsMode.Equals)
                        text += "=" + value;
                    values = values.AppendTo(value, conj);
                    result = result.AppendTo(text, conj);
                }
            }

            if (result == "")
                result = def;
            if (result != "")
            {
                if (mode == GetSelectedColumnsMode.Insert)
                    result = "(" + result + ") values (" + values + ")";
                else
                    result = keyword + " " + result + " ";
            }

            return result;
        }
        void UpdateEditor()
        {
            if (!loaded)
                return;

            if (!listsLoaded)
                return;

            if(innerJoin.Visible)
                S.Set("NewQueryInnerJoin", innerJoin.Checked);

            string from = "";
            List<string> tables = new List<string>();
            foreach (int i in tablesList.SelectedIndices)
            {
                ListDbTable item = tablesList.Items[i] as ListDbTable;
                if(innerJoin.Visible == false || innerJoin.Checked == false)
                    from = from.AppendTo(item.DisplayMember, ", ");
                else
                    tables.Add(item.dbTable.name);
            }

            if (tables.Count > 0)
                from = A.db.MakeFrom(tables, includeAliases.Checked);
            
            string sql = "";
            if (from != "")
            {
                if (queryType == "Select")
                {
                    sql += GetSelectedColumns("Select", ", ", "*");
                    sql += "from " + from + " ";
                    sql += GetSelectedColumns("Where", " and ", "", GetSelectedColumnsMode.Equals);
                    sql += GetSelectedColumns("Group by", ", ");
                    sql += GetSelectedColumns("Order by", ", ");
                }
                else if (queryType == "Insert")
                {
                    sql = "insert ";
                    if (S.Get("NewQueryInsertIgnore", false))
                        sql += "ignore ";
                    sql += "into " + from + " ";
                    sql += GetSelectedColumns("columns", ", ", "", GetSelectedColumnsMode.Insert);
                }
                else if (queryType == "Update")
                {
                    sql += GetSelectedColumns("columns", ", ", "", GetSelectedColumnsMode.Equals);
                    if(sql.Trim() != "")
                    {
                        sql = "update " + from + " set " + sql;
                        sql += GetSelectedColumns("Where", " and ", "", GetSelectedColumnsMode.Equals);
                    }
                    
                }
                else if (queryType == "Delete")
                {
                    sql = "delete from " + from + " ";
                    sql += GetSelectedColumns("Where", " and ", "", GetSelectedColumnsMode.Equals);
                }
            }

            sqlEditor.Text = sql;
            bOK.Enabled = sql != "";
        }

        #endregion

        #region events

        void queryTypeCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateColumnTabs();
            UpdateEditor();
        }

        void columnList_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateEditor();
        }

        void tablesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateColumnLists();
            UpdateEditor();
        }
        
        void includeAllCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if(listsLoaded)
                S.Toggle("NewQueryIncludeWorkingObjects", true);
        }

        void includeAliases_CheckedChanged(object sender, EventArgs e)
        {
            ListDbColumn.includeAlias = ListDbTable.includeAlias = includeAliases.Checked;
            UpdateTableList();
            UpdateColumnLists();
        }

        void parameterize_CheckedChanged(object sender, EventArgs e)
        {
            if (!listsLoaded)
                return;

            S.Toggle("NewQueryParameterize", false);
            UpdateEditor();
        }

        void insertIgnore_CheckedChanged(object sender, EventArgs e)
        {
            if (!listsLoaded)
                return;

            S.Toggle("NewQueryInsertIgnore", false);
            UpdateEditor();
        }

        void showAllColumns_CheckedChanged(object sender, EventArgs e)
        {
            if (!listsLoaded)
                return;

            S.Toggle("NewQueryShowAllColumns", false);
            UpdateColumnLists();
        }

        void tableSortCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateTableList();
            UpdateColumnLists();
        }

        void bOK_Click(object sender, EventArgs e)
        {
            includeWorkingObjects = workingTables.Count > 0 && S.Get("NewQueryIncludeWorkingObjects", true);
            sql = sqlEditor.Text;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void innerJoin_CheckedChanged(object sender, EventArgs e)
        {
            UpdateEditor();
        }

        #endregion

        private void DlgNewQuery_Shown(object sender, EventArgs e)
        {

        }

        private void DlgNewQuery_Load(object sender, EventArgs e)
        {
            UpdateEditor();
        }
    }
}
