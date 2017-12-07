namespace fp.sqratch
{
    partial class LayoutGraphTable
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
            this.optionsPanel = new System.Windows.Forms.Panel();
            this.showPanel = new System.Windows.Forms.Panel();
            this.showExplicit = new System.Windows.Forms.CheckBox();
            this.showColumns = new System.Windows.Forms.CheckBox();
            this.showInferred = new System.Windows.Forms.CheckBox();
            this.showLabel = new System.Windows.Forms.Label();
            this.layoutLabel = new System.Windows.Forms.Label();
            this.layoutCombo = new System.Windows.Forms.ComboBox();
            this.optionsPanel.SuspendLayout();
            this.showPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // optionsPanel
            // 
            this.optionsPanel.Controls.Add(this.showPanel);
            this.optionsPanel.Controls.Add(this.layoutLabel);
            this.optionsPanel.Controls.Add(this.layoutCombo);
            this.optionsPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.optionsPanel.Location = new System.Drawing.Point(0, 0);
            this.optionsPanel.Name = "optionsPanel";
            this.optionsPanel.Size = new System.Drawing.Size(2226, 77);
            this.optionsPanel.TabIndex = 1;
            // 
            // showPanel
            // 
            this.showPanel.Controls.Add(this.showExplicit);
            this.showPanel.Controls.Add(this.showColumns);
            this.showPanel.Controls.Add(this.showInferred);
            this.showPanel.Controls.Add(this.showLabel);
            this.showPanel.Location = new System.Drawing.Point(342, 8);
            this.showPanel.Name = "showPanel";
            this.showPanel.Size = new System.Drawing.Size(1282, 62);
            this.showPanel.TabIndex = 2;
            // 
            // showExplicit
            // 
            this.showExplicit.Appearance = System.Windows.Forms.Appearance.Button;
            this.showExplicit.AutoSize = true;
            this.showExplicit.Location = new System.Drawing.Point(111, 9);
            this.showExplicit.Name = "showExplicit";
            this.showExplicit.Size = new System.Drawing.Size(297, 42);
            this.showExplicit.TabIndex = 4;
            this.showExplicit.Text = "Explicit Relationships";
            this.showExplicit.UseVisualStyleBackColor = true;
            this.showExplicit.CheckedChanged += new System.EventHandler(this.OnShowCheckboxChanged);
            // 
            // showColumns
            // 
            this.showColumns.Appearance = System.Windows.Forms.Appearance.Button;
            this.showColumns.AutoSize = true;
            this.showColumns.Location = new System.Drawing.Point(228, 9);
            this.showColumns.Name = "showColumns";
            this.showColumns.Size = new System.Drawing.Size(137, 42);
            this.showColumns.TabIndex = 3;
            this.showColumns.Text = "Columns";
            this.showColumns.UseVisualStyleBackColor = true;
            this.showColumns.CheckedChanged += new System.EventHandler(this.OnShowCheckboxChanged);
            // 
            // showInferred
            // 
            this.showInferred.Appearance = System.Windows.Forms.Appearance.Button;
            this.showInferred.AutoSize = true;
            this.showInferred.Location = new System.Drawing.Point(414, 9);
            this.showInferred.Name = "showInferred";
            this.showInferred.Size = new System.Drawing.Size(302, 42);
            this.showInferred.TabIndex = 7;
            this.showInferred.Text = "Inferred Relationships";
            this.showInferred.UseVisualStyleBackColor = true;
            this.showInferred.CheckedChanged += new System.EventHandler(this.OnShowCheckboxChanged);
            // 
            // showLabel
            // 
            this.showLabel.AutoSize = true;
            this.showLabel.Location = new System.Drawing.Point(14, 14);
            this.showLabel.Name = "showLabel";
            this.showLabel.Size = new System.Drawing.Size(86, 32);
            this.showLabel.TabIndex = 4;
            this.showLabel.Text = "Show";
            // 
            // layoutLabel
            // 
            this.layoutLabel.AutoSize = true;
            this.layoutLabel.Location = new System.Drawing.Point(9, 22);
            this.layoutLabel.Name = "layoutLabel";
            this.layoutLabel.Size = new System.Drawing.Size(101, 32);
            this.layoutLabel.TabIndex = 2;
            this.layoutLabel.Text = "Layout";
            // 
            // layoutCombo
            // 
            this.layoutCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.layoutCombo.FormattingEnabled = true;
            this.layoutCombo.Location = new System.Drawing.Point(127, 19);
            this.layoutCombo.Name = "layoutCombo";
            this.layoutCombo.Size = new System.Drawing.Size(197, 39);
            this.layoutCombo.TabIndex = 2;
            this.layoutCombo.SelectedIndexChanged += new System.EventHandler(this.layoutCombo_SelectedIndexChanged);
            // 
            // TableGraph
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2226, 996);
            this.Controls.Add(this.optionsPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "TableGraph";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Table Relationships";
            this.optionsPanel.ResumeLayout(false);
            this.optionsPanel.PerformLayout();
            this.showPanel.ResumeLayout(false);
            this.showPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel optionsPanel;
        private System.Windows.Forms.Label showLabel;
        private System.Windows.Forms.CheckBox showColumns;
        private System.Windows.Forms.CheckBox showExplicit;
        private System.Windows.Forms.Label layoutLabel;
        private System.Windows.Forms.ComboBox layoutCombo;
        private System.Windows.Forms.CheckBox showInferred;
        private System.Windows.Forms.Panel showPanel;
    }
}