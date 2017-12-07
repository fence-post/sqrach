using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using fp.lib;

namespace fp.lib.forms
{
    public class ToolCheckBox : CheckBox
    {
        public static ToolTip toolTip = null;

        public ToolCheckBox(ImageList imageList, int imageIndex, EventHandler clickHandler = null, string toolTipText = null)
        {
            Appearance = System.Windows.Forms.Appearance.Button;
            BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            FlatAppearance.BorderSize = 0;
            FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            ImageIndex = imageIndex;
            ImageList = imageList;
            Location = new System.Drawing.Point(0, 0);
            Size = new System.Drawing.Size(20, 20);
            UseVisualStyleBackColor = false;
            if(toolTip != null && toolTipText != null)
                toolTip.SetToolTip(this, toolTipText);

            if (clickHandler != null)
                Click += clickHandler;
        }

        public void SetText(string txt, int image = -1)
        {
            if (image >= 0)
                ImageIndex = image;
            TextImageRelation = TextImageRelation.ImageBeforeText;
            TextAlign = ContentAlignment.MiddleLeft;
            Text = txt;
        }

        public void SetUIPreferences(Color backColor, Color hoverColor, Color checkedColor, ImageList i)
        {
            FlatAppearance.CheckedBackColor = checkedColor;
            FlatAppearance.MouseOverBackColor = hoverColor;
            BackColor = backColor;
            FlatAppearance.MouseDownBackColor = checkedColor;
            ImageList = i;
        }
    }

    public class ToolButton : Button
    {
        public static ToolTip toolTip = null;

        public ToolButton(ImageList imageList, int imageIndex, EventHandler clickHandler = null, string toolTipText = null)
        {
            BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            FlatAppearance.BorderSize = 0;
            FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            ImageIndex = imageIndex;
            ImageList = imageList;
            Location = new System.Drawing.Point(1042, 846);
            Size = new System.Drawing.Size(70, 70);
            UseVisualStyleBackColor = false;
            if (clickHandler != null)
                Click += clickHandler;
            if (toolTip != null && toolTipText != null)
                toolTip.SetToolTip(this, toolTipText);
        }

        public void SetText(string txt, int image = -1)
        {
            if (image >= 0)
                ImageIndex = image;
            TextImageRelation = TextImageRelation.ImageBeforeText;
            TextAlign = ContentAlignment.MiddleLeft;
            Text = txt;
        }

        public void SetUIPreferences(Color backColor, Color clickColor, ImageList i)
        {
            FlatAppearance.MouseOverBackColor = BackColor = backColor;
            FlatAppearance.MouseDownBackColor = clickColor;
            ImageList = i;
        }
    }

    public class Wait : IDisposable
    {
        public static Form form = null;
        public Form thisForm = null;
        public ToolStripProgressBar progress = null;
        public Wait(Form instanceForm = null, int max = -1, System.Windows.Forms.ToolStripProgressBar p = null)
        {
            thisForm = T.Coalesce(instanceForm, form);
            thisForm.Cursor = Cursors.WaitCursor;
            progress = p;

            if (p != null)
            {
                progress = p;
                p.Maximum = max;
                p.Minimum = 0;
                p.Visible = true;
            }

        }
        
        public void SetProgress(int n)
        {
            progress.Value = n;
            progress.Invalidate();
        }


        public bool ProgressIncrement(ref int n)
        {
            if (n >= (progress.Maximum - 1))
                return false;
            SetProgress(n++);
            return true;
        }

        public void Dispose()
        {
            thisForm.Cursor = Cursors.Default;
            if (progress != null)
                progress.Visible = false;
        }

    }
}
