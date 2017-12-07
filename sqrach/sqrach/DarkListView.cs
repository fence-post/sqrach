using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace fp.sqratch
{
    public class DarkListView : ListView
    {
        public Font headerFont = SystemFonts.MessageBoxFont;
        protected int loadingClientWidth = 0;
        protected void ExtendLastColumn()
        {
            if (Columns.Count == 0)
                return;

            int rightmostColumn = Columns.Count - 1;
            int totalSizeUsed = 0;
            int dataSizeUsed = 0;

            foreach (ColumnHeader col in Columns)
            {
                totalSizeUsed += col.Width;
                if (col.DisplayIndex != rightmostColumn)
                    dataSizeUsed += col.Width;
            }
                
            if (totalSizeUsed == ClientRectangle.Width + SystemInformation.VerticalScrollBarWidth)
            {
                Columns[rightmostColumn].Width -= SystemInformation.VerticalScrollBarWidth;
            }
            else
            {
                int width = loadingClientWidth = ClientRectangle.Width;
                Columns[rightmostColumn].Width = Math.Max(0, width - dataSizeUsed);
            }
        }

        protected void OnDrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
        }

        protected void OnDrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            using (StringFormat sf = new StringFormat())
            {
                sf.Trimming = StringTrimming.EllipsisCharacter;
                sf.LineAlignment = StringAlignment.Far;
                switch (e.Header.TextAlign)
                {
                    case HorizontalAlignment.Center:
                        sf.Alignment = StringAlignment.Center;
                        break;
                    case HorizontalAlignment.Right:
                        sf.Alignment = StringAlignment.Far;
                        break;
                }

                using (Brush brush = new SolidBrush(UI.headerBackColor))
                {
                    e.Graphics.FillRectangle(brush, e.Bounds);
                }
                Rectangle rect = new Rectangle(e.Bounds.Left, e.Bounds.Top, e.Bounds.Width - 1, e.Bounds.Height - 1);
                using (Pen pen = new Pen(UI.passiveControlNcColor))
                {
                    e.Graphics.DrawLine(pen, rect.Right, rect.Top, rect.Right, rect.Bottom);
                }

                rect.Inflate(-2, 0);
                using (Brush brush = new SolidBrush(UI.passiveForecolor1))
                {
                    e.Graphics.DrawString(e.Header.Text, headerFont, brush, rect, sf);
                }
            }
        }
    }
}
