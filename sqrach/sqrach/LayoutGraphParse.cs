using System;
using Microsoft.Msagl.Drawing;
using Color = System.Drawing.Color;
using DrawingNode = Microsoft.Msagl.Drawing.Node;
using fp.lib;
using fp.lib.sqlparser;
using fp.lib.forms;

namespace fp.sqratch
{
    public partial class LayoutGraphParse : LayoutGraph, IVisitor
    {
        public LayoutGraphParse(main m) : base(m, "parseGraph")
        {
        }

        protected override void Init()
        {
            InitializeComponent();

            layoutCombo.Items.AddRange(new[] { "Sugiyama", "Incremental", "MDS", "Ranking"});
            layoutCombo.SelectString(S.Get("ParseGraphLayout", "Sugiyama"));

            highlightCombo.Items.AddRange(new[] { "", "Relationships", "Criteria", "Columns" });
            highlightCombo.SelectString(S.Get("ParseGraphHighlight", ""));
            showTables.Checked = S.Get("ParseGraphShowTables", true);
            showColumns.Checked = S.Get("ParseGraphShowColumns", true);
            showKeywords.Checked = S.Get("ParseGraphShowKeywords", true);
            showExpressions.Checked = S.Get("ParseGraphShowExpressions", true);
            showIdentifiers.Checked = S.Get("ParseGraphShowIdentifiers", true);
            showLiterals.Checked = S.Get("ParseGraphShowLiterals", true);
            showOperators.Checked = S.Get("ParseGraphShowOperators", true);
        }

        protected override void OnUpdateGraph()
        {
            layoutCombo.Focus();

            string highlight = highlightCombo.SelectedItem.ToString();
            S.Set("ParseGraphLayout", layoutCombo.SelectedItem.ToString());
            S.Set("ParseGraphHighlight", highlight);
            if (highlight == "")
            {
                S.Set("ParseGraphShowTables", showTables.Checked);
                S.Set("ParseGraphShowColumns", showColumns.Checked);
                S.Set("ParseGraphShowKeywords", showKeywords.Checked);
                S.Set("ParseGraphShowExpressions", showExpressions.Checked);
                S.Set("ParseGraphShowIdentifiers", showIdentifiers.Checked);
                S.Set("ParseGraphShowLiterals", showLiterals.Checked);
                S.Set("ParseGraphShowOperators", showOperators.Checked);
            }

            drawingGraph = new Graph();
            
            if (Parser.Accept(this))
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
    
        bool Showing(Token t, bool ch)
        {
            string highlight = highlightCombo.SelectedItem.ToString();
            if (highlight == "Relationships")
            {
                if (t.tokenType == TokenType.Keyword || t.tokenType == TokenType.Identifier || t.tokenType == TokenType.Operator || t.tokenType == TokenType.Literal)
                    return false;
                if (t.isPrimaryKeyword && t.tokenType != TokenType.Select && t.tokenType != TokenType.From)
                    return false;
            }
            else if (highlight == "Criteria")
            {
                if (t.tokenType == TokenType.Identifier)
                    return false;
                if (t.isPrimaryKeyword && t.tokenType != TokenType.Select && t.tokenType != TokenType.Where)
                    return false;
            }
            else if (highlight == "Columns")
            {
                if (t.tokenType == TokenType.Keyword || t.tokenType == TokenType.Identifier || t.tokenType == TokenType.Operator || t.tokenType == TokenType.Literal)
                    return false;
                if (t.isPrimaryKeyword && t.tokenType != TokenType.Select)
                    return false;
            }
            else
            {
                return ch;
            }

            return true;
        }

        public void Visit(Token token)
        {
            string label = null; // token.debugText;
            Microsoft.Msagl.Drawing.Shape shape = Microsoft.Msagl.Drawing.Shape.Box;
            Color backColor = UI.GetTokenBackColor(token);
            Color borderColor = backColor;
            int radius = 20;
            if (token.tokenType == TokenType.Column && Showing(token, showColumns.Checked))
            {
                Column c = token as Column;
                shape = Microsoft.Msagl.Drawing.Shape.Msquare;
                label = c.columnName;
            }
            else if (token.tokenType == TokenType.Operator && Showing(token, showOperators.Checked))
            {
                label = token.name;
            }
            else if (token.tokenType == TokenType.Table && Showing(token, showTables.Checked))
            {
                shape = Microsoft.Msagl.Drawing.Shape.Msquare;
                Table t = token as Table;
                label = T.AppendTo(token.name, t.tableAlias, " ");
            }
            else if (token.isPrimaryKeyword && Showing(token, showKeywords.Checked))
            {
                label = token.name;
            }
            else if (token.tokenType == TokenType.Keyword && Showing(token, showKeywords.Checked))
            {
                label = token.name;
            }
            else if (token.tokenType == TokenType.Expression && Showing(token, showExpressions.Checked))
            {
                label = token.name;
                radius = 10;
            }
            else if (token.tokenType == TokenType.Identifier && Showing(token, showIdentifiers.Checked))
            {
                label = token.name;
                radius = 5;
            }
            else if (token.tokenType == TokenType.Literal && Showing(token, showLiterals.Checked))
            {
                label = token.name;
                radius = 5;
                shape = Microsoft.Msagl.Drawing.Shape.Ellipse;
            }
            else if (token.tokenType == TokenType.Query)
            {
                label = token.name;
                backColor = Color.WhiteSmoke;
                borderColor = Color.DarkGray;
            }

            if(label != null)
            {
                label = token.name.LimitToLength(30);
                DrawingNode node = drawingGraph.AddNode(token.id);
                node.Attr.FillColor = node.Attr.FillColor = GraphColor(backColor);
                node.Attr.Color = GraphColor(borderColor);
                node.Attr.LabelMargin = 10;
                node.Attr.Shape = shape;
                node.Attr.XRadius = radius;
                node.Attr.YRadius = radius;
                node.LabelText = label;
                if (token.parentToken != null)
                    drawingGraph.AddEdge(token.id, token.parentToken.id);
                else if (token.parentQuery != null)
                    drawingGraph.AddEdge(token.id, token.parentQuery.id);

            }
        }

        protected override void OnUpdateLayout()
        {
            viewer.SetBounds(0, viewer.Top, ClientRectangle.Width, ClientRectangle.Height - optionsPanel.Bottom);

            int space = 2;
            int x = showTables.Bounds.Right + space;
            showColumns.Left = x;
            x += showColumns.Width + space;
            showKeywords.Left = x;
            x += showKeywords.Width + space;
            showExpressions.Left = x;
            x += showExpressions.Width + space;
            showIdentifiers.Left = x;
            x += showIdentifiers.Width + space;
            showLiterals.Left = x;
            x += showLiterals.Width + space;
            showOperators.Left = x;
            x += showOperators.Width + space;

            highlightLabel.PositionToLeftOf(highlightCombo);
            layoutLabel.PositionToLeftOf(layoutCombo);
            showLabel.PositionToLeftOf(showTables);

            showPanel.Width = x;
            showPanel.Visible = (highlightCombo.SelectedItem.ToString() == "") && (showPanel.Left + showPanel.Width < Width);
        }

        private void OnShowCheckboxChanged(object sender, EventArgs e)
        {
            UpdateGraph();
        }

        private void layoutCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateGraph();
        }

        private void highlightCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateLayout();
            UpdateGraph();
        }
    }
}