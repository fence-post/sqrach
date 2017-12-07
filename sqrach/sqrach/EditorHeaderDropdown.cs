using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using fp.lib.forms;

namespace fp.sqratch
{
    public class EditorHeaderDropdown : Panel
    {
        public TextBox input;
        int dropDownimageIndex;

        public EditorHeaderDropdown(string nam)
        {
            Text = nam;
            dropDownimageIndex = 4;
            nam = nam.ToLower();
            Name = nam.ToLower() + "Combo";
            // combo.FormattingEnabled = true;
            Location = new System.Drawing.Point(0,0);
            TabIndex = 0;
            Font = SystemFonts.MessageBoxFont;
            
            Width = FormsToolbox.GetTextWidth(Text + "0000", Font);
            Height = Math.Max(20, FormsToolbox.GetTextHeight(Text + "0000", Font));
            // SetStyle(ControlStyles.UserPaint, true);
        }



        /*
        public void UpdateUIPreferences()
        {
            BackColor = UI.headerBackColor;
            ForeColor = UI.headerForeColor;
            Invalidate(true);
        }
        */

        void Draw(string txt, Color color, Graphics g, RectangleF bounds)
        {
            using (StringFormat sf = new StringFormat())
            {
                using (Brush brush = new SolidBrush(color))
                {
                    sf.Alignment = StringAlignment.Near;
                    sf.LineAlignment = StringAlignment.Center;
                    sf.Trimming = StringTrimming.EllipsisCharacter;
                    g.DrawString(txt, Font, brush, bounds, sf);
                }
            }
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            using (var brush = new SolidBrush(UI.headerBackColor))
            {
                using (var linePen = new Pen(UI.passiveControlNcColor, 1.0f))
                {
                    RectangleF rect = e.Graphics.VisibleClipBounds;
                    rect.Width = ClientSize.Width;
                    rect.Height = ClientSize.Height;
                    e.Graphics.FillRectangle(brush, rect);
                    e.Graphics.DrawLine(linePen, rect.Left, rect.Top, rect.Left, rect.Height);

                    float left = e.Graphics.MeasureString(Text, Font).Width;
                    float cy = (rect.Height - 20) / 2;
                    RectangleF imageRect = new RectangleF(left, rect.Top + cy, 20, 20);
                    e.Graphics.DrawImage(UI.buttonImages.Images[dropDownimageIndex], imageRect);
                    Draw(this.Text, UI.headerForeColor, e.Graphics, rect);
                }
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);

        }

    }
}
