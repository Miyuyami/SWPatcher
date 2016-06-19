using System;
using System.Windows.Forms;
using SWPatcher.Helpers;
using SWPatcher.Helpers.GlobalVar;
using System.IO;

namespace SWPatcher.Forms
{
    public partial class SettingsForm : Form
    {
        private string GameClientDirectory;
        private string PatcherWorkingDirectory;
        private bool WantToPatchSoulworkerExe;
        private bool PatcherRunasAdmin;

        public SettingsForm()
        {
            InitializeComponent();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            this.textBoxGameDirectory.Text = this.GameClientDirectory = UserSettings.GamePath;
            this.textBoxPatcherDirectory.Text = this.PatcherWorkingDirectory = UserSettings.PatcherPath;
            this.checkBoxPatchExe.Checked = this.WantToPatchSoulworkerExe = UserSettings.WantToPatchExe;
            this.checkBoxRunas.Checked = this.PatcherRunasAdmin = UserSettings.PatcherRunas;

            if ((this.Owner as MainForm).State == MainForm.States.Idle)
            {
                this.textBoxGameDirectory.TextChanged += new EventHandler(EnableApplyButton);
                this.textBoxPatcherDirectory.TextChanged += new EventHandler(EnableApplyButton);
                this.checkBoxPatchExe.CheckedChanged += new EventHandler(EnableApplyButton);
                this.checkBoxRunas.CheckedChanged += new EventHandler(EnableApplyButton);
            }
            else
            {
                this.buttonGameChangeDirectory.Enabled = false;
                this.buttonPatcherChangeDirectory.Enabled = false;
                this.checkBoxPatchExe.Enabled = false;
                this.checkBoxRunas.Enabled = false;
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
                Description = "Select your Soulworker game client folder."
            })
            {
                DialogResult result = folderDialog.ShowDialog();

                if (result == DialogResult.OK)
                    if (Methods.IsSwPath(folderDialog.SelectedPath))
                        if (Methods.IsValidSwPatcherPath(UserSettings.PatcherPath))
                            this.textBoxGameDirectory.Text = this.GameClientDirectory = folderDialog.SelectedPath;
                        else
                        {
                            var dialogResult = MsgBox.Question("The program is in the same or in a sub folder as your game client.\nThis will cause malfunctions or data corruption on your game client.\nAre you sure you want to set to this folder?");

                            if (dialogResult == DialogResult.Yes)
                                this.textBoxGameDirectory.Text = this.GameClientDirectory = folderDialog.SelectedPath;
                        }
                    else
                        MsgBox.Error("The selected folder is not a Soul Worker game client folder.");
            }
        }

        private void buttonPatcherChangeDirectory_Click(object sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog
            {
                Description = "Select your new desired patcher folder."
            })
            {
                DialogResult result = folderDialog.ShowDialog();

                if (result == DialogResult.OK)
                    if (Methods.IsValidSwPatcherPath(folderDialog.SelectedPath))
                        this.textBoxPatcherDirectory.Text = this.PatcherWorkingDirectory = folderDialog.SelectedPath;
                    else
                    {
                        var dialogResult = MsgBox.Question("The program is in the same or in a sub folder as your game client.\nThis will cause malfunctions or data corruption on your game client.\nAre you sure you want to set to this folder?");

                        if (dialogResult == DialogResult.Yes)
                            this.textBoxPatcherDirectory.Text = this.PatcherWorkingDirectory = folderDialog.SelectedPath;
                    }
            }
        }

        private void checkBoxPatchExe_CheckedChanged(object sender, EventArgs e)
        {
            this.WantToPatchSoulworkerExe = this.checkBoxPatchExe.Checked;
        }

        private void checkBoxRunas_CheckedChanged(object sender, EventArgs e)
        {
            this.PatcherRunasAdmin = this.checkBoxRunas.Checked;
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
            if (UserSettings.GamePath != this.GameClientDirectory)
                UserSettings.GamePath = this.GameClientDirectory;

            if (UserSettings.PatcherPath != this.PatcherWorkingDirectory)
            {
                try
                {
                    Methods.MoveOldPatcherFolder(UserSettings.PatcherPath, this.PatcherWorkingDirectory, (this.Owner as MainForm).GetComboBoxStringItems());
                }
                catch (IOException ex)
                {
                    Error.Log(ex);
                    MsgBox.Error(Error.ExeptionParser(ex));
                }

                UserSettings.PatcherPath = this.PatcherWorkingDirectory;
            }

            if (UserSettings.WantToPatchExe != this.WantToPatchSoulworkerExe)
                UserSettings.WantToPatchExe = this.WantToPatchSoulworkerExe;
            if (UserSettings.PatcherRunas != this.PatcherRunasAdmin)
                UserSettings.PatcherRunas = this.PatcherRunasAdmin;

            this.buttonApply.Enabled = false;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
