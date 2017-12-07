using System;
using System.Collections.Generic;
using Microsoft.Msagl.Drawing;
using Color = System.Drawing.Color;
using DrawingNode = Microsoft.Msagl.Drawing.Node;
using fp.lib;
using fp.lib.dbInfo;
using fp.lib.forms;

namespace fp.sqratch
{
    public partial class LayoutGraphTable : LayoutGraph
    {
        HashSet<string> paths = new HashSet<string>();

        public LayoutGraphTable(main f) : base(f, "tableGraph")
        {

        }

        protected override void Init()
        {
            InitializeComponent();
            showExplicit.Checked = S.Get("tableGraphShowExplicit", true);
            showInferred.Checked = S.Get("tableGraphShowInferred", true);
            layoutCombo.Items.AddRange(new[] { "Sugiyama", "Ranking", "Incremental", "MDS" });
            layoutCombo.SelectString(S.Get("tableGraphLayout", "MDS"));
        }

        protected override void OnUpdateGraph()
        {
            paths.Clear();
            layoutCombo.Focus();
            S.Set("tableGraphLayout", layoutCombo.SelectedItem.ToString());
            drawingGraph = new Graph();

            foreach (DbTable t in A.db.tables.Values)
                AddTable(t);
            if (true)
            {
                double width = 100;
                double height = 50;
                drawingGraph.Attr.LayerSeparation = height / 2;
                drawingGraph.Attr.NodeSeparation = width / 2;
                double arrowHeadLenght = width / 10;
                foreach (Microsoft.Msagl.Drawing.Edge e in drawingGraph.Edges)
                    e.Attr.ArrowheadLength = (float)arrowHeadLenght;
                drawingGraph.LayoutAlgorithmSettings = GetLayoutSettings(layoutCombo.SelectedItem.ToString()); // FastIncrementalLayoutSettings.CreateFastIncrementalLayoutSettings(); // new MdsLayoutSettings(); // new SugiyamaLayoutSettings();
                viewer.Graph = drawingGraph;
            }
        }


        public void AddTable(DbTable t)
        {
            int ct = 0;
            foreach (DbTableConstraint constraint in t.references.Each())
                if ((showExplicit.Checked && constraint.isForeignKey) || (showInferred.Checked && constraint.isSloppyForeignKey))
                    ct++;

            foreach (DbTableConstraint constraint in t.constraints.Values)
                if ((showExplicit.Checked && constraint.isForeignKey) || (showInferred.Checked && constraint.isSloppyForeignKey))
                    ct++;

            if (ct == 0)
                return;
          
            string leftTable = T.AppendTo(t.name, t.GetAlias(true), " ");
            Microsoft.Msagl.Drawing.Shape shape = Microsoft.Msagl.Drawing.Shape.Box;
            Color backColor = Color.DarkKhaki;
            Color borderColor = backColor;
            int radius = 20;
            DrawingNode node = drawingGraph.AddNode(leftTable);
            node.Attr.FillColor = node.Attr.FillColor = GraphColor(backColor);
            node.Attr.Color = GraphColor(borderColor);
            node.Attr.LabelMargin = 10;
            node.Attr.Shape = shape;
            node.Attr.XRadius = radius;
            node.Attr.YRadius = radius;
            node.LabelText = leftTable;
            foreach(DbTableConstraint constraint in t.constraints.Values)
            {
                if ((showExplicit.Checked && constraint.isForeignKey) || (showInferred.Checked && constraint.isSloppyForeignKey))
                {
                    string rightTable = T.AppendTo(constraint.referencedTable.name, constraint.referencedTable.GetAlias(true), " ");
                    string edgeInfo = constraint.RenderJoinCols(true);
                    Microsoft.Msagl.Drawing.Edge e = drawingGraph.AddEdge(leftTable, edgeInfo, rightTable);
                    e.Attr.Weight = 6;
                    paths.Add(t.name + "." + constraint.referencedTable.name);
                }
            }
        }
        
        protected override void OnUpdateLayout()
        {
            viewer.SetBounds(0, viewer.Top, ClientRectangle.Width, ClientRectangle.Height - optionsPanel.Bottom);

            int space = 2;
            int x = showExplicit.Bounds.Right + space;
            showInferred.Left = x;
            x += showInferred.Width + space;
            layoutLabel.PositionToLeftOf(layoutCombo);
            showLabel.PositionToLeftOf(showExplicit);
            showPanel.Width = x;
        }
     
        private void OnShowCheckboxChanged(object sender, EventArgs e)
        {
            UpdateGraph();
        }

        private void layoutCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateGraph();
        }
    }
}