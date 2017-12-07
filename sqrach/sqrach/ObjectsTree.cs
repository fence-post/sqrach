using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using fp.lib.forms;
using System.Drawing;

namespace fp.sqratch
{
    public class ObjectsTree : Panel
    {
        public ObjectsTreeView tree;
        public ParseTreeView parseTree;
        public bool allObjectsTree;

        Panel header;
        Label label;
        TextBox searchTextBox;
        ToolButton xButton;
        List<Control> buttons = new List<Control>();
        List<Control> parseTreeButtons = new List<Control>();
        ImageList buttonImages;
        ToolButton collapseButton;
        ToolButton expandButton;
        ToolCheckBox searchColumns;
        ToolCheckBox searchTables;

        ToolCheckBox showDetails;
        ToolCheckBox showMore;
        ToolCheckBox showBase;

        public ObjectsTree(bool all)
        {
            allObjectsTree = all;
            buttonImages = UI.buttonImages;

            searchTextBox = new TextBox();
            searchTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            searchTextBox.Location = new System.Drawing.Point(0, 0);
            searchTextBox.Name = "searchTextBox";
            searchTextBox.Size = new System.Drawing.Size(0, 31);
            searchTextBox.BackColor = UI.headerSearchBackground;
            searchTextBox.ForeColor = UI.passiveForeColor;
            searchTextBox.KeyUp += new KeyEventHandler(OnSearchKeyPress);
            searchTextBox.Visible = false;

            xButton = new ToolButton(this.buttonImages, 13, new EventHandler(OnClearSearch));
            xButton.BackColor = searchTextBox.BackColor;

            Resize += new EventHandler(OnResize);
            label = new System.Windows.Forms.Label();
            Font = label.Font = SystemFonts.MessageBoxFont;

            label.AutoSize = true;
            label.Location = new System.Drawing.Point(21, 29);
            label.ForeColor = UI.headerForeColor;
            label.Text = allObjectsTree ? "All Objects" : "Active Objects";
            header = new System.Windows.Forms.Panel();
            header.Font = SystemFonts.MessageBoxFont;
            header.BackColor = UI.backColor4;
            header.Controls.Add(searchTextBox);
            header.Controls.Add(label);
            header.Controls.Add(xButton);
            header.Location = new System.Drawing.Point(0, 0);
            header.Size = new System.Drawing.Size(10, 10);
            Controls.Add(header);

            if (allObjectsTree)
            {
                searchColumns = CreateCheckBox(buttons, 2, "Search Columns", new EventHandler(OnFilterColumns));
                searchTables = CreateCheckBox(buttons, 3, "Search Tables", new EventHandler(OnFilterTables));
            }
            else
            {
                CreateButton(buttons, 5, "Show Parse Tree", new EventHandler(OnParseTree));
                CreateButton(buttons, 4, "Show Active Objects", new EventHandler(OnObjects));
                showDetails = CreateCheckBox(parseTreeButtons, 7, "Show Details", new EventHandler(OnShowDetails));
                showMore = CreateCheckBox(parseTreeButtons, 8, "Show More", new EventHandler(OnShowMore));
                showBase = CreateCheckBox(parseTreeButtons, 6, "Show Base Structure", new EventHandler(OnShowBaseStructure));
                collapseButton = CreateButton(parseTreeButtons, 14, "Collapse", new EventHandler(OnCollapse));
                expandButton = CreateButton(parseTreeButtons, 15, "Expand", new EventHandler(OnExpand));
                parseTree = new ParseTreeView();
                Controls.Add(parseTree);
                parseTree.Visible = false;
                parseTree.BorderStyle = BorderStyle.None;
                parseTree.AfterExpand += parseTree.OnAfterExpand;
            }
       
            tree = new ObjectsTreeView(all ? "allObjectsTreeView" : "activeObjectsTreeView");
            tree.ImageList = UI.treeImages;
            tree.expandByDefault = !allObjectsTree;
            tree.Location = new System.Drawing.Point(0, 0);
            tree.ShowNodeToolTips = true;
            tree.Size = new System.Drawing.Size(300, 300);
            tree.TabIndex = 0;
            Controls.Add(tree);
            UpdateShowCheckboxes();
        }

