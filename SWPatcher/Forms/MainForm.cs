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
using System.IO;
using System.Windows.Forms;
using MadMilkman.Ini;
using SWPatcher.Downloading;
using SWPatcher.General;
using SWPatcher.Helpers;
using SWPatcher.Helpers.GlobalVariables;
using SWPatcher.Launching;
using SWPatcher.Patching;
using SWPatcher.RTPatch;

namespace SWPatcher.Forms
{
    internal partial class MainForm : Form
    {
        internal enum State
        {
            Idle = 0,
            Download,
            Patch,
            Prepare,
            WaitClient,
            WaitClose,
            RTPatch,
            RegionNotInstalled
        }

        private enum NextState
        {
            None = 0,
            Download,
            Play,
            PlayRaw
        }

        private State _state;
        private NextState _nextState;
        private readonly Downloader Downloader;
        private readonly Patcher Patcher;
        private readonly RTPatcher RTPatcher;
        private readonly GameStarter GameStarter;

        internal State CurrentState
        {
            get
            {
                return this._state;
            }
            private set
            {
                if (this._state != value)
                {
                    switch (value)
                    {
                        case State.Idle:
                            this.ComboBoxLanguages.Enabled = true;
                            this.ComboBoxRegions.Enabled = true;
                            this.ButtonDownload.Enabled = true;
                            this.ButtonDownload.Text = StringLoader.GetText("button_download_translation");
                            this.ButtonPlay.Enabled = true;
                            this.ButtonPlay.Text = StringLoader.GetText("button_play");
                            this.ToolStripMenuItemStartRaw.Enabled = true;
                            this.ForceToolStripMenuItem.Enabled = true;
                            this.RefreshToolStripMenuItem.Enabled = true;
                            this.ToolStripStatusLabel.Text = StringLoader.GetText("form_status_idle");
                            this.ToolStripProgressBar.Value = this.ToolStripProgressBar.Minimum;
                            this.ToolStripProgressBar.Style = ProgressBarStyle.Blocks;

                            break;
                        case State.Download:
                            this.ComboBoxLanguages.Enabled = false;
                            this.ComboBoxRegions.Enabled = false;
                            this.ButtonDownload.Enabled = true;
                            this.ButtonDownload.Text = StringLoader.GetText("button_cancel");
                            this.ButtonPlay.Enabled = false;
                            this.ButtonPlay.Text = StringLoader.GetText("button_play");
                            this.ToolStripMenuItemStartRaw.Enabled = false;
                            this.ForceToolStripMenuItem.Enabled = false;
                            this.RefreshToolStripMenuItem.Enabled = false;
                            this.ToolStripStatusLabel.Text = StringLoader.GetText("form_status_download");
                            this.ToolStripProgressBar.Value = this.ToolStripProgressBar.Minimum;
                            this.ToolStripProgressBar.Style = ProgressBarStyle.Blocks;

                            break;
                        case State.Patch:
                            this.ComboBoxLanguages.Enabled = false;
                            this.ComboBoxRegions.Enabled = false;
                            this.ButtonDownload.Enabled = true;
                            this.ButtonDownload.Text = StringLoader.GetText("button_cancel");
                            this.ButtonPlay.Enabled = false;
                            this.ButtonPlay.Text = StringLoader.GetText("button_play");
                            this.ToolStripMenuItemStartRaw.Enabled = false;
                            this.ForceToolStripMenuItem.Enabled = false;
                            this.RefreshToolStripMenuItem.Enabled = false;
                            this.ToolStripStatusLabel.Text = StringLoader.GetText("form_status_patch");
                            this.ToolStripProgressBar.Value = this.ToolStripProgressBar.Minimum;
                            this.ToolStripProgressBar.Style = ProgressBarStyle.Blocks;

                            break;
                        case State.Prepare:
                            this.ComboBoxLanguages.Enabled = false;
                            this.ComboBoxRegions.Enabled = false;
                            this.ButtonDownload.Enabled = false;
                            this.ButtonDownload.Text = StringLoader.GetText("button_download_translation");
                            this.ButtonPlay.Enabled = false;
                            this.ButtonPlay.Text = StringLoader.GetText("button_play");
                            this.ToolStripMenuItemStartRaw.Enabled = false;
                            this.ForceToolStripMenuItem.Enabled = false;
                            this.RefreshToolStripMenuItem.Enabled = false;
                            this.ToolStripStatusLabel.Text = StringLoader.GetText("form_status_prepare");
                            this.ToolStripProgressBar.Value = this.ToolStripProgressBar.Minimum;
                            this.ToolStripProgressBar.Style = ProgressBarStyle.Marquee;

                            break;
                        case State.WaitClient:
                            this.ComboBoxLanguages.Enabled = false;
                            this.ComboBoxRegions.Enabled = false;
                            this.ButtonDownload.Enabled = false;
                            this.ButtonDownload.Text = StringLoader.GetText("button_download_translation");
                            this.ButtonPlay.Enabled = true;
                            this.ButtonPlay.Text = StringLoader.GetText("button_cancel");
                            this.ToolStripMenuItemStartRaw.Enabled = false;
                            this.ForceToolStripMenuItem.Enabled = false;
                            this.RefreshToolStripMenuItem.Enabled = false;
                            this.ToolStripStatusLabel.Text = StringLoader.GetText("form_status_wait_client");
                            this.ToolStripProgressBar.Value = this.ToolStripProgressBar.Minimum;
                            this.ToolStripProgressBar.Style = ProgressBarStyle.Blocks;

                            break;
                        case State.WaitClose:
                            this.ComboBoxLanguages.Enabled = false;
                            this.ComboBoxRegions.Enabled = false;
                            this.ButtonDownload.Enabled = false;
                            this.ButtonDownload.Text = StringLoader.GetText("button_download_translation");
                            this.ButtonPlay.Enabled = false;
                            this.ButtonPlay.Text = StringLoader.GetText("button_play");
                            this.ToolStripMenuItemStartRaw.Enabled = false;
                            this.ForceToolStripMenuItem.Enabled = false;
                            this.RefreshToolStripMenuItem.Enabled = false;
                            this.ToolStripStatusLabel.Text = StringLoader.GetText("form_status_wait_close");
                            this.ToolStripProgressBar.Value = this.ToolStripProgressBar.Minimum;
                            this.ToolStripProgressBar.Style = ProgressBarStyle.Marquee;
                            this.WindowState = FormWindowState.Minimized;

                            break;
                        case State.RTPatch:
                            this.ComboBoxLanguages.Enabled = false;
                            this.ComboBoxRegions.Enabled = false;
                            this.ButtonDownload.Enabled = true;
                            this.ButtonDownload.Text = StringLoader.GetText("button_cancel");
                            this.ButtonPlay.Enabled = false;
                            this.ButtonPlay.Text = StringLoader.GetText("button_play");
                            this.ToolStripMenuItemStartRaw.Enabled = false;
                            this.ForceToolStripMenuItem.Enabled = false;
                            this.RefreshToolStripMenuItem.Enabled = false;
                            this.ToolStripStatusLabel.Text = StringLoader.GetText("form_status_update_client");
                            this.ToolStripProgressBar.Value = this.ToolStripProgressBar.Minimum;
                            this.ToolStripProgressBar.Style = ProgressBarStyle.Blocks;

                            break;
                        case State.RegionNotInstalled:
                            this.ComboBoxLanguages.Enabled = false;
                            this.ButtonDownload.Enabled = false;
                            this.ButtonDownload.Text = StringLoader.GetText("button_download_translation");
                            this.ButtonPlay.Enabled = false;
                            this.ButtonPlay.Text = StringLoader.GetText("button_play");
                            this.ToolStripMenuItemStartRaw.Enabled = false;
                            this.ForceToolStripMenuItem.Enabled = false;
                            this.RefreshToolStripMenuItem.Enabled = false;
                            this.ToolStripStatusLabel.Text = StringLoader.GetText("form_status_idle");
                            this.ToolStripProgressBar.Value = this.ToolStripProgressBar.Minimum;
                            this.ToolStripProgressBar.Style = ProgressBarStyle.Blocks;

                            break;
                    }

                    Logger.Info($"State=[{value}]");
                    this.ComboBoxLanguages_SelectionChangeCommitted(this, EventArgs.Empty);
                    this._state = value;
                }
            }
        }

