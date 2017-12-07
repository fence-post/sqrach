using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using ScintillaNET;
using fp.lib;
using System.Configuration;
using System.Diagnostics;
using fp.lib.sqlite;
using fp.lib.forms;
using fp.lib.sqlparser;
using fp.lib.dbInfo;

namespace fp.sqratch
{
    partial class main : Form
    {
        void HidePopups()
        {
            autoComplete.Hide();
        }

        bool Reload(int dbId)
        {
            SetAppStatus(Status.Initializing);
            CloseDatabase();
            A.CloseDatabase();
            if(A.OpenDatabase(dbId))
            {
                Background.status = BackgroundStatus.Loading;
                StartBackgroundLoading();
                return true;
            }

            return false;
        }

        protected void CloseDatabase()
        {
            SuspendLayout();
            HideParseGraph();
            leftTabs.Clear();
            SetEditorText("", true);
            queryHistory.Clear();
            allObjectsTree.Clear();
            activeObjectsTree.Clear();
            StartBackgroundLoading();
            ResumeLayout();
        }

        void StartBackgroundLoading()
        {
            SetAppStatus(Status.Initializing);
            loading.Hide(menu);
            loading.Hide(editorHeader);
            loading.Hide(goButton);
            loading.Hide(addButton);
            loading.Hide(rightTabs);
            loading.Hide(leftTabs);
            loading.Hide(resultsList);
            loading.Hide(editor);
            loading.Hide(objectSplitter);
            loading.Darken(horzSplitter);
            loading.Darken(vertSplitter);
            horzSplitter.Panel2Collapsed = true;

            if (A.dbId > 0 && loading.started == false)
                loading.Start();
            focusTextbox.Focus();
            timer.Stop();
            timer.Interval = 300;
            timer.Start();
        }

        protected void DoneLoading()
        {
            LoadLeftTabs();
            leftTabs.SelectTab(0);
            SetEditorText(selectedQuery.query, true);
            horzSplitter.Panel2Collapsed = false;
            UpdateLeftPanelLayout();
            UpdateRightPanelLayout();
            allObjectsTree.tree.UpdateObjects(true);
            /*
            if (Settings.Get("showParseGraph", false))
                ShowParseGraph();
                */
            timer.Interval = 1000;
            focusTextbox.Visible = false;
            A.SetStatus(Status.Ready);
            tabsDirty = true;
            loading.Stop();
        }

        public void LoadLeftTabs()
        {
            leftTabs.Clear();

            if (A.dbId == 0)
                return;

            List<Query> queries = new List<Query>();
            S.GetQueries(queries);
            foreach (Query q in queries)
                leftTabs.SetItemData(leftTabs.AddTab(q.label), q);


            if (leftTabs.Count == 0)
            {
                Query q = new Query(Settings.AddQuery());
                leftTabs.SetItemData(leftTabs.AddTab(q.label), q);
            }
            leftTabs.SelectTab(0);
        }

        void UpdateLeftTabs()
        {
            tabsDirty = false;
            int ct = 0;
            for (int i = 0; i < leftTabs.Count; i++)
            {
                Query q = leftTabs.GetItemData(i) as Query;
                string text = q.label == "" ? "Query " + (++ct) : q.label;
                if (q.dirty)
                    text += "*";
                leftTabs[i].Text = text;
            }
            leftTabs.UpdateTabs();
            UpdateLeftPanelLayout(true);
        }

        void SetEditorText(string sql, bool clearUndoHistory)
        {
            if (sql == null)
                sql = "";

            ClearParseViews();
            Parser.TextChanged(sql, 0, true);
            editor.Text = sql;
            if (clearUndoHistory)
                editor.EmptyUndoBuffer();
            editor.Focus();
        }

        public void SelectQuery(int id)
        {
            resultsList.ClearResults();
            selectedQuery.currentQueryVersionId = id;
            SetEditorText(selectedQuery.query, true);
        }

        void SaveAllDirtyTabs()
        {
            for (int i = 0; i < leftTabs.Count; i++)
            {
                Query q = leftTabs.GetItemData(i) as Query;
                if (q.dirty)
                    q.Save();
            }

            tabsDirty = true;
            queryHistory.dirty = true;
        }

