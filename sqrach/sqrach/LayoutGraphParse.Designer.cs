namespace fp.sqratch
{
    partial class LayoutGraphParse
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
            this.showLiterals = new System.Windows.Forms.CheckBox();
            this.highlightLabel = new System.Windows.Forms.Label();
            this.showExpressions = new System.Windows.Forms.CheckBox();
            this.showLabel = new System.Windows.Forms.Label();
            this.layoutLabel = new System.Windows.Forms.Label();
            this.showOperators = new System.Windows.Forms.CheckBox();
            this.highlightCombo = new System.Windows.Forms.ComboBox();
            this.layoutCombo = new System.Windows.Forms.ComboBox();
            this.showTables = new System.Windows.Forms.CheckBox();
            this.showIdentifiers = new System.Windows.Forms.CheckBox();
            this.showKeywords = new System.Windows.Forms.CheckBox();
            this.showColumns = new System.Windows.Forms.CheckBox();
            this.focusTextbox = new System.Windows.Forms.TextBox();
            this.showPanel = new System.Windows.Forms.Panel();
            this.optionsPanel.SuspendLayout();
            this.showPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // optionsPanel
            // 
            this.optionsPanel.Controls.Add(this.showPanel);
            this.optionsPanel.Controls.Add(this.highlightLabel);
            this.optionsPanel.Controls.Add(this.layoutLabel);
            this.optionsPanel.Controls.Add(this.highlightCombo);
            this.optionsPanel.Controls.Add(this.layoutCombo);
            this.optionsPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.optionsPanel.Location = new System.Drawing.Point(0, 0);
            this.optionsPanel.Name = "optionsPanel";
            this.optionsPanel.Size = new System.Drawing.Size(2226, 77);
            this.optionsPanel.TabIndex = 1;
            // 
            // showLiterals
            // 
            this.showLiterals.Appearance = System.Windows.Forms.Appearance.Button;
            this.showLiterals.AutoSize = true;
            this.showLiterals.Location = new System.Drawing.Point(867, 9);
            this.showLiterals.Name = "showLiterals";
            this.showLiterals.Size = new System.Drawing.Size(118, 42);
            this.showLiterals.TabIndex = 8;
            this.showLiterals.Text = "Literals";
            this.showLiterals.UseVisualStyleBackColor = true;
            this.showLiterals.CheckedChanged += new System.EventHandler(this.OnShowCheckboxChanged);
            // 
            // highlightLabel
            // 
            this.highlightLabel.AutoSize = true;
            this.highlightLabel.Location = new System.Drawing.Point(339, 22);
            this.highlightLabel.Name = "highlightLabel";
            this.highlightLabel.Size = new System.Drawing.Size(128, 32);
            this.highlightLabel.TabIndex = 3;
            this.highlightLabel.Text = "Highlight";
            // 
            // showExpressions
            // 
            this.showExpressions.Appearance = System.Windows.Forms.Appearance.Button;
            this.showExpressions.AutoSize = true;
            this.showExpressions.Location = new System.Drawing.Point(526, 9);
            this.showExpressions.Name = "showExpressions";
            this.showExpressions.Size = new System.Drawing.Size(180, 42);
            this.showExpressions.TabIndex = 7;
            this.showExpressions.Text = "Expressions";
            this.showExpressions.UseVisualStyleBackColor = true;
            this.showExpressions.CheckedChanged += new System.EventHandler(this.OnShowCheckboxChanged);
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
            // showOperators
            // 
            this.showOperators.Appearance = System.Windows.Forms.Appearance.Button;
            this.showOperators.AutoSize = true;
            this.showOperators.Location = new System.Drawing.Point(991, 9);
            this.showOperators.Name = "showOperators";
            this.showOperators.Size = new System.Drawing.Size(151, 42);
            this.showOperators.TabIndex = 6;
            this.showOperators.Text = "Operators";
            this.showOperators.UseVisualStyleBackColor = true;
            this.showOperators.CheckedChanged += new System.EventHandler(this.OnShowCheckboxChanged);
            // 
            // highlightCombo
            // 
            this.highlightCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.highlightCombo.FormattingEnabled = true;
            this.highlightCombo.Location = new System.Drawing.Point(484, 19);
            this.highlightCombo.Name = "highlightCombo";
            this.highlightCombo.Size = new System.Drawing.Size(223, 39);
            this.highlightCombo.TabIndex = 4;
            this.highlightCombo.SelectedIndexChanged += new System.EventHandler(this.highlightCombo_SelectedIndexChanged);
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
            // showTables
            // 
            this.showTables.Appearance = System.Windows.Forms.Appearance.Button;
            this.showTables.AutoSize = true;
            this.showTables.Location = new System.Drawing.Point(111, 9);
            this.showTables.Name = "showTables";
            this.showTables.Size = new System.Drawing.Size(111, 42);
            this.showTables.TabIndex = 4;
            this.showTables.Text = "Tables";
            this.showTables.UseVisualStyleBackColor = true;
            this.showTables.CheckedChanged += new System.EventHandler(this.OnShowCheckboxChanged);
            // 
            // showIdentifiers
            // 
            this.showIdentifiers.Appearance = System.Windows.Forms.Appearance.Button;
            this.showIdentifiers.AutoSize = true;
            this.showIdentifiers.Location = new System.Drawing.Point(712, 9);
            this.showIdentifiers.Name = "showIdentifiers";
            this.showIdentifiers.Size = new System.Drawing.Size(149, 42);
            this.showIdentifiers.TabIndex = 2;
            this.showIdentifiers.Text = "Identifiers";
            this.showIdentifiers.UseVisualStyleBackColor = true;
            this.showIdentifiers.CheckedChanged += new System.EventHandler(this.OnShowCheckboxChanged);
            // 
            // showKeywords
            // 
            this.showKeywords.Appearance = System.Windows.Forms.Appearance.Button;
            this.showKeywords.AutoSize = true;
            this.showKeywords.Location = new System.Drawing.Point(371, 9);
            this.showKeywords.Name = "showKeywords";
            this.showKeywords.Size = new System.Drawing.Size(149, 42);
            this.showKeywords.TabIndex = 5;
            this.showKeywords.Text = "Keywords";
            this.showKeywords.UseVisualStyleBackColor = true;
            this.showKeywords.CheckedChanged += new System.EventHandler(this.OnShowCheckboxChanged);
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
            // focusTextbox
            // 
            this.focusTextbox.Location = new System.Drawing.Point(1194, 11);
            this.focusTextbox.Name = "focusTextbox";
            this.focusTextbox.Size = new System.Drawing.Size(40, 38);
            this.focusTextbox.TabIndex = 9;
            // 
            // showPanel
            // 
            this.showPanel.Controls.Add(this.focusTextbox);
            this.showPanel.Controls.Add(this.showTables);
            this.showPanel.Controls.Add(this.showLiterals);
            this.showPanel.Controls.Add(this.showColumns);
            this.showPanel.Controls.Add(this.showKeywords);
            this.showPanel.Controls.Add(this.showExpressions);
            this.showPanel.Controls.Add(this.showIdentifiers);
            this.showPanel.Controls.Add(this.showLabel);
            this.showPanel.Controls.Add(this.showOperators);
            this.showPanel.Location = new System.Drawing.Point(755, 10);
            this.showPanel.Name = "showPanel";
            this.showPanel.Size = new System.Drawing.Size(1282, 62);
            this.showPanel.TabIndex = 2;
            // 
            // ParseGraph
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2226, 996);
            this.Controls.Add(this.optionsPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "ParseGraph";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Query Layout";
            this.optionsPanel.ResumeLayout(false);
            this.optionsPanel.PerformLayout();
            this.showPanel.ResumeLayout(false);
            this.showPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel optionsPanel;
        private System.Windows.Forms.CheckBox showKeywords;
        private System.Windows.Forms.Label showLabel;
        private System.Windows.Forms.CheckBox showIdentifiers;
        private System.Windows.Forms.CheckBox showColumns;
        private System.Windows.Forms.CheckBox showTables;
        private System.Windows.Forms.Label highlightLabel;
        private System.Windows.Forms.Label layoutLabel;
        private System.Windows.Forms.ComboBox highlightCombo;
        private System.Windows.Forms.ComboBox layoutCombo;
        private System.Windows.Forms.CheckBox showOperators;
        private System.Windows.Forms.CheckBox showExpressions;
        private System.Windows.Forms.CheckBox showLiterals;
        private System.Windows.Forms.TextBox focusTextbox;
        private System.Windows.Forms.Panel showPanel;
    }
}