using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Msagl.GraphViewerGdi;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl;
using Microsoft.Msagl.Prototype.Ranking;
using Microsoft.Msagl.Layout.Layered;
using Microsoft.Msagl.Layout.MDS;
using Microsoft.Msagl.Core.Layout;
using Microsoft.Msagl.Layout.Incremental;
using Color = System.Drawing.Color;
using DrawingNode = Microsoft.Msagl.Drawing.Node;
using fp.lib;
using fp.lib.dbInfo;
using fp.lib.forms;

namespace fp.sqratch
{
    public class LayoutGraph : Form
    {
        protected Graph drawingGraph;
        protected GViewer viewer = new GViewer();
        main mainForm;
        bool loading = true;
        string settingsPrefix;

        public LayoutGraph()
        {

        }
        
        public LayoutGraph(main m, string pre)
        {
            settingsPrefix = pre;
            mainForm = m;
            Font = SystemFonts.MessageBoxFont;

            Init();

            this.Load += new System.EventHandler(this.LayoutGraphLoad);
            this.Shown += new System.EventHandler(this.LayoutGraphShown);
            this.VisibleChanged += new System.EventHandler(this.LayoutGraphVisibleChanged);
            this.Move += new System.EventHandler(this.LayoutGraphMove);
            this.Resize += new System.EventHandler(this.LayoutGraphResize);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LayoutGraphFormClosing);

            SuspendLayout();
            Controls.Add(viewer);
            viewer.Dock = DockStyle.Bottom;
            ResumeLayout();
            viewer.ToolBarIsVisible = false;
        }

        protected virtual void Init()
        {

        }

        public void ClearGraph()
        {
            drawingGraph = new Graph();
            viewer.Graph = drawingGraph;
        }

        private void LayoutGraphFormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Settings.Set(settingsPrefix + "Show", false);
            Hide();
        }
        
        private void LayoutGraphMove(object sender, EventArgs e)
        {
            if (loading)
                return;
            Settings.Set(settingsPrefix + "Left", Left);
            Settings.Set(settingsPrefix + "Top", Top);
        }

        private void LayoutGraphResize(object sender, EventArgs e)
        {
            if (loading)
                return;
            Settings.Set(settingsPrefix + "Width", Width);
            Settings.Set(settingsPrefix + "Height", Height);
            UpdateLayout();
        }

        private void LayoutGraphLoad(object sender, EventArgs e)
        {
            if (S.initSettings.screen != "")
                if (!this.SelectScreen(S.initSettings.screen))
                    S.initSettings.screen = "";
            loading = false;
            UpdateGraph();
        }

        private void LayoutGraphShown(object sender, EventArgs e)
        {
            UpdateLayout();
        }

        private void LayoutGraphVisibleChanged(object sender, EventArgs e)
        {
            UpdateLayout();
        }
        
        protected void UpdateLayout()
        {
            if (!loading)
                OnUpdateLayout();
        }

        protected virtual void OnUpdateLayout()
        {

        }

        public void UpdateGraph()
        {
            if (loading)
                return;
            try
            {
                OnUpdateGraph();
            }
            catch(Exception e)
            {
                A.AddToLog(e.Message);
            }
        }

        protected virtual void OnUpdateGraph()
        {

        }

        public void ShowGraph()
        {
//            Rectangle rect = mainForm.RectangleToScreen(mainForm.ClientRectangle);
            if(loading)
            {
                Rectangle rect = mainForm.ClientRectangle;
                Height = T.MinMax(500, rect.Height, Settings.Get(settingsPrefix + "Height", 500));
                Width = T.MinMax(809, rect.Width, Settings.Get(settingsPrefix + "Width", 809)); // https://en.wikipedia.org/wiki/Golden_rectangle
                Left = T.MinMax(rect.Left, rect.Right - Width, Settings.Get(settingsPrefix + "Left", rect.Right - Width));
                Top = T.MinMax(rect.Top, rect.Bottom - Height, Settings.Get(settingsPrefix + "Top", rect.Bottom - Height));
            }
            Show(mainForm);
        }

        protected LayoutAlgorithmSettings GetLayoutSettings(string layoutMethod)
        {
            switch (layoutMethod)
            {
                case "MDS":
                    return new MdsLayoutSettings();
                case "Ranking":
                    return new RankingLayoutSettings();
                case "Incremental":
                    return FastIncrementalLayoutSettings.CreateFastIncrementalLayoutSettings();
                default:
                    return new SugiyamaLayoutSettings();
            }
        }

        protected Microsoft.Msagl.Drawing.Color GraphColor(Color color)
        {
            return new Microsoft.Msagl.Drawing.Color(color.A, color.R, color.G, color.B);
        }

    }
}