        internal MainForm()
        {
            this.Downloader = new Downloader();
            this.Downloader.DownloaderProgressChanged += this.Downloader_DownloaderProgressChanged;
            this.Downloader.DownloaderCompleted += this.Downloader_DownloaderCompleted;

            this.Patcher = new Patcher();
            this.Patcher.PatcherProgressChanged += this.Patcher_PatcherProgressChanged;
            this.Patcher.PatcherCompleted += this.Patcher_PatcherCompleted;

            this.RTPatcher = new RTPatcher();
            this.RTPatcher.RTPatcherDownloadProgressChanged += this.RTPatcher_DownloadProgressChanged;
            this.RTPatcher.RTPatcherProgressChanged += this.RTPatcher_ProgressChanged;
            this.RTPatcher.RTPatcherCompleted += this.RTPatcher_Completed;

            this.GameStarter = new GameStarter();
            this.GameStarter.GameStarterProgressChanged += this.GameStarter_GameStarterProgressChanged;
            this.GameStarter.GameStarterCompleted += this.GameStarter_GameStarterCompleted;

            InitializeComponent();
            InitializeTextComponent();

            Logger.Info($"[{this.Text}] starting in UI Language [{UserSettings.UILanguageCode}]; Patcher Folder [{UserSettings.PatcherPath}]");
        }

