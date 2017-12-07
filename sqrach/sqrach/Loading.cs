using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fp.lib;
using fp.lib.forms;

namespace fp.sqratch
{
    public partial class Loading : UserControl
    {
        HashSet<Control> hiddenControls = new HashSet<Control>();
        Dictionary<Control, Color> darkenedControls = new Dictionary<Control, Color>();
        double startTime = 0;
        double maxPanelWidth;
        double duration = 0;
        bool needShow = false;
        public bool done { get { return Environment.TickCount - startTime > duration; } }
        public bool started { get { return startTime > 0; } }

        public Loading(Form parent)
        {
            Parent = parent;
            int defaultLoadTime = A.dbId > 0 ? 5000 : 3000;
            duration = S.initSettings.lastLoadTime == 0 ? defaultLoadTime : T.MinMax(1000, 20000, S.initSettings.lastLoadTime);
            Font = SystemFonts.MessageBoxFont;
            InitializeComponent();
            BackColor = Color.FromArgb(122, 193, 66);
            panel1.Width = 0;
            panel1.BackColor = Color.Black;
            pictureBox1.Left = (ClientRectangle.Width - (panel1.Left + panel1.Left) - pictureBox1.Width) / 2;
            maxPanelWidth = Width - (panel1.Left + panel1.Left);
            Visible = false;
        }


        public void Hide(Control c)
        {
            c.Visible = false;
            hiddenControls.AddIfNotExists(c);
        }

        public void Darken(Control c)
        {
            darkenedControls.AddIfNotExists(c, c.BackColor);
            c.BackColor = UI.passiveBackColor;           
        }

        public void RestoreHiddenControls()
        {
            foreach (Control c in hiddenControls)
                c.Visible = true;
            foreach (Control c in darkenedControls.Keys)
                c.BackColor = darkenedControls[c];
        }

        public void UpdateProgress()
        {
            if(needShow)
            {
                needShow = false;
                Show();
                BringToFront();
            }
            if (Visible)
            {
                double timeSoFar = Environment.TickCount - startTime;
                Left = (Parent.ClientRectangle.Width - Width) / 2;
                Top = (Parent.ClientRectangle.Height - Height) / 2;
                panel1.Width = Convert.ToInt32(Math.Min(maxPanelWidth, timeSoFar / duration * maxPanelWidth));
            }
        }

        public void Start()
        {
            SuspendLayout();
            startTime = Environment.TickCount;
            if (!Parent.Controls.Contains(this))
                Parent.Controls.Add(this);
            Left = Parent.ClientRectangle.Width - Width / 2;
            Top = Parent.ClientRectangle.Height - Height / 2;
            ResumeLayout();
            needShow = true;
        }

        public void Stop()
        {
            S.initSettings.lastLoadTime = Environment.TickCount - Convert.ToInt32(startTime);
            startTime = 0;
            RestoreHiddenControls();
            hiddenControls.Clear();
            darkenedControls.Clear();
            Hide();
        }
    }
}
