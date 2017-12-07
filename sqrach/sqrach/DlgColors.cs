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
using fp.lib.forms;

namespace fp.sqratch
{
    public partial class DlgColors : Form
    {
        ImageList whiteImages;
        ImageList darkImages;

        public DlgColors(ImageList w, ImageList d)
        {
            whiteImages = w;
            darkImages = d;
            Font = SystemFonts.MessageBoxFont;
            InitializeComponent();
            images.BorderStyle = backColors.BorderStyle = foreColors.BorderStyle = BorderStyle.None;
            images.DrawMode = backColors.DrawMode = foreColors.DrawMode = DrawMode.OwnerDrawFixed;
            images.ItemHeight = 30;
            backColors.ItemHeight = foreColors.ItemHeight = 8 + FormsToolbox.GetTextHeight("00", Font);
            images.Height = images.ItemHeight * 6;
            foreColors.Height = backColors.Height = backColors.ItemHeight * 6;

            for(int i = 0; i < Math.Min(whiteImages.Images.Count, darkImages.Images.Count); i++)
            {
                images.Items.Add(new ImageDefinition(i.ToString(), i));
            }

            AddForeColor("Unk", 214, 157, 133);
            AddForeColor("Unk", 181, 206, 168);
            AddForeColor("turquoise green", 78, 201, 176);
            AddForeColor("green symbol", 124, 165, 160);
            AddForeColor("blue", 95, 149, 250);
            AddForeColor("enum breen", 184, 215, 163);
            AddForeColor("blue", 86, 156, 214);
            AddForeColor("gray litteral", 218, 218, 218);
            AddForeColor("gray identifier", 220, 220, 220);
            AddForeColor("line number", 42, 145, 175);
            AddForeColor("dark text", 20, 72, 82);

            AddForeColor("unk", 86, 156, 214);
            AddForeColor("standard forecolor", 220, 220, 220);
            AddForeColor("violet", 202, 121, 236);
            AddForeColor("yellow peach", 255, 219, 163);
            AddForeColor("blush red", 216, 80, 80);
            AddForeColor("brown tan", 215, 186, 125);

            AddBackColor("ncBackColor", 45, 45, 48);
            AddBackColor("backColor", 30, 30, 30);
            AddBackColor("backColor2", 62, 62, 66);
            AddBackColor("backColor3", 104, 104, 104);
            AddBackColor("backColor0", 45, 45, 48);
            AddBackColor("backColor4", 51, 51, 51);
            AddBackColor("backColor5", 37, 37, 38);
            AddBackColor("backColor6", 51, 51, 55);
            AddBackColor("headerBackColor", 51, 51, 55);

        }

        void AddForeColor(string nam, int r, int g, int b)
        {
            foreColors.Items.Add(new ColorDefinition(nam, Color.FromArgb(r, g, b)));
        }

        void AddBackColor(string nam, int r, int g, int b)
        {
            backColors.Items.Add(new ColorDefinition(nam, Color.FromArgb(r, g, b)));
        }

        private void colorList_DrawItem(object sender, DrawItemEventArgs e)
        {
            ColorDefinition def = foreColors.Items[e.Index] as ColorDefinition;
            DrawItem(def.color, UI.passiveBackColor, def.name, e);
        }

        private void backColors_DrawItem(object sender, DrawItemEventArgs e)
        {
            ColorDefinition def = backColors.Items[e.Index] as ColorDefinition;
            DrawItem(UI.passiveForeColor, def.color, def.name + " " + ColorToString(def.color), e);

        }

        void DrawItem(Color foreColor, Color backColor, string name, DrawItemEventArgs e)
        {
            e.DrawBackground();
            using (Brush bc = new SolidBrush(backColor))
            {
                using (Brush fc = new SolidBrush(foreColor))
                {
                    using (StringFormat sf = new StringFormat(StringFormat.GenericTypographic))
                    {
                        sf.LineAlignment = StringAlignment.Center;
                        sf.Alignment = StringAlignment.Near;
                        RectangleF rc = e.Bounds;
                        rc.Inflate(-2, -2);
                        e.Graphics.FillRectangle(bc, rc);
                        e.Graphics.DrawString(name, Font, fc, rc, sf);
                    }
                }
            }
        }

        string ColorToString(Color c)
        {
            return c.R + ", " + c.G + ", " + c.B;
        }

        private void colorList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(foreColors.SelectedItem != null)
            {
                ColorDefinition def = foreColors.SelectedItem as ColorDefinition;
                foreColor.Text = ColorToString(def.color);
                test.ForeColor = ForeColor = def.color;
                Invalidate(true);
            }
        }

        private void backColors_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (backColors.SelectedItem != null)
            {
                ColorDefinition def = backColors.SelectedItem as ColorDefinition;
                backColor.Text = ColorToString(def.color);
                test.BackColor = BackColor = def.color;
                Invalidate(true);
            }
        }

        private void images_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            using (Brush bc = new SolidBrush(BackColor))
            {
                using (Brush fc = new SolidBrush(ForeColor))
                {
                    using (StringFormat sf = new StringFormat(StringFormat.GenericTypographic))
                    {
                        ImageDefinition def = images.Items[e.Index] as ImageDefinition;

                        sf.LineAlignment = StringAlignment.Center;
                        sf.Alignment = StringAlignment.Near;
                        RectangleF rc = e.Bounds;
                        rc.Inflate(-2, -2);
                        e.Graphics.FillRectangle(bc, rc);
                        e.Graphics.DrawImageUnscaled(whiteImages.Images[def.index], Convert.ToInt32(rc.Left), Convert.ToInt32(rc.Top));
                        e.Graphics.DrawImageUnscaled(darkImages.Images[def.index], Convert.ToInt32(rc.Left) + 30, Convert.ToInt32(rc.Top));

                        rc.Offset(60, 0);
                        rc.Width -= 60;
                        e.Graphics.DrawString(def.name, Font, fc, rc, sf);
                    }
                }
            }
        }
    }

    public class ImageDefinition
    {
        public string name;
        public int index;

        public ImageDefinition(string nam, int i)
        {
            index = i;
            name = nam;
        }
    }
    

    public class ColorDefinition
    {
        public string name;
        public Color color;
  
        public ColorDefinition(string nam, Color c)
        {
            color = c;
            name = nam;
        }
    }

}
