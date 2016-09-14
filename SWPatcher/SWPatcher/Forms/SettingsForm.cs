using SWPatcher.Helpers;
using SWPatcher.Helpers.GlobalVar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SWPatcher.Forms
{
    public partial class SettingsForm : Form
    {
        private bool PendingRestart;
        private string GameClientDirectory;
        private string PatcherWorkingDirectory;
        private bool WantToPatchSoulworkerExe;
        private string GameUserId;
        private string GameUserPassword;
        private bool WantToLogin;
        private byte InterfaceMode;

        public SettingsForm()
        {
            InitializeComponent();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            this.PendingRestart = false;
            this.textBoxGameDirectory.Text = this.GameClientDirectory = UserSettings.GamePath;
            this.textBoxPatcherDirectory.Text = this.PatcherWorkingDirectory = UserSettings.PatcherPath;
            this.checkBoxPatchExe.Checked = this.WantToPatchSoulworkerExe = UserSettings.WantToPatchExe;
            this.textBoxId.Text = this.GameUserId = UserSettings.GameId;
            this.textBoxPassword.Text = this.GameUserPassword = UserSettings.GamePw;
            this.textBoxId.Enabled = this.textBoxPassword.Enabled = this.checkBoxWantToLogin.Checked = this.WantToLogin = UserSettings.WantToLogin;
            switch (this.InterfaceMode = UserSettings.InterfaceMode)
            {
                case 0:
                    this.radioButtonFull.Checked = true;
                    break;
                case 1:
                    this.radioButtonMinimal.Checked = true;
                    break;
            }

            if ((this.Owner as MainForm).CurrentState == MainForm.State.Idle)
            {
                this.textBoxGameDirectory.TextChanged += EnableApplyButton;
                this.textBoxPatcherDirectory.TextChanged += EnableApplyButton;
                this.checkBoxPatchExe.CheckedChanged += EnableApplyButton;
                this.textBoxId.TextChanged += EnableApplyButton;
                this.textBoxPassword.TextChanged += EnableApplyButton;
                this.checkBoxWantToLogin.CheckedChanged += EnableApplyButton;
                this.radioButtonFull.CheckedChanged += EnableApplyButton;
            }
            else
            {
                this.buttonGameChangeDirectory.Enabled = false;
                this.buttonPatcherChangeDirectory.Enabled = false;
                this.checkBoxPatchExe.Enabled = false;
                this.textBoxId.ReadOnly = true;
                this.textBoxPassword.ReadOnly = true;
                this.checkBoxWantToLogin.Enabled = false;
                this.radioButtonFull.Enabled = false;
                this.radioButtonMinimal.Enabled = false;
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

        private void textBoxId_TextChanged(object sender, EventArgs e)
        {
            this.GameUserId = this.textBoxId.Text;
        }

        private void textBoxPassword_TextChanged(object sender, EventArgs e)
        {
            this.GameUserPassword = this.textBoxPassword.Text;
        }

        private void checkBoxWantToLogin_CheckedChanged(object sender, EventArgs e)
        {
            this.textBoxId.Enabled = this.textBoxPassword.Enabled = this.WantToLogin = this.checkBoxWantToLogin.Checked;
        }

        private void radioButtonFull_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButtonFull.Checked)
                this.InterfaceMode = 0;
        }

        private void radioButtonMinimal_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButtonMinimal.Checked)
                this.InterfaceMode = 1;
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            if (this.buttonApply.Enabled)
                this.ApplyChanges();

            this.DialogResult = DialogResult.OK;
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
                    MoveOldPatcherFolder(UserSettings.PatcherPath, this.PatcherWorkingDirectory, (this.Owner as MainForm).GetComboBoxStringItems());
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

            if (UserSettings.GameId != this.GameUserId)
                UserSettings.GameId = this.GameUserId;

            if (UserSettings.GamePw != this.GameUserPassword)
                UserSettings.GamePw = this.GameUserPassword;

            if (UserSettings.WantToLogin != this.WantToLogin)
                UserSettings.WantToLogin = this.WantToLogin;

            if (UserSettings.InterfaceMode != this.InterfaceMode)
            {
                UserSettings.InterfaceMode = this.InterfaceMode;
                this.PendingRestart = true;
            }

            this.buttonApply.Enabled = false;

            if (this.PendingRestart)
                MsgBox.Notice("Some changings will only be applied after restarting the application.");
        }

        private static void MoveOldPatcherFolder(string oldPath, string newPath, IEnumerable<string> translationFolders)
        {
            string[] movingFolders = translationFolders.Where(s => Directory.Exists(s)).ToArray();
            string backupDirectory = Path.Combine(oldPath, Strings.FolderName.Backup);
            string rtpLogsDirectory = Path.Combine(oldPath, Strings.FolderName.RTPatchLogs);
            string logFilePath = Path.Combine(oldPath, Strings.FileName.Log);
            string gameExePath = Path.Combine(oldPath, Strings.FileName.GameExe);

            foreach (var folder in movingFolders)
                MoveDirectory(Path.Combine(oldPath, folder), newPath);

            MoveDirectory(backupDirectory, newPath);
            MoveDirectory(rtpLogsDirectory, newPath);

            MoveFile(logFilePath, newPath, false);
            MoveFile(gameExePath, newPath, false);
        }

        private static bool MoveDirectory(string directory, string newPath)
        {
            if (Directory.Exists(directory))
            {
                string destination = Path.Combine(newPath, Path.GetFileName(directory));
                Directory.CreateDirectory(destination);

                foreach (var dirPath in Directory.GetDirectories(directory, "*", SearchOption.AllDirectories))
                    Directory.CreateDirectory(dirPath.Replace(directory, destination));

                foreach (var filePath in Directory.GetFiles(directory, "*", SearchOption.AllDirectories))
                    MoveFile(filePath, filePath.Replace(directory, destination), true);

                Directory.Delete(directory, true);
                return true;
            }

            return false;
        }

        private static bool MoveFile(string file, string newPath, bool newPathHasFileName)
        {
            if (File.Exists(file))
            {
                string newFilePath = "";
                if (newPathHasFileName)
                    newFilePath = newPath;
                else
                    newFilePath = Path.Combine(newPath, Path.GetFileName(file));

                if (File.Exists(newFilePath))
                    File.Delete(newFilePath);
                File.Move(file, newFilePath);

                return true;
            }

            return false;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
