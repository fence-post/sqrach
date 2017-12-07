namespace fp.sqratch
{
    partial class DlgColors
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
            this.foreColors = new System.Windows.Forms.ListBox();
            this.foreColor = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.backColor = new System.Windows.Forms.TextBox();
            this.backColors = new System.Windows.Forms.ListBox();
            this.test = new System.Windows.Forms.TextBox();
            this.images = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // foreColors
            // 
            this.foreColors.FormattingEnabled = true;
            this.foreColors.ItemHeight = 31;
            this.foreColors.Location = new System.Drawing.Point(892, 203);
            this.foreColors.Name = "foreColors";
            this.foreColors.Size = new System.Drawing.Size(400, 283);
            this.foreColors.TabIndex = 0;
            this.foreColors.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.colorList_DrawItem);
            this.foreColors.SelectedIndexChanged += new System.EventHandler(this.colorList_SelectedIndexChanged);
            // 
            // foreColor
            // 
            this.foreColor.Location = new System.Drawing.Point(892, 157);
            this.foreColor.Name = "foreColor";
            this.foreColor.ReadOnly = true;
            this.foreColor.Size = new System.Drawing.Size(400, 38);
            this.foreColor.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(886, 122);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(135, 32);
            this.label1.TabIndex = 2;
            this.label1.Text = "Forecolor";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(457, 124);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(140, 32);
            this.label2.TabIndex = 4;
            this.label2.Text = "Backcolor";
            // 
            // backColor
            // 
            this.backColor.Location = new System.Drawing.Point(463, 159);
            this.backColor.Name = "backColor";
            this.backColor.ReadOnly = true;
            this.backColor.Size = new System.Drawing.Size(400, 38);
            this.backColor.TabIndex = 3;
            // 
            // backColors
            // 
            this.backColors.FormattingEnabled = true;
            this.backColors.ItemHeight = 31;
            this.backColors.Location = new System.Drawing.Point(463, 203);
            this.backColors.Name = "backColors";
            this.backColors.Size = new System.Drawing.Size(400, 283);
            this.backColors.TabIndex = 14;
            this.backColors.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.backColors_DrawItem);
            this.backColors.SelectedIndexChanged += new System.EventHandler(this.backColors_SelectedIndexChanged);
            // 
            // test
            // 
            this.test.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.test.Location = new System.Drawing.Point(463, 43);
            this.test.Name = "test";
            this.test.ReadOnly = true;
            this.test.Size = new System.Drawing.Size(290, 31);
            this.test.TabIndex = 15;
            this.test.Text = "Abc Def Ghi 12345";
            // 
            // images
            // 
            this.images.FormattingEnabled = true;
            this.images.ItemHeight = 31;
            this.images.Location = new System.Drawing.Point(12, 159);
            this.images.Name = "images";
            this.images.Size = new System.Drawing.Size(198, 345);
            this.images.TabIndex = 16;
            this.images.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.images_DrawItem);
            // 
            // DlgColors
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1396, 774);
            this.Controls.Add(this.images);
            this.Controls.Add(this.test);
            this.Controls.Add(this.backColors);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.backColor);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.foreColor);
            this.Controls.Add(this.foreColors);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "DlgColors";
            this.Text = "DlgColors";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox foreColors;
        private System.Windows.Forms.TextBox foreColor;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox backColor;
        private System.Windows.Forms.ListBox backColors;
        private System.Windows.Forms.TextBox test;
        private System.Windows.Forms.ListBox images;
    }
}