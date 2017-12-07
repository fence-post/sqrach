using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Configuration;
using System.Runtime.InteropServices;
using ScintillaNET;
using fp.lib;
using fp.lib.sqlite;
using fp.lib.mysql;
using fp.lib.sqlparser;
using fp.lib.forms;

namespace fp.sqratch
{
    public partial class main : Form
    {
        public Query selectedQuery { get { return leftTabs.Count == 0 ? null : leftTabs.GetItemData(selectedTabIndex) as Query; } }
        AutoSuggest autoComplete = null;
        ClipboardHelper clipboardHelper;
        LayoutGraphParse parseGraph;
        LayoutGraphTable tableGraph;
        Background background = new Background();
        EditorHeader editorHeader;
        bool allowDebugging { get { return ConfigurationManager.AppSettings["testPath"] != null; } }
        int selectedTabIndex { get { return leftTabs.SelectedIndex; } }
        ObjectsTree allObjectsTree;
        ObjectsTree activeObjectsTree;
        bool objectTreesDirty = false;
        int lastEditorChange = 0;
        bool tabsDirty = false;
        bool inIdle = false;
        int lastStatusUpdate = 0;
        bool tabPositionsDirty = false;
        int maximizedChanged = 0;
        bool parseViewsDirty = true;
        bool allObjectsTreeDirty = false;
        bool queryWaitCursorEnabled = false;
        ToolButton addButton;
        ToolButton goButton;
        NiceTabControl leftTabs;
        NiceTabControl rightTabs;
        QueryListView queryHistory;
        ResultsListView resultsList;
        bool layoutDirty = false;
        Loading loading;
        Timer timer = new Timer();
        string baseUrl = "http://fence-post.com/";
   
        public main()
        {
            Font = SystemFonts.MessageBoxFont;
            InitializeComponent();

            A.Initialize(this);
            ToolButton.toolTip = ToolCheckBox.toolTip = toolTip;
            Wait.form = this;
            UI.LoadPreferences();
            focusTextbox.BackColor = horzSplitter.Panel1.BackColor = horzSplitter.Panel2.BackColor = vertSplitter.Panel1.BackColor = vertSplitter.Panel2.BackColor = UI.passiveBackColor;
         
            SuspendLayout();

            goButton = new ToolButton(bigButtonImages, 0, new EventHandler(OnRunClicked), null);
            goButton.SetText("Go");
            horzSplitter.Panel1.Controls.Add(goButton);
            addButton = new ToolButton(bigButtonImages, 7, new EventHandler(OnNewClicked), "New Query");
            horzSplitter.Panel1.Controls.Add(addButton);

            leftTabs = new NiceTabControl();
            leftTabs.MinHeight = 32;
            leftTabs.ContextMenuStrip = this.tabContextMenu;
            leftTabs.ImageList = bigButtonImages;
            leftTabs.DropdownImageIndex = 6;
            leftTabs.OnSelectedTabChanged += new EventHandler(OnLeftTabChanged);
            horzSplitter.Panel1.Controls.Add(leftTabs);

            rightTabs = new NiceTabControl();
            rightTabs.ImageList = bigButtonImages;
            rightTabs.MinHeight = 32;
            rightTabs.DropdownImageIndex = 6;
            rightTabs.OnSelectedTabChanged += new EventHandler(OnRightTabChanged);
            rightTabs.AddTab("Structure", objectSplitter);// , S.Get("showDatabaseObjects", true));
            rightTabs.AddTab("Query History", queryHistory);// , Settings.Get("showQueryHistory", false));
            rightTabs.AddTab("Log", logTextBox);// , Settings.Get("showLog", false));
            horzSplitter.Panel2.Controls.Add(rightTabs);

            allObjectsTree = CreateObjectTree(objectSplitter.Panel2, "allObjectsTree", "All Objects");
            objectSplitter.Panel2.Controls.Add(allObjectsTree);
            activeObjectsTree = CreateObjectTree(objectSplitter.Panel1, "activeObjectsTree", "Active Objects");
            objectSplitter.Panel1.Controls.Add(activeObjectsTree);
      
            rowCount.Width = FormsToolbox.GetTextWidth("00000000000000", Font);
            queryTime.Width = FormsToolbox.GetTextWidth(" 00:00 ", Font);
            rowCount.Text = queryTime.Text = "";

            autoComplete = new AutoSuggest(this, editor);
            Controls.Add(autoComplete);

            editorHeader = new EditorHeader(this, autoComplete);
            horzSplitter.Panel1.Controls.Add(editorHeader);
        
            resultsList = new ResultsListView(this);
            resultsList.ContextMenuStrip = resultsMenu;
            vertSplitter.Panel2.Controls.Add(resultsList);
            Query.resultsList = resultsList;
            clipboardHelper = new ClipboardHelper(editor, logTextBox, resultsList);

            queryHistory = new QueryListView();
            queryHistory.ContextMenuStrip = queryHistoryMenuStrip;
            horzSplitter.Panel2.Controls.Add(queryHistory);
            UpdateUIPreferences(false);
            
            Parser.onParsed = OnParsed;

            objectSplitter.SplitterWidth = vertSplitter.SplitterWidth = horzSplitter.SplitterWidth = 10;
            ResumeLayout();

            background.DoWork += new System.ComponentModel.DoWorkEventHandler(BackgroundInitialize);
            Background.status = BackgroundStatus.Loading;
            background.RunWorkerAsync();

            Width = Math.Max(600, T.Coalesce(S.initSettings.width, 900));
            Height = Math.Max(400, T.Coalesce(S.initSettings.height, 600));

            editor.MouseDown += new MouseEventHandler(OnEditorMouseDown);
            editor.KeyDown += new KeyEventHandler(OnEditorKeyDown);
            editor.Leave += new EventHandler(OnEditorLostFocus);
            timer.Tick += OnIdle;

            loading = new Loading(this);
            StartBackgroundLoading();
            
            Application.Idle += OnIdle;
            queryHistory.dirty = true;
        }

