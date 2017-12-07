using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScintillaNET;
using System.Drawing;
using System.Configuration;
using System.Windows.Forms;
using fp.lib;
using fp.lib.sqlparser;
using fp.lib.dbInfo;

namespace fp.sqratch
{
    public static class UI
    {
        public static Color GetTokenBackColor(Token t)
        {
            if (t.isPrimaryKeyword || t.tokenType == TokenType.Keyword)
            {
                Keyword k = t as Keyword;
                switch (k.keywordType)
                {
                    case KeywordType.Primary:
                        return Color.FromArgb(181, 206, 168);
                    case KeywordType.Secondary:
                        return Color.FromArgb(126, 101, 81);
                    case KeywordType.DataType:
                        return Color.LightPink;
                }
            }
            else
            {
                switch (t.tokenType)
                {
                    case TokenType.Operator:
                        return Color.FromArgb(144,56,64);
                    case TokenType.Expression:
                        return Color.FromArgb(214, 157, 133);
                    case TokenType.Identifier:
                        return Color.FromArgb(11, 97, 140);
                    case TokenType.Table:
                        return Color.DarkKhaki;
                    case TokenType.Column:
                        return Color.DarkGoldenrod;
                    case TokenType.Literal:
                        return Color.FromArgb(215, 186, 124); //  Color.LightGoldenrodYellow;
                }
            }
            return Color.WhiteSmoke;
        }

        public static Color GetTokenForeColor(Token t)
        {
            if(t.isPrimaryKeyword)
            {
                return UI.primaryKeywordColor;
            }
            else if (t.tokenType == TokenType.Keyword)
            {
                Keyword k = t as Keyword;
                switch (k.keywordType)
                {
                    case KeywordType.Secondary:
                        return UI.secondaryKeywordColor;
                    case KeywordType.Logic:
                        return UI.logicKeywordColor;
                    case KeywordType.DataType:
                        return UI.datatypeKeywordColor;
                }
            }
            else
            {
                switch (t.tokenType)
                {
                    case TokenType.Table:
                        return UI.tableColor;
                    case TokenType.Column:
                        return UI.columnColor;
                    case TokenType.Operator:
                        return UI.operatorColor;
                    case TokenType.Literal:
                        return UI.stringColor;
                }
            }
            return UI.passiveForecolor1;
        }

        #region sql editor initialize

