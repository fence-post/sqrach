using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using fp.lib;
using fp.lib.forms;
using fp.lib.dbInfo;

namespace fp.sqratch
{
    public class ObjectsTreeNode : TreeNode
    {
        public string rightStuff;

        public ObjectsTreeNode(string txt) : base(txt)
        {

        }
    }

    public class ObjectsTreeView : MultiSelectTreeView
    {
        public bool expandByDefault = false;
        public Dictionary<string, ObjectsTreeNode> nodesByObjectName = new Dictionary<string, ObjectsTreeNode>();
        public string search = "";
        public bool searchTables { get { return _filterTables; } set { _filterTables = value; if (_filterTables == false && _filterColumns == false) _filterTables = searchColumns = true; } }
        public bool searchColumns { get { return _filterColumns; } set { _filterColumns = value; if (_filterTables == false && _filterColumns == false) _filterTables = searchColumns = true; } }

        bool _filterTables = true;
        bool _filterColumns = true;
        bool updating = false;

        public ObjectsTreeView(string nam)
        {
            Name = nam;
            DrawMode = TreeViewDrawMode.OwnerDrawAll;
            DrawNode += new DrawTreeNodeEventHandler(MyDrawNode);
            AfterExpand += new TreeViewEventHandler(OnAfterExpand);
            AfterCollapse += new TreeViewEventHandler(OnAfterCollapse);
            BorderStyle = BorderStyle.None;
            ShowLines = false;
            ShowRootLines = false;
            Indent = 0;
        }
        void OnAfterExpand(object sender, TreeViewEventArgs e)
        {
            ObjectsTreeNode node = e.Node as ObjectsTreeNode;
            SaveNodeExpandSetting(node.Text, true);
        }

        void OnAfterCollapse(object sender, TreeViewEventArgs e)
        {
            ObjectsTreeNode node = e.Node as ObjectsTreeNode;
            SaveNodeExpandSetting(node.Text, false);
        }

        #region get db objects from nodes and manipulations on db objects

        public bool IsTable(TreeNode t)
        {
            return t.Level == 0;
        }

        public List<DbTable> GetSelectedTables(bool includeTablesOfSelectedColumns = true)
        {
            List<DbTable> result = new List<DbTable>();
            foreach (TreeNode t in SelectedNodes)
            {
                if (IsTable(t))
                    result.Add(GetDbTable(t));
                else if (includeTablesOfSelectedColumns && !t.Parent.IsSelected)
                    result.Add(GetDbTable(t.Parent));
            }

            return result;
        }

        public List<DbColumn> GetSelectedColumns()
        {
            List<DbColumn> result = new List<DbColumn>();
            foreach (TreeNode t in SelectedNodes)
                if (!IsTable(t))
                    result.Add(GetDbColumn(t));
            
            return result;
        }

        public DbTable GetDbTable(TreeNode t)
        {
            return A.db.tables[t.Text];
        }

        public DbColumn GetDbColumn(TreeNode t)
        {
            return GetDbTable(t.Parent).columns[t.Text];
        }

        public void MakeSelectedTablesShowAllColumns(bool yes)
        {
            foreach (TreeNode t in SelectedNodes)
                if (IsTable(t))
                    GetDbTable(t).showAllColumns = yes;
        }
        
        public void MakeSelectedObjectsSticky()
        {
            foreach (TreeNode t in SelectedNodes)
                if (!IsTable(t))
                {
                    DbColumn c = GetDbColumn(t);
                    c.bookmarked = true;
                    c.table.bookmarked = true;
                }
            foreach (TreeNode t in SelectedNodes)
            {
                if (IsTable(t))
                {
                    GetDbTable(t).bookmarked = true;
                }
            }
        }

        public void MakeSelectedObjectsNotSticky()
        {
            foreach (TreeNode t in SelectedNodes)
            {
                if (IsTable(t))
                {
                    DbTable table = GetDbTable(t);
                    table.bookmarked = false;
                    foreach(DbColumn column in table.columns.Values)
                        column.bookmarked = false;
                }
                else
                    GetDbColumn(t).bookmarked = false;
            }
        }

        public bool SelectedTablesAreShowAllColumns()
        {
            foreach (TreeNode t in SelectedNodes)
                if (!GetDbTable(t).showAllColumns)
                    return false;

            return true;
        }

        public bool OnlyTablesAreSelected()
        {
            bool tableFound = false;
            foreach (TreeNode t in SelectedNodes)
            {
                if (IsTable(t))
                    tableFound = true;
                else
                    return false;
            }
            return tableFound;
        }

        public int AreSelectedObjectsSticky()
        {
            List<TreeNode> nodes = SelectedNodes;

            int active = 0;
            int notActive = 0;
            foreach (TreeNode t in nodes)
            {
                if (IsTable(t))
                {
                    if (GetDbTable(t).bookmarked)
                        active++;
                    else
                        notActive++;
                }
                else
                {
                    if (GetDbColumn(t).bookmarked)
                        active++;
                    else
                        notActive++;                    
                }
            }

            if (active > 0 && notActive > 0)
                return 0;
            else if (active > 0)
                return 1;
            else
                return -1;
        }
      
        public List<string> GetSelectedTableNames()
        {
            List<TreeNode> nodes = SelectedNodes;
            List<string> result = new List<string>();
            foreach (TreeNode t in nodes)
                if (IsTable(t))
                    result.Add(GetDbTable(t).name);

            return result;
        }

        #endregion
             
        bool ExpandNodeBySettings(DbTable t, TreeNode node)
        {
            int val = S.Get("objTreeExp" + Name + t.name, 0);
            if (val > 0)
                node.Expand();
            else if (val < 0)
                node.Collapse();

            return val != 0;
        }

