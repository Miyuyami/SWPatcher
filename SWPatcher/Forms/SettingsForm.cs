/*
 * This file is part of Soulworker Patcher.
 * Copyright (C) 2016 Miyu
 * 
 * Soulworker Patcher is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * Soulworker Patcher is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Soulworker Patcher. If not, see <http://www.gnu.org/licenses/>.
 */

using SWPatcher.General;
using SWPatcher.Helpers;
using SWPatcher.Helpers.GlobalVariables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
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
        private string UILanguage;

        public SettingsForm()
        {
            InitializeComponent();
            InitializeTextComponent();
        }

        private void InitializeTextComponent()
        {
            this.Text = StringLoader.GetText("form_settings");
            this.buttonOk.Text = StringLoader.GetText("button_ok");
            this.buttonCancel.Text = StringLoader.GetText("button_cancel");
            this.buttonApply.Text = StringLoader.GetText("button_apply");
            this.tabPageGame.Text = StringLoader.GetText("tab_game");
            this.groupBoxGameDirectory.Text = StringLoader.GetText("box_game_dir");
            this.buttonGameChangeDirectory.Text = this.buttonPatcherChangeDirectory.Text = StringLoader.GetText("button_change");
            this.groupBoxPatchExe.Text = StringLoader.GetText("box_patch_exe");
            this.checkBoxPatchExe.Text = StringLoader.GetText("check_patch_exe");
            this.tabPageCredentials.Text = StringLoader.GetText("tab_credentials");
            this.groupBoxGameUserId.Text = StringLoader.GetText("box_id");
            this.groupBoxGameUserPassword.Text = StringLoader.GetText("box_pw");
            this.groupBoxGameWantLogin.Text = StringLoader.GetText("box_want_login");
            this.checkBoxWantToLogin.Text = StringLoader.GetText("check_want_login");
            this.tabPagePatcher.Text = StringLoader.GetText("tab_patcher");
            this.groupBoxPatcherDirectory.Text = StringLoader.GetText("box_patcher_dir");
            this.groupBoxUILanguagePicker.Text = StringLoader.GetText("box_language");
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            this.PendingRestart = false;
            this.textBoxGameDirectory.Text = this.GameClientDirectory = UserSettings.GamePath;
            this.textBoxPatcherDirectory.Text = this.PatcherWorkingDirectory = UserSettings.PatcherPath;
            this.checkBoxPatchExe.Checked = this.WantToPatchSoulworkerExe = UserSettings.WantToPatchExe;
            this.textBoxId.Text = this.GameUserId = UserSettings.GameId;
            this.textBoxId.Enabled = this.textBoxPassword.Enabled = this.checkBoxWantToLogin.Checked = this.WantToLogin = UserSettings.WantToLogin;

            string encryptedEmptyString = String.Empty;
            using (var secure = Methods.ToSecureString(encryptedEmptyString))
            {
                encryptedEmptyString = Methods.EncryptString(secure);
            }
            string encryptedGamePw = UserSettings.GamePw;
            this.textBoxPassword.Text = this.GameUserPassword = encryptedGamePw != encryptedEmptyString ? MaskPassword(encryptedGamePw) : "";

            var def = new ResxLanguage(StringLoader.GetText("match_windows"), "default");
            var en = new ResxLanguage("English", "en");
            var ko = new ResxLanguage("한국어", "ko");
            var vi = new ResxLanguage("Tiếng Việt", "vi");
            this.comboBoxUILanguage.DataSource = new ResxLanguage[] { def, en, ko, vi };
            string savedCode = this.UILanguage = UserSettings.UILanguageCode;
            if (en.Code == savedCode)
            {
                this.comboBoxUILanguage.SelectedItem = en;
            }
            else if (ko.Code == savedCode)
            {
                this.comboBoxUILanguage.SelectedItem = ko;
            }
            else if (vi.Code == savedCode)
            {
                this.comboBoxUILanguage.SelectedItem = vi;
            }
            else
            {
                this.comboBoxUILanguage.SelectedItem = def;
            }

            if ((this.Owner as MainForm).CurrentState == MainForm.State.Idle)
            {
                this.textBoxGameDirectory.TextChanged += this.EnableApplyButton;
                this.textBoxPatcherDirectory.TextChanged += this.EnableApplyButton;
                this.checkBoxPatchExe.CheckedChanged += this.EnableApplyButton;
                this.textBoxId.TextChanged += this.EnableApplyButton;
                this.textBoxPassword.TextChanged += this.EnableApplyButton;
                this.checkBoxWantToLogin.CheckedChanged += this.EnableApplyButton;
                this.comboBoxUILanguage.SelectedIndexChanged += this.EnableApplyButton;
            }
            else
            {
                this.buttonGameChangeDirectory.Enabled = false;
                this.buttonPatcherChangeDirectory.Enabled = false;
                this.checkBoxPatchExe.Enabled = false;
                this.textBoxId.ReadOnly = true;
                this.textBoxPassword.ReadOnly = true;
                this.checkBoxWantToLogin.Enabled = false;
                this.comboBoxUILanguage.Enabled = false;
            }
        }

        private void EnableApplyButton(object sender, EventArgs e)
        {
            this.buttonApply.Enabled = true;
        }

        private void ButtonChangeDirectory_Click(object sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog
            {
                ShowNewFolderButton = false,
                Description = StringLoader.GetText("dialog_folder_change_game_dir")
            })
            {
                DialogResult result = folderDialog.ShowDialog();

                if (result == DialogResult.OK)
                    if (Methods.IsSwPath(folderDialog.SelectedPath))
                        if (Methods.IsValidSwPatcherPath(UserSettings.PatcherPath))
                            this.textBoxGameDirectory.Text = this.GameClientDirectory = folderDialog.SelectedPath;
                        else
                        {
                            DialogResult dialogResult = MsgBox.Question(StringLoader.GetText("question_folder_same_path_game"));

                            if (dialogResult == DialogResult.Yes)
                                this.textBoxGameDirectory.Text = this.GameClientDirectory = folderDialog.SelectedPath;
                        }
                    else
                        MsgBox.Error(StringLoader.GetText("exception_folder_not_game_folder"));
            }
        }

        private void ButtonPatcherChangeDirectory_Click(object sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog
            {
                Description = StringLoader.GetText("dialog_folder_change_patcher_dir")
            })
            {
                DialogResult result = folderDialog.ShowDialog();

                if (result == DialogResult.OK)
                {
                    if (Methods.IsValidSwPatcherPath(folderDialog.SelectedPath))
                        this.textBoxPatcherDirectory.Text = this.PatcherWorkingDirectory = folderDialog.SelectedPath;
                    else
                    {
                        DialogResult dialogResult = MsgBox.Question(StringLoader.GetText("question_folder_same_path_game"));

                        if (dialogResult == DialogResult.Yes)
                            this.textBoxPatcherDirectory.Text = this.PatcherWorkingDirectory = folderDialog.SelectedPath;
                    }
                }
            }
        }

        private void CheckBoxPatchExe_CheckedChanged(object sender, EventArgs e)
        {
            this.WantToPatchSoulworkerExe = this.checkBoxPatchExe.Checked;
        }

        private void TextBoxId_TextChanged(object sender, EventArgs e)
        {
            this.GameUserId = this.textBoxId.Text;
        }

        private void TextBoxPassword_TextChanged(object sender, EventArgs e)
        {
            this.GameUserPassword = this.textBoxPassword.Text;
        }

        private void CheckBoxWantToLogin_CheckedChanged(object sender, EventArgs e)
        {
            this.textBoxId.Enabled = this.textBoxPassword.Enabled = this.WantToLogin = this.checkBoxWantToLogin.Checked;
        }

        private void ComboBoxUILanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.UILanguage = (this.comboBoxUILanguage.SelectedItem as ResxLanguage).Code;
        }

        private void ButtonOk_Click(object sender, EventArgs e)
        {
            if (this.buttonApply.Enabled)
                this.ApplyChanges();

            this.DialogResult = DialogResult.OK;
        }

        private void ButtonApply_Click(object sender, EventArgs e)
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
                    MoveOldPatcherFolder(UserSettings.PatcherPath, this.PatcherWorkingDirectory, (this.Owner as MainForm).GetComboBoxItemsAsString());
                }
                catch (IOException ex)
                {
                    Logger.Error(ex);
                    MsgBox.Error(Logger.ExeptionParser(ex));
                }

                UserSettings.PatcherPath = this.PatcherWorkingDirectory;
            }

            if (UserSettings.WantToPatchExe != this.WantToPatchSoulworkerExe)
                UserSettings.WantToPatchExe = this.WantToPatchSoulworkerExe;

            if (UserSettings.GameId != this.GameUserId)
                UserSettings.GameId = this.GameUserId;

            if (MaskPassword(UserSettings.GamePw) != this.GameUserPassword)
            {
                using (var secure = Methods.ToSecureString(this.GameUserPassword))
                {
                    UserSettings.GamePw = Methods.EncryptString(secure);
                }
                MsgBox.Success("PASSWORD SAVED!!!");
            }

            if (UserSettings.WantToLogin != this.WantToLogin)
                UserSettings.WantToLogin = this.WantToLogin;

            if (UserSettings.UILanguageCode != this.UILanguage)
            {
                UserSettings.UILanguageCode = this.UILanguage;
                this.PendingRestart = true;
            }

            this.buttonApply.Enabled = false;

            if (this.PendingRestart)
                MsgBox.Notice(StringLoader.GetText("notice_pending_restart"));
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

        private static string SHA256String(string str)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] result = sha256.ComputeHash(Encoding.Unicode.GetBytes(str));
                StringBuilder sb = new StringBuilder();

                foreach (byte b in result)
                    sb.Append(b.ToString("x2"));

                return sb.ToString();
            }
        }

        private static string MaskPassword(string password)
        {
            using (SecureString secure = Methods.DecryptString(password))
            {
                return SHA256String(Methods.ToInsecureString(secure));
            }
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
