using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using fp.lib.forms;
using System.IO;
using fp.lib;

namespace fp.sqratch
{
    public partial class DlgSaveAs : Form
    {
        public RenderResults renderer;
        public bool tableMode;
        bool loading = true;
        public Query query;
        public List<int> selectedIndices = null;
        string fileType { get { return fileTypeCombo.GetValue();  } }
        bool expanded = false;

        public DlgSaveAs(List<int> selected, Query q = null)
        {
            query = q;
            selectedIndices = selected;
            tableMode = selectedIndices == null;
            InitializeComponent();
            Font = SystemFonts.MessageBoxFont;
            directoryTextbox.Text = S.Get("saveAsPath", Directory.GetCurrentDirectory());
           
            tableLabel.Visible = tableCombo.Visible = tableMode;
            saveSelected.Visible = !tableMode;
            if (tableMode)
            {
                tableCombo.Items.Clear();
                tableCombo.Items.AddRange(A.db.tables.Keys.ToArray());
                tableCombo.SelectedIndex = 0;
                if (S.Get("DlgSaveTable") != "")
                    tableCombo.SelectString(S.Get("DlgSaveTable"));
                UpdateTableColumns();
            }
            else
            {
                foreach (QueryColumnInfo info in query.columns)
                    columnsList.Items.Add(info.label);
                columnsList.SelectAll();
                saveSelected.Checked = selectedIndices.Count > 1;
            }
            formatCombo.Initialize("SQL Insert", "C# Object", "C# Array", "PHP Array", "JSON Array", "JSON Object", "Python List");
            fileTypeCombo.Initialize("Comma Separated", "Tab Separated", "Plain Text", "Code", "Custom");
            UpdateOptions();
            rowTemplate.Height = columnTemplate.Height = bOK.Bottom - columnTemplate.Top;
            browseButton.AlignMiddleVerticallyWith(fileNameTextbox);
            moreOptionsButton.Top = browseButton.Top;
            saveSelected.AlignMiddleVerticallyWith(fileTypeCombo);

            padStrings.Enable();
            singleQuoteStrings.Enable();
            printNulls.Enable();
            doubleQuoteStrings.Enable();
            removeNewlines.Enable();
            stripNonAscii.Enable();
            removeTime.Enable();
         }

        private void DlgSaveAs_Load(object sender, EventArgs e)
        {
            fileNameTextbox.SelectText(0, 10);
            loading = false;
        }

        void UpdateOptions()
        {
            SuspendLayout();

            if (fileType == "Comma Separated")
            {
                rowSeparator.Text = "CRLF";
                colSeparator.Text = ",";
                columnTitles.Enable();
                escapeWhenNecessary.Enable();
            }
            else if (fileType == "Tab Separated")
            {
                rowSeparator.Text = "CRLF";
                colSeparator.Text = "TAB";
                columnTitles.Enable();
                escapeWhenNecessary.Enable();
            }
            else if (fileType == "Plain Text")
            {
                rowSeparator.Text = "CRLF";
                colSeparator.Text = " ";
                columnTitles.Enable();
                escapeWhenNecessary.Hide();
            }
            else if (fileType == "Code")
            {
                rowSeparator.Text = "CRLF";
                colSeparator.Text = ",";
                columnTitles.Hide();
                escapeWhenNecessary.Hide();
               
            }
            else if (fileType == "Custom")
            {
                rowSeparator.Text = S.Get("rowSeparator" + fileType, "\n").Replace("\\", "\\\\");
                colSeparator.Text = S.Get("colSeparator" + fileType, ", ").Replace("\\", "\\\\");
                columnTitles.Enable();
                escapeWhenNecessary.Enable();
            }

            singleQuoteStrings.Visible = doubleQuoteStrings.Visible = padStrings.Visible = fileType != "Code";

            moreOptionsButton.Visible = fileType.IsOneOf("Code", "Custom");
            if (!moreOptionsButton.Visible)
                expanded = false;
            UpdateLayout();

            formatLabel.Visible = formatCombo.Visible = fileType == "Code";
            if (fileNameTextbox.Text == "")
                fileNameTextbox.Text = DateTime.Now.ToString("yyMMddHHmm") + ext;
            if (!fileNameTextbox.Text.EndsWith(ext))
                fileNameTextbox.Text = T.GetFileNameFromFilePath(fileNameTextbox.Text, true) + ext;

            ResumeLayout();
            ValidateInputs();
        }

        string ext
        {
            get
            {
                if (fileType == "Comma Separated")
                    return ".csv";
                if (fileType == "Tab Separated")
                    return ".tsv";
                 if (fileType == "Plain Text")
                    return ".txt";
                if (fileType == "Custom")
                    return ".dat";
                if (fileType == "Code")
                {
                    string format = formatCombo.GetValue();
                    if (format == "SQL Insert")
                        return ".sql";
                    if (format == "C# Object")
                        return ".cs";
                    if (format == "C# Array")
                        return ".cs";
                    if (format == "PHP Array")
                        return ".php";
                    if (format == "JSON Array")
                        return ".json";
                    if (format == "JSON Object")
                        return ".json";
                    if (format == "Python List")
                        return ".py";
                }
                return "";
            }
        }
        
        void UpdateTableColumns()
        {
            columnsList.Items.Clear();
            columnsList.Items.AddRange(A.db.tables[tableCombo.GetValue()].columns.Keys.ToArray());
            columnsList.SelectAll();            
        }
        
        void UpdateLayout()
        {
            int right = expanded ? rowTemplate.Right : directoryTextbox.Right;
            Width = right + directoryTextbox.Left + this.NcWidth();
            moreOptionsButton.Text = expanded ? "<<Less" : "More>>";
        }