        public static void InitializeEditor(ScintillaNET.Scintilla sqlEditor)
        {
            sqlEditor.StyleResetDefault();
            sqlEditor.Styles[Style.Default].Font = UI.editorFontName;
            sqlEditor.Styles[Style.Default].Size = Convert.ToInt32(UI.editorFontSize); ;
            sqlEditor.Styles[Style.Default].ForeColor = UI.activeForeColor;
            sqlEditor.Styles[Style.Default].BackColor = UI.passiveBackColor;
            
            sqlEditor.CaretForeColor = UI.dark ? Color.White : Color.Blue;
            sqlEditor.SetSelectionBackColor(true, UI.selectionBackColor);

            sqlEditor.Margins[0].BackColor = UI.passiveBackColor;
            sqlEditor.Margins[0].Width = UI.dark ? 30 : 30;
            sqlEditor.Margins[0].Type = MarginType.Number;
            sqlEditor.Margins[1].BackColor = UI.passiveBackColor;
            sqlEditor.Margins[1].Width = 15;
            sqlEditor.Margins[1].Type = MarginType.Color;
            sqlEditor.Margins[2].BackColor = UI.passiveLineColor;
            sqlEditor.Margins[2].Width = 2;
            sqlEditor.Margins[2].Type = MarginType.Color;
            sqlEditor.Margins[4].BackColor = UI.passiveBackColor;
            sqlEditor.Margins[4].Width = 3;
            sqlEditor.Margins[4].Type = MarginType.Color;

            sqlEditor.StyleClearAll();

            sqlEditor.Lexer = Lexer.Sql;

            // Set the Styles
            sqlEditor.Styles[Style.LineNumber].ForeColor = UI.lineNumberColor;
            sqlEditor.Styles[Style.LineNumber].BackColor = UI.passiveBackColor;
            
            sqlEditor.Styles[Style.Sql.Comment].ForeColor = UI.commentColor;
            sqlEditor.Styles[Style.Sql.CommentLine].ForeColor = UI.commentColor;
            sqlEditor.Styles[Style.Sql.CommentLineDoc].ForeColor = UI.commentColor;
            sqlEditor.Styles[Style.Sql.Number].ForeColor = UI.numberColor;
            sqlEditor.Styles[Style.Sql.Word].ForeColor = UI.primaryKeywordColor;
            sqlEditor.Styles[Style.Sql.Word2].ForeColor = UI.secondaryKeywordColor;
            sqlEditor.Styles[Style.Sql.User1].ForeColor = UI.logicKeywordColor;
            sqlEditor.Styles[Style.Sql.User2].ForeColor = UI.datatypeKeywordColor;
            sqlEditor.Styles[Style.Sql.User3].ForeColor = UI.tableColor;
            sqlEditor.Styles[Style.Sql.User4].ForeColor = UI.columnColor;

            sqlEditor.Styles[Style.Sql.String].ForeColor = UI.stringColor;
            sqlEditor.Styles[Style.Sql.Character].ForeColor = UI.charColor;
            sqlEditor.Styles[Style.Sql.Operator].ForeColor = UI.operatorColor;

            sqlEditor.IndentationGuides = IndentView.LookBoth;
            sqlEditor.Styles[Style.BraceLight].BackColor = UI.braceLightBackColor;
            sqlEditor.Styles[Style.BraceLight].ForeColor = UI.braceLightColor;
            sqlEditor.Styles[Style.BraceBad].ForeColor = UI.braceBadColor;


            // sqlEditor.SetKeywords(0, @"add alter as authorization backup begin bigint binary bit break browse bulk by cascade case catch check checkpoint close clustered column commit compute constraint containstable continue create current cursor cursor database date datetime datetime2 datetimeoffset dbcc deallocate decimal declare default delete deny desc disk distinct distributed double drop dump else end errlvl escape except exec execute exit external fetch file fillfactor float for foreign freetext freetexttable from full function goto grant group having hierarchyid holdlock identity identity_insert identitycol if image index insert int intersect into key kill lineno load merge money national nchar nocheck nocount nolock nonclustered ntext numeric nvarchar of off offsets on open opendatasource openquery openrowset openxml option order over percent plan precision primary print proc procedure public raiserror read readtext real reconfigure references replication restore restrict return revert revoke rollback rowcount rowguidcol rule save schema securityaudit select set setuser shutdown smalldatetime smallint smallmoney sql_variant statistics table table tablesample text textsize then time timestamp tinyint to top tran transaction trigger truncate try union unique uniqueidentifier update updatetext use user values varbinary varchar varying view waitfor when where while with writetext xml go ");
            // sqlEditor.SetKeywords(1, @"ascii cast char charindex ceiling coalesce collate contains convert current_date current_time current_timestamp current_user floor isnull max min nullif object_id session_user substring system_user tsequal ");
            // sqlEditor.SetKeywords(4, @"all and any between cross exists in inner is join left like not null or outer pivot right some unpivot ( ) * ");

            // Set keyword lists
            // Word = 0 - keywords
            sqlEditor.SetKeywords(0, DbInfo.PrimaryKeywords);
            // Word2 = 1
            sqlEditor.SetKeywords(1, DbInfo.SecondaryKeywords);
            // User1 = 4
            sqlEditor.SetKeywords(4, DbInfo.LogicKeywords);
            // User2 = 5
            sqlEditor.SetKeywords(5, DbInfo.DatatypeKeywords);
            // User3 = 6
            sqlEditor.SetKeywords(6, A.db.tables.Keys.Join(" "));
            // User4 = 7
            sqlEditor.SetKeywords(7, A.db.tablesByColumnName.Keys.Join(" "));

            sqlEditor.AutoCSeparator = '|';
            sqlEditor.AutoCIgnoreCase = true;
        }

        #endregion

        public static void LoadPreferences()
        {
            dark = S.initSettings.dark;
            systemFont = SystemFonts.MessageBoxFont;
            string fontName = T.Coalesce(ConfigurationManager.AppSettings["environmentFontFamily"], SystemFonts.MessageBoxFont.FontFamily.Name);
            environmentFont = new Font(fontName, GetFontSize("environment")); ;
            consoleFont = new Font(ConfigurationManager.AppSettings["logFontFamily"], GetFontSize("log"));
            editorFontName = ConfigurationManager.AppSettings["editorFontFamily"];
            editorFontSize = GetFontSize("editor");
        }

        private static float GetFontSize(string type = "")
        {
            string sizeName = Settings.Get("fontSize", "medium");
            sizeName = sizeName.Substring(0, 1).ToUpper() + sizeName.Substring(1);
            return Convert.ToSingle(ConfigurationManager.AppSettings[type + "fontSize" + sizeName]);
        }