        #region init

        void main_Load(object sender, EventArgs e)
        {
            Padding p = new Padding(0, 0, 0, 0);
            vertSplitter.Margin = horzSplitter.Margin = objectSplitter.Margin = p;
            objectSplitter.Padding = vertSplitter.Padding = horzSplitter.Padding = p;
            horzSplitter.Panel1.Padding = horzSplitter.Panel2.Padding = p;
            vertSplitter.Panel1.Padding = p;
            objectSplitter.Panel1.Padding = objectSplitter.Panel2.Padding = p;
            activeObjectsTree.Margin = allObjectsTree.Margin = p;
            vertSplitter.Panel1.Padding = vertSplitter.Panel2.Padding = p;
            logTextBox.BorderStyle = queryHistory.BorderStyle = editor.BorderStyle = allObjectsTree.BorderStyle = activeObjectsTree.BorderStyle = resultsList.BorderStyle = BorderStyle.None; //  UI.dark ? BorderStyle.None : BorderStyle.FixedSingle;
            progress.Minimum = progress.Maximum = progress.Value = 0;
            progress.Visible = false;
            focusTextbox.Bounds = new Rectangle(0, ClientRectangle.Height - 2, 2, focusTextbox.Height);
            
            WindowState = FormWindowState.Normal;
            if (S.initSettings.screen != "")
                if (!this.SelectScreen(S.initSettings.screen))
                    S.initSettings.screen = "";
            WindowState = S.initSettings.maximized ? FormWindowState.Maximized : FormWindowState.Normal;
            loading.UpdateProgress();
            focusTextbox.Focus();
            A.loading = false;
        }

        bool objectSplitterDistanceDirty = true;
        void UiLoad()
        {
            SuspendLayout();
   
            UpdateUIPreferences(false);
            UI.InitializeEditor(editor);
            horzSplitter.Panel2Collapsed = false;
            vertSplitter.SplitterDistance = T.Coalesce(S.initSettings.vertSplitterPos, vertSplitter.Height / 3);
            int width = ClientRectangle.Width / 4;
            width = T.MinMax(200, 400, width);
            horzSplitter.SplitterDistance = T.Coalesce(S.initSettings.horzSplitterPos, ClientRectangle.Width - width);
            objectSplitter.SplitterDistance = vertSplitter.SplitterDistance + editorHeader.Height;
            // objectSplitter.SplitterDistance = vertSplitter.SplitterDistance + (vertSplitter.SplitterWidth - leftTabs.Bottom);
            objectSplitterDistanceDirty = true;
            parseStatus.Width = parseStatus.Height;
            rightTabs.ShowTab("Structure", S.Get("showDatabaseObjects", true));
            rightTabs.ShowTab("Query History", S.Get("showQueryHistory", false));
            rightTabs.ShowTab("Log", S.Get("showLog", false));
            UpdateLeftPanelLayout();
            UpdateRightPanelLayout();
            A.AddToLog("ui initialized");
            rightTabs.SelectTab(S.Get("rightTab", "Structure"));
            ResumeLayout();
            A.uiLoaded = true;
        }

