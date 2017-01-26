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

using MadMilkman.Ini;
using SWPatcher.Downloading;
using SWPatcher.General;
using SWPatcher.Helpers;
using SWPatcher.Helpers.GlobalVariables;
using SWPatcher.Launching;
using SWPatcher.Patching;
using SWPatcher.RTPatch;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace SWPatcher.Forms
{
    partial class MainForm : Form
    {
        public enum State
        {
            Idle = 0,
            Download,
            Patch,
            Prepare,
            WaitClient,
            WaitClose,
            RTPatch
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

        public State CurrentState
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
                            this.comboBoxLanguages.Enabled = true;
                            this.buttonDownload.Enabled = true;
                            this.buttonDownload.Text = StringLoader.GetText("button_download_translation");
                            this.buttonPlay.Enabled = true;
                            this.buttonPlay.Text = StringLoader.GetText("button_play");
                            this.toolStripMenuItemStartRaw.Enabled = true;
                            this.forceStripMenuItem.Enabled = true;
                            this.refreshToolStripMenuItem.Enabled = true;
                            this.toolStripStatusLabel.Text = StringLoader.GetText("form_status_idle");
                            this.toolStripProgressBar.Value = this.toolStripProgressBar.Minimum;
                            this.toolStripProgressBar.Style = ProgressBarStyle.Blocks;
                            break;
                        case State.Download:
                            this.comboBoxLanguages.Enabled = false;
                            this.buttonDownload.Enabled = true;
                            this.buttonDownload.Text = StringLoader.GetText("button_cancel");
                            this.buttonPlay.Enabled = false;
                            this.buttonPlay.Text = StringLoader.GetText("button_play");
                            this.toolStripMenuItemStartRaw.Enabled = false;
                            this.forceStripMenuItem.Enabled = false;
                            this.refreshToolStripMenuItem.Enabled = false;
                            this.toolStripStatusLabel.Text = StringLoader.GetText("form_status_download");
                            this.toolStripProgressBar.Value = this.toolStripProgressBar.Minimum;
                            this.toolStripProgressBar.Style = ProgressBarStyle.Blocks;
                            break;
                        case State.Patch:
                            this.comboBoxLanguages.Enabled = false;
                            this.buttonDownload.Enabled = true;
                            this.buttonDownload.Text = StringLoader.GetText("button_cancel");
                            this.buttonPlay.Enabled = false;
                            this.buttonPlay.Text = StringLoader.GetText("button_play");
                            this.toolStripMenuItemStartRaw.Enabled = false;
                            this.forceStripMenuItem.Enabled = false;
                            this.refreshToolStripMenuItem.Enabled = false;
                            this.toolStripStatusLabel.Text = StringLoader.GetText("form_status_patch");
                            this.toolStripProgressBar.Value = this.toolStripProgressBar.Minimum;
                            this.toolStripProgressBar.Style = ProgressBarStyle.Blocks;
                            break;
                        case State.Prepare:
                            this.comboBoxLanguages.Enabled = false;
                            this.buttonDownload.Enabled = false;
                            this.buttonDownload.Text = StringLoader.GetText("button_download_translation");
                            this.buttonPlay.Enabled = false;
                            this.buttonPlay.Text = StringLoader.GetText("button_play");
                            this.toolStripMenuItemStartRaw.Enabled = false;
                            this.forceStripMenuItem.Enabled = false;
                            this.refreshToolStripMenuItem.Enabled = false;
                            this.toolStripStatusLabel.Text = StringLoader.GetText("form_status_prepare");
                            this.toolStripProgressBar.Value = this.toolStripProgressBar.Minimum;
                            this.toolStripProgressBar.Style = ProgressBarStyle.Marquee;
                            break;
                        case State.WaitClient:
                            this.comboBoxLanguages.Enabled = false;
                            this.buttonDownload.Enabled = false;
                            this.buttonDownload.Text = StringLoader.GetText("button_download_translation");
                            this.buttonPlay.Enabled = true;
                            this.buttonPlay.Text = StringLoader.GetText("button_cancel");
                            this.toolStripMenuItemStartRaw.Enabled = false;
                            this.forceStripMenuItem.Enabled = false;
                            this.refreshToolStripMenuItem.Enabled = false;
                            this.toolStripStatusLabel.Text = StringLoader.GetText("form_status_wait_client");
                            this.toolStripProgressBar.Value = this.toolStripProgressBar.Minimum;
                            this.toolStripProgressBar.Style = ProgressBarStyle.Blocks;
                            break;
                        case State.WaitClose:
                            this.comboBoxLanguages.Enabled = false;
                            this.buttonDownload.Enabled = false;
                            this.buttonDownload.Text = StringLoader.GetText("button_download_translation");
                            this.buttonPlay.Enabled = false;
                            this.buttonPlay.Text = StringLoader.GetText("button_play");
                            this.toolStripMenuItemStartRaw.Enabled = false;
                            this.forceStripMenuItem.Enabled = false;
                            this.refreshToolStripMenuItem.Enabled = false;
                            this.toolStripStatusLabel.Text = StringLoader.GetText("form_status_wait_close");
                            this.toolStripProgressBar.Value = this.toolStripProgressBar.Minimum;
                            this.toolStripProgressBar.Style = ProgressBarStyle.Marquee;
                            this.WindowState = FormWindowState.Minimized;
                            break;
                        case State.RTPatch:
                            this.comboBoxLanguages.Enabled = false;
                            this.buttonDownload.Enabled = true;
                            this.buttonDownload.Text = StringLoader.GetText("button_cancel");
                            this.buttonPlay.Enabled = false;
                            this.buttonPlay.Text = StringLoader.GetText("button_play");
                            this.toolStripMenuItemStartRaw.Enabled = false;
                            this.forceStripMenuItem.Enabled = false;
                            this.refreshToolStripMenuItem.Enabled = false;
                            this.toolStripStatusLabel.Text = StringLoader.GetText("form_status_update_client");
                            this.toolStripProgressBar.Value = this.toolStripProgressBar.Minimum;
                            this.toolStripProgressBar.Style = ProgressBarStyle.Blocks;
                            break;
                    }

                    Logger.Info($"State=[{value}]");
                    this.ComboBoxLanguages_SelectedIndexChanged(this, null);
                    this._state = value;
                }
            }
        }

        public MainForm()
        {
            this.Downloader = new Downloader();
            this.Downloader.DownloaderProgressChanged += new DownloaderProgressChangedEventHandler(this.Downloader_DownloaderProgressChanged);
            this.Downloader.DownloaderCompleted += new DownloaderCompletedEventHandler(this.Downloader_DownloaderCompleted);
            this.Patcher = new Patcher();
            this.Patcher.PatcherProgressChanged += new PatcherProgressChangedEventHandler(this.Patcher_PatcherProgressChanged);
            this.Patcher.PatcherCompleted += new PatcherCompletedEventHandler(this.Patcher_PatcherCompleted);
            this.RTPatcher = new RTPatcher();
            this.RTPatcher.RTPatcherDownloadProgressChanged += this.RTPatcher_DownloadProgressChanged;
            this.RTPatcher.RTPatcherProgressChanged += this.RTPatcher_ProgressChanged;
            this.RTPatcher.RTPatcherCompleted += this.RTPatcher_Completed;
            this.GameStarter = new GameStarter();
            this.GameStarter.GameStarterProgressChanged += this.GameStarter_GameStarterProgressChanged;
            this.GameStarter.GameStarterCompleted += this.GameStarter_GameStarterCompleted;
            InitializeComponent();
            InitializeTextComponent();
            Logger.Info($"[{this.Text}] starting in UI Language=[{UserSettings.UILanguageCode}]");
        }

        private void InitializeTextComponent()
        {
            this.menuToolStripMenuItem.Text = StringLoader.GetText("form_menu");
            this.settingsToolStripMenuItem.Text = StringLoader.GetText("form_settings");
            this.refreshToolStripMenuItem.Text = StringLoader.GetText("form_refresh");
            this.aboutToolStripMenuItem.Text = StringLoader.GetText("form_about");
            this.forceStripMenuItem.Text = StringLoader.GetText("form_force_patch");
            this.openSWWebpageToolStripMenuItem.Text = StringLoader.GetText("form_open_sw_webpage");
            this.uploadLogToPastebinToolStripMenuItem.Text = StringLoader.GetText("form_upload_log");
            this.buttonDownload.Text = StringLoader.GetText("button_download_translation");
            this.buttonPlay.Text = StringLoader.GetText("button_play");
            this.buttonExit.Text = StringLoader.GetText("button_exit");
            this.notifyIcon.BalloonTipText = StringLoader.GetText("notify_balloon_text");
            this.notifyIcon.BalloonTipTitle = StringLoader.GetText("notify_balloon_title");
            this.notifyIcon.Text = StringLoader.GetText("notify_text");
            this.toolStripMenuItemStartRaw.Text = StringLoader.GetText("button_play_raw");
            this.Text = AssemblyAccessor.Title + " " + AssemblyAccessor.Version;
        }

        private void Downloader_DownloaderProgressChanged(object sender, DownloaderProgressChangedEventArgs e)
        {
            if (this.CurrentState == State.Download)
            {
                this.toolStripStatusLabel.Text = $"{StringLoader.GetText("form_status_download")} {e.FileName} ({e.FileNumber}/{e.FileCount})";
                this.toolStripProgressBar.Value = e.Progress;
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
                MsgBox.Error(Logger.ExeptionParser(e.Error));
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
                        this.toolStripStatusLabel.Text = $"{StringLoader.GetText("form_status_patch")} {StringLoader.GetText("form_status_patch_load")}";
                        this.toolStripProgressBar.Style = ProgressBarStyle.Marquee;

                        break;
                    case Patcher.State.Patch:
                        this.toolStripStatusLabel.Text = $"{StringLoader.GetText("form_status_patch")} {StringLoader.GetText("form_status_patch_patch")}";
                        this.toolStripProgressBar.Style = ProgressBarStyle.Blocks;
                        if (e.Progress != -1)
                        {
                            this.toolStripProgressBar.Value = e.Progress;
                        }

                        break;
                    case Patcher.State.Save:
                        this.toolStripStatusLabel.Text = $"{StringLoader.GetText("form_status_patch")} {StringLoader.GetText("form_status_patch_save")}";
                        this.toolStripProgressBar.Style = ProgressBarStyle.Marquee;

                        break;
                    case Patcher.State.ExePatch:
                        this.toolStripStatusLabel.Text = $"{StringLoader.GetText("form_status_patch")} {StringLoader.GetText("form_status_patch_exe")}";
                        this.toolStripProgressBar.Style = ProgressBarStyle.Marquee;

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
                MsgBox.Error(Logger.ExeptionParser(e.Error));
                DeleteTmpFiles(e.Language);
            }
            else
            {
                Logger.Debug($"{sender.ToString()} successfuly completed");
                IniFile ini = new IniFile(new IniOptions
                {
                    KeyDuplicate = IniDuplication.Ignored,
                    SectionDuplicate = IniDuplication.Ignored
                });
                ini.Load(Path.Combine(UserSettings.GamePath, Strings.IniName.ClientVer));
                string clientVer = ini.Sections[Strings.IniName.Ver.Section].Keys[Strings.IniName.Ver.Key].Value;

                string iniPath = Path.Combine(e.Language.Name, Strings.IniName.Translation);
                if (!File.Exists(iniPath))
                {
                    File.Create(iniPath).Dispose();
                }

                ini.Sections.Clear();
                ini.Load(iniPath);
                ini.Sections.Add(Strings.IniName.Patcher.Section);
                ini.Sections[Strings.IniName.Patcher.Section].Keys.Add(Strings.IniName.Patcher.KeyDate);
                ini.Sections[Strings.IniName.Patcher.Section].Keys[Strings.IniName.Patcher.KeyDate].Value = Methods.DateToString(e.Language.LastUpdate);
                ini.Sections[Strings.IniName.Patcher.Section].Keys.Add(Strings.IniName.Patcher.KeyVer);
                ini.Sections[Strings.IniName.Patcher.Section].Keys[Strings.IniName.Patcher.KeyVer].Value = clientVer;
                ini.Sections[Strings.IniName.Patcher.Section].Keys.Add(Strings.IniName.Patcher.KeyRegion);
                ini.Sections[Strings.IniName.Patcher.Section].Keys[Strings.IniName.Patcher.KeyRegion].Value = UserSettings.ClientRegion.ToString();
                ini.Save(iniPath);
            }

            SWFileManager.DisposeFileData();
            GC.Collect();
            this.CurrentState = State.Idle;
        }

        private void RTPatcher_DownloadProgressChanged(object sender, RTPatcherDownloadProgressChangedEventArgs e)
        {
            if (this.CurrentState == State.RTPatch)
            {
                this.toolStripStatusLabel.Text = $"{StringLoader.GetText("form_status_update_client")} {e.FileName} - {e.DownloadSpeed}";
                this.toolStripProgressBar.Value = e.Progress;
            }
        }

        private void RTPatcher_ProgressChanged(object sender, RTPatcherProgressChangedEventArgs e)
        {
            if (this.CurrentState == State.RTPatch)
            {
                if (e.Progress == -1)
                {
                    this.toolStripStatusLabel.Text = $"{StringLoader.GetText("form_status_prepare")} {e.FileName} ({e.FileNumber}/{e.FileCount})";
                    this.toolStripProgressBar.Style = ProgressBarStyle.Marquee;
                }
                else
                {
                    this.toolStripStatusLabel.Text = $"{StringLoader.GetText("form_status_update_client")} {e.FileName} ({e.FileNumber}/{e.FileCount})";
                    this.toolStripProgressBar.Style = ProgressBarStyle.Blocks;
                    this.toolStripProgressBar.Value = e.Progress;
                }
            }
        }

        private void RTPatcher_Completed(object sender, AsyncCompletedEventArgs e)
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
                    MsgBox.Error(Logger.ExeptionParser(e.Error));
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
                        this.Downloader.Run(this.comboBoxLanguages.SelectedItem as Language);

                        break;
                    case NextState.Play:
                        this.GameStarter.Run(this.comboBoxLanguages.SelectedItem as Language);

                        break;
                    case NextState.PlayRaw:
                        this.GameStarter.Run();

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

        private void GameStarter_GameStarterCompleted(object sender, RunWorkerCompletedEventArgs e)
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
            else if (e.Result != null && Convert.ToBoolean(e.Result))
            {
                MsgBox.Notice(StringLoader.GetText("notice_outdated_translation"));
                ForceStripMenuItem_Click(sender, e);

                return;
            }
            else
            {
                Logger.Debug($"{sender.ToString()} successfuly completed");
                this.RestoreFromTray();
            }

            try
            {
                RestoreBackup(this.comboBoxLanguages.SelectedItem as Language);
            }
            finally
            {
                this.CurrentState = State.Idle;
            }
        }
    }
}
