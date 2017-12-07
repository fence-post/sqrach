using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using fp.lib;
using fp.lib.forms;

namespace fp.sqratch
{
    public partial class DlgConnection : Form
    {
        public int databaseIdResult = 0;

        public DlgConnection()
        {
            InitializeComponent();
            typeCombo.Items.AddRange(new[] { "MySql", "Sql Server", "SQLite" });
            typeCombo.SelectString("MySql");
            Font = SystemFonts.MessageBoxFont;
            UpdateList();
        }

        List<KeyValuePair<string, int>> connections;

        private void UpdateList()
        {
            connectionsList.DataSource = null;
            connectionsList.Items.Clear();
            int selectedIndex;
            connections = S.GetConnections(out selectedIndex);
            connectionsList.SetDataSource(connections, "Key", "Value");
            if (selectedIndex >= 0)
                connectionsList.SelectedIndex = selectedIndex;
            ValidateInput();
        }

        private void bOK_Click(object sender, EventArgs e)
        {
            databaseIdResult = Settings.SaveConnection(typeCombo.SelectedItem.ToString(), host.Text, database.Text, userId.Text, password.Text, savePasswordCheckbox.Checked, bOpenByDefault.Checked);
            DialogResult = DialogResult.OK;
            Close();

            /*
        if (Settings.LoadConnectionSettings(fileName))
        {
            Settings.initSettings.password = password.Text; // initialize this here if user did not save password 
            DialogResult = DialogResult.OK;
            Close();
        }
        */
        }

        private void bCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void bNew_Click(object sender, EventArgs e)
        {
            connectionsList.SelectedIndex = -1;
            host.Text = database.Text = userId.Text = password.Text = "";
            host.Enabled = database.Enabled = typeCombo.Enabled = true;
            savePasswordCheckbox.Checked = false;
            bOpenByDefault.Checked = false;
            ValidateInput();
        }

        private void connectionsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            bDelete.Enabled = connectionsList.SelectedIndex >= 0;
            if (connectionsList.SelectedIndex >= 0)
            {
                host.Enabled = database.Enabled = typeCombo.Enabled = false;
                LoadConnection(Convert.ToInt32(connectionsList.SelectedValue));
            }
        }

        private void LoadConnection(int id)
        {
            ConnectSettings c = S.GetConnection(id);
            typeCombo.SelectString(c.type);
            PopulateField("host", c.host);
            PopulateField("database", c.database);
            PopulateField("userId", c.user);
            PopulateField("password", c.password);
            savePasswordCheckbox.Checked = password.Text != "";
            ValidateInput();
        }

        private void PopulateField(string nam, string val)
        {
            connectionGroupBox.Controls[nam].Text = val;
        }

        private void host_TextChanged(object sender, EventArgs e)
        {
            ValidateInput();
        }

        string databaseType { get { return typeCombo.SelectedItem == null ? null : typeCombo.SelectedItem.ToString();  } }

        private void ValidateInput()
        {
            if (databaseType == "SQLite")
            {
                host.Text = "localhost";
                browseButton.Visible = true;
                host.Visible = hostLabel.Visible = false;
                userId.Text = password.Text = "";
                userId.Enabled = password.Enabled = false;
                userIdLabel.Enabled = passwordLabel.Enabled = false;
                savePasswordCheckbox.Checked = bOpenByDefault.Checked = false;
                bOK.Enabled = database.Text != "";
                savePasswordCheckbox.Enabled = bOpenByDefault.Enabled = false;

            }
            else
            {
                browseButton.Visible = false;
                host.Visible = hostLabel.Visible = true;
                userId.Enabled = password.Enabled = true;
                userIdLabel.Enabled = passwordLabel.Enabled = true;
                bOK.Enabled = (host.Text != "" && database.Text != "" && userId.Text != "" && password.Text != "");
                savePasswordCheckbox.Enabled = password.Text != "";
                if (savePasswordCheckbox.Enabled)
                {
                    bOpenByDefault.Enabled = true;
                }
                else
                {
                    savePasswordCheckbox.Checked = false;
                    bOpenByDefault.Checked = false;
                    bOpenByDefault.Enabled = false;
                }
            }
        }

        private void bDelete_Click(object sender, EventArgs e)
        {
            if (this.YesNoBox("Are you sure you want to delete?"))
            {
                S.DeleteConnection(Convert.ToInt32(connectionsList.SelectedValue));
                UpdateList();
                PopulateField("host", "");
                PopulateField("database", "");
                PopulateField("userId", "");
                PopulateField("password", "");
                savePasswordCheckbox.Checked = password.Text != "";
                ValidateInput();
            }
        }

        private void typeCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidateInput();
            /*
            if (databaseType == "SQLite")
            {
                host.Text = "localhost";
                browseButton.Visible = true;
                host.Visible = hostLabel.Visible = false;
                userId.Text = password.Text = "";
                userId.Enabled = password.Enabled = false;
                savePasswordCheckbox.Checked = bOpenByDefault.Checked = false;
            }
            else
            {
                browseButton.Visible = false;
                host.Visible = hostLabel.Visible = true;
                userId.Enabled = password.Enabled = true;
            }
            */
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            openFileDialog.SetExtensions("SQLite Databases|*.sqlite", ".sqlite");
            openFileDialog.SupportMultiDottedExtensions = true;
            openFileDialog.InitialDirectory = S.Get("dlgConnectionBrowse", Directory.GetCurrentDirectory());
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                S.Set("dlgConnectionBrowse", T.GetPathFromFilePath(openFileDialog.FileName));
                database.Text = openFileDialog.FileName;
                ValidateInput();
            }
        }
    }
}
