using System;
using System.Drawing;
using System.Windows.Forms;

namespace fp.sqratch
{
    public partial class DlgOptions : Form
    {
     
        private Options options = new Options();
        public DlgOptions()
        {
            InitializeComponent();
            Font = SystemFonts.MessageBoxFont;
            optionsPropertyGrid.SelectedObject = options;
        }

        private void bOk_Click(object sender, EventArgs e)
        {
            Close();
            Dispose();
        }

        private void bCancel_Click(object sender, EventArgs e)
        {
            Close();
            Dispose();
        }
    }
}
