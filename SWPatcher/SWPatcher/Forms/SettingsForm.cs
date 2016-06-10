using System;
using System.Windows.Forms;
using SWPatcher.Helpers;
using SWPatcher.Helpers.GlobalVar;

namespace SWPatcher.Forms
{
    public partial class SettingsForm : Form
    {
        private string GameClientDirectory;
        private string PatcherWorkingDirectory;

        public SettingsForm()
        {
            InitializeComponent();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            this.GameClientDirectory = Paths.GameRoot;
            this.PatcherWorkingDirectory = Paths.PatcherRoot;
            this.textBoxGameDirectory.Text = this.GameClientDirectory;
            this.textBoxPatcherDirectory.Text = this.PatcherWorkingDirectory;

            if ((this.Owner as MainForm).State == MainForm.States.Idle)
            {
                this.textBoxGameDirectory.TextChanged += new EventHandler(EnableApplyButton);
                this.textBoxPatcherDirectory.TextChanged += new EventHandler(EnableApplyButton);
            }
            else
            {
                this.buttonGameChangeDirectory.Enabled = false;
                this.buttonPatcherChangeDirectory.Enabled = false;
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
                        if (Methods.IsValidSwPatcherPath(Paths.PatcherRoot))
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
            if (Paths.GameRoot != this.GameClientDirectory)
                Paths.GameRoot = this.GameClientDirectory;

            if (Paths.PatcherRoot != this.PatcherWorkingDirectory)
            {
                Methods.MoveOldPatcherFolder(Paths.PatcherRoot, this.PatcherWorkingDirectory);
                Paths.PatcherRoot = this.PatcherWorkingDirectory;
            }

            this.buttonApply.Enabled = false;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
