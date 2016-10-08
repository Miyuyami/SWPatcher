using MadMilkman.Ini;
using Microsoft.Win32;
using SWPatcherTest.Downloading;
using SWPatcherTest.General;
using SWPatcherTest.Helpers;
using SWPatcherTest.Helpers.GlobalVariables;
using SWPatcherTest.Patching;
using SWPatcherTest.RTPatch;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web;
using System.Windows.Forms;

namespace SWPatcherTest.Forms
{
    public partial class MainForm : Form
    {
        public enum State
        {
            Idle = 0,
            Download,
            Patch,
            Prepare,
            WaitClient,
            Apply,
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

        private IContainer components = null;
        private State _state;
        private NextState _nextState;
        private readonly Downloader Downloader;
        private readonly Patcher Patcher;
        private readonly BackgroundWorker Worker;
        private readonly RTPatcher RTPatcher;
        private readonly List<SWFile> SWFiles;

        public State CurrentState
        {
            get
            {
                return _state;
            }
            private set
            {
                if (_state != value)
                {
                    switch (value)
                    {
                        case State.Idle:
                            comboBoxLanguages.Enabled = true;
                            buttonDownload.Enabled = true;
                            buttonDownload.Text = StringLoader.GetText("button_download_translation");
                            buttonPlay.Enabled = true;
                            buttonPlay.Text = StringLoader.GetText("button_play");
                            toolStripMenuItemStartRaw.Enabled = true;
                            forceStripMenuItem.Enabled = true;
                            refreshToolStripMenuItem.Enabled = true;
                            toolStripStatusLabel.Text = StringLoader.GetText("form_status_idle");
                            toolStripProgressBar.Value = toolStripProgressBar.Minimum;
                            toolStripProgressBar.Style = ProgressBarStyle.Blocks;
                            break;
                        case State.Download:
                            comboBoxLanguages.Enabled = false;
                            buttonDownload.Enabled = true;
                            buttonDownload.Text = StringLoader.GetText("button_cancel");
                            buttonPlay.Enabled = false;
                            buttonPlay.Text = StringLoader.GetText("button_play");
                            toolStripMenuItemStartRaw.Enabled = false;
                            forceStripMenuItem.Enabled = false;
                            refreshToolStripMenuItem.Enabled = false;
                            toolStripStatusLabel.Text = StringLoader.GetText("form_status_download");
                            toolStripProgressBar.Value = toolStripProgressBar.Minimum;
                            toolStripProgressBar.Style = ProgressBarStyle.Blocks;
                            break;
                        case State.Patch:
                            comboBoxLanguages.Enabled = false;
                            buttonDownload.Enabled = true;
                            buttonDownload.Text = StringLoader.GetText("button_cancel");
                            buttonPlay.Enabled = false;
                            buttonPlay.Text = StringLoader.GetText("button_play");
                            toolStripMenuItemStartRaw.Enabled = false;
                            forceStripMenuItem.Enabled = false;
                            refreshToolStripMenuItem.Enabled = false;
                            toolStripStatusLabel.Text = StringLoader.GetText("form_status_patch");
                            toolStripProgressBar.Value = toolStripProgressBar.Minimum;
                            toolStripProgressBar.Style = ProgressBarStyle.Blocks;
                            break;
                        case State.Prepare:
                            comboBoxLanguages.Enabled = false;
                            buttonDownload.Enabled = false;
                            buttonDownload.Text = StringLoader.GetText("button_download_translation");
                            buttonPlay.Enabled = false;
                            buttonPlay.Text = StringLoader.GetText("button_play");
                            toolStripMenuItemStartRaw.Enabled = false;
                            forceStripMenuItem.Enabled = false;
                            refreshToolStripMenuItem.Enabled = false;
                            toolStripStatusLabel.Text = StringLoader.GetText("form_status_prepare");
                            toolStripProgressBar.Value = toolStripProgressBar.Minimum;
                            toolStripProgressBar.Style = ProgressBarStyle.Marquee;
                            break;
                        case State.WaitClient:
                            comboBoxLanguages.Enabled = false;
                            buttonDownload.Enabled = false;
                            buttonDownload.Text = StringLoader.GetText("button_download_translation");
                            buttonPlay.Enabled = true;
                            buttonPlay.Text = StringLoader.GetText("button_cancel");
                            toolStripMenuItemStartRaw.Enabled = false;
                            forceStripMenuItem.Enabled = false;
                            refreshToolStripMenuItem.Enabled = false;
                            toolStripStatusLabel.Text = StringLoader.GetText("form_status_wait_client");
                            toolStripProgressBar.Value = toolStripProgressBar.Minimum;
                            toolStripProgressBar.Style = ProgressBarStyle.Blocks;
                            break;
                        case State.Apply:
                            comboBoxLanguages.Enabled = false;
                            buttonDownload.Enabled = false;
                            buttonDownload.Text = StringLoader.GetText("button_download_translation");
                            buttonPlay.Enabled = false;
                            buttonPlay.Text = StringLoader.GetText("button_play");
                            toolStripMenuItemStartRaw.Enabled = false;
                            forceStripMenuItem.Enabled = false;
                            refreshToolStripMenuItem.Enabled = false;
                            toolStripStatusLabel.Text = StringLoader.GetText("form_status_apply");
                            toolStripProgressBar.Value = toolStripProgressBar.Minimum;
                            toolStripProgressBar.Style = ProgressBarStyle.Marquee;
                            break;
                        case State.WaitClose:
                            comboBoxLanguages.Enabled = false;
                            buttonDownload.Enabled = false;
                            buttonDownload.Text = StringLoader.GetText("button_download_translation");
                            buttonPlay.Enabled = false;
                            buttonPlay.Text = StringLoader.GetText("button_play");
                            toolStripMenuItemStartRaw.Enabled = false;
                            forceStripMenuItem.Enabled = false;
                            refreshToolStripMenuItem.Enabled = false;
                            toolStripStatusLabel.Text = StringLoader.GetText("form_status_wait_close");
                            toolStripProgressBar.Value = toolStripProgressBar.Minimum;
                            toolStripProgressBar.Style = ProgressBarStyle.Marquee;
                            WindowState = FormWindowState.Minimized;
                            break;
                        case State.RTPatch:
                            comboBoxLanguages.Enabled = false;
                            buttonDownload.Enabled = true;
                            buttonDownload.Text = StringLoader.GetText("button_cancel");
                            buttonPlay.Enabled = false;
                            buttonPlay.Text = StringLoader.GetText("button_play");
                            toolStripMenuItemStartRaw.Enabled = false;
                            forceStripMenuItem.Enabled = false;
                            refreshToolStripMenuItem.Enabled = false;
                            toolStripStatusLabel.Text = StringLoader.GetText("form_status_update_client");
                            toolStripProgressBar.Value = toolStripProgressBar.Minimum;
                            toolStripProgressBar.Style = ProgressBarStyle.Blocks;
                            break;
                    }

                    Logger.Info($"State=[{value}]");
                    this.comboBoxLanguages_SelectedIndexChanged(this, null);
                    _state = value;
                }
            }
        }