        void UpdateParseViews()
        {
            parseViewsDirty = false;

            if (parseGraph != null && parseGraph.Visible)
                parseGraph.UpdateGraph();
            activeObjectsTree.tree.UpdateObjects(true);
            allObjectsTreeDirty = true;
            UpdateParseTree();
        }

        void ClearParseViews()
        {
            if (parseGraph != null)
                parseGraph.ClearGraph();
            if (activeObjectsTree != null)
                activeObjectsTree.tree.Nodes.Clear();
        }

        void SaveTabPositions()
        {
            tabPositionsDirty = false;
            Dictionary<int, int> positions = new Dictionary<int, int>();
            for (int i = 0; i < leftTabs.Count; i++)
            {
                Query p = leftTabs.GetItemData(i) as Query;
                positions.Add(p.tabId, i + 1);
            }

            S.SetTabPositions(positions);
        }

        void OpenQuery(string sql, QueryInfo info = null, bool makeNewCopy = false, bool runNow = false)
        {
            int index = info == null ? -1 : GetTabIndexByTabId(info.queryId);
            if (index >= 0)
            {
                Query q = leftTabs.GetItemData(index) as Query;
                q.query = sql;
                leftTabs.SelectTab(index);
            }
            else
            {
                Query q;
                if (info == null)
                {
                    q = new Query(Settings.AddQuery("", sql));
                }
                else if (makeNewCopy)
                {
                    T.Assert(sql == info.expr);
                    q = new Query(Settings.AddQuery(info.tabLabel, info.expr));
                }
                else
                {
                    S.OpenQuery(info.queryId, sql);
                    q = new Query(info);
                }
                leftTabs.SetItemData(leftTabs.AddTab(q.label), q);
                leftTabs.SelectTab(leftTabs.Count - 1);
                SetEditorText(sql, true);
            }
            // SetEditorText(sql);
            tabsDirty = true;
            tabPositionsDirty = true;

            if (runNow)
                background.QueueQuery(selectedQuery);
        }

        bool ShowConnectDialog(bool closeOnCancel)
        {
            SetAppStatus(Status.Ready);
            DlgConnection dlg = new DlgConnection();
            if (DialogResult.OK == dlg.ShowDialog(this))
            {
                if(Reload(dlg.databaseIdResult))
                    return true;
                closeOnCancel = true;
            }
            if(closeOnCancel)
                Close();

            return false;
        }

        void ShowNewQueryDialog(List<DbTable> tables, List<DbColumn> columns)
        {
            DlgNewQuery dlg = new DlgNewQuery(tables, columns);

            if (DialogResult.OK == dlg.ShowDialog(this))
            {
                if (dlg.includeWorkingObjects)
                {
                    foreach (DbTable t in dlg.workingTables)
                    {
                        t.bookmarked = true;
                        foreach (DbColumn c in dlg.workingColumns)
                            c.bookmarked = true;
                    }

                }
                OpenQuery(dlg.sql);
            }
        }

        void ShowTableGraph()
        {
            if (tableGraph == null)
                tableGraph = new LayoutGraphTable(this);
            if (!tableGraph.Visible)
                tableGraph.ShowGraph();
        }
        
        void HideTableGraph()
        {
            if (tableGraph != null)
                tableGraph.Hide();
        }

        void ShowParseGraph()
        {
            if (parseGraph == null)
                parseGraph = new LayoutGraphParse(this);
            parseGraph.ShowGraph();
        }

        void HideParseGraph()
        {
            if (parseGraph != null)
                parseGraph.Hide();
        }

        public int WriteMessagesToLog(int max = 50)
        {
            int ct = 0;
            lock (A.messages)
            {
                for (int i = 0; A.messages.Count > 0 && i < max; i++)
                {
                    Msg msg = A.messages[0];
                    A.messages.RemoveAt(0);
                    AppendToLog(msg);

                    if (msg.showLog && rightTabs.SelectedTab.Text != "Log")
                    {
                        Settings.Set("showLog", true);
                        rightTabs.ShowTab("Log", true);
                        rightTabs.SelectTab("Log");
                        editor.Focus();
                        ct++;
                    }

                }
            }
            return ct;
        }
    }
}
