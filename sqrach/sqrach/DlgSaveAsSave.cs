using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using fp.lib;

namespace fp.sqratch
{
    public partial class DlgSaveAsSave : Form
    {
        DlgSaveAs dlg;
        int expected;

        public DlgSaveAsSave(DlgSaveAs d, int e)
        {
            expected = e;
            dlg = d;
            Font = SystemFonts.MessageBoxFont;

            InitializeComponent();
        }

        private void DlgSaveAsSave_Shown(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            bCancel.Cursor = Cursors.Default;
            
            line1.Text = "Writing " + expected + " rows...";
            progressBar1.Minimum = 0;
            progressBar1.Maximum = expected;
            Application.DoEvents();
            int updateAt = T.MinMax(10,100,expected / 500);
            while(dlg.WriteNextRow())
            {
                if (cancelPressed)
                    break;
                if (dlg.renderer.rowsWritten >= expected)
                {
                    int n = 1;
                }
                else if (dlg.renderer.rowsWritten % updateAt == 0)
                {
                    progressBar1.Value = dlg.renderer.rowsWritten;
                    Application.DoEvents();
                }
            }
        
            if (cancelPressed)
            {
                DialogResult = DialogResult.Cancel;
                Close();
            }
                

            Cursor = Cursors.Default;
            Text = "Saved";
            line1.Text = dlg.renderer.rowsWritten + " rows written to";
            line2.Text = dlg.renderer.filePath.FitText(progressBar1.Width, Font);
            line2.Visible = true;
            progressBar1.Visible = false;
            fileLocation.Visible = true;
            bCancel.Text = "Close";
        }

        bool cancelPressed = false;

        private void bCancel_Click(object sender, EventArgs e)
        {
            if (bCancel.Text == "Cancel")
                cancelPressed = true;
            else
            {
                DialogResult = DialogResult.No;
                Close();
            }
        }

        private void fileLocation_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
            Close();
        }
    }
}