        public static int GetTreeImageIndexForTokenType(TokenType t)
        {
            switch(t)
            {
                case TokenType.Table:
                    return 0;
            }

            return 11;
        }

        public static int GetTreeImageIndexForObject(object o)
        {
            int i = 0;
            DbColumn dbColumn = o as DbColumn;
            if(dbColumn != null)
            {
                i = 4;
                if (dbColumn.dataType.IsNumericType())
                    i = 7;
                else if (dbColumn.dataType == typeof(DateTime))
                    i = 1;

                if (dbColumn.primaryKey)
                    i += 2;
                else if (dbColumn.allowNulls)
                    i += 1;

                return i;
            }
            DbTable dbTable = o as DbTable;
            if (dbTable != null)
            {
                if (dbTable.tableType == "BASE TABLE")
                    return 0;
                else if (dbTable.tableType == "VIEW")
                    return 10;
                else
                    throw new ArgumentException();
            }

            return 0;
        }

        public static bool dark;
        public static float fontSize;
        public static string consoleFontName;
        public static Font systemFont;
        public static Font consoleFont;
        public static Font environmentFont;
        public static string editorFontName;
        public static float editorFontSize;
        public static ImageList buttonImages;
        public static ImageList smallButtonImages;
        public static ImageList treeImages;

        public static Color selectionBackColor { get { return dark ? Color.FromArgb(51, 153, 255) : Color.LightGray;  } }

        public static Color braceLightColor { get { return dark ? Color.FromArgb(124, 165, 160) : Color.LightGray; } }
        public static Color braceLightBackColor { get { return dark ? UI.activeBackColor : Color.BlueViolet; } }
        public static Color braceBadColor { get { return dark ? Color.FromArgb(216, 80, 80) : Color.Red; } }
        
        public static Color operatorColor { get { return dark ? Color.FromArgb(184, 215, 163) : Color.Black; } }
        public static Color commentColor { get { return dark ? Color.FromArgb(87, 166, 74) : Color.Green; } }
        public static Color numberColor { get { return dark ? Color.FromArgb(181, 206, 168) : Color.Maroon; } }
        public static Color charColor { get { return dark ? Color.FromArgb(215, 186, 125) : Color.Red; } }
        public static Color stringColor { get { return dark ? Color.FromArgb(214, 157, 133) : Color.Red; } }
        public static Color tableColor { get { return dark ? Color.FromArgb(218, 218, 218) : Color.FromArgb(255, 00, 128, 192); } }
        public static Color columnColor { get { return dark ? Color.FromArgb(220, 220, 220) : Color.Purple; } }
        public static Color primaryKeywordColor { get { return dark ? Color.FromArgb(78, 201, 176) : Color.Blue; } }
        public static Color secondaryKeywordColor { get { return dark ? Color.FromArgb(184, 215, 163) : Color.Fuchsia; } }
        public static Color logicKeywordColor { get { return dark ? Color.FromArgb(184, 215, 163) : Color.Fuchsia; } }
        public static Color datatypeKeywordColor { get { return dark ? Color.Purple : Color.Purple; } }
        public static Color lineNumberColor { get { return dark ? Color.FromArgb(43, 145, 175) : Color.FromArgb(255, 128, 128, 128); } }
        public static Color lineNumberBackColor { get { return dark ? backColor2 : Color.FromArgb(255, 228, 228, 228); } }
        public static Color marginColor0 { get { return dark ? backColor2 : Color.Black; } }
        public static Color marginColor1 { get { return dark ? backColor3 : Color.DarkGray; } }
        public static Color marginColor2 { get { return dark ? passiveBackColor : SystemColors.Window; } }

        public static Color activeForeColor { get { return dark ? Color.White : Color.Black; } }
        public static Color activeBackColor { get { return dark ? backColor : SystemColors.Window; } }

        public static Color passiveForeColor { get { return dark ? Color.FromArgb(220, 220, 220) : Color.FromArgb(32, 32, 32); } }
        public static Color passiveBackColor { get { return dark ? backColor : SystemColors.Window; } }
        public static Color passiveLineColor { get { return dark ? backColor3 : Color.DarkGray; } }

        public static Color activeRed { get { return Color.FromArgb(140, 47, 47); } }
        public static Color passiveRed { get { return Color.FromArgb(119, 56, 0); } }