        void SetAppStatus(Status status)
        {
            A.SetStatus(status);
            UpdateStatusBar();
        }


        private void main_Shown(object sender, EventArgs e)
        {
            if (S.initSettings.initError != null)
            {
                A.OkBox(this, "Could not load settings.\r\n" + S.initSettings.initError, MessageBoxIcon.Error);
                Shutdown();
            }
            else if (A.dbId == 0)
                ShowConnectDialog(true);
        }
        #endregion

        protected void OnIdle(Object sender, EventArgs e)
        {           
            if (inIdle)
                return;

            if (loading.started)
            {
                loading.UpdateProgress();
                focusTextbox.Focus();
            }
                
            if (A.loading)
                return;

            if (A.currentStatus == Status.Closing)
                return;

            if (Background.status == BackgroundStatus.Loading)
                return;

            if (A.db == null)
                return;

            inIdle = true;
            idles++;
            if (loading.Visible)
            {
                if(!A.uiLoaded)
                    UiLoad();
                DoneLoading();
            }
            else if(A.ready)
            {
                int workDone = 0;
                editorHeader.OnIdle(selectedQuery);
                if (queryWaitCursorEnabled && A.currentStatus != Status.Executing)
                {
                    Cursor = Cursors.Default;
                    editor.UseWaitCursor = false;
                    queryWaitCursorEnabled = false;
                    goButton.SetText("Go", 0);
                }
                else if (queryWaitCursorEnabled && Cursor != Cursors.WaitCursor)
                {
                    queryWaitCursorEnabled = true;
                    Cursor = Cursors.WaitCursor;
                    editor.UseWaitCursor = true;
                    goButton.Cursor = Cursors.Default;
                }
                if(A.currentStatus == Status.LoadingRows)
                {
                    UpdateProgress();
                    workDone += 5;
                }
                if (layoutDirty)
                {
                    layoutDirty = false;
                    UpdateLeftPanelLayout();
                    UpdateRightPanelLayout();
                    workDone += 5;
                }
                if (tabsDirty)
                {
                    UpdateLeftTabs();
                    workDone++;
                }
                if (tabPositionsDirty)
                {
                    SaveTabPositions();
                    workDone += 1;
                }
                if (resultsList.resultsReady)
                {
                    resultsList.LoadResults();
                    workDone += 10;
                    allObjectsTreeDirty = true;
                }
                if (resultsList.lastColumnDirty)
                {
                    resultsList.UpdateLastColumn();
                    workDone++;
                }
                if (parseViewsDirty)
                {
                    UpdateParseViews();
                    workDone += 10;
                }
                if (queryHistory != null && queryHistory.dirty)
                {
                    queryHistory.UpdateQueryHistory();
                    workDone += 10;
                }
                if(workDone < 20)
                {
                    if (allObjectsTreeDirty)
                    {
                        allObjectsTreeDirty = false;
                        // activeObjectsTree.tree.UpdateObjects(false);
                        allObjectsTree.tree.UpdateObjects(false);
                        workDone += 10;
                    }

                    if(objectTreesDirty)
                    {
                        objectTreesDirty = false;
                        activeObjectsTree.tree.UpdateObjects(true);
                        allObjectsTree.tree.UpdateObjects(true);
                    }

                    if (queryHistory.columnWidthsDirty >= 0)
                        queryHistory.UpdateColumnWidths();
                    if (Environment.TickCount - lastStatusUpdate > 500)
                    {
                        UpdateStatusBar();
                        workDone += 2;
                    }
                    activeObjectsTree.OnIdle();
                    if (maximizedChanged != 0 && Environment.TickCount - maximizedChanged > 1000)
                    {
                        Settings.initSettings.maximized = WindowState == FormWindowState.Maximized;
                        maximizedChanged = 0;
                    }
                    if(Parser.suggestionsReady)
                    {
                        Parser.suggestionsReady = false;
                        editorHeader.UpdateSuggestions();
                    }

                    if(WriteMessagesToLog() > 10)
                        workDone += 10;

                    if (workDone == 0)
                    {
                        if (Parser.OnIdle(editor.Text, editor.CurrentPosition))
                            workDone += 5;
                    }

                    if (workDone == 0)
                        resultsList.FixForScroll();
                }
            }
            inIdle = false;
        }

