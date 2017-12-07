using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace fp.lib.forms
{
    public static class FormsToolbox
    {
        public static int GetTextWidth(int chars, Font f)
        {
            return GetTextWidth(new string('x', chars), f);
        }

        public static int GetTextWidth(string txt, Font f)
        {
            return TextRenderer.MeasureText(txt, f).Width;
        }

        public static int GetTextHeight(string txt, Font f)
        {
            return TextRenderer.MeasureText(txt, f).Height;
        }


        public static void OpenUrl(string url)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo(url);
            Process.Start(sInfo);
        }
    }
}
