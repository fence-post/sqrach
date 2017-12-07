using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using fp.lib;
using fp.lib.mysql;
using System.Diagnostics;
using fp.lib.sqlparser;
using System.Text;
using fp.lib.forms;

namespace fp.sqratch
{
    partial class main : Form
    {
        private void resultsMenu_Opening(object sender, CancelEventArgs e)
        {
            resultsList.GetMousePosition();
            bool fit = false;
            bool fitEnabled = false;
            pinToLeftToolStripMenuItem.Checked = false;
            pinToLeftToolStripMenuItem.Enabled = false;
            if (resultsList.colUnderMouse >= 0)
            {
                QueryColumnInfo columnInfo = selectedQuery.columns[resultsList.colUnderMouse];
                fitEnabled = fit = columnInfo.fitData;
                if (columnInfo.pinned)
                    pinToLeftToolStripMenuItem.Checked = true;
                pinToLeftToolStripMenuItem.Enabled = pinToLeftToolStripMenuItem.Checked || resultsList.GetPinnedColumns() > 0 || resultsList.colUnderMouse > 0;

                if (resultsList.ColumnDataTruncated(resultsList.colUnderMouse))
                    fitEnabled = true;
            }
            fitDataInColumnToolStripMenuItem.Checked = fit;
            fitDataInColumnToolStripMenuItem.Enabled = fitEnabled;
            copyRowsToolStripMenuItem.Enabled = resultsList.SelectedIndices.Count > 0;
        }
       
        private void pinToLeftToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (resultsList.colUnderMouse >= 0 && resultsList.colUnderMouse < selectedQuery.columns.Count)
                resultsList.TogglePinColumnToLeft(resultsList.colUnderMouse);
        }

        private void saveAsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            List<int> selectedIndices = new List<int>();
            resultsList.GetSelectedIndices(selectedIndices);
            DlgSaveAs dlg = new DlgSaveAs(selectedIndices, selectedQuery);
            dlg.ShowDialog(this);
        }

        private void selectAllToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            using (new Wait())
            {
                resultsList.SelectAll();
            }
        }

        private void fitDataInColumnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            resultsList.FitDataInColumn(resultsList.colUnderMouse, true);
        }
        
        private void copyRowsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*
            char colSeparator = true ? ',' : '\t';
            char lineSeparator = '\n';
            */
            using (new Wait())
            {
                resultsList.CopySelectedRows();
                // Clipboard.SetText(resultsList.GetSelectedRowData(colSeparator.ToString(), lineSeparator.ToString()));
            }
        }

        private void copyCellToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int col = resultsList.colUnderMouse;
            int row = resultsList.rowUnderMouse;
            if(col >= 0 && col < selectedQuery.columns.Count && row >= 0 && row < selectedQuery.rows.Count)
            {
                string text = selectedQuery.rows[row][col];
                Clipboard.SetText(text);
            }
        }

        private void copyValuesInColumnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            char lineSeparator = '\n';
            int iCol = resultsList.colUnderMouse;
            StringBuilder sb = new StringBuilder();

            using (new Wait())
            {
                Clipboard.SetText(resultsList.GetColData(iCol, lineSeparator.ToString(), resultsList.SelectedIndices.Count > 0));
            }
        }

        private void invertSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (new Wait())
            {
                resultsList.InvertSelection();
            }
        }

        private void selectNoneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (new Wait())
            {
                resultsList.SelectNone();
            }    
        }

        void sQLINClauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            resultsList.CopyColumnAs("SQL Insert");
        }

        private void jsonInitializerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            resultsList.CopyColumnAs("JSON Array");
        }

        private void cInitializerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            resultsList.CopyColumnAs("C# Array");
        }

    }
}
