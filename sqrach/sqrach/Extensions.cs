using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using fp.lib;
using fp.lib.forms;
using System.Drawing;

namespace fp.sqratch
{
    public static class Extensions
    {
        public static string FitText(this string txt, int width, Font font)
        {
            if (FormsToolbox.GetTextWidth(txt, font) > width)
            { 
                for (int i = (txt.Length - 3) / 2; i > 5; i -= 1)
                {
                    string test = txt.Left(i) + "..." + txt.Right(i);
                    if (FormsToolbox.GetTextWidth(test, font) < width)
                        return test;
                }
            }
            return txt;
        }

        public static void Initialize(this ComboBox cb, params string[] items)
        {
            cb.Items.Clear();
            cb.Items.AddRange(items);
            cb.SelectString(S.Get(cb.Parent.Name + cb.Name, items.First()));
        }

        public static string GetValue(this ComboBox cb)
        {
            return S.Set(cb.Parent.Name + cb.Name, cb.SelectedItem.ToString());
        }

        public static void Enable(this CheckBox cb)
        {
            cb.Visible = true;
            cb.Checked = S.Get(cb.Parent.Name + cb.Name, false);
        }

        public static bool GetValue(this CheckBox cb)
        {
            return S.Set(cb.Parent.Name + cb.Name, cb.Checked);
        }
    }
}
