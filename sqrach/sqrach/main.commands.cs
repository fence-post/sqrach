using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ScintillaNET;
using fp.lib;
using System.Configuration;
using System.Diagnostics;
using fp.lib.sqlite;
using fp.lib.forms;
using fp.lib.sqlparser;

namespace fp.sqratch
{
    partial class main : Form
    {
        void loadDataFromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DlgLoadData dlg = new DlgLoadData();
            dlg.ShowDialog(this);
        }

        void saveDataToFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DlgSaveAs dlg = new DlgSaveAs(null);
            dlg.ShowDialog(this);
        }

        void optionsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DlgOptions dlg = new DlgOptions();
            dlg.ShowDialog(this);
            UpdateUIPreferences(true);
            Parser.MakeSuggestionsDirty();
        }

        void sqratchDocumentationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormsToolbox.OpenUrl(baseUrl + "sqrachGuide.php");
        }
        void websiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormsToolbox.OpenUrl(baseUrl + "sqrach.php");
        }

        void syntaxHelp_Click(object sender, EventArgs e)
        {
            OpenSyntaxHelpInBrowser(sender);
        }

        void OpenSyntaxHelpInBrowser(object sender)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (item != null)
            {
                string keyword = item.Text.ToLower().Replace("...", "");
                FormsToolbox.OpenUrl(baseUrl + "sqrachSyntaxHelp.php?k=" + keyword + "&t=" + A.db.databaseType);
            }
        }

        void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this.OkCancelBox("Are you sure you want to permanently delete these queries?"))
                return;

            queryHistory.DeleteSelected();
        }

        void newQueryOptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowNewQueryDialog(null, null);
        }

        void fileToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            saveToolStripMenuItem.Enabled = selectedQuery.dirty && editor.Text != "";
            duplicateQueryToolStripMenuItem.Enabled = saveAsToolStripMenuItem.Enabled = editor.Text != "";
            saveAllToolStripMenuItem.Enabled = TabsAreDirty();
        }

        void viewToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            string fontSize = Settings.Get("fontSize", "medium");
            showDatabaseObjects.Checked = S.Get("showDatabaseObjects", true);
            parseGraphToolStripMenuItem.Checked = Settings.Get("showParseGraph", false);
            logToolStripMenuItem.Checked = Settings.Get("showLog", false);
            queryHistoryToolStripMenuItem.Checked = Settings.Get("showQueryHistory", false);
            darkToolStripMenuItem.Checked = S.initSettings.dark;
            lightToolStripMenuItem.Checked = !S.initSettings.dark;
            smallToolStripMenuItem.Checked = fontSize == "small";
            mediumToolStripMenuItem.Checked = fontSize == "medium";
            bigToolStripMenuItem.Checked = fontSize == "big";
        }

        void backToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectQuery(selectedQuery.prevQueryId);
            editor.Focus();
        }

        void crashOnParserErrorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            S.Toggle("QuietExecutionErrors", true);
        }

        void helpToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            crashOnParserErrorsToolStripMenuItem.Visible = saveTestToolStripMenuItem.Visible = allowDebugging;
            crashOnParserErrorsToolStripMenuItem.Checked = !S.Get("QuietParserErrors", true);
        }

        void saveTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filePath = ConfigurationManager.AppSettings["testPath"] + "\\" + DateTime.Now.ToString("yyMMddHHmm") + ".sql";
            System.IO.File.WriteAllText(filePath, editor.Text);
        }

        void structureSplitContainer_SplitterMoved(object sender, SplitterEventArgs e)
        {
            Settings.Set("structureSliderPos", objectSplitter.SplitterDistance);
            editor.Focus();
        }
        
       
        void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            clipboardHelper.Cut();
        }

        void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            clipboardHelper.Copy();
        }

        void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            clipboardHelper.Paste();
        }

        void editToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            copyToolStripMenuItem.Enabled = clipboardHelper.CanCopy();
            cutToolStripMenuItem.Enabled = clipboardHelper.CanCut();
            pasteToolStripMenuItem.Enabled = clipboardHelper.CanPaste();
            selectAllToolStripMenuItem.Enabled = clipboardHelper.Focused();
            clearToolStripMenuItem.Enabled = editor.Focused;
        }

        void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            clipboardHelper.SelectAll();
        }

        void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearParseViews();
            editor.Text = "";
        }

        void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
        
        void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DlgAbout dlg = new DlgAbout();
            dlg.ShowDialog(this);
        }

        void CloseTab(int indexToDrop)
        {
            leftTabs.RemoveTab(indexToDrop);
        }

        void closeAllButThisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseAllButThis();
        }

        void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedQuery.dirty)
                if (!this.OkCancelBox("This query has changed.  Are you sure you want to close it without saving?"))
                    return;
            Settings.CloseQuery(selectedQuery.tabId);
            int indexToDrop = selectedTabIndex;
            CloseTab(indexToDrop);
        }

        void queryHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rightTabs.ShowTab("Query History", S.Toggle("showQueryHistory", false));
            UpdateRightPanelLayout();
        }

        void activeObjectsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rightTabs.ShowTab("Structure", S.Toggle("showDatabaseObjects", true));
            UpdateRightPanelLayout();
        }

        void logToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rightTabs.ShowTab("Log", S.Toggle("showLog", false));
            UpdateRightPanelLayout();
        }

        private void parseGraphToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowParseGraph();
            /*
                        if (S.Toggle("showParseGraph", false))
                            ShowParseGraph();
                        else
                            parseGraph.Hide();
              */
        }

        void smallToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings.Set("fontSize", "small");
            UpdateUIPreferences(true);
        }

        void mediumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings.Set("fontSize", "medium");
            UpdateUIPreferences(true);
        }

        void bigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings.Set("fontSize", "big");
            UpdateUIPreferences(true);
        }

        void lightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            S.initSettings.dark = false;
            UpdateUIPreferences(true);
        }

        void darkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            S.initSettings.dark = true;
            UpdateUIPreferences(true);
        }

        void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog.SetExtensions("SQL|*.sql");
            saveFileDialog.SupportMultiDottedExtensions = true;
            saveFileDialog.InitialDirectory = S.Get("saveAsPath", System.IO.Directory.GetCurrentDirectory()); // directoryTextbox.Text;
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                S.Get("saveAsPath", T.GetPathFromFilePath(saveFileDialog.FileName));
                System.IO.File.WriteAllText(saveFileDialog.FileName, editor.Text);
            }
        }

        void OnNewClicked(object sender, EventArgs e)
        {
            OpenQuery("");
            /*
            QueryPage p = new QueryPage(Settings.AddQuery());
            queryPages.Add(p);
            leftTabs.SelectTab(leftTabs.AddTab(""));
            UpdateLeftTabs();
            */
        }

        void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            selectedQuery.Save();
            tabsDirty = true;
            queryHistory.dirty = true;
        }

        void saveAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAllDirtyTabs();
        }

        void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DlgOptions dlg = new DlgOptions();
            DialogResult res = dlg.ShowDialog(this);
        }

        void main_Resize(object sender, EventArgs e)
        {
            if (!A.loading)
            {
                HidePopups();
                if (WindowState != FormWindowState.Maximized)
                {
                    Settings.Set("width", Width);
                    Settings.Set("height", Height);
                }
            }
        }

        void bStop_Click(object sender, EventArgs e)
        {
            background.CancelQuery();
            editor.Focus();
        }

        void OnPrevClicked(object sender, EventArgs e)
        {
            SelectQuery(selectedQuery.prevQueryId);
            editor.Focus();
        }

        void OnNextClicked(object sender, EventArgs e)
        {
            SelectQuery(selectedQuery.nextQueryId);
            editor.Focus();
        }

        void OnStopClicked(object sender, EventArgs e)
        {
            if (Background.status == BackgroundStatus.ExecutingQuery)
                background.CancelQuery();
        }

        void OnRunClicked(object sender, EventArgs e)
        {
            HidePopups();
            if (selectedQuery != null)
            {
                queryWaitCursorEnabled = true;
                resultsList.ClearResults();
                if(S.Get("SaveQueryOnExecute", true))
                    selectedQuery.Save(true);
                background.QueueQuery(selectedQuery);
                goButton.SetText("Stop", 1);
                tabsDirty = true;
            }
            editor.Focus();
        }
    
        void selectDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowConnectDialog(false);
        }

        private void duplicateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenQuery(editor.Text);
        }

        void duplicateQueryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenQuery(editor.Text);
        }

        void ShowRenameTabDialog(Query page)
        {
            DlgTabName dlg = new DlgTabName(page.label);
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                Settings.SetQueryName(page.tabId, dlg.tabName);
                page.label = dlg.tabName;
                tabsDirty = true;
                queryHistory.dirty = true;
            }
        }

        void renameQueryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowRenameTabDialog(selectedQuery);
        }

        void queryToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            parseToolStripMenuItem.Enabled = executeToolStripMenuItem.Enabled = editor.Text != "";
            backToolStripMenuItem.Enabled = selectedQuery.canGoPrev;
            forwardToolStripMenuItem.Enabled = selectedQuery.canGoNext;
            duplicateQueryToolStripMenuItem.Enabled = editor.Text != "";
        }

        void verticalSplitter_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if(A.uiLoaded)
            {
                S.initSettings.vertSplitterPos = vertSplitter.SplitterDistance;
                editor.Focus();
            }
        }

        void horizontalSplitter_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (A.uiLoaded)
            {
                S.initSettings.horzSplitterPos = horzSplitter.SplitterDistance;
                editor.Focus();
            }
        }

        void tabContextMenu_Opening(object sender, CancelEventArgs e)
        {
            int i  = leftTabs.TabIndexUnderPoint(Cursor.Position);
            if (i != selectedTabIndex)
                e.Cancel = true;

            moveLeftToolStripMenuItem.Enabled = leftTabs.CanMoveTabLeft;
            moveRightToolStripMenuItem.Enabled = leftTabs.CanMoveTabRight;
            
        }

        void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowRenameTabDialog(selectedQuery);
        }

        void moveLeftToolStripMenuItem_Click(object sender, EventArgs e)
        {
            leftTabs.MoveSelectedTabLeft();
            tabPositionsDirty = true;
        }

        void moveRightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            leftTabs.MoveSelectedTabRight();
            tabPositionsDirty = true;
        }

        void queryHistoryMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            showClosedQueriesToolStripMenuItem.Checked = !S.Get("ShowClosedQueries", false);
            deleteToolStripMenuItem.Enabled = queryHistory.SelectedIndices.Count > 0;
        }

        void showClosedQueriesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            S.Toggle("ShowClosedQueries", false);
            queryHistory.UpdateQueryHistory();
        }

        void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<QueryInfo> queries = S.GetQueryInfo(queryHistory.GetItemTagsAsIntegers());
            foreach (QueryInfo query in queries)
                OpenQuery(query.expr, query);
        }

        private void menu_MenuActivate(object sender, EventArgs e)
        {
            HidePopups();
        }

        void formatSqlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormatSqlInEditor();
        }

        public void FormatSqlInEditor()
        {
            if (Parser.query != null)
            {
                SqlBuilder b = new SqlBuilder(Parser.query);
                string sql = b.Format();
                SetEditorText(sql, false);

            }
        }

        void resetAllSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!A.OkCancelBox(this, "Are you sure you want to reset settings?", MessageBoxIcon.Asterisk))
                return;
            S.ClearSettings();
            UpdateUIPreferences(true);
            Reload(A.dbId);
        }

        void colorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DlgColors dlg = new DlgColors(smallLightImages, bigButtonImages);
            dlg.ShowDialog(this);
        }

        void closeAllButThisToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            CloseAllButThis();
        }

        void CloseAllButThis()
        { 
            bool dirty = TabsAreDirty(true);
            if (dirty)
                if (!this.OkCancelBox("Queries have changed.  Are you sure you want to close without saving?"))
                    return;

            /*
            foreach (QueryPage p in queryPages)
                if (p != selectedQuery)
                    Settings.CloseQuery(p.tabId);
            QueryPage page = selectedQuery;
            queryPages.Clear();
            leftTabs.Clear();
            queryPages.Add(page);
            leftTabs.AddTab(page.label);
            */
            for(int i = 0; i < leftTabs.Count; i++)
            {
                Query p = leftTabs.GetItemData(i) as Query;
                if (p != selectedQuery)
                    Settings.CloseQuery(p.tabId);
            }
        
            Query q = selectedQuery;
            leftTabs.Clear();
            leftTabs.SetItemData(leftTabs.AddTab(q.label), q);
            leftTabs.SelectTab(0);
            queryHistory.dirty = true;
            tabsDirty = true;
        }

        private void tableRelationshipViewerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowTableGraph();
        }

        private void nextTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedTabIndex < leftTabs.Count - 1)
                leftTabs.SelectTab(selectedTabIndex + 1);
            else if (selectedTabIndex > 0 && selectedTabIndex == leftTabs.Count - 1)
                leftTabs.SelectTab(0);
        }

        private void previousQueryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedTabIndex > 0)
                leftTabs.SelectTab(selectedTabIndex - 1);
            else if (selectedTabIndex == 0 && leftTabs.Count > 1)
                leftTabs.SelectTab(leftTabs.Count - 1);
        }
    }
}