        public MainForm()
        {
            this.SWFiles = new List<SWFile>();
            this.Downloader = new Downloader(SWFiles);
            this.Downloader.DownloaderProgressChanged += new DownloaderProgressChangedEventHandler(Downloader_DownloaderProgressChanged);
            this.Downloader.DownloaderCompleted += new DownloaderCompletedEventHandler(Downloader_DownloaderCompleted);
            this.Patcher = new Patcher(SWFiles);
            this.Patcher.PatcherProgressChanged += new PatcherProgressChangedEventHandler(Patcher_PatcherProgressChanged);
            this.Patcher.PatcherCompleted += new PatcherCompletedEventHandler(Patcher_PatcherCompleted);
            this.Worker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            this.Worker.DoWork += Worker_DoWork;
            this.Worker.ProgressChanged += Worker_ProgressChanged;
            this.Worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            this.RTPatcher = new RTPatcher();
            this.RTPatcher.RTPatcherDownloadProgressChanged += RTPatcher_DownloadProgressChanged;
            this.RTPatcher.RTPatcherProgressChanged += RTPatcher_ProgressChanged;
            this.RTPatcher.RTPatcherCompleted += RTPatcher_Completed;
            byte uiMode = UserSettings.InterfaceMode;
            switch (uiMode)
            {
                case 0:
                    InitializeComponentFull();
                    break;
                case 1:
                    InitializeComponentMinimal();
                    break;
            }
            InitializeTextComponent();
            this.Text = AssemblyAccessor.Title + " " + AssemblyAccessor.Version;
            Logger.Info($"[{this.Text}] starting in UI Mode=[{uiMode}] UI Language=[{UserSettings.UILanguageCode}]");
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
                if (e.Progress == -1)
                {
                    this.toolStripStatusLabel.Text = StringLoader.GetText("form_status_patch_exe");
                    this.toolStripProgressBar.Style = ProgressBarStyle.Marquee;
                }
                else
                {
                    this.toolStripStatusLabel.Text = $"{StringLoader.GetText("form_status_patch")} Step {e.FileNumber}/{e.FileCount}";
                    this.toolStripProgressBar.Value = e.Progress;
                }
            }
        }

