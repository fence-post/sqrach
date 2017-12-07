namespace fp.sqratch
{
    partial class DlgOptions
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
            this.optionsPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.bOk = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // optionsPropertyGrid
            // 
            this.optionsPropertyGrid.LineColor = System.Drawing.SystemColors.ControlDark;
            this.optionsPropertyGrid.Location = new System.Drawing.Point(12, 12);
            this.optionsPropertyGrid.Name = "optionsPropertyGrid";
            this.optionsPropertyGrid.Size = new System.Drawing.Size(806, 561);
            this.optionsPropertyGrid.TabIndex = 2;
            // 
            // bOk
            // 
            this.bOk.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bOk.Location = new System.Drawing.Point(646, 596);
            this.bOk.Name = "bOk";
            this.bOk.Size = new System.Drawing.Size(172, 58);
            this.bOk.TabIndex = 3;
            this.bOk.Text = "Close";
            this.bOk.UseVisualStyleBackColor = true;
            this.bOk.Click += new System.EventHandler(this.bOk_Click);
            // 
            // DlgOptions
            // 
            this.AcceptButton = this.bOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bOk;
            this.ClientSize = new System.Drawing.Size(836, 678);
            this.Controls.Add(this.bOk);
            this.Controls.Add(this.optionsPropertyGrid);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "DlgOptions";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Options";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.PropertyGrid optionsPropertyGrid;
        private System.Windows.Forms.Button bOk;
    }
}