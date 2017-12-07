using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ScintillaNET;
using fp.lib;
using fp.lib.forms;
using System.Configuration;
using System.Diagnostics;

namespace fp.sqratch
{
    partial class main : Form
    {
        private void UpdateUIPreferences(bool updateControls)
        {
            UI.LoadPreferences();
            UI.buttonImages = bigButtonImages; // UI.dark ? smallDarkImages : smallLightImages;
            UI.smallButtonImages = smallButtonImages; // UI.dark ? smallDarkImages : smallLightImages;
            UI.treeImages = treeImages; // treeImages; // darkImages : whiteImages;

            addButton.SetUIPreferences(UI.frameBackground, UI.hoverTabColor, UI.buttonImages);
            goButton.SetUIPreferences(UI.frameBackground, UI.hoverTabColor, UI.buttonImages);
            goButton.ForeColor = UI.activeForeColor;
            goButton.Font = leftTabs.Font = rightTabs.Font = UI.environmentFont;

            autoComplete.UpdateUIPreferences();
            allObjectsTree.UpdateUIPreferences();
            activeObjectsTree.UpdateUIPreferences();
            queryHistory.UpdateUIPreferences();
            resultsList.UpdateUIPreferences();
            editorHeader.UpdateUIPreferences();
        
            rightTabs.ImageList = leftTabs.ImageList = UI.buttonImages;
            rightTabs.ForeColor = leftTabs.ForeColor = UI.activeForeColor;
            rightTabs.BackColor = leftTabs.BackColor = UI.frameBackground;
            rightTabs.SelectedTabColor = leftTabs.SelectedTabColor = UI.selectedTabColor;
            rightTabs.MouseOverTabColor = leftTabs.MouseOverTabColor = UI.hoverTabColor;
            rightTabs.UpdateTabs();
            UpdateLeftTabs();
            
            objectSplitter.BackColor = vertSplitter.BackColor = horzSplitter.BackColor = UI.frameBackground;
            
            logTextBox.BackColor = UI.passiveBackColor; 
            logTextBox.ForeColor = UI.passiveForeColor; //  dark ? foreColor : SystemColors.WindowText;
            logTextBox.Font = UI.consoleFont;

            if(updateControls)
            {
                UI.InitializeEditor(editor);
                queryHistory.dirty = true;
                allObjectsTreeDirty = true;
                logTextBox.Text = "";
                parseViewsDirty = true;
                layoutDirty = true;
            }

        }

        private void horizontalSplitter_Panel2_Resize(object sender, EventArgs e)
        {
            UpdateRightPanelLayout();
        }

        private void horizontalSplitter_Panel1_Resize(object sender, EventArgs e)
        {
            UpdateLeftPanelLayout();
        }

        private void UpdateRightPanelLayout()
        {
            if (rightTabs == null)
                return;

            horzSplitter.Panel2Collapsed = rightTabs.Count == 0;
           
            int offset = 0;
            Rectangle rect = horzSplitter.Panel2.ClientRectangle;
            rightTabs.UpdateTabs();
            rightTabs.Top = 0; // menu.Bottom + 10;
            rightTabs.Left = offset;
            rightTabs.Width = rect.Width - offset;
            rightTabs.SendToBack();

            rect = new Rectangle(rect.Left, rightTabs.Bottom, rect.Width, rect.Height - rightTabs.Bottom);
            queryHistory.Bounds = logTextBox.Bounds = objectSplitter.Bounds = rect;

            objectSplitterDistanceDirty = true;
        }

        private void UpdateLeftPanelLayout(bool onlyButtons = false)
        {             
            if (leftTabs == null)
                return;
            
            int buttonSize = 32;
            int offset = 0;
            leftTabs.UpdateTabs();
            leftTabs.Top = 0; // menu.Bottom + 10;
            leftTabs.Left = offset;
            leftTabs.Width = horzSplitter.Panel1.ClientRectangle.Width - offset;
            leftTabs.BlankSpaceToRightOfTabs = buttonSize * 5;
            leftTabs.SendToBack();
            leftTabs.UpdateTabs();
            
            Rectangle rc = new Rectangle(leftTabs.MoreButtonLeft, leftTabs.Bottom - (buttonSize + leftTabs.BottomLineWidth), buttonSize, buttonSize);

            rc.Offset(-buttonSize, 0);
            int width = buttonSize + FormsToolbox.GetTextWidth("STOP", goButton.Font);
            goButton.Bounds = new Rectangle(rc.Right - width, rc.Top, width, rc.Height);
            rc.Offset(-buttonSize, 0);
            addButton.Bounds = new Rectangle(leftTabs.RightExtent, rc.Top, rc.Width, rc.Height);
            if (onlyButtons)
                return;

            int top = leftTabs.Bottom;
            editorHeader.Width = horzSplitter.Panel1.ClientRectangle.Width - offset;
            editorHeader.Top = top;
            top = editorHeader.Bottom;
            vertSplitter.Bounds = new Rectangle(offset, top, horzSplitter.Panel1.ClientRectangle.Width - offset, horzSplitter.Panel1.ClientRectangle.Height - (top));
            
            /*
            rc = vertSplitter.Panel2.ClientRectangle;
            top = 200;
            editorDropdown.Bounds = new Rectangle(rc.Left, rc.Top, rc.Width, top);
            resultsList.Bounds = new Rectangle(rc.Left, top, rc.Width, rc.Height - top);
            */
        }

        int idles = 0;
        protected void UpdateStatusBar()
        {
            bool err = Parser.parseFailed;
            if (err && parseStatus.Image == null)
                parseStatus.Image = UI.treeImages.Images[5];
            else if (err == false && parseStatus.Image != null)
                parseStatus.Image = null;

            string text;
            if(selectedQuery != null)
            {
                if (selectedQuery.executionTime != queryTime.Text)
                    queryTime.Text = selectedQuery.executionTime;

                text = selectedQuery.rows.Count > 0 ? selectedQuery.rows.Count.ToString() : "";
                if (text == "")
                    text = "(" + idles.ToString() + ")";
                if (text != rowCount.Text)
                    rowCount.Text = text;
            }

            lastStatusUpdate = Environment.TickCount;
            text = A.currentStatusText;
            if (text.Contains("ing"))
                text += "...";
            if (status.Text != text)
            {
                status.Text = text;
                status.ForeColor = status.Text == "Failed" ? Color.Red : Color.Black;
            }

            UpdateProgress(true);
        }

        public void UpdateProgress(bool updateNow = false)
        {
            if (A.currentStatus == Status.Closing)
                return;

            if (A.progressValue >= 0 && A.progressValue < A.progressMax)
            {
                if (progress.Minimum != 0)
                    progress.Minimum = 0;
                if (progress.Maximum != A.progressMax)
                    progress.Maximum = A.progressMax;
                if (!progress.Visible)
                    progress.Visible = true;
                progress.Value = A.progressValue;
                if (updateNow)
                    progress.Invalidate();
            }
            else if (progress.Visible)
                progress.Visible = false;
        }
    }
}