        private void Patcher_PatcherCompleted(object sender, PatcherCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                Logger.Debug($"{sender.ToString()} cancelled");
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

                string iniPath = Path.Combine(e.Language.Lang, Strings.IniName.Translation);
                if (!File.Exists(iniPath))
                    File.Create(iniPath).Dispose();

                ini.Sections.Clear();
                ini.Load(iniPath);
                ini.Sections.Add(Strings.IniName.Patcher.Section);
                ini.Sections[Strings.IniName.Patcher.Section].Keys.Add(Strings.IniName.Pack.KeyDate);
                ini.Sections[Strings.IniName.Patcher.Section].Keys[Strings.IniName.Pack.KeyDate].Value = Methods.DateToString(e.Language.LastUpdate);
                ini.Sections[Strings.IniName.Patcher.Section].Keys.Add(Strings.IniName.Patcher.KeyVer);
                ini.Sections[Strings.IniName.Patcher.Section].Keys[Strings.IniName.Patcher.KeyVer].Value = clientVer;
                ini.Save(iniPath);
            }

            this.CurrentState = State.Idle;
        }

        private void RTPatcher_DownloadProgressChanged(object sender, RTPatcherDownloadProgressChangedEventArgs e)
        {
            if (this.CurrentState == State.RTPatch)
            {
                this.toolStripStatusLabel.Text = $"{StringLoader.GetText("form_status_update_client")} {e.FileName}";
                this.toolStripProgressBar.Value = e.Progress;
            }
        }

        private void RTPatcher_ProgressChanged(object sender, RTPatcherProgressChangedEventArgs e)
        {
            if (this.CurrentState == State.RTPatch)
            {
                this.toolStripStatusLabel.Text = $"{StringLoader.GetText("form_status_update_client")} {e.FileName} ({e.FileNumber}/{e.FileCount})";
                this.toolStripProgressBar.Value = e.Progress;
            }
        }

