using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScintillaNET;
using fp.lib.dbInfo;
using fp.lib;
using fp.lib.forms;
using fp.lib.sqlparser;

namespace fp.sqratch
{
    public partial class AutoSuggest : UserControl
    {
        main parent = null;
        Scintilla editor = null;
        int leftWidth = 0;
        int rightWidth = 0;
        int middleWidth = 0;
        ImageList imageList;
        int maxVisibleItems = 10;
        SuggestionList suggestionList;

        public AutoSuggest(main p, Scintilla e)
        {
            parent = p;
            editor = e;
            InitializeComponent();
            Font = UI.environmentFont;
            theList.BackColor = BackColor = UI.autoCompleteBackColor;
            theList.ForeColor = ForeColor = UI.activeForeColor;
            theList.DrawMode = DrawMode.OwnerDrawFixed;
            theList.DrawItem += new DrawItemEventHandler(OnDrawItem);
        }
        
        private void AutoSuggest_Load(object sender, EventArgs e)
        {
            Left = parent.ClientRectangle.Right - Width;
            Top = parent.ClientRectangle.Bottom - Height;
            Hide();
        }

        public void UpdateUIPreferences()
        {
            Hide();
            Font = UI.environmentFont;
            imageList = UI.treeImages;
            theList.BackColor = BackColor = UI.headerBackColor; // UI.autoCompleteBackColor;
            theList.ForeColor = ForeColor = UI.activeForeColor;
        }

        public bool UpdateSuggestions(SuggestionList suggestions, Point p)
        {
            
            suggestionList = suggestions;
            theList.Sorted = false; //  !suggestionList.queryMode;
                
            Point pOrig = p;
            p = parent.PointToClient(p);

            leftWidth = 0;
            rightWidth = 0;
            middleWidth = 0;
            
            int lineHeight = theList.ItemHeight = FormsToolbox.GetTextHeight("0", this.Font) + 4;

            // suggestions.Clear();
            if (suggestions != null && suggestions.Count > 0)
            {
                int maxWidth = 0;
                theList.BeginUpdate();
                theList.Items.Clear();
                theList.SelectedIndex = -1;
                Suggestion selectedItem = null;
                string match = suggestions.textEntered.ToLower().Trim();
                foreach (Suggestion s in suggestions.GetSuggestions(!suggestionList.queryMode, match))
                {
                    if (s.expr.Trim().Length > 0)
                    {
                        if (s.expr.ToLower().Contains(match))
                        {
                            leftWidth = Math.Max(leftWidth, 20 + FormsToolbox.GetTextWidth(s.expr + "00", Font));
                            if (s.middleStuff != null)
                                middleWidth = Math.Max(middleWidth, FormsToolbox.GetTextWidth(s.middleStuff + "00", SystemFonts.MessageBoxFont));
                            if (s.rightStuff != null)
                                rightWidth = Math.Max(rightWidth, FormsToolbox.GetTextWidth(s.rightStuff + "00", SystemFonts.MessageBoxFont));
                            theList.Items.Add(s);
                            if (s.expr.ToLower().StartsWith(match)
                                && (selectedItem == null || s.position < selectedItem.position))
                                selectedItem = s;
                        }
                    }
                }
                theList.EndUpdate();
                if (selectedItem != null)
                {
                    theList.SelectedItem = selectedItem;
                    int topItem = Math.Max(0, theList.SelectedIndex - (maxVisibleItems / 2));
                    theList.TopIndex = topItem;
                }

                maxWidth = leftWidth + middleWidth + rightWidth;

                if (theList.Items.Count > 0)
                {
                    if (!Visible)
                    {
                        Show();
                        BringToFront();
                    }
                    T.Debug(p.Y.ToString() + " " + pOrig.Y.ToString());
                    int maxHeight = maxVisibleItems * lineHeight;
                    theList.Size = new Size(T.MinMax(20, 600, maxWidth), T.MinMax(lineHeight, maxHeight, lineHeight * theList.Items.Count));
                    int y = p.Y;
                    if(!suggestionList.queryMode)
                        y += lineHeight;
                    Bounds = new Rectangle(p.X, y, theList.Width, theList.Height);
                    editor.Focus();
                    return theList.Items.Count > 0;
                }
            }
            Hide();
            editor.Focus();
            return false;
        }

        public bool HasFocus()
        {
            return this.Focused || theList.Focused;
        }

        public void TakeFocus(Keys keyCode)
        {
            Focus();
            if (keyCode == Keys.Down && (theList.Items.Count - 1) > theList.SelectedIndex)
                theList.SelectedIndex += 1;
            else if (keyCode == Keys.Up && theList.SelectedIndex > 0)
                theList.SelectedIndex -= 1;
            theList.Focus();
            // suggestList.SelectedIndex = Math.Min(1, suggestList.Items.Count - 1);
        }

