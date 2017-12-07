namespace fp.sqratch
{
    partial class DlgSaveAs
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
            this.columnTitles = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.fileTypeCombo = new System.Windows.Forms.ComboBox();
            this.bCancel = new System.Windows.Forms.Button();
            this.bOK = new System.Windows.Forms.Button();
            this.browseButton = new System.Windows.Forms.Button();
            this.directoryTextbox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.fileNameTextbox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.colSeparator = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.rowSeparator = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.columnsList = new System.Windows.Forms.ListBox();
            this.moreOptionsButton = new System.Windows.Forms.Button();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.escapeWhenNecessary = new System.Windows.Forms.CheckBox();
            this.tableLabel = new System.Windows.Forms.Label();
            this.tableCombo = new System.Windows.Forms.ComboBox();
            this.saveSelected = new System.Windows.Forms.CheckBox();
            this.doubleQuoteStrings = new System.Windows.Forms.CheckBox();
            this.singleQuoteStrings = new System.Windows.Forms.CheckBox();
            this.printNulls = new System.Windows.Forms.CheckBox();
            this.removeTime = new System.Windows.Forms.CheckBox();
            this.rowTemplate = new System.Windows.Forms.TextBox();
            this.columnTemplate = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.formatLabel = new System.Windows.Forms.Label();
            this.formatCombo = new System.Windows.Forms.ComboBox();
            this.stripNonAscii = new System.Windows.Forms.CheckBox();
            this.padStrings = new System.Windows.Forms.CheckBox();
            this.removeNewlines = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // columnTitles
            // 
            this.columnTitles.AutoSize = true;
            this.columnTitles.Location = new System.Drawing.Point(30, 675);
            this.columnTitles.Name = "columnTitles";
            this.columnTitles.Size = new System.Drawing.Size(326, 36);
            this.columnTitles.TabIndex = 37;
            this.columnTitles.Text = "Include Column Titles";
            this.columnTitles.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(24, 26);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(132, 32);
            this.label5.TabIndex = 43;
            this.label5.Text = "File Type";
            // 
            // fileTypeCombo
            // 
            this.fileTypeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fileTypeCombo.FormattingEnabled = true;
            this.fileTypeCombo.Items.AddRange(new object[] {
            "Comma Delimited",
            "Tab Delimited",
            "Plain Text",
            "Custom"});
            this.fileTypeCombo.Location = new System.Drawing.Point(30, 61);
            this.fileTypeCombo.Name = "fileTypeCombo";
            this.fileTypeCombo.Size = new System.Drawing.Size(355, 39);
            this.fileTypeCombo.TabIndex = 42;
            this.fileTypeCombo.SelectedIndexChanged += new System.EventHandler(this.fileTypeCombo_SelectedIndexChanged);
            // 
            // bCancel
            // 
            this.bCancel.AccessibleDescription = "Cancel";
            this.bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bCancel.Location = new System.Drawing.Point(578, 665);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new System.Drawing.Size(147, 55);
            this.bCancel.TabIndex = 41;
            this.bCancel.Text = "Cancel";
            this.bCancel.UseVisualStyleBackColor = true;
            // 
            // bOK
            // 
            this.bOK.AccessibleDescription = "";
            this.bOK.Enabled = false;
            this.bOK.Location = new System.Drawing.Point(415, 665);
            this.bOK.Name = "bOK";
            this.bOK.Size = new System.Drawing.Size(147, 55);
            this.bOK.TabIndex = 40;
            this.bOK.Text = "Save";
            this.bOK.UseVisualStyleBackColor = true;
            this.bOK.Click += new System.EventHandler(this.bOK_Click);
            // 
            // browseButton
            // 
            this.browseButton.Location = new System.Drawing.Point(416, 148);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(147, 55);
            this.browseButton.TabIndex = 49;
            this.browseButton.Text = "Browse...";
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // directoryTextbox
            // 
            this.directoryTextbox.Location = new System.Drawing.Point(30, 249);
            this.directoryTextbox.Name = "directoryTextbox";
            this.directoryTextbox.Size = new System.Drawing.Size(696, 38);
            this.directoryTextbox.TabIndex = 52;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 214);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(128, 32);
            this.label1.TabIndex = 53;
            this.label1.Text = "Directory";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 122);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(144, 32);
            this.label2.TabIndex = 55;
            this.label2.Text = "File Name";
            // 
            // fileNameTextbox
            // 
            this.fileNameTextbox.Location = new System.Drawing.Point(30, 157);
            this.fileNameTextbox.Name = "fileNameTextbox";
            this.fileNameTextbox.Size = new System.Drawing.Size(355, 38);
            this.fileNameTextbox.TabIndex = 54;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(1086, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(245, 32);
            this.label3.TabIndex = 57;
            this.label3.Text = "Column Separator";
            // 
            // colSeparator
            // 
            this.colSeparator.Location = new System.Drawing.Point(1092, 61);
            this.colSeparator.Name = "colSeparator";
            this.colSeparator.Size = new System.Drawing.Size(300, 38);
            this.colSeparator.TabIndex = 56;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(763, 26);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(203, 32);
            this.label4.TabIndex = 59;
            this.label4.Text = "Row Separator";
            // 
            // rowSeparator
            // 
            this.rowSeparator.Location = new System.Drawing.Point(769, 61);
            this.rowSeparator.Name = "rowSeparator";
            this.rowSeparator.Size = new System.Drawing.Size(300, 38);
            this.rowSeparator.TabIndex = 58;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(24, 304);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(127, 32);
            this.label6.TabIndex = 61;
            this.label6.Text = "Columns";
            // 
            // columnsList
            // 
            this.columnsList.FormattingEnabled = true;
            this.columnsList.IntegralHeight = false;
            this.columnsList.ItemHeight = 31;
            this.columnsList.Location = new System.Drawing.Point(30, 339);
            this.columnsList.Name = "columnsList";
            this.columnsList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.columnsList.Size = new System.Drawing.Size(355, 306);
            this.columnsList.TabIndex = 60;
            // 
            // moreOptionsButton
            // 
            this.moreOptionsButton.Location = new System.Drawing.Point(580, 148);
            this.moreOptionsButton.Name = "moreOptionsButton";
            this.moreOptionsButton.Size = new System.Drawing.Size(146, 55);
            this.moreOptionsButton.TabIndex = 66;
            this.moreOptionsButton.Text = "More>>";
            this.moreOptionsButton.UseVisualStyleBackColor = true;
            this.moreOptionsButton.Click += new System.EventHandler(this.moreOptionsButton_Click);
            // 
            // escapeWhenNecessary
            // 
            this.escapeWhenNecessary.AutoSize = true;
            this.escapeWhenNecessary.Location = new System.Drawing.Point(416, 354);
            this.escapeWhenNecessary.Name = "escapeWhenNecessary";
            this.escapeWhenNecessary.Size = new System.Drawing.Size(251, 36);
            this.escapeWhenNecessary.TabIndex = 27;
            this.escapeWhenNecessary.Text = "Escape \\n and \"";
            this.escapeWhenNecessary.UseVisualStyleBackColor = true;
            // 
            // tableLabel
            // 
            this.tableLabel.AutoSize = true;
            this.tableLabel.Location = new System.Drawing.Point(410, 26);
            this.tableLabel.Name = "tableLabel";
            this.tableLabel.Size = new System.Drawing.Size(87, 32);
            this.tableLabel.TabIndex = 69;
            this.tableLabel.Text = "Table";
            // 
            // tableCombo
            // 
            this.tableCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tableCombo.FormattingEnabled = true;
            this.tableCombo.Items.AddRange(new object[] {
            "Comma Delimited",
            "Tab Delimited",
            "Plain Text",
            "Custom"});
            this.tableCombo.Location = new System.Drawing.Point(416, 61);
            this.tableCombo.Name = "tableCombo";
            this.tableCombo.Size = new System.Drawing.Size(308, 39);
            this.tableCombo.TabIndex = 68;
            this.tableCombo.SelectedIndexChanged += new System.EventHandler(this.tableCombo_SelectedIndexChanged);
            // 
            // saveSelected
            // 
            this.saveSelected.AutoSize = true;
            this.saveSelected.Location = new System.Drawing.Point(418, 64);
            this.saveSelected.Name = "saveSelected";
            this.saveSelected.Size = new System.Drawing.Size(308, 36);
            this.saveSelected.TabIndex = 70;
            this.saveSelected.Text = "Only Selected Rows";
            this.saveSelected.UseVisualStyleBackColor = true;
            // 
            // doubleQuoteStrings
            // 
            this.doubleQuoteStrings.AutoSize = true;
            this.doubleQuoteStrings.Location = new System.Drawing.Point(415, 564);
            this.doubleQuoteStrings.Name = "doubleQuoteStrings";
            this.doubleQuoteStrings.Size = new System.Drawing.Size(189, 36);
            this.doubleQuoteStrings.TabIndex = 71;
            this.doubleQuoteStrings.Text = "Strings in \"";
            this.doubleQuoteStrings.UseVisualStyleBackColor = true;
            // 
            // singleQuoteStrings
            // 
            this.singleQuoteStrings.AutoSize = true;
            this.singleQuoteStrings.Location = new System.Drawing.Point(605, 564);
            this.singleQuoteStrings.Name = "singleQuoteStrings";
            this.singleQuoteStrings.Size = new System.Drawing.Size(88, 36);
            this.singleQuoteStrings.TabIndex = 72;
            this.singleQuoteStrings.Text = "in \'";
            this.singleQuoteStrings.UseVisualStyleBackColor = true;
            // 
            // printNulls
            // 
            this.printNulls.AutoSize = true;
            this.printNulls.Location = new System.Drawing.Point(416, 522);
            this.printNulls.Name = "printNulls";
            this.printNulls.Size = new System.Drawing.Size(216, 36);
            this.printNulls.TabIndex = 73;
            this.printNulls.Text = "Include Nulls";
            this.printNulls.UseVisualStyleBackColor = true;
            // 
            // removeTime
            // 
            this.removeTime.AutoSize = true;
            this.removeTime.Location = new System.Drawing.Point(416, 438);
            this.removeTime.Name = "removeTime";
            this.removeTime.Size = new System.Drawing.Size(228, 36);
            this.removeTime.TabIndex = 74;
            this.removeTime.Text = "Remove Time";
            this.removeTime.UseVisualStyleBackColor = true;
            // 
            // rowTemplate
            // 
            this.rowTemplate.Location = new System.Drawing.Point(768, 148);
            this.rowTemplate.Multiline = true;
            this.rowTemplate.Name = "rowTemplate";
            this.rowTemplate.Size = new System.Drawing.Size(624, 261);
            this.rowTemplate.TabIndex = 75;
            // 
            // columnTemplate
            // 
            this.columnTemplate.Location = new System.Drawing.Point(768, 459);
            this.columnTemplate.Multiline = true;
            this.columnTemplate.Name = "columnTemplate";
            this.columnTemplate.Size = new System.Drawing.Size(624, 261);
            this.columnTemplate.TabIndex = 76;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(762, 113);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(197, 32);
            this.label7.TabIndex = 77;
            this.label7.Text = "Row Template";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(763, 424);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(239, 32);
            this.label8.TabIndex = 78;
            this.label8.Text = "Column Template";
            // 
            // formatLabel
            // 
            this.formatLabel.AutoSize = true;
            this.formatLabel.Location = new System.Drawing.Point(410, 304);
            this.formatLabel.Name = "formatLabel";
            this.formatLabel.Size = new System.Drawing.Size(104, 32);
            this.formatLabel.TabIndex = 80;
            this.formatLabel.Text = "Format";
            // 
            // formatCombo
            // 
            this.formatCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.formatCombo.FormattingEnabled = true;
            this.formatCombo.Items.AddRange(new object[] {
            "Comma Delimited",
            "Tab Delimited",
            "Plain Text",
            "Custom"});
            this.formatCombo.Location = new System.Drawing.Point(416, 339);
            this.formatCombo.Name = "formatCombo";
            this.formatCombo.Size = new System.Drawing.Size(309, 39);
            this.formatCombo.TabIndex = 79;
            this.formatCombo.SelectedIndexChanged += new System.EventHandler(this.formatCombo_SelectedIndexChanged);
            // 
            // stripNonAscii
            // 
            this.stripNonAscii.AutoSize = true;
            this.stripNonAscii.Location = new System.Drawing.Point(415, 480);
            this.stripNonAscii.Name = "stripNonAscii";
            this.stripNonAscii.Size = new System.Drawing.Size(278, 36);
            this.stripNonAscii.TabIndex = 81;
            this.stripNonAscii.Text = "Remove NonAscii";
            this.stripNonAscii.UseVisualStyleBackColor = true;
            // 
            // padStrings
            // 
            this.padStrings.AutoSize = true;
            this.padStrings.Location = new System.Drawing.Point(416, 606);
            this.padStrings.Name = "padStrings";
            this.padStrings.Size = new System.Drawing.Size(200, 36);
            this.padStrings.TabIndex = 82;
            this.padStrings.Text = "Pad Strings";
            this.padStrings.UseVisualStyleBackColor = true;
            // 
            // removeNewlines
            // 
            this.removeNewlines.AutoSize = true;
            this.removeNewlines.Location = new System.Drawing.Point(416, 396);
            this.removeNewlines.Name = "removeNewlines";
            this.removeNewlines.Size = new System.Drawing.Size(189, 36);
            this.removeNewlines.TabIndex = 83;
            this.removeNewlines.Text = "Remove \\n";
            this.removeNewlines.UseVisualStyleBackColor = true;
            // 
            // DlgSaveAs
            // 
            this.AcceptButton = this.bOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bCancel;
            this.ClientSize = new System.Drawing.Size(1421, 748);
            this.Controls.Add(this.removeNewlines);
            this.Controls.Add(this.padStrings);
            this.Controls.Add(this.stripNonAscii);
            this.Controls.Add(this.formatLabel);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.columnTemplate);
            this.Controls.Add(this.rowTemplate);
            this.Controls.Add(this.removeTime);
            this.Controls.Add(this.printNulls);
            this.Controls.Add(this.singleQuoteStrings);
            this.Controls.Add(this.doubleQuoteStrings);
            this.Controls.Add(this.colSeparator);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.columnsList);
            this.Controls.Add(this.saveSelected);
            this.Controls.Add(this.rowSeparator);
            this.Controls.Add(this.tableLabel);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tableCombo);
            this.Controls.Add(this.moreOptionsButton);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.columnTitles);
            this.Controls.Add(this.escapeWhenNecessary);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.fileNameTextbox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.directoryTextbox);
            this.Controls.Add(this.browseButton);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.fileTypeCombo);
            this.Controls.Add(this.bCancel);
            this.Controls.Add(this.bOK);
            this.Controls.Add(this.formatCombo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "DlgSaveAs";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Save Rows";
            this.Load += new System.EventHandler(this.DlgSaveAs_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.CheckBox columnTitles;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox fileTypeCombo;
        private System.Windows.Forms.Button bCancel;
        private System.Windows.Forms.Button bOK;
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.TextBox directoryTextbox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox fileNameTextbox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox colSeparator;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox rowSeparator;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ListBox columnsList;
        private System.Windows.Forms.Button moreOptionsButton;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.CheckBox escapeWhenNecessary;
        private System.Windows.Forms.Label tableLabel;
        private System.Windows.Forms.ComboBox tableCombo;
        private System.Windows.Forms.CheckBox saveSelected;
        private System.Windows.Forms.CheckBox doubleQuoteStrings;
        private System.Windows.Forms.CheckBox singleQuoteStrings;
        private System.Windows.Forms.CheckBox printNulls;
        private System.Windows.Forms.CheckBox removeTime;
        private System.Windows.Forms.TextBox rowTemplate;
        private System.Windows.Forms.TextBox columnTemplate;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label formatLabel;
        private System.Windows.Forms.ComboBox formatCombo;
        private System.Windows.Forms.CheckBox stripNonAscii;
        private System.Windows.Forms.CheckBox padStrings;
        private System.Windows.Forms.CheckBox removeNewlines;
    }
}