        protected override void WndProc(ref Message m)
        {
            if (!A.loading)
            {
                // WM_SYSCOMMAND
                if (m.Msg == 0x0112)
                {
                    if (m.WParam.ToInt32() == 0x0000f030 || m.WParam.ToInt32() == 0x0000f012) // Restore event - SC_RESTORE from Winuser.h
                    {
                        maximizedChanged = Environment.TickCount;   
                    }
                }
            }
            
            base.WndProc(ref m);
        }
       
        public void OnParsed()
        {
            parseViewsDirty = true;
        }
        
        void OnRightTabChanged(object sender, EventArgs e)
        {
            if(A.uiLoaded)
                  S.Set("rightTab", rightTabs.SelectedTab.Text);
        }
        
        void OnLeftTabChanged(object sender, EventArgs e)
        {
            //A.ClearResults();
            Parser.Clear();
            resultsList.Clear();
            SetEditorText(selectedQuery != null && selectedQuery.query  != null ? selectedQuery.query : "", true);

            if (A.uiLoaded)
            {
                if(selectedQuery != null)
                    S.Set("lastTab", selectedQuery.tabId);
                queryHistory.dirty = true;
            }              
        }

        void main_Move(object sender, EventArgs e)
        {
            if (A.loading == false)
            {
                HidePopups();
                string nam = this.GetCurrentScreen();
                if (S.initSettings.screen != nam)
                {
                    S.initSettings.screen = nam;
                    System.Diagnostics.Debug.WriteLine(S.initSettings.screen);
                }
            }
        }

        bool TabsAreDirty(bool exceptSelectedOne = false)
        {
            for(int i = 0; i < leftTabs.Count; i++)
            {
                Query p = leftTabs.GetItemData(i) as Query;
                if ((exceptSelectedOne == false || selectedTabIndex != i) && p.dirty)
                    return true;
            }
            return false;
        }

        public void AppendToLog(Msg msg)
        {
            Color color = T.Case(msg.status == MsgStatus.Warning, UI.activeYellow, msg.status == MsgStatus.Error, UI.activeRed, Color.Empty);
            logTextBox.AppendText((msg.includeWhen ? msg.when.ToString("HH:mm:ss") + " " : "") + msg.msg + "\r\n", color);
        }

        int GetTabIndexByTabId(int tabId)
        {
            for (int i = 0; i < leftTabs.Count; i++)
            {
                Query p = leftTabs.GetItemData(i) as Query;
                if (p.tabId == tabId)
                    return i;
            }
            return -1;
        }

        #region shutdown 

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // overriding this helps to release parseGraph or autocomplete when visible
            e.Cancel = false;
            base.OnFormClosing(e);
        }

        void main_FormClosed(object sender, FormClosedEventArgs e)
        {
            logTextBox.Dispose();
            progress.Dispose();
            Dispose();
        }

        void main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (TabsAreDirty())
            {
                DialogResult result = this.YesNoCancelBox("Queries have been changed.  Do you want to save them before closing?");
                if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
                else if (result == DialogResult.Yes)
                    SaveAllDirtyTabs();
            }
            Shutdown();
        }

        void Shutdown()
        {
            using (new Wait())
            {
                this.horzSplitter.Panel2.Controls.Remove(logTextBox);
                logTextBox.Dispose();
                this.statusStrip.Items.Remove(progress);
                progress.Dispose();
                A.SetStatus(Status.Closing);
                background.Shutdown();
                UpdateStatusBar();
                statusStrip.Invalidate();
                statusStrip.Update();
                bool visible = false;
                if (parseGraph != null)
                {
                    visible = parseGraph.Visible;
                    parseGraph.Close();
                        parseGraph = null;
                }
                Settings.Set("showParseGraph", visible);
                Settings.SaveSettings(true);
                
                for(int i = 0; i < 10; i++)
                {
                    T.Sleep(500);
                    T.Debug("waiting for background to stop");
                    if (Background.status == BackgroundStatus.Stopped)
                        break;
                }
            }
        }

        #endregion
        
    }
}
