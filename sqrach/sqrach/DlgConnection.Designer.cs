namespace fp.sqratch
{
    partial class DlgConnection
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
            this.passwordLabel = new System.Windows.Forms.Label();
            this.password = new System.Windows.Forms.TextBox();
            this.userIdLabel = new System.Windows.Forms.Label();
            this.userId = new System.Windows.Forms.TextBox();
            this.hostLabel = new System.Windows.Forms.Label();
            this.host = new System.Windows.Forms.TextBox();
            this.bCancel = new System.Windows.Forms.Button();
            this.bOK = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.database = new System.Windows.Forms.TextBox();
            this.connectionsList = new System.Windows.Forms.ListBox();
            this.connectionGroupBox = new System.Windows.Forms.GroupBox();
            this.browseButton = new System.Windows.Forms.Button();
            this.typeCombo = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.bOpenByDefault = new System.Windows.Forms.CheckBox();
            this.savePasswordCheckbox = new System.Windows.Forms.CheckBox();
            this.bNew = new System.Windows.Forms.Button();
            this.bDelete = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.connectionGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // passwordLabel
            // 
            this.passwordLabel.AutoSize = true;
            this.passwordLabel.Location = new System.Drawing.Point(349, 321);
            this.passwordLabel.Name = "passwordLabel";
            this.passwordLabel.Size = new System.Drawing.Size(139, 32);
            this.passwordLabel.TabIndex = 15;
            this.passwordLabel.Text = "Password";
            // 
            // password
            // 
            this.password.Location = new System.Drawing.Point(353, 358);
            this.password.Name = "password";
            this.password.PasswordChar = '*';
            this.password.Size = new System.Drawing.Size(266, 38);
            this.password.TabIndex = 3;
            this.password.TextChanged += new System.EventHandler(this.host_TextChanged);
            // 
            // userIdLabel
            // 
            this.userIdLabel.AutoSize = true;
            this.userIdLabel.Location = new System.Drawing.Point(36, 321);
            this.userIdLabel.Name = "userIdLabel";
            this.userIdLabel.Size = new System.Drawing.Size(97, 32);
            this.userIdLabel.TabIndex = 13;
            this.userIdLabel.Text = "UserId";
            // 
            // userId
            // 
            this.userId.Location = new System.Drawing.Point(40, 358);
            this.userId.Name = "userId";
            this.userId.Size = new System.Drawing.Size(266, 38);
            this.userId.TabIndex = 2;
            this.userId.TextChanged += new System.EventHandler(this.host_TextChanged);
            // 
            // hostLabel
            // 
            this.hostLabel.AutoSize = true;
            this.hostLabel.Location = new System.Drawing.Point(36, 131);
            this.hostLabel.Name = "hostLabel";
            this.hostLabel.Size = new System.Drawing.Size(73, 32);
            this.hostLabel.TabIndex = 11;
            this.hostLabel.Text = "Host";
            // 
            // host
            // 
            this.host.Location = new System.Drawing.Point(40, 166);
            this.host.Name = "host";
            this.host.Size = new System.Drawing.Size(579, 38);
            this.host.TabIndex = 0;
            this.host.TextChanged += new System.EventHandler(this.host_TextChanged);
            // 
            // bCancel
            // 
            this.bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bCancel.Location = new System.Drawing.Point(1157, 529);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new System.Drawing.Size(172, 55);
            this.bCancel.TabIndex = 5;
            this.bCancel.Text = "Close";
            this.bCancel.UseVisualStyleBackColor = true;
            this.bCancel.Click += new System.EventHandler(this.bCancel_Click);
            // 
            // bOK
            // 
            this.bOK.Enabled = false;
            this.bOK.Location = new System.Drawing.Point(979, 529);
            this.bOK.Name = "bOK";
            this.bOK.Size = new System.Drawing.Size(172, 55);
            this.bOK.TabIndex = 1;
            this.bOK.Text = "Connect";
            this.bOK.UseVisualStyleBackColor = true;
            this.bOK.Click += new System.EventHandler(this.bOK_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(36, 226);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(137, 32);
            this.label4.TabIndex = 17;
            this.label4.Text = "Database";
            // 
            // database
            // 
            this.database.Location = new System.Drawing.Point(40, 261);
            this.database.Name = "database";
            this.database.Size = new System.Drawing.Size(579, 38);
            this.database.TabIndex = 1;
            this.database.TextChanged += new System.EventHandler(this.host_TextChanged);
            // 
            // connectionsList
            // 
            this.connectionsList.FormattingEnabled = true;
            this.connectionsList.ItemHeight = 31;
            this.connectionsList.Location = new System.Drawing.Point(27, 28);
            this.connectionsList.Name = "connectionsList";
            this.connectionsList.Size = new System.Drawing.Size(612, 562);
            this.connectionsList.TabIndex = 2;
            this.connectionsList.SelectedIndexChanged += new System.EventHandler(this.connectionsList_SelectedIndexChanged);
            // 
            // connectionGroupBox
            // 
            this.connectionGroupBox.Controls.Add(this.browseButton);
            this.connectionGroupBox.Controls.Add(this.typeCombo);
            this.connectionGroupBox.Controls.Add(this.label5);
            this.connectionGroupBox.Controls.Add(this.bOpenByDefault);
            this.connectionGroupBox.Controls.Add(this.savePasswordCheckbox);
            this.connectionGroupBox.Controls.Add(this.hostLabel);
            this.connectionGroupBox.Controls.Add(this.host);
            this.connectionGroupBox.Controls.Add(this.label4);
            this.connectionGroupBox.Controls.Add(this.userId);
            this.connectionGroupBox.Controls.Add(this.database);
            this.connectionGroupBox.Controls.Add(this.userIdLabel);
            this.connectionGroupBox.Controls.Add(this.passwordLabel);
            this.connectionGroupBox.Controls.Add(this.password);
            this.connectionGroupBox.Location = new System.Drawing.Point(663, 12);
            this.connectionGroupBox.Name = "connectionGroupBox";
            this.connectionGroupBox.Size = new System.Drawing.Size(666, 486);
            this.connectionGroupBox.TabIndex = 0;
            this.connectionGroupBox.TabStop = false;
            // 
            // browseButton
            // 
            this.browseButton.Location = new System.Drawing.Point(458, 186);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(161, 55);
            this.browseButton.TabIndex = 20;
            this.browseButton.Text = "Browse...";
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // typeCombo
            // 
            this.typeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.typeCombo.FormattingEnabled = true;
            this.typeCombo.Location = new System.Drawing.Point(42, 75);
            this.typeCombo.Name = "typeCombo";
            this.typeCombo.Size = new System.Drawing.Size(264, 39);
            this.typeCombo.TabIndex = 19;
            this.typeCombo.SelectedIndexChanged += new System.EventHandler(this.typeCombo_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(36, 40);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(78, 32);
            this.label5.TabIndex = 13;
            this.label5.Text = "Type";
            // 
            // bOpenByDefault
            // 
            this.bOpenByDefault.AutoSize = true;
            this.bOpenByDefault.Location = new System.Drawing.Point(40, 419);
            this.bOpenByDefault.Name = "bOpenByDefault";
            this.bOpenByDefault.Size = new System.Drawing.Size(258, 36);
            this.bOpenByDefault.TabIndex = 18;
            this.bOpenByDefault.Text = "Open by Default";
            this.bOpenByDefault.UseVisualStyleBackColor = true;
            // 
            // savePasswordCheckbox
            // 
            this.savePasswordCheckbox.AutoSize = true;
            this.savePasswordCheckbox.Location = new System.Drawing.Point(353, 419);
            this.savePasswordCheckbox.Name = "savePasswordCheckbox";
            this.savePasswordCheckbox.Size = new System.Drawing.Size(249, 36);
            this.savePasswordCheckbox.TabIndex = 4;
            this.savePasswordCheckbox.Text = "Save Password";
            this.savePasswordCheckbox.UseVisualStyleBackColor = true;
            // 
            // bNew
            // 
            this.bNew.Location = new System.Drawing.Point(663, 529);
            this.bNew.Name = "bNew";
            this.bNew.Size = new System.Drawing.Size(125, 55);
            this.bNew.TabIndex = 3;
            this.bNew.Text = "New";
            this.bNew.UseVisualStyleBackColor = true;
            this.bNew.Click += new System.EventHandler(this.bNew_Click);
            // 
            // bDelete
            // 
            this.bDelete.Enabled = false;
            this.bDelete.Location = new System.Drawing.Point(794, 529);
            this.bDelete.Name = "bDelete";
            this.bDelete.Size = new System.Drawing.Size(125, 55);
            this.bDelete.TabIndex = 4;
            this.bDelete.Text = "Delete";
            this.bDelete.UseVisualStyleBackColor = true;
            this.bDelete.Click += new System.EventHandler(this.bDelete_Click);
            // 
            // DlgConnection
            // 
            this.AcceptButton = this.bOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bCancel;
            this.ClientSize = new System.Drawing.Size(1359, 620);
            this.Controls.Add(this.bDelete);
            this.Controls.Add(this.bNew);
            this.Controls.Add(this.connectionGroupBox);
            this.Controls.Add(this.connectionsList);
            this.Controls.Add(this.bCancel);
            this.Controls.Add(this.bOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DlgConnection";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Connect";
            this.connectionGroupBox.ResumeLayout(false);
            this.connectionGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label passwordLabel;
        private System.Windows.Forms.TextBox password;
        private System.Windows.Forms.Label userIdLabel;
        private System.Windows.Forms.TextBox userId;
        private System.Windows.Forms.Label hostLabel;
        private System.Windows.Forms.TextBox host;
        private System.Windows.Forms.Button bCancel;
        private System.Windows.Forms.Button bOK;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox database;
        private System.Windows.Forms.ListBox connectionsList;
        private System.Windows.Forms.GroupBox connectionGroupBox;
        private System.Windows.Forms.CheckBox savePasswordCheckbox;
        private System.Windows.Forms.Button bNew;
        private System.Windows.Forms.Button bDelete;
        private System.Windows.Forms.CheckBox bOpenByDefault;
        private System.Windows.Forms.ComboBox typeCombo;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
    }
}