        private void InitializeTextComponent()
        {
            this.MenuToolStripMenuItem.Text = StringLoader.GetText("form_menu");
            this.ForceToolStripMenuItem.Text = StringLoader.GetText("form_force_patch");
            this.OpenSWWebpageToolStripMenuItem.Text = StringLoader.GetText("form_open_sw_webpage");
            this.UploadLogToPastebinToolStripMenuItem.Text = StringLoader.GetText("form_upload_log");
            this.SettingsToolStripMenuItem.Text = StringLoader.GetText("form_settings");
            this.RefreshToolStripMenuItem.Text = StringLoader.GetText("form_refresh");
            this.AboutToolStripMenuItem.Text = StringLoader.GetText("form_about");
            this.LabelRegionPick.Text = StringLoader.GetText("form_region_pick");
            this.LabelLanguagePick.Text = StringLoader.GetText("form_language_pick");
            this.ButtonDownload.Text = StringLoader.GetText("button_download_translation");
            this.ButtonPlay.Text = StringLoader.GetText("button_play");
            this.ButtonExit.Text = StringLoader.GetText("button_exit");
            this.NotifyIcon.BalloonTipText = StringLoader.GetText("notify_balloon_text");
            this.NotifyIcon.BalloonTipTitle = StringLoader.GetText("notify_balloon_title");
            this.NotifyIcon.Text = StringLoader.GetText("notify_text");
            this.ToolStripMenuItemStartRaw.Text = StringLoader.GetText("button_play_raw");
            this.Text = AssemblyAccessor.Title + " " + AssemblyAccessor.Version;
        }

        private void Downloader_DownloaderProgressChanged(object sender, DownloaderProgressChangedEventArgs e)
        {
            if (this.CurrentState == State.Download)
            {
                this.ToolStripStatusLabel.Text = $"{StringLoader.GetText("form_status_download")} {e.FileName} ({e.FileNumber}/{e.FileCount})";
                this.ToolStripProgressBar.Value = e.Progress;
            }
        }

        private void Downloader_DownloaderCompleted(object sender, DownloaderCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                Logger.Debug($"{sender.ToString()} cancelled");
            }
            else if (e.Error != null)
            {
                Logger.Error(e.Error);
                MsgBox.Error(Methods.ExeptionParser(e.Error));
            }
            else
            {
                Logger.Debug($"{sender.ToString()} successfuly completed");
                this.CurrentState = State.Patch;
                this.Patcher.Run(e.Language);

                return;
            }

            this.CurrentState = State.Idle;
        }

        private void Patcher_PatcherProgressChanged(object sender, PatcherProgressChangedEventArgs e)
        {
            if (this.CurrentState == State.Patch)
            {
                switch (e.PatcherState)
                {
                    case Patcher.State.Load:
                        this.ToolStripStatusLabel.Text = $"{StringLoader.GetText("form_status_patch")} {StringLoader.GetText("form_status_patch_load")}";
                        this.ToolStripProgressBar.Style = ProgressBarStyle.Marquee;

                        break;
                    case Patcher.State.Patch:
                        this.ToolStripStatusLabel.Text = $"{StringLoader.GetText("form_status_patch")} {StringLoader.GetText("form_status_patch_patch")}";
                        this.ToolStripProgressBar.Style = ProgressBarStyle.Blocks;
                        if (e.Progress != -1)
                        {
                            this.ToolStripProgressBar.Value = e.Progress;
                        }

                        break;
                    case Patcher.State.Save:
                        this.ToolStripStatusLabel.Text = $"{StringLoader.GetText("form_status_patch")} {StringLoader.GetText("form_status_patch_save")}";
                        this.ToolStripProgressBar.Style = ProgressBarStyle.Marquee;

                        break;
                    case Patcher.State.ExePatch:
                        this.ToolStripStatusLabel.Text = $"{StringLoader.GetText("form_status_patch")} {StringLoader.GetText("form_status_patch_exe")}";
                        this.ToolStripProgressBar.Style = ProgressBarStyle.Marquee;

                        break;
                }
            }
        }