        public void Clear()
        {
            tree.Nodes.Clear();
            tree.nodesByObjectName.Clear();
            if (parseTree != null)
                parseTree.Nodes.Clear();
        }

        public void OnIdle()
        {
            if(collapseButton != null && expandButton != null)
            {
                collapseButton.Enabled = parseTree != null && parseTree.canCollapse;
                expandButton.Enabled = parseTree != null && parseTree.canExpand;
            }
        }

        void UpdateShowCheckboxes()
        {
            if(showDetails != null)
            {
                showDetails.Checked = S.Get("parseTreeShow") == "Details";
                showMore.Checked = S.Get("parseTreeShow") == "More";
                showBase.Checked = S.Get("parseTreeShow", "Sparse") == "Sparse";
            }
        }

        void OnShowDetails(object sender, EventArgs e)
        {
            S.Set("parseTreeShow", "Details");
            UpdateShowCheckboxes();
            UpdateParseTree();

        }

        void OnShowMore(object sender, EventArgs e)
        {
            S.Set("parseTreeShow", "More");
            UpdateShowCheckboxes();
            UpdateParseTree();
        }

        void OnShowBaseStructure(object sender, EventArgs e)
        {
            S.Set("parseTreeShow", "Sparse");
            UpdateShowCheckboxes();
            UpdateParseTree();
        }

        void OnCollapse(object sender, EventArgs e)
        {
            parseTree.CollapseOneLevel();
        }

        void OnExpand(object sender, EventArgs e)
        {
            parseTree.ExpandOneLevel();
        }

        public void UpdateParseTree()
        {
            if (Parser.query != null && parseTree.Visible)
            {
                lock (Parser.query)
                {
                    parseTree.UpdateTree(Parser.query);
                }
            }
        }

        ToolButton CreateButton(List<Control> list, int image, string text, EventHandler h)
        {
            ToolButton b = new ToolButton(buttonImages, image, h, text);
            header.Controls.Add(b);
            list.Add(b);
            return b;
        }

        ToolCheckBox CreateCheckBox(List<Control> list, int image, string text, EventHandler h)
        {
            ToolCheckBox b = new ToolCheckBox(buttonImages, image, h, text);
            header.Controls.Add(b);
            list.Add(b);
            return b;
        }

        void OnSearchKeyPress(object sender, KeyEventArgs e)
        {
            tree.search = searchTextBox.Text;
            UpdateFilters();
        }

        void OnClearSearch(object sender, EventArgs args)
        {
            tree.search = searchTextBox.Text = "";
            tree.UpdateObjects(true);
        }

        void OnParseTree(object sender, EventArgs args)
        {
            tree.Visible = false;
            parseTree.Visible = true;
            label.Text = "Parse Tree";
            UpdateLayout();
            UpdateParseTree();
        }

        void OnObjects(object sender, EventArgs args)
        {
            tree.Visible = true;
            parseTree.Visible = false;
            label.Text = "Active Objects";
            UpdateLayout();
        }

        void OnFilterTables(object sender, EventArgs args)
        {
            tree.searchTables = searchTables.Checked;
            UpdateFilters();
        }

        bool updatingFilters = false;
        void UpdateFilters()
        {
            if (!updatingFilters)
            {
                updatingFilters = true;
                if (tree.searchTables == false && tree.searchColumns == false)
                {
                    tree.searchTables = true;
                    tree.searchColumns = true;
                }
                searchTables.Checked = tree.searchTables;
                searchColumns.Checked = tree.searchColumns;
                tree.UpdateObjects(true);
                updatingFilters = false;
            }

        }

