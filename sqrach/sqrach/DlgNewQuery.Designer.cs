namespace fp.sqratch
{
    partial class DlgNewQuery
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tablesList = new System.Windows.Forms.ListBox();
            this.bCancel = new System.Windows.Forms.Button();
            this.bOK = new System.Windows.Forms.Button();
            this.queryTypeCombo = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.columnsTabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.columnList = new System.Windows.Forms.ListBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.previewFrame = new System.Windows.Forms.Panel();
            this.includeAllCheckbox = new System.Windows.Forms.CheckBox();
            this.innerJoin = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.showAllColumns = new System.Windows.Forms.CheckBox();
            this.parameterize = new System.Windows.Forms.CheckBox();
            this.insertIgnore = new System.Windows.Forms.CheckBox();
            this.includeAliases = new System.Windows.Forms.CheckBox();
            this.columnsTabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tablesList
            // 
            this.tablesList.FormattingEnabled = true;
            this.tablesList.IntegralHeight = false;
            this.tablesList.ItemHeight = 31;
            this.tablesList.Location = new System.Drawing.Point(31, 179);
            this.tablesList.Name = "tablesList";
            this.tablesList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.tablesList.Size = new System.Drawing.Size(522, 284);
            this.tablesList.TabIndex = 0;
            this.tablesList.SelectedIndexChanged += new System.EventHandler(this.tablesList_SelectedIndexChanged);
            // 
            // bCancel
            // 
            this.bCancel.AccessibleDescription = "Cancel";
            this.bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bCancel.Location = new System.Drawing.Point(933, 906);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new System.Drawing.Size(172, 55);
            this.bCancel.TabIndex = 11;
            this.bCancel.Text = "Cancel";
            this.bCancel.UseVisualStyleBackColor = true;
            // 
            // bOK
            // 
            this.bOK.AccessibleDescription = "";
            this.bOK.Enabled = false;
            this.bOK.Location = new System.Drawing.Point(755, 906);
            this.bOK.Name = "bOK";
            this.bOK.Size = new System.Drawing.Size(172, 55);
            this.bOK.TabIndex = 7;
            this.bOK.Text = "Create";
            this.bOK.UseVisualStyleBackColor = true;
            this.bOK.Click += new System.EventHandler(this.bOK_Click);
            // 
            // queryTypeCombo
            // 
            this.queryTypeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.queryTypeCombo.FormattingEnabled = true;
            this.queryTypeCombo.Location = new System.Drawing.Point(31, 71);
            this.queryTypeCombo.Name = "queryTypeCombo";
            this.queryTypeCombo.Size = new System.Drawing.Size(522, 39);
            this.queryTypeCombo.TabIndex = 12;
            this.queryTypeCombo.SelectedIndexChanged += new System.EventHandler(this.queryTypeCombo_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(25, 36);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(162, 32);
            this.label5.TabIndex = 13;
            this.label5.Text = "Query Type";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(25, 144);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(101, 32);
            this.label6.TabIndex = 14;
            this.label6.Text = "Tables";
            // 
            // columnsTabControl
            // 
            this.columnsTabControl.Controls.Add(this.tabPage1);
            this.columnsTabControl.Controls.Add(this.tabPage2);
            this.columnsTabControl.Controls.Add(this.tabPage3);
            this.columnsTabControl.Controls.Add(this.tabPage4);
            this.columnsTabControl.Location = new System.Drawing.Point(583, 71);
            this.columnsTabControl.Name = "columnsTabControl";
            this.columnsTabControl.SelectedIndex = 0;
            this.columnsTabControl.Size = new System.Drawing.Size(567, 392);
            this.columnsTabControl.TabIndex = 20;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.columnList);
            this.tabPage1.Location = new System.Drawing.Point(10, 48);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(547, 334);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Columns";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // columnList
            // 
            this.columnList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.columnList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.columnList.FormattingEnabled = true;
            this.columnList.IntegralHeight = false;
            this.columnList.ItemHeight = 31;
            this.columnList.Location = new System.Drawing.Point(0, 0);
            this.columnList.Name = "columnList";
            this.columnList.Size = new System.Drawing.Size(547, 334);
            this.columnList.TabIndex = 0;
            this.columnList.SelectedIndexChanged += new System.EventHandler(this.columnList_SelectedIndexChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(10, 48);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(547, 334);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Where";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Location = new System.Drawing.Point(10, 48);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(547, 334);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Group";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            this.tabPage4.Location = new System.Drawing.Point(10, 48);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(547, 334);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Order";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // previewFrame
            // 
            this.previewFrame.Location = new System.Drawing.Point(31, 543);
            this.previewFrame.Name = "previewFrame";
            this.previewFrame.Size = new System.Drawing.Size(1119, 335);
            this.previewFrame.TabIndex = 21;
            // 
            // includeAllCheckbox
            // 
            this.includeAllCheckbox.AutoSize = true;
            this.includeAllCheckbox.Location = new System.Drawing.Point(31, 916);
            this.includeAllCheckbox.Name = "includeAllCheckbox";
            this.includeAllCheckbox.Size = new System.Drawing.Size(452, 36);
            this.includeAllCheckbox.TabIndex = 28;
            this.includeAllCheckbox.Text = "Include all objects in workspace";
            this.includeAllCheckbox.UseVisualStyleBackColor = true;
            this.includeAllCheckbox.CheckedChanged += new System.EventHandler(this.includeAllCheckbox_CheckedChanged);
            // 
            // innerJoin
            // 
            this.innerJoin.AutoSize = true;
            this.innerJoin.Location = new System.Drawing.Point(376, 139);
            this.innerJoin.Name = "innerJoin";
            this.innerJoin.Size = new System.Drawing.Size(177, 36);
            this.innerJoin.TabIndex = 26;
            this.innerJoin.Text = "Inner Join";
            this.innerJoin.UseVisualStyleBackColor = true;
            this.innerJoin.CheckedChanged += new System.EventHandler(this.innerJoin_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(577, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(127, 32);
            this.label1.TabIndex = 32;
            this.label1.Text = "Columns";
            // 
            // showAllColumns
            // 
            this.showAllColumns.AutoSize = true;
            this.showAllColumns.Location = new System.Drawing.Point(583, 485);
            this.showAllColumns.Name = "showAllColumns";
            this.showAllColumns.Size = new System.Drawing.Size(283, 36);
            this.showAllColumns.TabIndex = 30;
            this.showAllColumns.Text = "Show All Columns";
            this.showAllColumns.UseVisualStyleBackColor = true;
            this.showAllColumns.CheckedChanged += new System.EventHandler(this.showAllColumns_CheckedChanged);
            // 
            // parameterize
            // 
            this.parameterize.AutoSize = true;
            this.parameterize.Location = new System.Drawing.Point(331, 485);
            this.parameterize.Name = "parameterize";
            this.parameterize.Size = new System.Drawing.Size(222, 36);
            this.parameterize.TabIndex = 27;
            this.parameterize.Text = "Parameterize";
            this.parameterize.UseVisualStyleBackColor = true;
            this.parameterize.CheckedChanged += new System.EventHandler(this.parameterize_CheckedChanged);
            // 
            // insertIgnore
            // 
            this.insertIgnore.AutoSize = true;
            this.insertIgnore.Location = new System.Drawing.Point(940, 485);
            this.insertIgnore.Name = "insertIgnore";
            this.insertIgnore.Size = new System.Drawing.Size(210, 36);
            this.insertIgnore.TabIndex = 34;
            this.insertIgnore.Text = "Insert Ignore";
            this.insertIgnore.UseVisualStyleBackColor = true;
            this.insertIgnore.CheckedChanged += new System.EventHandler(this.insertIgnore_CheckedChanged);
            // 
            // includeAliases
            // 
            this.includeAliases.AutoSize = true;
            this.includeAliases.Location = new System.Drawing.Point(31, 485);
            this.includeAliases.Name = "includeAliases";
            this.includeAliases.Size = new System.Drawing.Size(245, 36);
            this.includeAliases.TabIndex = 37;
            this.includeAliases.Text = "Include Aliases";
            this.includeAliases.UseVisualStyleBackColor = true;
            this.includeAliases.CheckedChanged += new System.EventHandler(this.includeAliases_CheckedChanged);
            // 
            // DlgNewQuery
            // 
            this.AcceptButton = this.bOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bCancel;
            this.ClientSize = new System.Drawing.Size(1176, 998);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.insertIgnore);
            this.Controls.Add(this.parameterize);
            this.Controls.Add(this.includeAliases);
            this.Controls.Add(this.showAllColumns);
            this.Controls.Add(this.innerJoin);
            this.Controls.Add(this.includeAllCheckbox);
            this.Controls.Add(this.previewFrame);
            this.Controls.Add(this.columnsTabControl);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.queryTypeCombo);
            this.Controls.Add(this.tablesList);
            this.Controls.Add(this.bCancel);
            this.Controls.Add(this.bOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "DlgNewQuery";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "New Query";
            this.Load += new System.EventHandler(this.DlgNewQuery_Load);
            this.Shown += new System.EventHandler(this.DlgNewQuery_Shown);
            this.columnsTabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ListBox tablesList;
        private System.Windows.Forms.Button bCancel;
        private System.Windows.Forms.Button bOK;
        private System.Windows.Forms.ComboBox queryTypeCombo;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TabControl columnsTabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Panel previewFrame;
        private System.Windows.Forms.ListBox columnList;
        private System.Windows.Forms.CheckBox includeAllCheckbox;
        private System.Windows.Forms.CheckBox innerJoin;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox showAllColumns;
        private System.Windows.Forms.CheckBox parameterize;
        private System.Windows.Forms.CheckBox insertIgnore;
        private System.Windows.Forms.CheckBox includeAliases;
    }
}