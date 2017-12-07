using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace fp.sqratch
{
    public partial class DlgTabName : Form
    {
        public string tabName;

        public DlgTabName(string n)
        {
            tabName = n;
            InitializeComponent();
            nameOfTab.Text = tabName;
            
        }

        private void bOk_Click(object sender, EventArgs e)
        {
            string s = nameOfTab.Text.Trim();
            if(s.Length > 0)
            {
                tabName = s;
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void bCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
