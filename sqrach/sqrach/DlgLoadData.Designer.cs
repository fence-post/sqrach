namespace fp.sqratch
{
    partial class DlgLoadData
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
            this.columnsList = new System.Windows.Forms.ListBox();
            this.label6 = new System.Windows.Forms.Label();
            this.moreOptionsButton = new System.Windows.Forms.Button();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.label2 = new System.Windows.Forms.Label();
            this.fileNameTextbox = new System.Windows.Forms.TextBox();
            this.browseButton = new System.Windows.Forms.Button();
            this.fileTypeCombo = new System.Windows.Forms.ComboBox();
            this.bCancel = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.includeAliases = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // columnsList
            // 
            this.columnsList.FormattingEnabled = true;
            this.columnsList.IntegralHeight = false;
            this.columnsList.ItemHeight = 31;
            this.columnsList.Location = new System.Drawing.Point(31, 331);
            this.columnsList.Name = "columnsList";
            this.columnsList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.columnsList.Size = new System.Drawing.Size(395, 274);
            this.columnsList.TabIndex = 60;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(26, 296);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(127, 32);
            this.label6.TabIndex = 61;
            this.label6.Text = "Columns";
            // 
            // moreOptionsButton
            // 
            this.moreOptionsButton.Location = new System.Drawing.Point(473, 331);
            this.moreOptionsButton.Name = "moreOptionsButton";
            this.moreOptionsButton.Size = new System.Drawing.Size(172, 55);
            this.moreOptionsButton.TabIndex = 79;
            this.moreOptionsButton.Text = "Test";
            this.moreOptionsButton.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(159, 32);
            this.label2.TabIndex = 78;
            this.label2.Text = "Source File";
            // 
            // fileNameTextbox
            // 
            this.fileNameTextbox.Location = new System.Drawing.Point(31, 69);
            this.fileNameTextbox.Name = "fileNameTextbox";
            this.fileNameTextbox.Size = new System.Drawing.Size(614, 38);
            this.fileNameTextbox.TabIndex = 77;
            // 
            // browseButton
            // 
            this.browseButton.Location = new System.Drawing.Point(474, 120);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(172, 55);
            this.browseButton.TabIndex = 74;
            this.browseButton.Text = "Browse...";
            this.browseButton.UseVisualStyleBackColor = true;
            // 
            // fileTypeCombo
            // 
            this.fileTypeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fileTypeCombo.FormattingEnabled = true;
            this.fileTypeCombo.Items.AddRange(new object[] {
            "CSV",
            "TSV",
            "Custom"});
            this.fileTypeCombo.Location = new System.Drawing.Point(31, 243);
            this.fileTypeCombo.Name = "fileTypeCombo";
            this.fileTypeCombo.Size = new System.Drawing.Size(395, 39);
            this.fileTypeCombo.TabIndex = 72;
            // 
            // bCancel
            // 
            this.bCancel.AccessibleDescription = "Cancel";
            this.bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bCancel.Location = new System.Drawing.Point(474, 549);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new System.Drawing.Size(172, 55);
            this.bCancel.TabIndex = 71;
            this.bCancel.Text = "Cancel";
            this.bCancel.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(26, 208);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(141, 32);
            this.label5.TabIndex = 73;
            this.label5.Text = "Into Table";
            // 
            // includeAliases
            // 
            this.includeAliases.AutoSize = true;
            this.includeAliases.Location = new System.Drawing.Point(32, 130);
            this.includeAliases.Name = "includeAliases";
            this.includeAliases.Size = new System.Drawing.Size(233, 36);
            this.includeAliases.TabIndex = 69;
            this.includeAliases.Text = "Skip First Line";
            this.includeAliases.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(474, 488);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(172, 55);
            this.button1.TabIndex = 80;
            this.button1.Text = "Run!";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(358, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(273, 32);
            this.label1.TabIndex = 81;
            this.label1.Text = "30 rows, 10 columns";
            // 
            // DlgLoadData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bCancel;
            this.ClientSize = new System.Drawing.Size(688, 648);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.columnsList);
            this.Controls.Add(this.moreOptionsButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.fileNameTextbox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.browseButton);
            this.Controls.Add(this.fileTypeCombo);
            this.Controls.Add(this.bCancel);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.includeAliases);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "DlgLoadData";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Load Data";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ListBox columnsList;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button moreOptionsButton;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox fileNameTextbox;
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.ComboBox fileTypeCombo;
        private System.Windows.Forms.Button bCancel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox includeAliases;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
    }
}