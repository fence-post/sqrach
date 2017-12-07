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
    public class ClipboardHelper
    {
        Scintilla editor;
        RichTextBox log;
        ResultsListView results;

        public ClipboardHelper(Scintilla e, RichTextBox l, ResultsListView r)
        {
            editor = e;
            log = l;
            results = r;
        }

        public void Paste()
        {
            if (editor.Focused)
                editor.Paste();
            if (log.Focused)
                log.Paste();
        }

        public void Cut()
        {
            if (editor.Focused)
                editor.Cut();
            if (log.Focused)
                log.Cut();
        }

        public void Copy()
        {
            if (editor.Focused)
                editor.Copy();
            if (log.Focused)
                log.Copy();
            if (results.Focused)
                results.CopySelectedRows();
        }

        public void Clear()
        {
            if (editor.Focused)
                editor.Clear();
            if (log.Focused)
                log.Clear();
        }

        public void SelectAll()
        {
            if (editor.Focused)
                editor.SelectAll();
            if (log.Focused)
                log.SelectAll();
            if (results.Focused)
                results.SelectAll();
        }

        public bool Focused()
        {
            return editor.Focused || log.Focused || results.Focused;
        }

        public bool CanPaste()
        {
            return (Clipboard.ContainsText() && editor.Focused);
        }

        public bool CanCopy()
        {
            return CanCopyOrCut(false);
        }

        public bool CanCut()
        {
            return CanCopyOrCut(true);
        }

        bool CanCopyOrCut(bool cut)
        {
            if (editor.Focused)
                return editor.SelectedText != "";
            if (cut == false && log.Focused)
                return log.SelectedText != "";
            if (cut == false && results.Focused)
                return results.SelectedIndices.Count > 0;

            return false;
        }
    }
}
