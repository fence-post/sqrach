using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using fp.lib;
using fp.lib.forms;

namespace fp.lib.forms
{
    public partial class NiceTabControl : UserControl
    {
        public class NiceTab : RadioButton
        {
            public bool CanMoveLeft { get { return TabIndex > 0; } }
            public bool CanMoveRight { get { return TabIndex <= Parent.Controls.Count - 1; } }
            public int MoveLeft() { return 0; }
            public int MoveRight() { return 0; }
            public Control AssociatedControl = null;
            public bool IsVisible = true;
            public object Data = null;
           

            public NiceTab(NiceTabControl parent, string name, Control ctrl = null)
            {
                Parent = parent;
                AssociatedControl = ctrl;
                FlatStyle = FlatStyle.Flat;
                ForeColor = Color.White;
                Appearance = System.Windows.Forms.Appearance.Button;
                AutoSize = true;
                FlatAppearance.BorderSize = 0;
                FlatAppearance.CheckedBackColor = parent.SelectedTabColor;
                FlatAppearance.MouseDownBackColor = FlatAppearance.MouseOverBackColor = parent.MouseOverTabColor; // System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
                BackColor = parent.BackColor;
                Location = new System.Drawing.Point(0, 100);
                Name = "tab" + name.Replace(" ", "");
                Size = new System.Drawing.Size(100, 10);
                TabIndex = parent.Controls.Count;
                TabStop = false;
                Text = name;
                UseVisualStyleBackColor = false;
                Padding = new Padding(2, 1, 2, 0);
                TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
                Image = parent.UnselectedTabImage;
            }
        }

        public EventHandler OnSelectedTabChanged;

        public NiceTab this[int i] { get { return tabs[i]; } }
        public NiceTab this[string nam] { get { return tabsByName[nam]; } }

        public int BlankSpaceToRightOfTabs = -1;
        public int RightExtent = -1;
        public Color MouseOverTabColor;
        public Color SelectedTabColor { get { return bottomLine.BackColor; } set { bottomLine.BackColor = value; } }
        public int BottomLineWidth { get { return bottomLine.Height; } set { bottomLine.Height = value; } }

        public NiceTab SelectedTab = null;
        public Image SelectedTabImage = null;
        public Image UnselectedTabImage = null;
        public int SelectedIndex { get { return SelectedTab == null ? -1 : SelectedTab.TabIndex; } }


        public ImageList ImageList;
        public int DropdownImageIndex = -1;
        public int CloseImageIndex = -1;

        List<NiceTab> visibleTabs = new List<NiceTab>();
        List<NiceTab> tabs = new List<NiceTab>();
        Dictionary<string, NiceTab> tabsByName = new Dictionary<string, NiceTab>();

        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel bottomLine;
        private System.Windows.Forms.Button moreTabsButton;
        private System.Windows.Forms.ContextMenuStrip dropDown;

