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
using System.Windows.Forms;

namespace SWPatcher.Forms
{
    internal partial class MainForm
    {
        private void MainForm_Load(object sender, EventArgs e)
        {
            this.InitRegionsConfigData();

            StartupBackupCheck(this.ComboBoxLanguages.SelectedItem as Language);

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
                    this.RTPatcher.Run(this.ComboBoxLanguages.SelectedItem as Language);

                    break;
                case State.Download:
                    this.ButtonDownload.Text = StringLoader.GetText("button_cancelling");
                    this.Downloader.Cancel();

                    break;
                case State.Patch:
                    this.ButtonDownload.Text = StringLoader.GetText("button_cancelling");
                    this.Patcher.Cancel();

                    break;
                case State.RTPatch:
                    this.ButtonDownload.Text = StringLoader.GetText("button_cancelling");
                    this.RTPatcher.Cancel();

                    break;
            }
        }

        private void ButtonPlay_Click(object sender, EventArgs e)
        {
            switch (this.CurrentState)
            {
                case State.Idle:
                    this.CurrentState = State.RTPatch;
                    this._nextState = NextState.Play;
                    this.RTPatcher.Run(this.ComboBoxLanguages.SelectedItem as Language);

                    break;
                case State.WaitClient:
                    this.ButtonPlay.Text = StringLoader.GetText("button_cancelling");
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
                    this.RTPatcher.Run(this.ComboBoxLanguages.SelectedItem as Language);

                    break;
            }
        }

        private void ForceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Language language = this.ComboBoxLanguages.SelectedItem as Language;

            ResetTranslation(language);

            this.CurrentState = State.RTPatch;
            this._nextState = NextState.Download;
            this.RTPatcher.Run(language);
        }

        private void ComboBoxLanguages_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (this.ComboBoxLanguages.SelectedItem is Language language)
            {
                Logger.Info($"Selected language '{language}'");
                UserSettings.LanguageId = this.ComboBoxLanguages.SelectedIndex == -1 ? null : (this.ComboBoxLanguages.SelectedItem as Language).Id;

                if (Methods.HasNewTranslations(language))
                {
                    this.LabelNewTranslations.Text = StringLoader.GetText("form_label_new_translation", language, Methods.DateToLocalString(language.LastUpdate));
                }
                else
                {
                    this.LabelNewTranslations.Text = String.Empty;
                }
            }
        }

        private void ComboBoxRegions_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (this.ComboBoxRegions.SelectedItem is Region region)
            {
                Logger.Info($"Selected region '{region}'");
                UserSettings.RegionId = this.ComboBoxRegions.SelectedIndex == -1 ? null : (this.ComboBoxRegions.SelectedItem as Region).Id;

                Language[] languages = region.AppliedLanguages;

                this.ComboBoxLanguages.DataSource = languages.Length > 0 ? languages : null;

                if (this.ComboBoxLanguages.DataSource != null)
                {
                    if (String.IsNullOrEmpty(UserSettings.LanguageId))
                    {
                        UserSettings.LanguageId = (this.ComboBoxLanguages.SelectedItem as Language).Id;
                    }
                    else
                    {
                        int index = this.ComboBoxLanguages.Items.IndexOf(new Language(UserSettings.LanguageId));
                        this.ComboBoxLanguages.SelectedIndex = index == -1 ? 0 : index;
                    }

                    this.ComboBoxLanguages_SelectionChangeCommitted(sender, e);
                }

                string newGamePath;

                switch (region.Id)
                {
                    case "jp":
                        newGamePath = GetJPSwPathFromRegistry();

                        break;
                    case "gjp":
                        newGamePath = GetJPSwPathFromRegistry();

                        break;
                    case "kr":
                        newGamePath = GetKRSwPathFromRegistry();

                        break;
                    case "nkr":
                        newGamePath = GetNaverKRSwPathFromRegistry();

                        break;
                    case "gf":
                        newGamePath = GetGameforgeSwPath();

                        break;
                    default:
                        throw new Exception(StringLoader.GetText("exception_region_unknown", region.Id));
                }

                if (String.IsNullOrWhiteSpace(newGamePath))
                {
                    this.CurrentState = State.RegionNotInstalled;
                    MsgBox.Error(StringLoader.GetText("exception_game_install_not_found", region.ToString()));
                }
                else if (!Directory.Exists(newGamePath))
                {
                    this.CurrentState = State.RegionNotInstalled;
                    MsgBox.Error(StringLoader.GetText("exception_directory_not_exist", newGamePath));
                }
                else
                {
                    if (newGamePath != UserSettings.GamePath)
                    {
                        string gameExePatchedPath = Path.Combine(UserSettings.PatcherPath, region.Folder, Methods.GetGameExeName(region.Id));
                        if (File.Exists(gameExePatchedPath))
                        {
                            File.Delete(gameExePatchedPath);
                        }

                        UserSettings.GamePath = newGamePath;
                    }

                    this.CurrentState = State.Idle;
                }
            }
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.NotifyIcon.Visible = true;
                this.NotifyIcon.ShowBalloonTip(500);

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
            this.InitRegionsConfigData();
        }

        private void OpenSWWebpageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Region region = (this.ComboBoxRegions.SelectedItem as Region);
            switch (region.Id)
            {
                case "jp":
                    Process.Start(Urls.SoulworkerJPHangameHome);

                    break;
                case "gjp":
                    Process.Start(Urls.SoulworkerJPGamecomHome);

                    break;
                case "kr":
                    Process.Start(Urls.SoulworkerKRHome);

                    break;
                case "nkr":
                    Process.Start(Urls.SoulworkerNaverKRHome);

                    break;
                case "gf":
                    Process.Start(Urls.SoulworkerGFHome);

                    break;
                default:
                    throw new Exception(StringLoader.GetText("exception_region_unknown", region.Id));
            }
        }

        private void UploadLogToPastebinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!File.Exists(Strings.FileName.Log))
            {
                MsgBox.Error(StringLoader.GetText("exception_log_not_exist"));

                return;
            }
#if DEBUG
            Process.Start(Strings.FileName.Log);
#else
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
#endif
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
            else if (!this.CurrentState.In(State.Idle, State.RegionNotInstalled))
            {
                MsgBox.Error(StringLoader.GetText("exception_cannot_close", AssemblyAccessor.Title));

                e.Cancel = true;
            }
            else
            {
                Logger.Info($"{this.Text} closing. Reason=[{e.CloseReason.ToString()}]");
            }
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