        public static Color activeYellow { get { return Color.FromArgb(233, 213, 133); } }
        public static Color passiveYellow { get { return Color.FromArgb(215, 186, 125); } }
        public static Color passivePassiveForeColor { get { return Color.DarkGray; } }
        public static Color passiveForecolor1 = Color.FromArgb(133,133,133);
        public static Color passiveControlNcColor { get { return dark ? Color.FromArgb(59, 59, 62) : Color.LightGray; } }

        public static Color tabForeColor { get { return activeForeColor; } }
        public static Color selectedTabColor { get { return Color.FromArgb(120, 191, 64); } }
        public static Color hoverTabColor { get { return Color.FromArgb(128, 120, 191, 64); } }

        public static Color frameBackground { get { return dark ? backColor0 : SystemColors.Control; } }
        public static Color headerBackColor { get { return dark ? UI.backColor4 : SystemColors.Control; } }
        public static Color headerForeColor { get { return dark ? UI.passiveForecolor1 : UI.passiveForecolor1; } }
        public static Color toolCheckboxCheckedColor { get { return dark ? UI.backColor2 : SystemColors.ControlDark; } }
        public static Color headerSearchBackground { get { return dark ? UI.backColor8 : SystemColors.ControlDark; } }

        public static Color stickyForeColor { get { return dark ? Color.FromArgb(137,187,130) : Color.FromArgb(130, 80, 0); } }
        public static Color autoCompleteBackColor { get { return backColor5; } }

        public static Color buttonCheckedColor { get { return Color.FromArgb(0,0,0); } }
        public static Color buttonHoverColor { get { return backColor; } }


        private static Color ncBackColor = Color.FromArgb(45, 45, 48);
        private static Color ncForeColor = Color.DarkGray;
        private static Color foreColor = Color.LightGray;
        private static Color backColor = Color.FromArgb(30, 30, 30); // Color.FromArgb(0, 122, 204);
        private static Color backColor2 = Color.FromArgb(62, 62, 66);
        public static Color backColor3 = Color.FromArgb(104, 104, 104);
        public static Color backColor0 = Color.FromArgb(45,45,48);
        public static Color backColor4 = Color.FromArgb(51, 51, 51);
        private static Color backColor5 = Color.FromArgb(37, 37, 38);
        private static Color backColor6 = Color.FromArgb(51, 51, 55);
        private static Color backColor7 = Color.FromArgb(51, 51, 51);
        private static Color backColor8 = Color.FromArgb(63, 63, 70);
        
        
    }

    /*
        // Color.FromArgb(51, 153, 255)
        //public static Color stickyForeColor { get { return dark ? Color.FromArgb(118, 146, 60) : Color.FromArgb(130, 80, 0); } }
        //        public static Color selectedTabColor { get { return dark ? Color.FromArgb(120, 191, 64) : Color.FromArgb(0, 122, 204); } }
        //      public static Color hoverTabColor { get { return dark ? Color.FromArgb(128, 120, 191, 64) : Color.FromArgb(28, 151, 234);  } }
    brown - 126,101,81
brown - 130,80,0
brick red - 140,47,47
dull red - 119,56,0
dark brick red - 80,30,0
peach - 255,219,163
blush - 214,157,133
nice yellow - 233,213,133
beige yellow - 215,186,125
beige - 237,224,209
peachy beighe - 214,157,133
green - 118,146,60
green - 80,116,112
green - 96,139,78
green - 137,187,130
green - 181,206,168* 
    * 78, 201, 176
    * 184, 215, 163
    * 124, 165, 160
    * 218, 218, 218
    * 220, 220, 220
brown tan - 215,186,125
blush red 216,80,80
yellow peach - 255,219,163
violet 202,121,236
    * 184, 215, 163
standard forecolor Color.Argb(220, 220, 220);
selected text background: 51,153,255
86, 156, 214
20,72,82 - dark text
42,145,175 - line number
220,220,220 - gray identifier
218,218,218 - gray litteral
86,156,214 - blue
95,149,250 - blue
95, 149, 250
184,215,163 - enum breen
124,165,160 - green symbol
78,201,176 - turquoise green
Color.Argb(87, 166, 74)
selected text background: 51,153,255
181,206,168
number 
214,157,133
20,72,82 - dark text
42,145,175 - line number
220,220,220 - gray identifier
218,218,218 - gray litteral
86,156,214 - blue
95,149,250 - blue
184,215,163 - enum breen
124,165,160 - green symbol
78,201,176 - turquoise green
181,206,168
*/

}