        private void Patcher_PatcherCompleted(object sender, PatcherCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                Logger.Debug($"{sender.ToString()} cancelled");
                DeleteTmpFiles(e.Language);
            }
            else if (e.Error != null)
            {
                Logger.Error(e.Error);
                MsgBox.Error(Methods.ExeptionParser(e.Error));
                DeleteTmpFiles(e.Language);
            }
            else
            {
                Logger.Debug($"{sender.ToString()} successfuly completed");

                string clientIniPath = Path.Combine(UserSettings.GamePath, Strings.IniName.ClientVer);
                if (!Methods.LoadVerIni(out IniFile clientIni, clientIniPath))
                {
                    throw new Exception(StringLoader.GetText("exception_generic_read_error", clientIniPath));
                }
                IniSection clientVerSection = clientIni.Sections[Strings.IniName.Ver.Section];

                string translationIniPath = Path.Combine(e.Language.Path, Strings.IniName.Translation);
                IniFile translationIni = new IniFile();

                IniKey translationDateKey = new IniKey(translationIni, Strings.IniName.Patcher.KeyDate, Methods.DateToString(e.Language.LastUpdate));
                IniSection translationPatcherSection = new IniSection(translationIni, Strings.IniName.Patcher.Section, translationDateKey);

                translationIni.Sections.Add(translationPatcherSection);
                translationIni.Sections.Add(clientVerSection.Copy(translationIni));
                Logger.Debug($"Saving translation config to [{translationIniPath}]");
                translationIni.Save(translationIniPath);
            }