        private void moreOptionsButton_Click(object sender, EventArgs e)
        {
            expanded = !expanded;
            UpdateLayout();
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            saveFileDialog.SetExtensions("Plain Text|*.txt|Comma Separated|*.csv|Tab Separated|*.tsv", ext);
            saveFileDialog.SupportMultiDottedExtensions = true;
            saveFileDialog.InitialDirectory = directoryTextbox.Text;
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                fileNameTextbox.Text = T.GetFileNameFromFilePath(saveFileDialog.FileName, false);
                directoryTextbox.Text = T.GetPathFromFilePath(saveFileDialog.FileName);
                S.Set("saveAsPath", directoryTextbox.Text);
                ValidateInputs();
            }
        }

        private void fileTypeCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (loading)
                return;
            UpdateOptions();
        }

        private void bOK_Click(object sender, EventArgs e)
        {
            SaveData();
        }

        private void tableCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (loading)
                return;
            UpdateTableColumns();
        }

        public void SaveData()
        {
            if (!Directory.Exists(directoryTextbox.Text))
                return;
            
            using (renderer = new RenderResults(T.AddFileToPath(directoryTextbox.Text, fileNameTextbox.Text)))
            {
                renderer.betweenColumns = colSeparator.Text;
                renderer.betweenRows = rowSeparator.Text;
                renderer.betweenColumns = colSeparator.Text;
                renderer.includeHeaders = columnTitles.GetValue();
                renderer.escapeWhenNecessary = escapeWhenNecessary.GetValue();
                renderer.padStrings = padStrings.GetValue();
                renderer.putStringColumnsInSingleQuotes = singleQuoteStrings.GetValue();
                renderer.putStringColumnsInDoubleQuotes = doubleQuoteStrings.GetValue();
                renderer.removeNewLines = removeNewlines.GetValue();
                renderer.removeNonAscii = stripNonAscii.GetValue();
                renderer.includeNulls = printNulls.GetValue(); 
                renderer.removeTime = removeTime.GetValue();
                renderer.format = fileType == "Code" ? formatCombo.GetValue() : "";
                renderer.rowTemplate = fileType.IsOneOf("Code", "Custom") ? S.Set("rowTemplate", rowTemplate.Text) : "";
                renderer.columnTemplate = fileType.IsOneOf("Code", "Custom") ? S.Set("columnTemplate", columnTemplate.Text) : "";

                string selectedTable = tableMode ? tableCombo.GetValue() : null;
                int rowsExpected = 0;
                string sql = null;
                if (tableMode)
                {
                    sql = "select " + columnsList.GetSelectedStrings(",") + " from " + selectedTable;
                    rowsExpected = A.db.GetExpectedRowsInQuery(sql);
                }
                else
                {
                    rowsExpected = selectedIndices != null && saveSelected.Checked ? selectedIndices.Count : query.rows.Count;
                }

                selectedColumnsIndexes.Clear();
                for (int i = 0; i < columnsList.Items.Count; i++)
                {
                    int pos = 0;
                    if (columnsList.GetSelected(i))
                    {
                        selectedColumnsIndexes.Add(i);
                        if (tableMode)
                            renderer.AddColumn(new QueryColumnInfo(pos++, A.db.tables[selectedTable].columns[columnsList.Items[i].ToString()]));
                        else
                            renderer.AddColumn(query.columns[i]);
                    }
                }

                if (columnTitles.Checked)
                    renderer.WriteHeader();

                using (QSqlBase s = A.db.GetSql())
                {
                    sForWriting = s;
                    if (tableMode)
                        sForWriting.Open(sql);
                    DlgSaveAsSave dlg = new DlgSaveAsSave(this, rowsExpected);
                    DialogResult result = dlg.ShowDialog(this);
                    renderer.Flush();   
                    if (result == DialogResult.Yes)
                        System.Diagnostics.Process.Start(directoryTextbox.Text);
                    else if (result == DialogResult.Cancel)
                        sForWriting.CancelQuery();
                }
            }
            Close();
        }

        int rowIndex = 0;
        int indicesIndex = 0;
        QSqlBase sForWriting;
        HashSet<int> selectedColumnsIndexes = new HashSet<int>();

        public bool WriteNextRow()
        {
            List<string> row = new List<string>();
        
            if (tableMode)
            {
                if (sForWriting.GetRow())
                {
                    for (int i = 0; i < sForWriting.FieldCount; i++)
                        row.Add(sForWriting.GetString(i));
                }
            }
            else
            {
                if (selectedIndices != null && saveSelected.Checked)
                {
                    rowIndex = indicesIndex >= selectedIndices.Count ? -1 : selectedIndices[indicesIndex++];
                }
                else
                {
                    if (rowIndex >= query.rows.Count)
                        rowIndex = -1;
                }
                if(rowIndex >= 0)
                {
                    for (int i = 0; i < query.columns.Count; i++)
                        if (selectedColumnsIndexes.Contains(i))
                            row.Add(query.rows[rowIndex][i]);
                    rowIndex++;

                }
            }
            if (row.Count == 0)
                return false;
            
            renderer.WriteRow(row);
            return true;
        }

        private void ValidateInputs()
        {
            bOK.Enabled = directoryTextbox.Text != "" && Directory.Exists(directoryTextbox.Text) && fileNameTextbox.Text != "" 
                && columnsList.SelectedIndices.Count > 0;
        }

        private void formatCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(!loading)
            UpdateOptions();
        }
    }
}