        /*
        public string GetSelectionAndClose(out bool openAgain)
        {
            openAgain = false;
            string result = "";
            if (theList.SelectedIndex >= 0)
            {
                Suggestion s = theList.SelectedItem as Suggestion;
                result = s.expr.ToString();
                openAgain = s.openImmediatelyAfter;
                if (s.tokenType == TokenType.Column && s.tableAlias == null && s.tableName != null)
                {
                    string alias = A.db.GetAliasForTable(s.tableName, true);
                    result = T.AppendTo(alias, result, ".");
                }
                result += " ";
            }

            Hide();
            editor.Focus();

            return result;
        }
        */

        public string editorText = null;
        public DbColumn addedColumn = null;

        public bool TakeSelection()
        {
            bool openAgain = false;
            Suggestion s = theList.SelectedItem as Suggestion;
            if(s != null)
            {
                openAgain = s.openImmediatelyAfter;
                if (suggestionList.queryMode)
                {
                    if (s.dbColumn != null)
                    {
                        SqlBuilder b = new SqlBuilder(suggestionList.query);
                        b.AddColumn(suggestionList.keyword as Keyword, s.dbColumn, suggestionList.includeAliases);
                        string sql = b.Render();
                        editor.Text = sql;
                    }
                    else if (s.dbTable != null)
                    {
                        SqlBuilder b = new SqlBuilder(suggestionList.query);
                        b.AddTable(s.dbTable, suggestionList.includeAliases);
                        string sql = b.Render();
                        editor.Text = sql;
                    }
                }
                else
                {
                    string txt = s.expr;
                    if (txt != "")
                    {
                        txt = txt.Trim() + " ";

                        editor.Focus();
                        editor.DeleteRange(suggestionList.wordStartPosition, suggestionList.currentPos - suggestionList.wordStartPosition);
                        editor.InsertText(suggestionList.wordStartPosition, txt);
                        int pos = suggestionList.wordStartPosition + txt.Length;
                        while (editor.Text.Length > pos + 1 && editor.Text[pos - 1] == ' ' && editor.Text[pos] == ' ')
                            editor.DeleteRange(pos, 1);
                        if (s.dbColumn != null)
                        {
                            A.db.ColumnUsed(s.dbColumn);
                            A.db.TableUsed(s.dbColumn.table.name);
                            if (s.tableAlias != null && S.Get("RememberAliases", true))
                                A.db.AliasUsed(s.tableAlias, s.dbColumn.table.name);

                            if (S.Get("AutocompleteInsertTables", false))
                                AddTables(s.dbColumn);
                        }

                        editor.CurrentPosition = editor.SelectionStart = editor.SelectionEnd = pos;


                    }
                }

            }

            
            
            Hide();
            editor.Focus();
            return openAgain;
        }

        public void AddTables(DbColumn dbColumn)
        {
            if (dbColumn != null)
            {
                lib.sqlparser.Query tryQuery = lib.sqlparser.Query.Parse(A.db, editor.Text, false);
                if (tryQuery != null)
                {
                    SqlBuilder b = new SqlBuilder(tryQuery);
                    b.AddTable(dbColumn.table, suggestionList.includeAliases);
                    string sql = b.Render();
                    editor.Text = sql;
                    
                }
            }
        }
        
        private void suggestList_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            {
                TakeSelection();
            }
            else if (e.KeyCode == Keys.Escape)
                Hide();
        }