        void SaveNodeExpandSetting(string t, bool b)
        {
            if(!updating)
                S.Set("objTreeExp" + Name + t, b ? 1 : -1);
        }

        #region update nodes

        /*
        bool TableHasActiveOrStickyColumns(DbTable table)
        {
            if (Parser.activeTablesWithColumns.Contains(table))
                return true;
            if (table.HasBookmarkedColumns())
                return true;
            return false;

        }
        */
        public void UpdateObjects(bool clear)
        {
            bool all = Name.Contains("all");
            updating = true;
            BeginUpdate();
            if(clear)
            {
                nodesByObjectName.Clear();
                Nodes.Clear();
            }

            QListSort sort = S.Get("recentObjectsAtTop", false) ? QListSort.Descending : QListSort.None;
            foreach (DbTable table in A.db.tables.Each(sort))
            {
                if (search != "")
                {
                    bool matchingColumnExists = false;
                    if (searchColumns)
                    {
                        foreach (string columnName in table.columns.Keys)
                        {
                            if (columnName.StartsWith(search))
                            {
                                matchingColumnExists = true;
                                break;
                            }
                        }
                    }
                    if (matchingColumnExists == false)
                    {
                        if (searchTables == false || table.name.StartsWith(search) == false)
                            continue;
                    }
                }
            
                bool tableIsActive = table.active;
                bool tableIsSticky = table.bookmarked ;
                bool tableIsShowAllColumns = table.showAllColumns;
                bool showColumnsAnyways = !table.HasBookmarkedOrActiveColumns();

                if (all || tableIsActive || tableIsSticky)
                {
                    TreeNode tableNode = CreateOrUpdateTableTreeNode(table, tableIsActive, tableIsSticky);
                    foreach (string columnName in table.columns.Keys)
                    {
                        DbColumn column = table.columns[columnName];
                        if ((search == "") || (searchColumns == false) || columnName.StartsWith(search))
                        {
                            bool columnIsActive = column.active;
                            bool columnIsSticky = column.bookmarked;
                            if (all || tableIsShowAllColumns || columnIsActive || columnIsSticky || showColumnsAnyways)
                                CreateOrUpdateColumnTreeNode(tableNode, table, column, columnIsActive, columnIsSticky);
                        }

                    }
                    if(search != "" && searchColumns)
                    {
                        tableNode.Expand();
                    }
                    else if(!ExpandNodeBySettings(table, tableNode))
                    {
                        if (all == false && tableIsShowAllColumns)
                            tableNode.Expand();
                        else if (expandByDefault)
                            tableNode.Expand();
                    }
                }
            }
            EndUpdate();
            updating = false;
        }

        private TreeNode CreateOrUpdateTableTreeNode(DbTable table, bool active, bool sticky)
        {
            ObjectsTreeNode node = null;
            if (nodesByObjectName.ContainsKey(table.name))
            {
                node = nodesByObjectName[table.name];
            }
            else
            {
                node = new ObjectsTreeNode(table.name);
                nodesByObjectName.Add(table.name, node);
                Nodes.Add(node);
            }
            node.Tag = "";
            if (active)
                node.ForeColor = UI.stickyForeColor;
            node.ImageIndex = UI.GetTreeImageIndexForObject(table);
            
            return node;
        }

        private TreeNode CreateOrUpdateColumnTreeNode(TreeNode parentNode, DbTable table, DbColumn column, bool active, bool sticky)
        {
            bool stickyButNotActive = active == false && sticky;
            string relationships = column.firstForeignKeyLabel;
            /*
            string relationships = "";
            foreach (Relationship r in A.db.tables[table.name], column.name))
                relationships = T.AppendTo(relationships, r.parentTable.name + "." + r.parentColumn.name, " ");
                */
            if (relationships != "")
                relationships = "->" + relationships + "";

            string key = table.name + "!" + column.name;

            ObjectsTreeNode columnNode = null;
            if (nodesByObjectName.ContainsKey(key))
            {
                columnNode = nodesByObjectName[key];
            }
            else
            {
                columnNode = new ObjectsTreeNode(column.name);
                nodesByObjectName.Add(key, columnNode);
                parentNode.Nodes.Add(columnNode);
            }

            columnNode.ToolTipText = column.name + " - " + column.definition
                + (column.allowNulls ? " null" : "")
                + (column.primaryKey ? " *" : "") + column.columnInfo;

            columnNode.rightStuff = relationships;
            columnNode.ImageIndex = UI.GetTreeImageIndexForObject(column);
            if (active)
                columnNode.ForeColor = UI.stickyForeColor; // stickyButNotActive ? UI.passiveRedColor : UI.activeRed;
            return columnNode;
        }

        #endregion

        public void MyDrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            e.DrawDefault = true;
            if (e.Bounds.Width >= 300)
            {
                ObjectsTreeNode objectNode = e.Node as ObjectsTreeNode;
                string txt = objectNode.rightStuff;
                if (txt != null && txt != "")
                {
                    using (StringFormat sf = new StringFormat())
                    {
                        int left = e.Bounds.Left + FormsToolbox.GetTextWidth(e.Node.Text + "00", Font);
                        sf.Trimming = StringTrimming.EllipsisCharacter;
                        sf.LineAlignment = StringAlignment.Center;
                        sf.Alignment = StringAlignment.Far;
                        using (Brush gray = new SolidBrush(UI.passiveForecolor1))
                        {
                            e.Graphics.DrawString(txt,
                            SystemFonts.MessageBoxFont, gray,
                            new Rectangle(left, e.Bounds.Top, e.Bounds.Width - left, e.Bounds.Height), sf);
                        }
                    }
                }
            }
        }
    }
}