        private void RTPatcher_Completed(object sender, AsyncCompletedEventArgs e)
        {
            Methods.RTPatchCleanup();
            if (e.Cancelled)
            {
                Logger.Debug($"{sender.ToString()} cancelled");
            }
            else if (e.Error != null)
            {
                if (e.Error is ResultException)
                {
                    var ex = (ResultException)e.Error;
                    string logFileName = Path.GetFileName(ex.LogPath);
                    string logFileText = File.ReadAllText(ex.LogPath);

                    try
                    {
                        UploadToPasteBin(logFileName, logFileText, PasteBinExpiration.OneWeek, true, "text");
                    }
                    catch (PasteBinApiException)
                    {

                    }

                    Logger.Error($"See {logFileName} for details. Error Code=[{ex.Result}]");
                    MsgBox.Error(String.Format(StringLoader.GetText("exception_rtpatch_result"), ex.Result, logFileName));
                }
                else
                {
                    Logger.Error(e.Error);
                    MsgBox.Error(Logger.ExeptionParser(e.Error));
                }
            }
            else
            {
                Logger.Debug($"{sender.ToString()} successfuly completed");
                switch (this._nextState)
                {
                    case NextState.Download:
                        this.CurrentState = State.Download;
                        this.Downloader.Run(this.comboBoxLanguages.SelectedItem as Language);

                        break;
                    case NextState.Play:
                        this.Worker.RunWorkerAsync(this.comboBoxLanguages.SelectedItem as Language);

                        break;
                    case NextState.PlayRaw:
                        this.Worker.RunWorkerAsync();

                        break;
                }

                this._nextState = 0;
                return;
            }

            this.CurrentState = State.Idle;
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            this.Worker.ReportProgress((int)State.Prepare);
            if (e.Argument != null)
            {
                Language language = e.Argument as Language;

                Methods.SetSWFiles(this.SWFiles);

                if (IsTranslationOutdatedOrMissing(language, this.SWFiles))
                {
                    e.Result = true; // force patch = true
                    return;
                }

                if (UserSettings.WantToPatchExe)
                {
                    string gameExePath = Path.Combine(UserSettings.GamePath, Strings.FileName.GameExe);
                    string gameExePatchedPath = Path.Combine(UserSettings.PatcherPath, Strings.FileName.GameExe);
                    string backupFilePath = Path.Combine(Strings.FolderName.Backup, Strings.FileName.GameExe);
                    string backupFileDirectory = Path.GetDirectoryName(backupFilePath);

                    if (!File.Exists(gameExePatchedPath))
                    {
                        File.Copy(gameExePath, gameExePatchedPath);
                        Methods.PatchExeFile(gameExePatchedPath);

                        GC.Collect();
                    }

                    Directory.CreateDirectory(backupFileDirectory);

                    File.Move(gameExePath, backupFilePath);
                    File.Move(gameExePatchedPath, gameExePath);
                }

                Process clientProcess = null;
                ProcessStartInfo startInfo = null;
                if (UserSettings.WantToLogin)
                {
                    using (var client = new MyWebClient())
                    {
                        HangameLogin(client);
                        GetGameStartResponse(client);
                        string[] gameStartArgs = GetGameStartArguments(client);

                        startInfo = new ProcessStartInfo
                        {
                            UseShellExecute = true,
                            Verb = "runas",
                            Arguments = String.Join(" ", gameStartArgs.Select(s => "\"" + s + "\"")),
                            WorkingDirectory = UserSettings.GamePath,
                            FileName = Strings.FileName.GameExe
                        };
                    }
                }
                else
                {
                    this.Worker.ReportProgress((int)State.WaitClient);
                    while (true)
                    {
                        if (this.Worker.CancellationPending)
                        {
                            e.Cancel = true;
                            return;
                        }

                        clientProcess = GetProcess(Strings.FileName.GameExe);

                        if (clientProcess == null)
                            Thread.Sleep(1000);
                        else
                            break;
                    }
                }

                this.Worker.ReportProgress((int)State.Apply);
                BackupAndPlaceDataFiles(this.SWFiles, language);
                BackupAndPlaceOtherFiles(this.SWFiles, language);

                if (startInfo != null)
                    clientProcess = Process.Start(startInfo);

                this.Worker.ReportProgress((int)State.WaitClose);
                clientProcess.WaitForExit();
            }
            else
            {
                if (UserSettings.WantToLogin)
                {
                    ProcessStartInfo startInfo = null;
                    using (var client = new MyWebClient())
                    {
                        HangameLogin(client);
                        GetGameStartResponse(client);
                        string[] gameStartArgs = GetGameStartArguments(client);

                        startInfo = new ProcessStartInfo
                        {
                            UseShellExecute = true,
                            Verb = "runas",
                            Arguments = String.Join(" ", gameStartArgs.Select(s => "\"" + s + "\"")),
                            WorkingDirectory = UserSettings.GamePath,
                            FileName = Strings.FileName.GameExe
                        };
                    }

                    Process.Start(startInfo);
                    e.Cancel = true;
                }
                else
                {
                    throw new Exception(StringLoader.GetText("exception_not_login_option"));
                }
            }
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.CurrentState = (State)e.ProgressPercentage;
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
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
                forceStripMenuItem_Click(sender, e);

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

        public IEnumerable<string> GetComboBoxItemsAsString()
        {
            return this.comboBoxLanguages.Items.Cast<Language>().Select(s => s.Lang);
        }

        public void RestoreFromTray()
        {
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            this.Show();

            notifyIcon.Visible = false;
        }

        private static void StartupBackupCheck(Language language)
        {
            Methods.LogMethodFullName(System.Reflection.MethodBase.GetCurrentMethod());

            if (Directory.Exists(Strings.FolderName.Backup))
            {
                if (Directory.GetFiles(Strings.FolderName.Backup, "*", SearchOption.AllDirectories).Length > 0)
                {
                    var result = MsgBox.Question(String.Format(StringLoader.GetText("question_backup_files_found"), language.Lang));

                    if (result == DialogResult.Yes)
                        RestoreBackup(language);
                    else
                        Directory.Delete(Strings.FolderName.Backup, true);
                }
            }
            else
            {
                Directory.CreateDirectory(Strings.FolderName.Backup);
            }
        }

        private static void RestoreBackup(Language language)
        {
            Methods.LogMethodFullName(System.Reflection.MethodBase.GetCurrentMethod());

            if (!Directory.Exists(Strings.FolderName.Backup))
                return;

            string backupFilePath = Path.Combine(Strings.FolderName.Backup, Strings.FileName.GameExe);
            if (File.Exists(backupFilePath))
            {
                string gameExePath = Path.Combine(UserSettings.GamePath, Strings.FileName.GameExe);
                string gameExePatchedPath = Path.Combine(UserSettings.PatcherPath, Strings.FileName.GameExe);
                Logger.Debug($"original=[{gameExePath}] backup=[{gameExePatchedPath}]");

                if (File.Exists(gameExePath))
                    File.Move(gameExePath, gameExePatchedPath);
                File.Move(backupFilePath, gameExePath);
            }

            string[] filePaths = Directory.GetFiles(Strings.FolderName.Backup, "*", SearchOption.AllDirectories);

            foreach (var file in filePaths)
            {
                string path = Path.Combine(UserSettings.GamePath, file.Substring(Strings.FolderName.Backup.Length + 1));
                Logger.Debug($"original=[{path}] backup=[{file}]");

                if (File.Exists(path))
                    File.Move(path, Path.Combine(language.Lang, path.Substring(UserSettings.GamePath.Length + 1)));
                try
                {
                    File.Move(file, path);
                }
                catch (DirectoryNotFoundException)
                {
                    MsgBox.Error(String.Format("exception_cannot_restore_file", Path.GetFullPath(file)));
                    Logger.Error($"Cannot restore path=[{file}]");
                    File.Delete(file);
                }
            }
        }

        private static void DeleteTmpFiles(Language language)
        {
            string[] tmpFilePaths = Directory.GetFiles(language.Lang, "*.tmp", SearchOption.AllDirectories);

            foreach (var tmpFile in tmpFilePaths)
            {
                File.Delete(tmpFile);
                Logger.Debug($"Deleting tmp path=[{tmpFile}]");
            }
        }

        private static string GetSwPathFromRegistry()
        {
            if (!Environment.Is64BitOperatingSystem)
            {
                using (var key32 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\HanPurple\J_SW"))
                {
                    if (key32 != null)
                        return Convert.ToString(key32.GetValue("folder", String.Empty));
                    else
                        throw new Exception(StringLoader.GetText("exception_game_install_not_found"));
                }
            }
            else
            {
                using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\HanPurple\J_SW"))
                {
                    if (key != null)
                        return Convert.ToString(key.GetValue("folder", String.Empty));
                    else
                    {
                        using (var key32 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\HanPurple\J_SW"))
                        {
                            if (key32 != null)
                                return Convert.ToString(key32.GetValue("folder", String.Empty));
                            else
                                throw new Exception(StringLoader.GetText("exception_game_install_not_found"));
                        }
                    }
                }
            }
        }

        private static Language[] GetAvailableLanguages()
        {
            List<Language> langs = new List<Language>();

            using (var client = new WebClient())
            using (var file = new TempFile())
            {
                client.DownloadFile(Urls.PatcherGitHubHome + Strings.IniName.LanguagePack, file.Path);
                IniFile ini = new IniFile(new IniOptions
                {
                    Encoding = Encoding.UTF8
                });
                ini.Load(file.Path);

                foreach (var section in ini.Sections)
                    langs.Add(new Language(section.Name, Methods.ParseDate(section.Keys[Strings.IniName.Pack.KeyDate].Value)));
            }

            return langs.ToArray();
        }

        private static void DeleteTranslationIni(Language language)
        {
            string iniPath = Path.Combine(language.Lang, Strings.IniName.Translation);
            if (Directory.Exists(Path.GetDirectoryName(iniPath)))
                File.Delete(iniPath);
        }

        private static string GetSHA256(string filename)
        {
            using (var sha256 = SHA256.Create())
            using (var fs = File.OpenRead(filename))
            {
                return BitConverter.ToString(sha256.ComputeHash(fs)).Replace("-", "");
            }
        }

        private static bool IsTranslationOutdatedOrMissing(Language language, List<SWFile> swFiles)
        {
            if (Methods.IsTranslationOutdated(language))
                return true;

            var otherSWFilesPaths = swFiles.Where(f => String.IsNullOrEmpty(f.PathA)).Select(f => f.Path + Path.GetFileName(f.PathD));
            var archivesPaths = swFiles.Where(f => !String.IsNullOrEmpty(f.PathA)).Select(f => f.Path).Distinct();
            var translationPaths = archivesPaths.Union(otherSWFilesPaths).Select(f => Path.Combine(language.Lang, f));

            foreach (var path in translationPaths)
                if (!File.Exists(path))
                    return true;

            return false;
        }

        private static Process GetProcess(string name)
        {
            Process[] processesByName = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(name));

            if (processesByName.Length > 0)
                return processesByName[0];

            return null;
        }

        private static void BackupAndPlaceDataFiles(List<SWFile> swfiles, Language language)
        {
            var archives = swfiles.Where(f => !String.IsNullOrEmpty(f.PathA)).Select(f => f.Path).Distinct();
            foreach (var archive in archives)
            {
                string archivePath = Path.Combine(UserSettings.GamePath, archive);
                string backupFilePath = Path.Combine(Strings.FolderName.Backup, archive);
                string backupFileDirectory = Path.GetDirectoryName(backupFilePath);

                Directory.CreateDirectory(backupFileDirectory);

                File.Move(archivePath, backupFilePath);
                File.Move(Path.Combine(language.Lang, archive), archivePath);
            }
        }

        private static void BackupAndPlaceOtherFiles(List<SWFile> swfiles, Language language)
        {
            var swFiles = swfiles.Where(f => String.IsNullOrEmpty(f.PathA));
            foreach (var swFile in swFiles)
            {
                string swFileName = Path.Combine(swFile.Path, Path.GetFileName(swFile.PathD));
                string swFilePath = Path.Combine(language.Lang, swFileName);
                string filePath = Path.Combine(UserSettings.GamePath, swFileName);
                string backupFilePath = Path.Combine(Strings.FolderName.Backup, swFileName);
                string backupFileDirectory = Path.GetDirectoryName(backupFilePath);

                Directory.CreateDirectory(backupFileDirectory);

                File.Move(filePath, backupFilePath);
                File.Move(swFilePath, filePath);
            }
        }

        private static void HangameLogin(MyWebClient client)
        {
            var values = new NameValueCollection(2);
            string id = HttpUtility.UrlEncode(UserSettings.GameId);

            values[Strings.Web.PostEncodeId] = id;
            values[Strings.Web.PostEncodeFlag] = Strings.Web.PostEncodeFlagDefaultValue;
            values[Strings.Web.PostId] = id;
            using (var secure = Methods.DecryptString(UserSettings.GamePw))
            {
                values[Strings.Web.PostPw] = HttpUtility.UrlEncode(Methods.ToInsecureString(secure));
            }
            values[Strings.Web.PostClearFlag] = Strings.Web.PostClearFlagDefaultValue;
            values[Strings.Web.PostNextUrl] = Strings.Web.PostNextUrlDefaultValue;

            var loginResponse = Encoding.GetEncoding("shift-jis").GetString(client.UploadValues(Urls.HangameLogin, values));
            if (loginResponse.Contains(Strings.Web.CaptchaValidationText))
            {
                Process.Start(Strings.Web.CaptchaUrl);
                throw new Exception(StringLoader.GetText("exception_captcha_validation"));
            }
            try
            {
                string[] messages = GetVariableValue(loginResponse, Strings.Web.MessageVariable);
                if (messages[0].Length > 0)
                    throw new Exception(StringLoader.GetText("exception_incorrect_id_pw"));
            }
            catch (IndexOutOfRangeException)
            {

            }
        }

        private static string GetGameStartResponse(MyWebClient client)
        {
        again:
            string gameStartResponse = client.DownloadString(Urls.SoulworkerGameStart);
            try
            {
                if (GetVariableValue(gameStartResponse, Strings.Web.ErrorCodeVariable)[0] == "03")
                    throw new Exception(StringLoader.GetText("exception_not_tos"));
                else if (GetVariableValue(gameStartResponse, Strings.Web.MaintenanceVariable)[0] == "C")
                    throw new Exception(StringLoader.GetText("exception_game_maintenance"));
            }
            catch (IndexOutOfRangeException)
            {
                var dialog = MsgBox.ErrorRetry(StringLoader.GetText("exception_retry_validation_failed"));
                if (dialog == DialogResult.Retry)
                    goto again;

                throw new Exception(StringLoader.GetText("exception_validation_failed"));
            }

            return gameStartResponse;
        }

        private static string[] GetGameStartArguments(MyWebClient client)
        {
        again:
            try
            {
                client.UploadData(Urls.SoulworkerRegistCheck, new byte[] { });
            }
            catch (WebException webEx)
            {
                var dialog = MsgBox.ErrorRetry(StringLoader.GetText("exception_retry_validation_failed"));
                if (dialog == DialogResult.Retry)
                    goto again;

                var responseError = webEx.Response as HttpWebResponse;
                if (responseError.StatusCode == HttpStatusCode.NotFound)
                    throw new WebException(StringLoader.GetText("exception_validation_failed"), webEx);
                else
                    throw;
            }

            var reactorStartResponse = client.UploadData(Urls.SoulworkerReactorGameStart, new byte[] { });

            string[] gameStartArgs = new string[3];
            gameStartArgs[0] = GetVariableValue(Encoding.Default.GetString(reactorStartResponse), Strings.Web.GameStartArg)[0];
            gameStartArgs[1] = Strings.Server.IP;
            gameStartArgs[2] = Strings.Server.Port;

            return gameStartArgs;
        }

        private static string[] GetVariableValue(string fullText, string variableName)
        {
            string result;
            int valueIndex = fullText.IndexOf(variableName);

            if (valueIndex == -1)
                throw new IndexOutOfRangeException();

            result = fullText.Substring(valueIndex + variableName.Length + 1);
            result = result.Substring(0, result.IndexOf('"'));

            return result.Split(' ');
        }

        private string UploadToPasteBin(string title, string text, PasteBinExpiration expiration, bool isPrivate, string format)
        {
            var client = new PasteBinClient(Strings.PasteBin.DevKey);

            try
            {
                client.Login(Strings.PasteBin.Username, Strings.PasteBin.Password);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            var entry = new PasteBinEntry
            {
                Title = title,
                Text = text,
                Expiration = expiration,
                Private = isPrivate,
                Format = format
            };

            try
            {
                return client.Paste(entry);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                MsgBox.Error(StringLoader.GetText("exception_log_file_failed"));
            }
            finally
            {
                client.Logout();
            }

            return null;
        }

        private static string DecryptLog(string fileName)
        {
            if (!Logger.Encrypt)
                return File.ReadAllText(fileName);

            string message = "";

            using (var br = new BinaryReader(File.OpenRead(fileName)))
            {
                ushort[] messageShorts = new ushort[br.BaseStream.Length / 2];
                byte[] messageBytes = new byte[br.BaseStream.Length];

                for (int i = 0; i < br.BaseStream.Length / 2; i++)
                {
                    messageShorts[i] = br.ReadUInt16();
                    messageShorts[i] ^= 0x24;
                    messageBytes[i * 2] = (byte)(messageShorts[i] >> 8);
                    messageBytes[i * 2 + 1] = (byte)(messageShorts[i] & 255);
                }

                message = Encoding.BigEndianUnicode.GetString(messageBytes);
            }

            return message;
        }

        private static string TrimStringIfNecessary(string str)
        {
            int limit = 500000;

            byte[] strBytes = Encoding.UTF8.GetBytes(str);
            if (strBytes.Length > limit)
            {
                byte[] trimmedStrBytes = new byte[limit];
                Array.Copy(strBytes, strBytes.Length - limit, trimmedStrBytes, 0, limit);

                return Encoding.UTF8.GetString(trimmedStrBytes);
            }

            return str;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Language[] languages = GetAvailableLanguages();
            this.comboBoxLanguages.DataSource = languages.Length > 0 ? languages : null;

            var gamePath = UserSettings.GamePath;
            if (String.IsNullOrEmpty(gamePath) || !Methods.IsSwPath(gamePath))
                UserSettings.GamePath = GetSwPathFromRegistry();

            if (this.comboBoxLanguages.DataSource != null)
            {
                Logger.Info($"Loading languages: {String.Join(" ", languages.Select(l => l.ToString()))}");

                if (String.IsNullOrEmpty(UserSettings.LanguageName))
                {
                    UserSettings.LanguageName = (this.comboBoxLanguages.SelectedItem as Language).Lang;
                }
                else
                {
                    int index = this.comboBoxLanguages.Items.IndexOf(new Language(UserSettings.LanguageName, DateTime.UtcNow));
                    this.comboBoxLanguages.SelectedIndex = index == -1 ? 0 : index;
                }
            }

            StartupBackupCheck(this.comboBoxLanguages.SelectedItem as Language);

            if (!Methods.IsValidSwPatcherPath(Directory.GetCurrentDirectory()))
            {
                string error = StringLoader.GetText("exception_folder_same_path_game");

                Logger.Error(error);
                MsgBox.Error(error);
            }
        }

        private void buttonDownload_Click(object sender, EventArgs e)
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

        private void buttonPlay_MouseDown(object sender, MouseEventArgs e)
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
                    this.Worker.CancelAsync();

                    break;
            }
        }

        private void buttonStartRaw_Click(object sender, EventArgs e)
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

        private void forceStripMenuItem_Click(object sender, EventArgs e)
        {
            Language language = this.comboBoxLanguages.SelectedItem as Language;

            Logger.Info("Force patch selected");
            DeleteTranslationIni(language);
            this.labelNewTranslations.Text = String.Format(StringLoader.GetText("form_label_new_translation"), language.Lang, Methods.DateToString(language.LastUpdate));

            this.CurrentState = State.RTPatch;
            this._nextState = NextState.Download;
            this.RTPatcher.Run();
        }

        private void comboBoxLanguages_SelectedIndexChanged(object sender, EventArgs e)
        {
            Language language = this.comboBoxLanguages.SelectedItem as Language;

            if (language != null && Methods.HasNewTranslations(language))
                this.labelNewTranslations.Text = String.Format(StringLoader.GetText("form_label_new_translation"), language.Lang, Methods.DateToString(language.LastUpdate));
            else
                this.labelNewTranslations.Text = String.Empty;
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                notifyIcon.Visible = true;
                notifyIcon.ShowBalloonTip(500);

                this.ShowInTaskbar = false;
                this.Hide();
            }
        }

        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            this.RestoreFromTray();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsForm settingsForm = new SettingsForm();
            settingsForm.ShowDialog(this);
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void openSWWebpageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("iexplore.exe", Urls.SoulworkerHome);
        }

        private void uploadLogToPastebinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!File.Exists(Strings.FileName.Log))
            {
                MsgBox.Error(StringLoader.GetText("exception_log_not_exist"));

                return;
            }

            string logTitle = $"{AssemblyAccessor.Version} ({GetSHA256(Application.ExecutablePath).Substring(0, 12)}) at {Methods.DateToString(DateTime.UtcNow)}";
            string logText = DecryptLog(Strings.FileName.Log);
            logText = TrimStringIfNecessary(logText);
            var pasteUrl = UploadToPasteBin(logTitle, logText, PasteBinExpiration.OneHour, true, "text");

            if (!String.IsNullOrEmpty(pasteUrl))
            {
                Clipboard.SetText(pasteUrl);
                MsgBox.Success(String.Format(StringLoader.GetText("success_log_file_upload"), pasteUrl));
            }
            else
            {
                MsgBox.Error(StringLoader.GetText("exception_log_file_failed"));
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox aboutBox = new AboutBox();
            aboutBox.ShowDialog(this);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Methods.In(e.CloseReason, CloseReason.ApplicationExitCall, CloseReason.WindowsShutDown))
            {
                Logger.Debug($"{this.Text} closing abnormally. Reason=[{e.CloseReason.ToString()}]");
                this.CurrentState = State.Idle;
                this.RTPatcher.Cancel();
                this.Downloader.Cancel();
                this.Patcher.Cancel();
                this.Worker.CancelAsync();
            }
            if (this.CurrentState != State.Idle)
            {
                MsgBox.Error(String.Format(StringLoader.GetText("exception_cannot_close"), AssemblyAccessor.Title));

                e.Cancel = true;
            }
            else
            {
                Logger.Debug($"{this.Text} closing. Reason=[{e.CloseReason.ToString()}]");
                UserSettings.LanguageName = this.comboBoxLanguages.SelectedIndex == -1 ? null : (this.comboBoxLanguages.SelectedItem as Language).Lang;
            }
        }

        private void exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