        public NiceTabControl()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NiceTabControl));
            this.bottomLine = new System.Windows.Forms.Panel();
            this.moreTabsButton = new System.Windows.Forms.Button();
            this.dropDown = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.bottomLine.BackColor = System.Drawing.Color.Lime;
            this.bottomLine.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomLine.Location = new System.Drawing.Point(0, 314);
            this.bottomLine.Name = "panel1";
            this.bottomLine.Size = new System.Drawing.Size(1586, 4);
            this.bottomLine.TabIndex = 0;
            // 
            // moreTabsButton
            // 
            this.moreTabsButton.FlatAppearance.BorderSize = 0;
            this.moreTabsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.moreTabsButton.Location = new System.Drawing.Point(1027, 256);
            this.moreTabsButton.Name = "moreTabsButton";
            this.moreTabsButton.Size = new System.Drawing.Size(51, 48);
            this.moreTabsButton.TabIndex = 2;
            this.moreTabsButton.UseVisualStyleBackColor = true;
            this.moreTabsButton.Click += new System.EventHandler(this.MoreButtonClick);
            // 
            // dropDown
            // 
            this.dropDown.ImageScalingSize = new System.Drawing.Size(40, 40);
            this.dropDown.Name = "dropDown";
            this.dropDown.Size = new System.Drawing.Size(61, 4);
            // 
            // TabControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.Controls.Add(this.moreTabsButton);
            this.Controls.Add(this.bottomLine);
            this.Name = "TabControl";
            this.Size = new System.Drawing.Size(1586, 324);
            this.Resize += new System.EventHandler(this.TabControl_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        public void SetItemData(int i, object data)
        {
            tabs[i].Data = data;
        }
      
        public object GetItemData(int i)
        {
            return tabs[i].Data;
        }


        public int MoreButtonLeft = 0;
        public void UpdateTabs()
        {
            moreTabsButton.Left = Width - moreTabsButton.Width;
            moreTabsButton.Top = bottomLine.Top - moreTabsButton.Height;
            moreTabsButton.ImageList = ImageList;
            moreTabsButton.ImageIndex = DropdownImageIndex;

            MoreButtonLeft = moreTabsButton.Left;
            int rightBoundary = moreTabsButton.Left - (BlankSpaceToRightOfTabs + 10);
            

            visibleTabs.Clear();
            Height = Math.Max(MinHeight, bottomLine.Height + FormsToolbox.GetTextHeight("0", Font) + 10); //  FormsTo tabs[0].Height;
            bottomLine.Top = Height - bottomLine.Height;
            int left = 0;
            int ct = 0;
            foreach (NiceTab b in tabs)
            {
                if (b.IsVisible)
                {
                    visibleTabs.Add(b);
                    b.TabIndex = ct++;
                    if (left + b.Width >= rightBoundary)
                    {
                        b.Visible = false;
                    }
                    else
                    {
                        b.Visible = true;
                        b.FlatAppearance.CheckedBackColor = SelectedTabColor;
                        b.FlatAppearance.MouseOverBackColor = b.Checked ? SelectedTabColor : MouseOverTabColor;

                        // b.FlatAppearance.MouseOverBackColor = MouseOverTabColor;
                        b.BackColor = BackColor;
                        b.ForeColor = ForeColor;
                        b.Top = bottomLine.Top - b.Height;
                        b.Left = left;
                        left += b.Width;
                        RightExtent = left;
                        if (b.AssociatedControl != null)
                            b.AssociatedControl.Visible = b == SelectedTab;

                    }
                }
            }



        }

        public int MinHeight = 0;
        public int Count { get { return visibleTabs.Count; } }

        public void RemoveTab(int i)
        {
            int idx = SelectedIndex;
            NiceTab t = visibleTabs[i];
            tabsByName.Remove(t.Text);
            tabs.Remove(t);
            t.Dispose();
            UpdateTabs();
            idx = T.MinMax(0, visibleTabs.Count - 1, idx);
            if (idx >= 0)
                SelectTab(idx);
        }

        public void Clear()
        {
            foreach (NiceTab t in tabs)
                Controls.Remove(t);

            tabsByName.Clear();
            visibleTabs.Clear();
            tabs.Clear();

            UpdateTabs();

        }

        public void SelectTab(int i)
        {
            NiceTab t = visibleTabs[i];
            t.Checked = true;
            OnTabCheckedChanged(t, null);

        }

        public bool CanMoveTabRight { get { return SelectedTab.TabIndex < visibleTabs.Count - 1; } }
        public bool CanMoveTabLeft { get { return SelectedTab.TabIndex > 0; } }

        public int MoveSelectedTabRight()
        {
            NiceTab t1 = SelectedTab;
            NiceTab t2 = visibleTabs[t1.TabIndex + 1];
            int i1 = tabs.IndexOf(t1);
            int i2 = tabs.IndexOf(t2);
            tabs.Swap(i1, i2);
            UpdateTabs();
            // SelectTab(t1.TabIndex);
            return t1.TabIndex;

        }

        public int MoveSelectedTabLeft()
        {
            NiceTab t1 = SelectedTab;
            NiceTab t2 = visibleTabs[t1.TabIndex - 1];
            int i1 = tabs.IndexOf(t1);
            int i2 = tabs.IndexOf(t2);
            tabs.Swap(i1, i2);
            UpdateTabs();
            // SelectTab(t1.TabIndex);
            return t1.TabIndex;

        }
        public void SelectTab(string i)
        {
            SelectTab(tabsByName[i].TabIndex);
            /*
            Tab t = tabsByName[i];
            t.Checked = true;
            OnTabCheckedChanged(t, null);
            */
        }

        public void ShowTab(string label, bool visible)
        {
            int idx = SelectedIndex;
            NiceTab t = tabsByName[label];
            t.Visible = t.IsVisible = visible;
            UpdateTabs();
            if (visible)
            {
                SelectTab(label);
            }
            else
            {
                idx = T.MinMax(0, visibleTabs.Count - 1, idx);
                if (idx >= 0)
                    SelectTab(idx);
            }
        }

        public int lastTabIndexUnderPoint = -1;
        public int TabIndexUnderPoint(Point p)
        {
            foreach (NiceTab t in visibleTabs)
                if (t.Visible && RectangleToScreen(t.Bounds).Contains(p))
                    return lastTabIndexUnderPoint = t.TabIndex;

            return lastTabIndexUnderPoint = -1;
        }

        public int AddTab(string label, Control ctrl = null, bool visible = true)
        {
            NiceTab tab = new NiceTab(this, label, ctrl);
            tab.CheckedChanged += new System.EventHandler(OnTabCheckedChanged);
            tabs.Add(tab);
            if (!tabsByName.ContainsKey(label))
                tabsByName.Add(label, tab);
            if (tabs.Count == 1)
                tab.Checked = true;
            tab.Visible = tab.IsVisible = visible;
            UpdateTabs();

            return tabs.Count - 1;
        }

        private void OnTabCheckedChanged(object sender, EventArgs e)
        {
            NiceTab tab = sender as NiceTab;
            if (tab.Checked)
            {
                SelectedTab = tab;
                tab.FlatAppearance.CheckedBackColor = SelectedTabColor;
                tab.Image = tab.Checked ? SelectedTabImage : UnselectedTabImage;

                foreach (NiceTab t in tabs)
                {
                    t.FlatAppearance.MouseOverBackColor = t.Checked ? SelectedTabColor : MouseOverTabColor;

                    if (t.AssociatedControl != null)
                    {
                        if (t == SelectedTab)
                            t.AssociatedControl.Visible = true;
                        else
                            t.AssociatedControl.Visible = false;
                    }
                }


                if (OnSelectedTabChanged != null && tab.Checked)
                    OnSelectedTabChanged(this, null);

            }
        }


        private void TabControl_Resize(object sender, EventArgs e)
        {
            UpdateTabs();
        }

        private void OnDropdownItemSelected(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            SelectTab(Convert.ToInt32(item.Tag));
        }

        private void MoreButtonClick(object sender, EventArgs e)
        {
            dropDown.Items.Clear();
            // dropDown.BackColor = UI.passiveBackColor;
            // dropDown.ForeColor = Color.White;
            // render background color: https://stackoverflow.com/questions/25425948/c-sharp-winforms-toolstripmenuitem-change-background
            foreach (NiceTab t in visibleTabs)
            {
                ToolStripMenuItem item = dropDown.Items.Add(t.Text) as ToolStripMenuItem;
                item.ImageScaling = ToolStripItemImageScaling.None;
                item.Checked = SelectedTab == t;
                item.Tag = t.TabIndex;
                item.Click += new EventHandler(OnDropdownItemSelected);
            }

            dropDown.Show(PointToScreen(new Point(moreTabsButton.Right, Bottom)), ToolStripDropDownDirection.BelowLeft);
        }
    }
}