            SWFileManager.DisposeFileData();
            GC.Collect();
            this.CurrentState = State.Idle;
        }

        private void RTPatcher_DownloadProgressChanged(object sender, RTPatcherDownloadProgressChangedEventArgs e)
        {
            if (this.CurrentState == State.RTPatch)
            {
                if (e.Progress == -1)
                {
                    this.ToolStripStatusLabel.Text = $"{StringLoader.GetText("form_status_prepare")} {e.FileName}";
                    this.ToolStripProgressBar.Style = ProgressBarStyle.Marquee;
                }
                else
                {
                    this.ToolStripStatusLabel.Text = $"{StringLoader.GetText("form_status_update_client")} {e.FileName} - {e.DownloadSpeed}";
                    this.ToolStripProgressBar.Value = e.Progress;
                }
            }
        }

        private void RTPatcher_ProgressChanged(object sender, RTPatcherProgressChangedEventArgs e)
        {
            if (this.CurrentState == State.RTPatch)
            {
                if (e.Progress == -1)
                {
                    this.ToolStripStatusLabel.Text = $"{StringLoader.GetText("form_status_prepare")} {e.FileName} ({e.FileNumber}/{e.FileCount})";
                    this.ToolStripProgressBar.Style = ProgressBarStyle.Marquee;
                }
                else
                {
                    this.ToolStripStatusLabel.Text = $"{StringLoader.GetText("form_status_update_client")} {e.FileName} ({e.FileNumber}/{e.FileCount})";
                    this.ToolStripProgressBar.Style = ProgressBarStyle.Blocks;
                    this.ToolStripProgressBar.Value = e.Progress;
                }
            }
        }

        private void RTPatcher_Completed(object sender, RTPatcherCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                Logger.Debug($"{sender.ToString()} cancelled");
            }
            else if (e.Error != null)
            {
                if (e.Error is ResultException ex)
                {
                    string logFileName = Path.GetFileName(ex.LogPath);
                    switch (ex.Result)
                    {
                        case 4:
                            Logger.Error(ex.Message);
                            MsgBox.Error(StringLoader.GetText("exception_rtpatch_not_exist_directory"));
                            break;
                        case 7:
                            Logger.Error($"error=[{ex.Message}]@Version=[{ex.ClientVersion.ToString()}]");
                            MsgBox.Error(StringLoader.GetText("exception_rtpatch_error_open_patch_file"));
                            break;
                        case 9:
                            Logger.Error($"error=[{ex.Message}] file=[{Path.Combine(UserSettings.GamePath, ex.FileName)}] version=[{ex.ClientVersion}]");
                            MsgBox.Error(StringLoader.GetText("exception_rtpatch_corrupt", $"{Path.Combine(UserSettings.GamePath, ex.FileName)}@Version=[{ex.ClientVersion}]"));
                            break;
                        case 15:
                            Logger.Error($"error=[{ex.Message}] file=[{Path.Combine(UserSettings.GamePath, ex.FileName)}] version=[{ex.ClientVersion}]");
                            MsgBox.Error(StringLoader.GetText("exception_rtpatch_missing_file", $"{Path.Combine(UserSettings.GamePath, ex.FileName)}@Version=[{ex.ClientVersion}]"));
                            break;
                        case 18:
                            Logger.Error($"error=[{ex.Message}]@Version=[{ex.ClientVersion.ToString()}]");
                            MsgBox.Error(StringLoader.GetText("exception_rtpatch_open_patch_file_fail"));
                            break;
                        case 20:
                            Logger.Error($"error=[{ex.Message}]@Version=[{ex.ClientVersion.ToString()}]");
                            MsgBox.Error(StringLoader.GetText("exception_rtpatch_read_patch_file_fail"));
                            break;
                        case 22:
                            Logger.Error($"error=[{ex.Message}]@Version=[{ex.ClientVersion.ToString()}]");
                            MsgBox.Error(StringLoader.GetText("exception_rtpatch_rename_fail"));
                            break;
                        case 29:
                            Logger.Error($"error=[{ex.Message}]@Version=[{ex.ClientVersion.ToString()}]");
                            MsgBox.Error(StringLoader.GetText("exception_rtpatch_insufficient_storage"));
                            break;
                        case 32:
                            Logger.Error($"error=[{ex.Message}]@Version=[{ex.ClientVersion.ToString()}]");
                            MsgBox.Error(StringLoader.GetText("exception_rtpatch_time_date_fail"));
                            break;
                        case 36:
                            Logger.Error($"error=[{ex.Message}] file=[{Path.Combine(UserSettings.GamePath, ex.FileName)}] version=[{ex.ClientVersion}]");
                            MsgBox.Error(StringLoader.GetText("exception_rtpatch_corrupt_file", $"{Path.Combine(UserSettings.GamePath, ex.FileName)}@Version=[{ex.ClientVersion}]"));
                            break;
                        case 49:
                            Logger.Error($"error=[{ex.Message}]@Version=[{ex.ClientVersion.ToString()}]");
                            MsgBox.Error(StringLoader.GetText("exception_rtpatch_administrator_required"));
                            break;
                        default:
#if !DEBUG
                            string logFileText = File.ReadAllText(ex.LogPath);

                            try
                            {
                                UploadToPasteBin(logFileName, logFileText, PasteBinExpiration.OneWeek, true, "text");
                            }
                            catch (PasteBinApiException)
                            {

                            }

                            Logger.Error($"See {logFileName} for details. Error Code=[{ex.Result}]");
                            MsgBox.Error(StringLoader.GetText("exception_rtpatch_result", ex.Result, logFileName));
#endif
                            break;
                    }

                    Methods.RTPatchCleanup(true);
                }
                else
                {
                    Methods.RTPatchCleanup(false);
                    Logger.Error(e.Error);
                    MsgBox.Error(Methods.ExeptionParser(e.Error));
                }
            }
            else
            {
                Methods.RTPatchCleanup(true);
                Logger.Debug($"{sender.ToString()} successfuly completed");
                switch (this._nextState)
                {
                    case NextState.Download:
                        this.CurrentState = State.Download;
                        this.Downloader.Run(e.Language);

                        break;
                    case NextState.Play:
                        this.GameStarter.Run(e.Language, true);

                        break;
                    case NextState.PlayRaw:
                        this.GameStarter.Run(e.Language, false);

                        break;
                }

                this._nextState = 0;
                return;
            }

            this.CurrentState = State.Idle;
        }

        private void GameStarter_GameStarterProgressChanged(object sender, GameStarterProgressChangedEventArgs e)
        {
            this.CurrentState = e.State;
        }

        private void GameStarter_GameStarterCompleted(object sender, GameStarterCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                Logger.Debug($"{sender.ToString()} cancelled.");
            }
            else if (e.Error != null)
            {
                Logger.Error(e.Error);
                MsgBox.Error(e.Error.Message);
            }
            else if (e.NeedsForcePatch)
            {
                MsgBox.Notice(StringLoader.GetText("notice_outdated_translation"));
                ResetTranslation(e.Language);

                this.CurrentState = State.RTPatch;
                this._nextState = NextState.Download;
                this.RTPatcher.Run(e.Language);

                return;
            }
            else
            {
                Logger.Debug($"{sender.ToString()} successfuly completed");
                this.RestoreFromTray();
            }

            try
            {
                RestoreBackup(e.Language);
            }
            finally
            {
                this.CurrentState = State.Idle;
            }
        }
    }
}
