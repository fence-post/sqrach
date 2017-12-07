using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using fp.lib;
using fp.lib.forms;
using fp.lib.sqlparser;
using fp.lib.dbInfo;

namespace fp.sqratch
{
    public partial class EditorHeader : UserControl
    {
        EditorHeaderDropdown queryCombo;
        EditorHeaderDropdown selectCombo;
        EditorHeaderDropdown fromCombo;
        EditorHeaderDropdown whereCombo;
        EditorHeaderDropdown orderCombo;
        EditorHeaderDropdown groupCombo;
        main mainForm;
        AutoSuggest autoSuggest;
        bool loading = true;
        List<Control> buttons = new List<Control>();
        List<EditorHeaderDropdown> dropdowns = new List<EditorHeaderDropdown>();

        public ToolButton CreateButton(int image, string text, EventHandler h)
        {
            ToolButton b = new ToolButton(UI.buttonImages, image, h, text);
            buttons.Add(b);
            Controls.Add(b);
            return b;

        }

        ToolCheckBox CreateCheckBox(int image, string text, EventHandler h)
        {
            ToolCheckBox b = new ToolCheckBox(UI.buttonImages, image, h, text);
            Controls.Add(b);
            buttons.Add(b);
            return b;
        }

        void OnGo(object sender, EventArgs e)
        {

        }

        ToolButton prevButton;
        ToolButton nextButton;
        ToolButton formatButton;
        ToolCheckBox autoAliasCheckbox;
        ToolCheckBox autoTableCheckbox;

        public void OnIdle(Query selectedQuery)
        {
            nextButton.Visible = (selectedQuery != null && selectedQuery.canGoNext);
            prevButton.Visible = (selectedQuery != null && selectedQuery.canGoPrev);
            formatButton.Enabled = (selectedQuery != null && selectedQuery.query != "");
        }

        void OnFormatQuery(object sender, EventArgs a)
        {
            mainForm.FormatSqlInEditor();
        }

        void OnPrevQuery(object sender, EventArgs a)
        {
            mainForm.SelectQuery(mainForm.selectedQuery.prevQueryId);
        }

        void OnNextQuery(object sender, EventArgs a)
        {
            mainForm.SelectQuery(mainForm.selectedQuery.nextQueryId);
        }

        void OnAutoAlias(object sender, EventArgs a)
        {
            autoAliasCheckbox.Checked = S.Toggle("AutocompleteInsertAliases", false);
        }

        void OnAutoFrom(object sender, EventArgs a)
        {
            autoTableCheckbox.Checked = S.Toggle("AutocompleteInsertTables", false);
        }

        public EditorHeader(main f, AutoSuggest a)
        {
            autoSuggest = a;
            mainForm = f;
            InitializeComponent();
            
            nextButton = CreateButton(0, "Next", new EventHandler(OnNextQuery));
            prevButton = CreateButton(1, "Prev", new EventHandler(OnPrevQuery));
            formatButton = CreateButton(9, "Format Query", new EventHandler(OnFormatQuery));
            autoTableCheckbox = CreateCheckBox(11, "Auto From", new EventHandler(OnAutoFrom));
            autoAliasCheckbox = CreateCheckBox(10, "Auto Alias", new EventHandler(OnAutoAlias));

            autoTableCheckbox.Checked = S.Get("AutocompleteInsertTables", false);
            autoAliasCheckbox.Checked = S.Get("AutocompleteInsertAliases", false);

            queryCombo = AddCombo("Query");
            selectCombo = AddCombo("Select");
            fromCombo = AddCombo("From");
            whereCombo = AddCombo("Where");
            groupCombo = AddCombo("Group");
            orderCombo = AddCombo("Order");
            loading = false;
        }

        EditorHeaderDropdown AddCombo(string nam)
        {
            EditorHeaderDropdown combo = new EditorHeaderDropdown(nam);
            combo.BackColor = BackColor;
            combo.Click += new EventHandler(OnComboDropDown);
            dropdowns.Add(combo);
            Controls.Add(combo);
            return combo;
        }

        public void UpdateUIPreferences()
        {
            autoTableCheckbox.Checked = S.Get("AutocompleteInsertTables", false);
            autoAliasCheckbox.Checked = S.Get("AutocompleteInsertAliases", false);
            
            BackColor = UI.headerBackColor;
            ForeColor = UI.headerForeColor;
            foreach (Control c in buttons)
            {
                ToolButton b = c as ToolButton;
                if (b != null)
                    b.SetUIPreferences(BackColor, UI.buttonHoverColor, UI.smallButtonImages);
                ToolCheckBox cb = c as ToolCheckBox;
                if (cb != null)
                    cb.SetUIPreferences(BackColor, UI.buttonHoverColor, UI.buttonCheckedColor, UI.smallButtonImages);
            }


            // foreach (EditorHeaderDropdown combo in dropdowns)


            Invalidate(true);
        }

        private void OnComboDropDown(object sender, EventArgs e)
        {
            EditorHeaderDropdown c = sender as EditorHeaderDropdown;
            string keyword = c.Text.ToLower();
            if(Parser.querySuggestions.ContainsKey(keyword))
            {
                Point pMouse = Cursor.Position;
                Point p = new Point(c.Left, c.Bottom);
                p = PointToScreen(p);
                autoSuggest.UpdateSuggestions(Parser.querySuggestions[keyword], p);
            }
        }

        public void UpdateSuggestions()
        {
            bool querySuggestions = S.Get("QuerySuggestions", true);
            queryCombo.Visible = querySuggestions && Parser.querySuggestions.ContainsKey("query");
            selectCombo.Visible= querySuggestions && Parser.querySuggestions.ContainsKey("select");
            fromCombo.Visible  = querySuggestions && Parser.querySuggestions.ContainsKey("from");
            whereCombo.Visible = querySuggestions && Parser.querySuggestions.ContainsKey("where");
            groupCombo.Visible = querySuggestions && Parser.querySuggestions.ContainsKey("group");
            orderCombo.Visible = querySuggestions && Parser.querySuggestions.ContainsKey("order");
            EditorHeader_Resize(null, null);
        }

        private void EditorHeader_Resize(object sender, EventArgs e)
        {
            if (loading)
                return;

            int space = 2;
            Height = space + space + SystemInformation.MenuHeight;

            Rectangle rect = ClientRectangle;
            rect = new Rectangle(rect.Left, space, rect.Width, rect.Height - (space + space));
            int size = rect.Height;
            foreach (Control b in buttons)
            {
                b.SetBounds(rect.Right - size, rect.Top, size, size);
                rect.Width -= size + space + space + space;
            }
            int cy = (Height - queryCombo.Height) / 2;
            rect = new Rectangle(0,cy, rect.Width, queryCombo.Height);

            PositionCombo(orderCombo, ref rect);
            PositionCombo(groupCombo, ref rect);
            PositionCombo(whereCombo, ref rect);
            PositionCombo(fromCombo, ref rect);
            PositionCombo(selectCombo, ref rect);
            PositionCombo(queryCombo, ref rect);
           
        }

        void PositionCombo(EditorHeaderDropdown combo, ref Rectangle rect)
        {
            if(combo.Visible)
            {
                int width = combo.Width;
                combo.Bounds = new Rectangle(rect.Right - width, rect.Top, width, rect.Height);
                // rect.Offset(-width, 0);
                rect.Width -= width;
            }
        }
    }
}
