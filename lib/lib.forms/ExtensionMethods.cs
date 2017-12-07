using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace fp.lib.forms
{
    public static class ExtensionMethods
    {
        public static int NcWidth(this Control f)
        {
            return f.Width - f.ClientRectangle.Width;
        }

        public static int NcHeight(this Control f)
        {
            return f.Height - f.ClientRectangle.Height;
        }

        public static void SetExtensions(this FileDialog dialog, string filterDefinition, string defaultExtension = null)
        {
            var filters = filterDefinition.Split('|');
            if (defaultExtension == null)
                defaultExtension = filters[1];
            dialog.DefaultExt = defaultExtension;
            dialog.Filter = filterDefinition;
            dialog.UseDefaultExtAsFilterIndex();
           
        }


        public static void UseDefaultExtAsFilterIndex(this FileDialog dialog)
        {
            var ext = "*." + dialog.DefaultExt;
            var filter = dialog.Filter;
            var filters = filter.Split('|');
            for (int i = 1; i < filters.Length; i += 2)
            {
                if (filters[i] == ext)
                {
                    dialog.FilterIndex = 1 + (i - 1) / 2;
                    return;
                }
            }
        }

        public static void SelectText(this TextBox box, string s)
        {
            box.SelectText(box.Text.IndexOf(s), s.Length);         
        }

        public static void SelectText(this TextBox box, int start, int len)
        {
            box.SelectionStart = start;
            box.SelectionLength = len;
            box.Select();
            box.Focus();
        }

        public static void AddRange(this ListBox lb, IEnumerable<string> items)
        {
            foreach (string item in items)
                lb.Items.Add(item);
        }

        public static IEnumerable<T> GetSelected<T>(this ListBox lb)
        {
            for (int i = 0; i < lb.Items.Count; i++)
                if (lb.GetSelected(i))
                    yield return (T) lb.Items[i];
        }

        public static void GetSelectedStrings(this ListBox lb, List<string> result)
        {
            for (int i = 0; i < lb.Items.Count; i++)
                if (lb.GetSelected(i))
                    result.Add(lb.Items[i].ToString());
        }

        public static string GetSelectedStrings(this ListBox lb, string delimiter)
        {
            string result = "";
            for (int i = 0; i < lb.Items.Count; i++)
                if (lb.GetSelected(i))
                    result = result.AppendTo(lb.Items[i].ToString(), delimiter);
            return result;
        }
       

        public static void SelectAll(this ListBox lb, bool select = true)
        {
            for(int i = 0; i < lb.Items.Count; i++)
                lb.SetSelected(i, select);          
        }

        public static int SelectString(this ListBox box, string text)
        {
            box.SelectedIndex = -1;
            for (int i = 0; i < box.Items.Count; i++)
                if (box.Items[i].ToString() == text)
                {
                    box.SelectedIndex = i;
                    break;
                }


            return box.SelectedIndex;
        }

        public static int SelectString(this ComboBox box, string text)
        {
            box.SelectedIndex = -1;

            for (int i = 0; i < box.Items.Count; i++)
                if (box.Items[i].ToString() == text)
                {
                    box.SelectedIndex = i;
                    break;
                }
                    

            return box.SelectedIndex;
        }

        public static void AlignMiddleVerticallyWith(this Control left, Control right)
        {
            int cy = (right.Height - left.Height) / 2;
            left.Top = right.Top + cy;
        }


        public static void PositionToLeftOf(this Control label, Control ctrl)
        {
            int cx = 3;
            int cy = (ctrl.Height - label.Height) / 2;
            label.SetBounds(ctrl.Left - (cx + label.Width), ctrl.Top + cy, label.Width, label.Height);
        }

        public static bool AllRootNodesCollapsed(this TreeView tree)
        {
            foreach (TreeNode n in tree.Nodes)
                if (n.IsExpanded)
                    return false;

            return true;
        }

        public static bool AllRootNodesExpanded(this TreeView tree)
        {
            foreach (TreeNode n in tree.Nodes)
                if (!n.IsExpanded)
                    return false;

            return true;

        }

        public static bool AllNodesCollapsed(this TreeView tree)
        {
            foreach (TreeNode n in tree.Nodes)
                if (!n.AllNodesCollapsed())
                    return false;

            return true;
        }

        public static bool AllNodesExpanded(this TreeView tree)
        {
            foreach (TreeNode n in tree.Nodes)
                if (!n.AllNodesExpanded())
                    return false;

            return true;
        }

        public static bool AllNodesCollapsed(this TreeNode parentNode)
        {
            if (parentNode.IsExpanded)
                return false;
            foreach (TreeNode n in parentNode.Nodes)
                if (!n.AllNodesCollapsed())
                    return false;

            return true;
        }

        public static bool AllNodesExpanded(this TreeNode parentNode)
        {
            if (parentNode.IsExpanded == false)
                return false;
            foreach (TreeNode n in parentNode.Nodes)
                if (!n.AllNodesExpanded())
                    return false;
            return true;
        }


        public static void ExpandToLevel(this TreeView tree, int level)
        {
            foreach (TreeNode n in tree.Nodes)
                n.ExpandToLevel(level);
        }

        public static void ExpandToLevel(this TreeNode parentNode, int level)
        {
            if (parentNode.Level >= level)
                parentNode.Collapse();
            else
                parentNode.Expand();
            foreach (TreeNode n in parentNode.Nodes)
                n.ExpandToLevel(level);
        }

        public static void AppendText(this RichTextBox tb, string text, Color color, bool scrollToBottom = true)
        {
            Color prevColor = tb.SelectionColor;
            if (color == Color.Empty || color == prevColor)
                prevColor = Color.Empty;
            else
                tb.SelectionColor = color;
            tb.AppendText(text);
            if(scrollToBottom)
            {
                tb.SelectionStart = tb.TextLength;
                tb.ScrollToCaret();
            }
            if(prevColor != Color.Empty)
                tb.SelectionColor = prevColor;
        }

        public static List<int> GetItemTagsAsIntegers(this ListView lv)
        {
            List<int> result = new List<int>();
            foreach (ListViewItem item in lv.SelectedItems)
                result.Add(Convert.ToInt32(item.Tag));
            return result;
        }

        public static List<int> GetSelectedIndices(this ListView lv)
        {
            List<int> result = new List<int>();
            lv.GetSelectedIndices(result);
            return result;
        }

        public static void GetSelectedIndices(this ListView lv, List<int> result)
        {
            foreach (int i in lv.SelectedIndices)
                result.Add(i);
        }

        public static ListViewItem AddRow(this ListView lv, string col1, string col2 = null, string col3 = null, string col4 = null, string col5 = null, string col6 = null)
        {
            ListViewItem item = lv.Items.Add(col1);
            if (col2 != null)
                item.SubItems.Add(col2);
            if (col3 != null)
                item.SubItems.Add(col3);
            if (col4 != null)
                item.SubItems.Add(col4);
            if (col5 != null)
                item.SubItems.Add(col5);
            if (col6 != null)
                item.SubItems.Add(col6);
            return item;
        }

        public static void SetDataSource(this ListBox lb, object dataSource, string displayMember = null, string valueMember = null)
        {
            lb.DataSource = null;
            if (displayMember != null)
                lb.DisplayMember = displayMember;
            if (valueMember != null)
                lb.ValueMember = valueMember;
            lb.DataSource = dataSource;
        }

        public static string GetCurrentScreen(this System.Windows.Forms.Form form)
        {
            Screen screen = Screen.FromControl(form);
            return screen.DeviceName.Replace("\\", "").Replace(".", "");
        }

        public static bool SelectScreen(this Form form, string screenName)
        {
            for (int i = 0; i < Screen.AllScreens.Length; i++)
            {
                if (Screen.AllScreens[i].DeviceName.Contains(screenName))
                {
                    form.Location = Screen.AllScreens[i].WorkingArea.Location;
                    return true ;
                }
            }
            return false;
        }
    }
}
