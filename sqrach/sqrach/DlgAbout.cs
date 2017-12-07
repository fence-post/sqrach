using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using fp.lib.forms;

namespace fp.sqratch
{
    public partial class DlgAbout : Form
    {
        public DlgAbout()
        {         
            InitializeComponent();

            int ncHeight = Height - ClientRectangle.Height;
            int ncWidth = Width - ClientRectangle.Width;
            Height = pictureBox1.Bottom + ncHeight + 10;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            using (WebClient client = new WebClient())
            {
                quote.Hide();
                quote.Text = client.DownloadString("http://fence-post.com/quote/barequote.phtml");
                quote.Show();
            }
        }
    }
}
