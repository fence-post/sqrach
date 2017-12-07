using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ScintillaNET;
using fp.lib;
using System.Configuration;
using fp.lib.mysql;
using fp.lib.sqlparser;

namespace fp.sqratch
{
    partial class main : Form
    {
        void OnEditorMouseDown(object sender, MouseEventArgs e)
        {
            HidePopups();
        }

        void OnEditorLostFocus(object sender, EventArgs args)
        {
            if (autoComplete.Visible && autoComplete.HasFocus() == false)
                autoComplete.Hide();
        }

        void OnEditorKeyDown(object sender, KeyEventArgs args)
        {
            if(autoComplete.Visible)
            {
                if(args.KeyCode == Keys.Escape)
                {
                    autoComplete.Hide();
                }
                else if (args.KeyCode == Keys.Down || args.KeyCode == Keys.Up)
                {
                    autoComplete.TakeFocus(args.KeyCode);
                }
                else if (args.KeyCode == Keys.Tab)
                {
                    args.SuppressKeyPress = true;
                    OnTextChanged(autoComplete.TakeSelection());
                }
            }           
        }

        void sqlEditor_TextChanged(object sender, EventArgs e)
        {
            if (selectedQuery != null)
            {
                if (editor.Text != "" && selectedQuery.query != editor.Text)
                {
                    lastEditorChange = Environment.TickCount;
                    selectedQuery.query = editor.Text;
                    tabsDirty = true;
                }
            }
        }

        bool OnTextChanged(bool showAutoComplete)
        {
            selectedQuery.query = editor.Text;
            Parser.TextChanged(editor.Text, editor.CurrentPosition, false);
            if(showAutoComplete)
            {
                SuggestionList suggestions = Parser.GetSuggestions(editor.Text, editor.CurrentPosition);
                if (suggestions != null)
                {
                    if (!autoComplete.UpdateSuggestions(suggestions,
                        editor.PointToScreen(new Point(editor.PointXFromPosition(suggestions.wordStartPosition),
                        editor.PointYFromPosition(suggestions.wordStartPosition)))))
                        showAutoComplete = false;
                }
            }

            return showAutoComplete;
        }

        private void sqlEditor_CharAdded(object sender, CharAddedEventArgs e)
        {
            if (!S.Get("Autocomplete", true))
                return;

            bool hide = true;

            if (e.Char == '(' && S.Get("AutocompleteParenthesis", false))
            {
                editor.InsertText(editor.CurrentPosition, ")");
            }
            else if (e.Char == '\'' && S.Get("AutocompleteQuotes", true))
            {
                editor.InsertText(editor.CurrentPosition, "\'");
            }
            else if(e.Char == ',')
            {
                int pos = editor.CurrentPosition;
                if(pos > 2)
                {
                    if (editor.Text[pos - 2] == ' ' && editor.Text[pos - 3] != ' ')
                    {
                        editor.DeleteRange(pos - 2, 1);
                       // while (editor.Text.Length > pos + 2 && editor.Text[pos + 1] == ' ' && editor.Text[pos + 2] == ' ')
                        //    editor.DeleteRange(pos + 1, 1);
                        if (editor.Text.Length > pos + 1 && editor.Text[pos + 1] != ' ')
                            editor.InsertText(pos - 1, " ");
                    }
                }              
            }
            else 
            {
                if (OnTextChanged(true))
                    hide = false;
                    
            }
            if(hide)
                autoComplete.Hide();
            
        }

        private static bool IsBrace(int c)
        {
            switch (c)
            {
                case '(':
                case ')':

                    return true;
            }

            return false;
        }

        private int lastCaretPos = -1;

        private void sqlEditor_UpdateUI(object sender, UpdateUIEventArgs e)
        {
            // Has the caret changed position?
            var caretPos = editor.CurrentPosition;
            if (lastCaretPos != caretPos)
            {
                lastCaretPos = caretPos;
                var bracePos1 = -1;
                var bracePos2 = -1;

                // Is there a brace to the left or right?
                if (caretPos > 0 && IsBrace(editor.GetCharAt(caretPos - 1)))
                    bracePos1 = (caretPos - 1);
                else if (IsBrace(editor.GetCharAt(caretPos)))
                    bracePos1 = caretPos;

                if (bracePos1 >= 0)
                {
                    // Find the matching brace
                    bracePos2 = editor.BraceMatch(bracePos1);
                    if (bracePos2 == Scintilla.InvalidPosition)
                        editor.BraceBadLight(bracePos1);
                    else
                        editor.BraceHighlight(bracePos1, bracePos2);
                }
                else
                {
                    // Turn off brace matching
                    editor.BraceHighlight(Scintilla.InvalidPosition, Scintilla.InvalidPosition);
                }
            }
        }
    }
}

