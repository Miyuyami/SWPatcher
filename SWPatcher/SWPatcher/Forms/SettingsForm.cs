using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SWPatcher.Helpers.GlobalVar;
using SWPatcher.Helpers;

namespace SWPatcher.Forms
{
    public partial class SettingsForm : Form
    {
        private string GameClientDirectory;

        public SettingsForm()
        {
            InitializeComponent();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            this.GameClientDirectory = Paths.GameRoot;
            this.textBoxGameDirectory.Text = this.GameClientDirectory;

            if ((this.Owner as MainForm).State == MainForm.States.Idle)
                this.textBoxGameDirectory.TextChanged += new EventHandler(EnableApplyButton);
            else
            {
                this.textBoxGameDirectory.Enabled = false;
                this.buttonChangeDirectory.Enabled = false;
            }
        }

        private void EnableApplyButton(object sender, EventArgs e)
        {
            this.buttonApply.Enabled = true;
        }

        private void buttonChangeDirectory_Click(object sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog
            {
                ShowNewFolderButton = false,
                Description = "Select your Soul Worker game client folder."
            })
            {
                DialogResult result = folderDialog.ShowDialog();

                if (result == DialogResult.OK)
                    if (Methods.IsSwPath(folderDialog.SelectedPath))
                        if (Methods.IsValidSwPatcherPath(folderDialog.SelectedPath))
                            this.textBoxGameDirectory.Text = folderDialog.SelectedPath;
                        else
                            MsgBox.Error("The program is in the same or in a sub folder as your game client.\nThis will cause malfunctions or data corruption on your game client.\nPlease move the patcher in another location.");
                    else
                        MsgBox.Error("The selected folder is not a Soul Worker game client folder.");
            }
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            if (this.buttonApply.Enabled)
                this.ApplyChanges();

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            this.ApplyChanges();
        }

        private void ApplyChanges()
        {
            if (Methods.IsSwPath(this.textBoxGameDirectory.Text))
            {
                Paths.GameRoot = this.textBoxGameDirectory.Text;
                this.GameClientDirectory = this.textBoxGameDirectory.Text;
            }
            else
            {
                MsgBox.Error("The selected folder is not a Soul Worker game client folder.");
                this.textBoxGameDirectory.Text = this.GameClientDirectory;
            }

            this.buttonApply.Enabled = false;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
