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

using SWPatcher.General;
using SWPatcher.Helpers;
using SWPatcher.Helpers.GlobalVariables;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SWPatcher.Forms
{
    public partial class MainForm
    {
        private void MainForm_Load(object sender, EventArgs e)
        {
            Language[] languages = GetAvailableLanguages();
            this.comboBoxLanguages.DataSource = languages.Length > 0 ? languages : null;

            var gamePath = UserSettings.GamePath;
            if (String.IsNullOrEmpty(gamePath) || !Methods.IsSwPath(gamePath))
            {
                UserSettings.GamePath = GetSwPathFromRegistry();
            }
            Methods.EnsureDirectoryRights(UserSettings.GamePath);

            if (this.comboBoxLanguages.DataSource != null)
            {
                Logger.Info($"Loading languages: {String.Join(" ", languages.Select(l => l.ToString()))}");

                if (String.IsNullOrEmpty(UserSettings.LanguageName))
                {
                    UserSettings.LanguageName = (this.comboBoxLanguages.SelectedItem as Language).Name;
                }
                else
                {
                    int index = this.comboBoxLanguages.Items.IndexOf(new Language(UserSettings.LanguageName, DateTime.UtcNow));
                    this.comboBoxLanguages.SelectedIndex = index == -1 ? 0 : index;
                }
            }

            StartupBackupCheck(this.comboBoxLanguages.SelectedItem as Language);

            if (!Methods.IsValidSwPatcherPath(UserSettings.PatcherPath))
            {
                string error = StringLoader.GetText("exception_folder_same_path_game");

                Logger.Error(error);
                MsgBox.Error(error);
            }
        }

        private void ButtonDownload_Click(object sender, EventArgs e)
        {
            switch (this.CurrentState)
            {
                case State.Idle:
                    this.CurrentState = State.RTPatch;
                    this._nextState = NextState.Download;
                    this.RTPatcher.Run();

                    break;
                case State.Download:
                    this.buttonDownload.Text = StringLoader.GetText("button_cancelling");
                    this.Downloader.Cancel();

                    break;
                case State.Patch:
                    this.buttonDownload.Text = StringLoader.GetText("button_cancelling");
                    this.Patcher.Cancel();

                    break;
                case State.RTPatch:
                    this.buttonDownload.Text = StringLoader.GetText("button_cancelling");
                    this.RTPatcher.Cancel();

                    break;
            }
        }

        private void ButtonPlay_MouseDown(object sender, MouseEventArgs e)
        {
            switch (this.CurrentState)
            {
                case State.Idle:
                    this.CurrentState = State.RTPatch;
                    this._nextState = NextState.Play;
                    this.RTPatcher.Run();

                    break;
                case State.WaitClient:
                    this.buttonPlay.Text = StringLoader.GetText("button_cancelling");
                    this.GameStarter.Cancel();

                    break;
            }
        }

        private void ButtonStartRaw_Click(object sender, EventArgs e)
        {
            switch (this.CurrentState)
            {
                case State.Idle:
                    this.CurrentState = State.RTPatch;
                    this._nextState = NextState.PlayRaw;
                    this.RTPatcher.Run();

                    break;
            }
        }

        private void ForceStripMenuItem_Click(object sender, EventArgs e)
        {
            Language language = this.comboBoxLanguages.SelectedItem as Language;

            DeleteTranslationIni(language);
            this.labelNewTranslations.Text = StringLoader.GetText("form_label_new_translation", language.Name, Methods.DateToString(language.LastUpdate));

            this.CurrentState = State.RTPatch;
            this._nextState = NextState.Download;
            this.RTPatcher.Run();
        }

        private void ComboBoxLanguages_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBoxLanguages.SelectedItem is Language language && Methods.HasNewTranslations(language))
			{
                this.labelNewTranslations.Text = StringLoader.GetText("form_label_new_translation", language.Name, Methods.DateToString(language.LastUpdate));
			}
            else
			{
                this.labelNewTranslations.Text = String.Empty;
			}
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.notifyIcon.Visible = true;
                this.notifyIcon.ShowBalloonTip(500);

                this.ShowInTaskbar = false;
                this.Hide();
            }
        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            this.RestoreFromTray();
        }

        private void SettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsForm settingsForm = new SettingsForm();
            settingsForm.ShowDialog(this);
        }

        private void RefreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Language language = this.comboBoxLanguages.SelectedItem as Language;
            Language[] languages = GetAvailableLanguages();
            this.comboBoxLanguages.DataSource = languages.Length > 0 ? languages : null;

            if (language != null && this.comboBoxLanguages.DataSource != null)
            {
                int index = this.comboBoxLanguages.Items.IndexOf(language);
                this.comboBoxLanguages.SelectedIndex = index == -1 ? 0 : index;
            }
        }

        private void OpenSWWebpageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("iexplore.exe", Urls.SoulworkerHome);
        }

        private void UploadLogToPastebinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!File.Exists(Strings.FileName.Log))
            {
                MsgBox.Error(StringLoader.GetText("exception_log_not_exist"));

                return;
            }

            string logTitle = $"{AssemblyAccessor.Version} ({GetSHA256(Application.ExecutablePath).Substring(0, 12)}) at {Methods.DateToString(DateTime.UtcNow)}";
            byte[] logBytes = File.ReadAllBytes(Strings.FileName.Log);
            logBytes = TrimArrayIfNecessary(logBytes);
            string logText = BitConverter.ToString(logBytes).Replace("-", "");
            var pasteUrl = UploadToPasteBin(logTitle, logText, PasteBinExpiration.OneHour, true, "text");

            if (!String.IsNullOrEmpty(pasteUrl))
            {
                Clipboard.SetText(pasteUrl);
                MsgBox.Success(StringLoader.GetText("success_log_file_upload", pasteUrl));
            }
            else
            {
                MsgBox.Error(StringLoader.GetText("exception_log_file_failed"));
            }
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox aboutBox = new AboutBox();
            aboutBox.ShowDialog(this);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason.In(CloseReason.ApplicationExitCall, CloseReason.WindowsShutDown))
            {
                Logger.Info($"{this.Text} closing abnormally. Reason=[{e.CloseReason.ToString()}]");
                this.CurrentState = State.Idle;
                this.RTPatcher.Cancel();
                this.Downloader.Cancel();
                this.Patcher.Cancel();
                this.GameStarter.Cancel();
            }
            else if (this.CurrentState != State.Idle)
            {
                MsgBox.Error(StringLoader.GetText("exception_cannot_close", AssemblyAccessor.Title));

                e.Cancel = true;
            }
            else
            {
                Logger.Info($"{this.Text} closing. Reason=[{e.CloseReason.ToString()}]");
                UserSettings.LanguageName = this.comboBoxLanguages.SelectedIndex == -1 ? null : (this.comboBoxLanguages.SelectedItem as Language).Name;
            }
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