        void OnFilterColumns(object sender, EventArgs args)
        {
            tree.searchColumns = searchColumns.Checked;
            UpdateFilters();
        }

        void OnGo(object sender, EventArgs args)
        {
        }

        public void UpdateUIPreferences()
        {
            searchTextBox.BackColor = UI.headerSearchBackground;
            searchTextBox.ForeColor = UI.passiveForeColor;
            buttonImages = UI.smallButtonImages;
            tree.Font = Font = UI.environmentFont;
            tree.ImageList = UI.treeImages;
            label.ForeColor = UI.headerForeColor;
            header.BackColor = UI.headerBackColor;
            BackColor = tree.BackColor = UI.passiveBackColor;
            tree.ForeColor = ForeColor = UI.passiveForeColor;
            if (parseTree != null)
            {
                // parseTree.ImageList = UI.treeImages;
                parseTree.Font = Font;
                parseTree.BackColor = BackColor;
                parseTree.ForeColor = ForeColor;
                SetButtonUiPreferences(parseTreeButtons);

            }

            xButton.SetUIPreferences(header.BackColor, UI.toolCheckboxCheckedColor, buttonImages);
            SetButtonUiPreferences(buttons);

          }

        void SetButtonUiPreferences(List<Control> list)
        {
            foreach (Control c in list)
            {
                ToolCheckBox cb = c as ToolCheckBox;
                if (cb != null)
                    cb.SetUIPreferences(header.BackColor, UI.buttonHoverColor, UI.buttonCheckedColor, buttonImages);
                else
                {
                    ToolButton b = c as ToolButton;
                    if (b != null)
                        b.SetUIPreferences(header.BackColor, UI.buttonHoverColor, buttonImages);
                }
            }
        }

        void OnResize(object sender, EventArgs a)
        {
            UpdateLayout();
        }

        void UpdateLayout()
        { 
            if (header != null)
            {
                int space = 2;
                Rectangle rect = new Rectangle(0, 0, ClientRectangle.Width, space + space + SystemInformation.MenuHeight);
                header.Bounds = rect; //.SetBounds(rect.Left, rect.Top, rect.Width, 32);
                int cy = 1 + ((rect.Height - label.Height) / 2);
                label.SetBounds(0, cy, label.Width, label.Height);
                tree.SetBounds(0, header.Bottom, ClientRectangle.Width, ClientRectangle.Height - header.Bottom);
                if(parseTree != null)
                    parseTree.SetBounds(0, header.Bottom, ClientRectangle.Width, ClientRectangle.Height - header.Bottom);

                rect = new Rectangle(rect.Left, space, rect.Width, rect.Height - (space + space));
                int size = rect.Height;
                foreach (Control b in buttons)
                {
                    b.SetBounds(rect.Right - size, rect.Top, size, size);
                    rect.Width -= size;
                }
                bool showing = parseTreeButtons.Count > 0 && parseTree.Visible;
                if(showing)
                    rect.Width -= size;
                foreach (Control b in parseTreeButtons)
                {
                    b.Visible = parseTree.Visible;
                    if(showing)
                    {
                        b.SetBounds(rect.Right - size, rect.Top, size, size);
                        rect.Width -= size + space + space + space;
                    }                   
                }
                rect.Width -= size / 2;
                size = searchTextBox.Height;
                cy = (rect.Height - searchTextBox.Height) / 2;
                xButton.SetBounds(rect.Right - size, rect.Top + cy, size, size);
                rect.Width -= size;
                int width = Math.Min(200, rect.Width - (label.Width + 10));
                xButton.Visible = searchTextBox.Visible = Name == "allObjectsTree" && width > 100;
                if(searchTextBox.Visible)
                {
                    searchTextBox.SetBounds(rect.Right - (width), rect.Top + cy, width, searchTextBox.Height);
                }
            }
        }
    }
}
