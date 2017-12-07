namespace fp.sqratch
{
    partial class AutoSuggest
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.theList = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // theList
            // 
            this.theList.BackColor = System.Drawing.Color.WhiteSmoke;
            this.theList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.theList.FormattingEnabled = true;
            this.theList.ItemHeight = 31;
            this.theList.Location = new System.Drawing.Point(-2, -5);
            this.theList.Name = "theList";
            this.theList.Size = new System.Drawing.Size(442, 403);
            this.theList.TabIndex = 1;
            this.theList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.suggestList_MouseDoubleClick);
            this.theList.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.suggestList_PreviewKeyDown);
            // 
            // AutoSuggest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.theList);
            this.Name = "AutoSuggest";
            this.Size = new System.Drawing.Size(438, 392);
            this.Load += new System.EventHandler(this.AutoSuggest_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox theList;
    }
}
