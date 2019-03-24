/*
 * This file is part of Soulworker Patcher.
 * Copyright (C) 2016-2017 Miyu, Dramiel Leayal
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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using SWPatcher.General;
using SWPatcher.Helpers;
using SWPatcher.Helpers.GlobalVariables;

namespace SWPatcher.Forms
{
    internal partial class SettingsForm : Form
    {
        private bool PendingRestart;
        //private string GameClientDirectory;
        private string PatcherWorkingDirectory;
        private bool WantToPatchSoulworkerExe;
        private string GameUserId;
        private string GameUserPassword;
        private bool WantToLogin;
        private string UILanguage;

        internal SettingsForm()
        {
            InitializeComponent();
            InitializeTextComponent();
        }

        private void InitializeTextComponent()
        {
            this.Text = StringLoader.GetText("form_settings");
            this.ButtonOk.Text = StringLoader.GetText("button_ok");
            this.ButtonCancel.Text = StringLoader.GetText("button_cancel");
            this.ButtonApply.Text = StringLoader.GetText("button_apply");
            this.TabPageGame.Text = StringLoader.GetText("tab_game");
            this.GroupBoxGameDirectory.Text = StringLoader.GetText("box_game_dir");
            this.ButtonOpenGameDirectory.Text = StringLoader.GetText("button_open");
            this.ButtonChangePatcherDirectory.Text = StringLoader.GetText("button_change");
            this.GroupBoxPatchExe.Text = StringLoader.GetText("box_patch_exe");
            this.CheckBoxPatchExe.Text = StringLoader.GetText("check_patch_exe");
            this.TabPageCredentials.Text = StringLoader.GetText("tab_credentials");
            this.GroupBoxGameUserId.Text = StringLoader.GetText("box_id");
            this.GroupBoxGameUserPassword.Text = StringLoader.GetText("box_pw");
            this.GroupBoxGameWantLogin.Text = StringLoader.GetText("box_want_login");
            this.CheckBoxWantToLogin.Text = StringLoader.GetText("check_want_login");
            this.TabPagePatcher.Text = StringLoader.GetText("tab_patcher");
            this.GroupBoxPatcherDirectory.Text = StringLoader.GetText("box_patcher_dir");
            this.GroupBoxUILanguagePicker.Text = StringLoader.GetText("box_language");
            this.GroupBoxGameOptions.Text = StringLoader.GetText("box_game_options");
            this.ButtonOpenGameOptions.Text = StringLoader.GetText("button_game_options");
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            this.PendingRestart = false;
            this.TextBoxGameDirectory.Text = UserSettings.GamePath;// = this.GameClientDirectory = UserSettings.GamePath;
            this.TextBoxPatcherDirectory.Text = this.PatcherWorkingDirectory = UserSettings.PatcherPath;
            this.CheckBoxPatchExe.Checked = this.WantToPatchSoulworkerExe = UserSettings.WantToPatchExe;
            this.TextBoxId.Text = this.GameUserId = UserSettings.GameId;
            this.TextBoxId.Enabled = this.TextBoxPassword.Enabled = this.CheckBoxWantToLogin.Checked = this.WantToLogin = UserSettings.WantToLogin;

            string maskedEmptyString = SHA256String(String.Empty);
            string maskedGamePw = MaskPassword(UserSettings.GamePw);
            if (maskedEmptyString == maskedGamePw)
            {
                this.TextBoxPassword.Text = this.GameUserPassword = String.Empty;
            }
            else
            {
                this.TextBoxPassword.Text = this.GameUserPassword = maskedGamePw;
            }

            var def = new ResxLanguage(StringLoader.GetText("match_windows"), "default");
            var en = new ResxLanguage("English", "en");
            var ko = new ResxLanguage("한국어", "ko");
            var vi = new ResxLanguage("Tiếng Việt", "vi");
            var ru = new ResxLanguage("Русский", "ru");
            this.ComboBoxUILanguage.DataSource = new ResxLanguage[] { def, en, ko, vi, ru };
            string savedCode = this.UILanguage = UserSettings.UILanguageCode;
            if (en.Code == savedCode)
            {
                this.ComboBoxUILanguage.SelectedItem = en;
            }
            else if (ko.Code == savedCode)
            {
                this.ComboBoxUILanguage.SelectedItem = ko;
            }
            else if (vi.Code == savedCode)
            {
                this.ComboBoxUILanguage.SelectedItem = vi;
            }
            else if(ru.Code == savedCode)
            {
                this.ComboBoxUILanguage.SelectedItem = ru;
            }
            else
            {
                this.ComboBoxUILanguage.SelectedItem = def;
            }

            if ((this.Owner as MainForm).CurrentState == MainForm.State.Idle)
            {
                this.TextBoxPatcherDirectory.TextChanged += this.EnableApplyButton;
                this.CheckBoxPatchExe.CheckedChanged += this.EnableApplyButton;
                this.TextBoxId.TextChanged += this.EnableApplyButton;
                this.TextBoxPassword.TextChanged += this.EnableApplyButton;
                this.CheckBoxWantToLogin.CheckedChanged += this.EnableApplyButton;
                this.ComboBoxUILanguage.SelectedIndexChanged += this.EnableApplyButton;
            }
            else if ((this.Owner as MainForm).CurrentState == MainForm.State.RegionNotInstalled)
            {
                this.ButtonOpenGameDirectory.Enabled = false;
                this.ButtonOpenGameOptions.Enabled = false;
            }
            else
            {
                this.ButtonChangePatcherDirectory.Enabled = false;
                this.CheckBoxPatchExe.Enabled = false;
                this.TextBoxId.ReadOnly = true;
                this.TextBoxPassword.ReadOnly = true;
                this.CheckBoxWantToLogin.Enabled = false;
                this.ComboBoxUILanguage.Enabled = false;
            }
        }

        private void EnableApplyButton(object sender, EventArgs e)
        {
            this.ButtonApply.Enabled = true;
        }

        private void ButtonOpenGameDirectory_Click(object sender, EventArgs e)
        {
            Process.Start(this.TextBoxGameDirectory.Text);
        }

        private void ButtonChangePatcherDirectory_Click(object sender, EventArgs e)
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
                    {
                        this.TextBoxPatcherDirectory.Text = this.PatcherWorkingDirectory = folderDialog.SelectedPath;
                    }
                    else
                    {
                        DialogResult dialogResult = MsgBox.Question(StringLoader.GetText("question_folder_same_path_game"));

                        if (dialogResult == DialogResult.Yes)
                        {
                            this.TextBoxPatcherDirectory.Text = this.PatcherWorkingDirectory = folderDialog.SelectedPath;
                        }
                    }
                }
            }
        }

        private void CheckBoxPatchExe_CheckedChanged(object sender, EventArgs e)
        {
            this.WantToPatchSoulworkerExe = this.CheckBoxPatchExe.Checked;
        }

        private void TextBoxId_TextChanged(object sender, EventArgs e)
        {
            this.GameUserId = this.TextBoxId.Text;
        }

        private void TextBoxPassword_TextChanged(object sender, EventArgs e)
        {
            this.GameUserPassword = this.TextBoxPassword.Text;
        }

        private void CheckBoxWantToLogin_CheckedChanged(object sender, EventArgs e)
        {
            this.TextBoxId.Enabled = this.TextBoxPassword.Enabled = this.WantToLogin = this.CheckBoxWantToLogin.Checked;
        }

        private void ComboBoxUILanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.UILanguage = (this.ComboBoxUILanguage.SelectedItem as ResxLanguage).Code;
        }

        private void ButtonOk_Click(object sender, EventArgs e)
        {
            if (this.ButtonApply.Enabled)
            {
                this.ApplyChanges();
            }

            this.DialogResult = DialogResult.OK;
        }

        private void ButtonApply_Click(object sender, EventArgs e)
        {
            this.ApplyChanges();
        }

        private void ButtonOpenGameOptions_Click(object sender, EventArgs e)
        {
            string optionExePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Strings.FileName.OptionExe);

            var startInfo = new ProcessStartInfo
            {
                UseShellExecute = true,
                Verb = "runas",
                WorkingDirectory = UserSettings.GamePath,
                FileName = optionExePath
            };
            Process.Start(startInfo);
        }

        private void ApplyChanges()
        {
            try
            {
                if (UserSettings.PatcherPath != this.PatcherWorkingDirectory)
                {
                    MoveOldPatcherFolder(UserSettings.PatcherPath, this.PatcherWorkingDirectory, (this.Owner as MainForm).GetTranslationFolders());

                    UserSettings.PatcherPath = this.PatcherWorkingDirectory;
                }

                if (UserSettings.WantToPatchExe != this.WantToPatchSoulworkerExe)
                {
                    string regionId = (this.Owner as MainForm).GetSelectedRegionId();
                    string gameExePatchedPath = Path.Combine(UserSettings.PatcherPath, regionId, Methods.GetGameExeName(regionId));
                    if (File.Exists(gameExePatchedPath))
                    {
                        File.Delete(gameExePatchedPath);
                    }

                    UserSettings.WantToPatchExe = this.WantToPatchSoulworkerExe;
                }

                if (UserSettings.GameId != this.GameUserId)
                {
                    UserSettings.GameId = this.GameUserId;
                }

                if (this.GameUserPassword != MaskPassword(UserSettings.GamePw))
                {
                    using (var secure = Methods.ToSecureString(this.GameUserPassword))
                    {
                        UserSettings.GamePw = Methods.EncryptString(secure);
                    }
                }

                if (UserSettings.WantToLogin != this.WantToLogin)
                {
                    UserSettings.WantToLogin = this.WantToLogin;
                }

                if (UserSettings.UILanguageCode != this.UILanguage)
                {
                    UserSettings.UILanguageCode = this.UILanguage;
                    this.PendingRestart = true;
                }

                this.ButtonApply.Enabled = false;

                if (this.PendingRestart)
                {
                    MsgBox.Notice(StringLoader.GetText("notice_pending_restart"));
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                MsgBox.Error(Methods.ExeptionParser(ex));
            }
        }

        private static void MoveOldPatcherFolder(string oldPath, string newPath, IEnumerable<string> translationFolders)
        {
            string[] movingFolders = translationFolders.Where(s => Directory.Exists(s)).ToArray();
            string rtpLogsDirectory = Path.Combine(oldPath, Strings.FolderName.RTPatchLogs);
            string logFilePath = Path.Combine(oldPath, Strings.FileName.Log);

            foreach (var folder in movingFolders)
                MoveDirectory(Path.Combine(oldPath, folder), newPath);

            MoveDirectory(rtpLogsDirectory, newPath);

            MoveFile(logFilePath, newPath, false);
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
                {
                    newFilePath = newPath;
                }
                else
                {
                    newFilePath = Path.Combine(newPath, Path.GetFileName(file));
                }

                if (File.Exists(newFilePath))
                {
                    File.Delete(newFilePath);
                }

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
