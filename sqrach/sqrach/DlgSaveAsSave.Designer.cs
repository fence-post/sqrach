namespace fp.sqratch
{
    partial class DlgSaveAsSave
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
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.line1 = new System.Windows.Forms.Label();
            this.bCancel = new System.Windows.Forms.Button();
            this.line2 = new System.Windows.Forms.Label();
            this.fileLocation = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(27, 77);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(642, 33);
            this.progressBar1.TabIndex = 0;
            // 
            // line1
            // 
            this.line1.AutoSize = true;
            this.line1.Location = new System.Drawing.Point(21, 27);
            this.line1.Name = "line1";
            this.line1.Size = new System.Drawing.Size(93, 32);
            this.line1.TabIndex = 1;
            this.line1.Text = "label1";
            // 
            // bCancel
            // 
            this.bCancel.AccessibleDescription = "Cancel";
            this.bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bCancel.Location = new System.Drawing.Point(497, 131);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new System.Drawing.Size(172, 55);
            this.bCancel.TabIndex = 42;
            this.bCancel.Text = "Cancel";
            this.bCancel.UseVisualStyleBackColor = true;
            this.bCancel.Click += new System.EventHandler(this.bCancel_Click);
            // 
            // line2
            // 
            this.line2.AutoSize = true;
            this.line2.Location = new System.Drawing.Point(21, 77);
            this.line2.Name = "line2";
            this.line2.Size = new System.Drawing.Size(93, 32);
            this.line2.TabIndex = 44;
            this.line2.Text = "label1";
            this.line2.Visible = false;
            // 
            // fileLocation
            // 
            this.fileLocation.Location = new System.Drawing.Point(27, 131);
            this.fileLocation.Name = "fileLocation";
            this.fileLocation.Size = new System.Drawing.Size(352, 55);
            this.fileLocation.TabIndex = 45;
            this.fileLocation.Text = "Open File Location";
            this.fileLocation.UseVisualStyleBackColor = true;
            this.fileLocation.Visible = false;
            this.fileLocation.Click += new System.EventHandler(this.fileLocation_Click);
            // 
            // DlgSaveAsSave
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bCancel;
            this.ClientSize = new System.Drawing.Size(712, 250);
            this.ControlBox = false;
            this.Controls.Add(this.fileLocation);
            this.Controls.Add(this.line2);
            this.Controls.Add(this.bCancel);
            this.Controls.Add(this.line1);
            this.Controls.Add(this.progressBar1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "DlgSaveAsSave";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Saving";
            this.Shown += new System.EventHandler(this.DlgSaveAsSave_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label line1;
        private System.Windows.Forms.Button bCancel;
        private System.Windows.Forms.Label line2;
        private System.Windows.Forms.Button fileLocation;
    }
}