        private void suggestList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            TakeSelection();
        }

        int GetFactor(double val, double min, double max)
        {
            return Convert.ToInt32(T.MinMax(min, max, T.Normalize(val, 0, 1, max)));
        }

        void DrawContext(Graphics g, Rectangle rcBounds, DbColumn col)
        {
            rcBounds.Width = 400;
            if (col.valueVariances.Count > 0)
            {
                int steps = col.valueVariances.Count;
                int stepWidth = rcBounds.Width / steps;
                Rectangle rc = new Rectangle(rcBounds.Left, rcBounds.Top, stepWidth, rcBounds.Height);
                for (int i = 0; i < steps; i++)
                {
                    double factor1 = col.valueVariances[i];
                    double factor2 = col.countVariances[i];

                    int red = 128;
                    int green = GetFactor(factor1, 0, 254);
                    int blue = 254 - green;
                    Color clr = Color.FromArgb(30, red, green, blue);
                    double height = rcBounds.Height / 2;
                    height = GetFactor(factor2, 0, height);
                    // T.Debug(col.name + " factor1", factor1);
                    T.Debug(col.name + " factor2", factor2);
                    T.Debug("height", height);
                    T.Debug("---------");
                    SolidBrush brush = new SolidBrush(clr);
                    g.FillRectangle(brush, new RectangleF(rc.Left, (float)(Convert.ToDouble(rc.Bottom) - height),
                        rc.Width, (float)height));
                    rc.Offset(stepWidth, 0);
                }
            }
        }


        public void OnDrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();

            e.Graphics.TextRenderingHint |= System.Drawing.Text.TextRenderingHint.AntiAlias;
            using (StringFormat sf = new StringFormat(StringFormat.GenericTypographic))
            {
                using (Font bold = new Font(Font, FontStyle.Bold))
                {
                    sf.Trimming = StringTrimming.EllipsisCharacter;
                    sf.LineAlignment = StringAlignment.Center;
                    sf.Alignment = StringAlignment.Near;
                    Suggestion s = theList.Items[e.Index] as Suggestion;
                    
                    if (s.dbColumn != null && S.Get("AutoCompleteDataGraph", false) && S.Get("EnableExtendedDataProfiling", false))
                        DrawContext(e.Graphics, e.Bounds, s.dbColumn);
                    
                    string beforeMatch = "";
                    string match = "";
                    string afterMatch = "";
                    string expr = s.expr;
                    if (expr.ToLower().StartsWith(suggestionList.textEntered.ToLower()))
                    {
                        beforeMatch = "";
                        match = expr.Substring(0, suggestionList.textEntered.Length);
                        afterMatch = expr.Substring(match.Length);
                    }
                    else if (expr.ToLower().Contains(suggestionList.textEntered.ToLower()))
                    {
                        int at = expr.ToLower().IndexOf(suggestionList.textEntered.ToLower());
                        if (at > 0)
                        {
                            beforeMatch = expr.Substring(0, at);
                            match = expr.Substring(at, suggestionList.textEntered.Length);
                            afterMatch = expr.Substring(beforeMatch.Length + match.Length);
                        }
                    }
                    else
                    {
                        beforeMatch = expr;
                    }

                    float left = e.Bounds.Left;
                    int len;
                    float width;

                    using (Brush br = new SolidBrush(UI.activeForeColor))
                    {
                        float cy = (e.Bounds.Height - 16) / 2;
                        RectangleF rc = new RectangleF(e.Bounds.Left, e.Bounds.Top + cy, 16, 16);
                        int image = s.tokenType == TokenType.Column ? UI.GetTreeImageIndexForObject(s.dbColumn) : UI.GetTreeImageIndexForTokenType(s.tokenType);
                        e.Graphics.DrawImage(imageList.Images[image], rc);
                        left += 20;
                        if (beforeMatch.Length > 0)
                        {
                            width = e.Graphics.MeasureString(beforeMatch, Font, e.Bounds.Width, sf).Width;
                            len = beforeMatch.Length;
                            e.Graphics.DrawString(beforeMatch, Font, br,
                                                new RectangleF(left, e.Bounds.Top, width, e.Bounds.Height), sf);
                            left += width;
                        }
                        if (match.Length > 0)
                        {
                            width = e.Graphics.MeasureString(match, bold, e.Bounds.Width, sf).Width;
                            len = match.Length;
                            e.Graphics.DrawString(match, bold, br,
                                                new RectangleF(left, e.Bounds.Top, width, e.Bounds.Height), sf);
                            left += width;
                        }
                        if (afterMatch.Length > 0)
                        {
                            width = e.Bounds.Width - left;
                            len = afterMatch.Length;
                            e.Graphics.DrawString(afterMatch, Font, br,
                                                new RectangleF(left, e.Bounds.Top, width, e.Bounds.Height), sf);
                        }
                        if (s.middleStuff != null)
                        {
                            sf.Alignment = StringAlignment.Near;
                            using (Brush gray = new SolidBrush(UI.passiveForecolor1))
                            {
                                e.Graphics.DrawString(s.middleStuff,
                                SystemFonts.MessageBoxFont, br,
                                new RectangleF(e.Bounds.Left + leftWidth, e.Bounds.Top, middleWidth, e.Bounds.Height), sf);
                            }
                        }
                        if (s.rightStuff != null)
                        {
                            sf.Alignment = StringAlignment.Near;
                            using (Brush gray = new SolidBrush(UI.passiveForecolor1))
                            {
                                e.Graphics.DrawString(s.rightStuff,
                                SystemFonts.MessageBoxFont, br,
                                new RectangleF(e.Bounds.Right - rightWidth, e.Bounds.Top, rightWidth, e.Bounds.Height), sf);
                            }
                        }
                    }
                }
            }
        }

    }
}
