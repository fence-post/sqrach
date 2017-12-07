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
using fp.lib.forms;

namespace fp.sqratch
{
    partial class main : Form
    {
        private ObjectsTreeView focusedTree;

        ObjectsTree CreateObjectTree(SplitterPanel panel, string name, string label)
        {
            ObjectsTree result = new ObjectsTree(name == "allObjectsTree");
            result.tree.ContextMenuStrip = this.tableContextMenu;
            if (result.parseTree != null)
                result.parseTree.ContextMenuStrip = this.parseTreeMenu;
            result.Dock = System.Windows.Forms.DockStyle.Fill;
            result.Location = new System.Drawing.Point(0, 0);
            result.Name = name;
            result.Size = new System.Drawing.Size(300, 300);
            return result;
        }


        private void showDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (focusedTree.GetSelectedTables().Count == 1)
                OpenQuery("select * from " + focusedTree.GetSelectedTables()[0].name, null, false, true);
        }

        private void tableContextMenu_Opening(object sender, CancelEventArgs e)
        {
            ObjectsTreeView tree = focusedTree = allObjectsTree.tree.Focused ? allObjectsTree.tree : activeObjectsTree.tree;

            bool clickedOnSelected = tree.NodeAtLocationIsSelected(tree.PointToClient(Cursor.Position));
            bool objectsSelected = false;
            if(clickedOnSelected)
            {
                objectsSelected = tree.GetSelectedColumns().Count > 0 || tree.GetSelectedTables().Count > 0;
            }
            T.Debug("clicked on selected=" + (clickedOnSelected ? "yes" : "no"));

            bool isActiveObjectsTree = focusedTree == activeObjectsTree.tree;

            showDataToolStripMenuItem.Enabled = background.busy == false && tree.GetSelectedTables().Count == 1 && tree.GetSelectedColumns().Count == 0;
            addToToolStripMenuItem.Enabled = selectFromToolStripMenuItem.Enabled = objectsSelected ;
            addToWorkspaceToolStripMenuItem.Visible = !isActiveObjectsTree;
            stickyToolStripMenuItem.Visible = isActiveObjectsTree;
            showAllColumnsToolStripMenuItem.Visible = isActiveObjectsTree && focusedTree.OnlyTablesAreSelected();
            recentAtTopToolStripMenuItem.Checked = S.Get("recentObjectsAtTop", false);
            int selectedSticky = tree.AreSelectedObjectsSticky();
            stickyToolStripMenuItem.Checked = selectedSticky >= 0;
            addToWorkspaceToolStripMenuItem.Checked = selectedSticky > 0;
            collapseAllToolStripMenuItem.Enabled = !focusedTree.AllRootNodesCollapsed();
            expandToolStripMenuItem.Enabled = !focusedTree.AllRootNodesExpanded();
            if (focusedTree.OnlyTablesAreSelected())
                showAllColumnsToolStripMenuItem.Checked = focusedTree.SelectedTablesAreShowAllColumns();
            stickyToolStripMenuItem.Enabled = objectsSelected;
            addToWorkspaceToolStripMenuItem.Enabled = objectsSelected;
            showAllColumnsToolStripMenuItem.Enabled = objectsSelected;
        }

        private void recentAtTopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            S.Toggle("recentObjectsAtTop", false);
            activeObjectsTree.tree.UpdateObjects(true);
            allObjectsTree.tree.UpdateObjects(true);
        }

        private void expandToolStripMenuItem_Click(object sender, EventArgs e)
        {
            focusedTree.ExpandAll();
            if (focusedTree.Nodes.Count > 0)
                focusedTree.Nodes[0].EnsureVisible();
        }

        private void collapseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            focusedTree.CollapseAll();
            if(focusedTree.Nodes.Count > 0)
                focusedTree.Nodes[0].EnsureVisible();
        }
        
        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            objectTreesDirty = true;
        }

        private void showAllColumnsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            activeObjectsTree.tree.MakeSelectedTablesShowAllColumns(!menuItem.Checked);
            activeObjectsTree.tree.UpdateObjects(true);
        }

        private void addToWorkspaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            if(focusedTree == activeObjectsTree.tree)
            {
                if (menuItem.Checked == true || menuItem.CheckState == CheckState.Indeterminate)
                    focusedTree.MakeSelectedObjectsNotSticky();
            }
            else
            {
                if (menuItem.Checked == false || menuItem.CheckState == CheckState.Indeterminate)
                    focusedTree.MakeSelectedObjectsSticky();
                else
                    focusedTree.MakeSelectedObjectsNotSticky();
            }
            activeObjectsTree.tree.UpdateObjects(false);
            allObjectsTree.tree.UpdateObjects(false);
        }

        private void stickyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            if(menuItem.Checked)
                focusedTree.MakeSelectedObjectsNotSticky();
            else
                focusedTree.MakeSelectedObjectsSticky();
            activeObjectsTree.tree.UpdateObjects(true);
            allObjectsTreeDirty = true;
        }

        private void selectFromToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowNewQueryDialog(focusedTree.GetSelectedTables(), focusedTree.GetSelectedColumns());
        }
        
        private void parseTreeMenu_Opening(object sender, CancelEventArgs e)
        {
            deleteToolStripMenuItem.Enabled = activeObjectsTree.parseTree.SelectedNodes.Count > 0;
            showColumnsToolStripMenuItem.Checked = S.Get("parseTreeShowColumns", true);
            showAllTokensToolStripMenuItem.Checked = S.Get("parseTreeShowAllTokens", false);
            showExpressionsToolStripMenuItem.Checked = S.Get("parseTreeShowExpressions", false);
            expandAllToolStripMenuItem.Enabled = !activeObjectsTree.parseTree.AllNodesExpanded();
            collapseAllToolStripMenuItem.Enabled = !activeObjectsTree.parseTree.AllNodesCollapsed();
            expandThisBranchToolStripMenuItem.Enabled = activeObjectsTree.parseTree.SelectedNode != null && activeObjectsTree.parseTree.SelectedNode.Nodes.Count > 0 && activeObjectsTree.parseTree.SelectedNode.AllNodesExpanded() == false;
        }

        private void deleteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            lock (Parser.query)
            {
                SqlBuilder b = new SqlBuilder(Parser.query);
                activeObjectsTree.parseTree.DeleteSelectedNodes(b);
                SetEditorText(b.Render(), false);
            }
        }

        void UpdateParseTree()
        {
            activeObjectsTree.UpdateParseTree();
        }

        private void showExpressionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            S.Toggle("parseTreeShowExpressions", false);
            UpdateParseTree();
        }

        private void showColumnsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            S.Set("parseTreeShow", "Custom");
            S.Toggle("parseTreeShowColumns", false);
            UpdateParseTree();
        }
        
        private void showAllTokensToolStripMenuItem_Click(object sender, EventArgs e)
        {
            S.Set("parseTreeShow", "Custom");
            S.Toggle("parseTreeShowAllTokens", false);
            UpdateParseTree();
        }

        private void showTokenDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            S.Set("parseTreeShow", "Custom");
            S.Toggle("parseTreeShowDetails", false);
            UpdateParseTree();
        }

        private void expandAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            activeObjectsTree.parseTree.ExpandAll();
        }

        private void collapseAllToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            activeObjectsTree.parseTree.CollapseAll();
        }
    }
}
