using MadMilkman.Ini;
using PubPluginLib;
using SWPatcher.Helpers;
using SWPatcher.General;
using SWPatcher.Helpers.GlobalVar;
using SWPatcher.Patching;
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
using System.Windows.Forms;

namespace SWPatcher.Forms
{
    public partial class MainForm : Form
    {
        public enum States
        {
            Idle = 0,
            Downloading = 1,
            Patching = 2,
            Preparing = 3,
            WaitingClient = 4,
            Applying = 5,
            WaitingClose = 6,
            RTPatching = 7
        }

        private States _state;
        private readonly Downloader Downloader;
        private readonly Patcher Patcher;
        private readonly BackgroundWorker Worker;
        private readonly RTPatcher RTPatcher;
        private readonly List<SWFile> SWFiles;

        public States State
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
                        case States.Idle:
                            comboBoxLanguages.Enabled = true;
                            buttonDownload.Enabled = true;
                            buttonDownload.Text = Strings.FormText.Download;
                            buttonPlay.Enabled = true;
                            buttonPlay.Text = Strings.FormText.Play;
                            toolStripMenuItemStartRaw.Enabled = true;
                            forceStripMenuItem.Enabled = true;
                            refreshToolStripMenuItem.Enabled = true;
                            toolStripStatusLabel.Text = Strings.FormText.Status.Idle;
                            toolStripProgressBar.Value = toolStripProgressBar.Minimum;
                            toolStripProgressBar.Style = ProgressBarStyle.Blocks;
                            break;
                        case States.Downloading:
                            comboBoxLanguages.Enabled = false;
                            buttonDownload.Enabled = true;
                            buttonDownload.Text = Strings.FormText.Cancel;
                            buttonPlay.Enabled = false;
                            buttonPlay.Text = Strings.FormText.Play;
                            toolStripMenuItemStartRaw.Enabled = false;
                            forceStripMenuItem.Enabled = false;
                            refreshToolStripMenuItem.Enabled = false;
                            toolStripStatusLabel.Text = Strings.FormText.Status.Download;
                            toolStripProgressBar.Value = toolStripProgressBar.Minimum;
                            toolStripProgressBar.Style = ProgressBarStyle.Blocks;
                            break;
                        case States.Patching:
                            comboBoxLanguages.Enabled = false;
                            buttonDownload.Enabled = true;
                            buttonDownload.Text = Strings.FormText.Cancel;
                            buttonPlay.Enabled = false;
                            buttonPlay.Text = Strings.FormText.Play;
                            toolStripMenuItemStartRaw.Enabled = false;
                            forceStripMenuItem.Enabled = false;
                            refreshToolStripMenuItem.Enabled = false;
                            toolStripStatusLabel.Text = Strings.FormText.Status.Patch;
                            toolStripProgressBar.Value = toolStripProgressBar.Minimum;
                            toolStripProgressBar.Style = ProgressBarStyle.Blocks;
                            break;
                        case States.Preparing:
                            comboBoxLanguages.Enabled = false;
                            buttonDownload.Enabled = false;
                            buttonDownload.Text = Strings.FormText.Download;
                            buttonPlay.Enabled = false;
                            buttonPlay.Text = Strings.FormText.Play;
                            toolStripMenuItemStartRaw.Enabled = false;
                            forceStripMenuItem.Enabled = false;
                            refreshToolStripMenuItem.Enabled = false;
                            toolStripStatusLabel.Text = Strings.FormText.Status.Prepare;
                            toolStripProgressBar.Value = toolStripProgressBar.Minimum;
                            toolStripProgressBar.Style = ProgressBarStyle.Marquee;
                            break;
                        case States.WaitingClient:
                            comboBoxLanguages.Enabled = false;
                            buttonDownload.Enabled = false;
                            buttonDownload.Text = Strings.FormText.Download;
                            buttonPlay.Enabled = true;
                            buttonPlay.Text = Strings.FormText.Cancel;
                            toolStripMenuItemStartRaw.Enabled = false;
                            forceStripMenuItem.Enabled = false;
                            refreshToolStripMenuItem.Enabled = false;
                            toolStripStatusLabel.Text = Strings.FormText.Status.WaitClient;
                            toolStripProgressBar.Value = toolStripProgressBar.Minimum;
                            toolStripProgressBar.Style = ProgressBarStyle.Blocks;
                            break;
                        case States.Applying:
                            comboBoxLanguages.Enabled = false;
                            buttonDownload.Enabled = false;
                            buttonDownload.Text = Strings.FormText.Download;
                            buttonPlay.Enabled = false;
                            buttonPlay.Text = Strings.FormText.Play;
                            toolStripMenuItemStartRaw.Enabled = false;
                            forceStripMenuItem.Enabled = false;
                            refreshToolStripMenuItem.Enabled = false;
                            toolStripStatusLabel.Text = Strings.FormText.Status.ApplyFiles;
                            toolStripProgressBar.Value = toolStripProgressBar.Minimum;
                            toolStripProgressBar.Style = ProgressBarStyle.Marquee;
                            break;
                        case States.WaitingClose:
                            comboBoxLanguages.Enabled = false;
                            buttonDownload.Enabled = false;
                            buttonDownload.Text = Strings.FormText.Download;
                            buttonPlay.Enabled = false;
                            buttonPlay.Text = Strings.FormText.Play;
                            toolStripMenuItemStartRaw.Enabled = false;
                            forceStripMenuItem.Enabled = false;
                            refreshToolStripMenuItem.Enabled = false;
                            toolStripStatusLabel.Text = Strings.FormText.Status.WaitClose;
                            toolStripProgressBar.Value = toolStripProgressBar.Minimum;
                            toolStripProgressBar.Style = ProgressBarStyle.Marquee;
                            WindowState = FormWindowState.Minimized;
                            break;
                        case States.RTPatching:
                            comboBoxLanguages.Enabled = false;
                            buttonDownload.Enabled = true;
                            buttonDownload.Text = Strings.FormText.Cancel;
                            buttonPlay.Enabled = false;
                            buttonPlay.Text = Strings.FormText.Play;
                            toolStripMenuItemStartRaw.Enabled = false;
                            forceStripMenuItem.Enabled = false;
                            refreshToolStripMenuItem.Enabled = false;
                            toolStripStatusLabel.Text = Strings.FormText.Status.UpdatingClient;
                            toolStripProgressBar.Value = toolStripProgressBar.Minimum;
                            toolStripProgressBar.Style = ProgressBarStyle.Blocks;
                            break;
                    }

                    this.comboBoxLanguages_SelectedIndexChanged(null, null);
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
            this.RTPatcher.RTPatchDownloadProgressChanged += RTPatcher_DownloadProgressChanged;
            this.RTPatcher.RTPatchProgressChanged += RTPatcher_ProgressChanged;
            this.RTPatcher.RTPatchCompleted += RTPatcher_Completed;
            InitializeComponent();
            this.Text = AssemblyAccessor.Title + " " + AssemblyAccessor.Version;
        }

        private void Downloader_DownloaderProgressChanged(object sender, DownloaderProgressChangedEventArgs e)
        {
            if (this.State == States.Downloading)
            {
                this.toolStripStatusLabel.Text = String.Format("{0} {1} ({2}/{3})", Strings.FormText.Status.Download, e.FileName, e.FileNumber, e.FileCount);
                this.toolStripProgressBar.Value = e.Progress;
            }
        }

        private void Downloader_DownloaderCompleted(object sender, DownloaderCompletedEventArgs e)
        {
            if (e.Cancelled) { }
            else if (e.Error != null)
            {
                Error.Log(e.Error);
                MsgBox.Error(Error.ExeptionParser(e.Error));
            }
            else
            {
                this.State = States.Patching;
                this.Patcher.Run(e.Language);

                return;
            }

            this.State = States.Idle;
        }

        private void Patcher_PatcherProgressChanged(object sender, PatcherProgressChangedEventArgs e)
        {
            if (this.State == States.Patching)
            {
                if (e.Progress == -1)
                {
                    this.toolStripStatusLabel.Text = Strings.FormText.Status.PatchingExe;
                    this.toolStripProgressBar.Style = ProgressBarStyle.Marquee;
                }
                else
                {
                    this.toolStripStatusLabel.Text = String.Format("{0} Step {1}/{2}", Strings.FormText.Status.Patch, e.FileNumber, e.FileCount);
                    this.toolStripProgressBar.Value = e.Progress;
                }
            }
        }

        private void Patcher_PatcherCompleted(object sender, PatcherCompletedEventArgs e)
        {
            if (e.Cancelled) { }
            else if (e.Error != null)
            {
                Error.Log(e.Error);
                MsgBox.Error(Error.ExeptionParser(e.Error));
                DeleteTmpFiles(e.Language);
            }
            else
            {
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

            this.State = States.Idle;
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            this.Worker.ReportProgress((int)States.Preparing);
            if (e.Argument != null)
            {
                Language language = e.Argument as Language;

                Methods.SetSWFiles(this.SWFiles);

                if (IsTranslationOutdated(language, this.SWFiles))
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

                    if (!Directory.Exists(backupFileDirectory))
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
                    this.Worker.ReportProgress((int)States.WaitingClient);
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

                this.Worker.ReportProgress((int)States.Applying);
                BackupAndPlaceDataFiles(this.SWFiles, language);
                BackupAndPlaceOtherFiles(this.SWFiles, language);

                if (startInfo != null)
                    clientProcess = Process.Start(startInfo);

                this.Worker.ReportProgress((int)States.WaitingClose);
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
                    //}
                }
                else
                {
                    throw new Exception("Direct login option is not active.");
                }
            }
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.State = (States)e.ProgressPercentage;
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled) { }
            else if (e.Error != null)
            {
                Error.Log(e.Error);
                MsgBox.Error(e.Error.Message);
            }
            else if (e.Result != null && Convert.ToBoolean(e.Result))
            {
                MsgBox.Notice("Outdated translation files, force patching will now commence.");
                forceStripMenuItem_Click(sender, e);

                return;
            }
            else
                this.RestoreFromTray();

            try
            {
                RestoreBackup(this.comboBoxLanguages.SelectedItem as Language);
            }
            finally
            {
                this.State = States.Idle;
            }
        }

        private void RTPatcher_DownloadProgressChanged(object sender, RTPatchDownloadProgressChangedEventArgs e)
        {
            if (this.State == States.Downloading)
            {
                this.toolStripStatusLabel.Text = String.Format("{0} {1}", Strings.FormText.Status.Download, e.FileName);
                this.toolStripProgressBar.Value = e.Progress;
            }
        }

        private void RTPatcher_ProgressChanged(object sender, RTPatchProgressChangedEventArgs e)
        {
            if (this.State == States.Downloading)
            {
                this.toolStripStatusLabel.Text = String.Format("{0} {1} ({2}/{3})", Strings.FormText.Status.ApplyFiles, e.FileName, e.FileNumber, e.FileCount);
                this.toolStripProgressBar.Value = e.Progress;
            }
        }

        private void RTPatcher_Completed(object sender, RTPatchCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                this.State = States.Idle;

                return;
            }
            else if (e.Error != null)
            {
                Error.Log(e.Error);
                MsgBox.Error(Error.ExeptionParser(e.Error));
                this.State = States.Idle;

                return;
            }
            else if (e.Version != null)
            {
                IniFile ini = new IniFile(new IniOptions
                {
                    KeyDuplicate = IniDuplication.Ignored,
                    SectionDuplicate = IniDuplication.Ignored
                });
                string iniPath = Path.Combine(UserSettings.GamePath, Strings.IniName.ClientVer);
                ini.Load(iniPath);
                string serverVer = ini.Sections[Strings.IniName.Ver.Section].Keys[Strings.IniName.Ver.Key].Value;
                string clientVer = e.Version.ToString();
                if (serverVer != clientVer)
                {
                    ini.Sections[Strings.IniName.Ver.Section].Keys[Strings.IniName.Ver.Key].Value = clientVer;
                    ini.Save(iniPath);
                }
            }

            // TODO: e.Result somewhere someday
            MsgBox.Notice($"Result=[{e.Result}]");

            switch (e.Caller.ToLower())
            {
                case "downloader":
                    {
                        this.State = States.Downloading;
                        this.Downloader.Run(this.comboBoxLanguages.SelectedItem as Language, false);

                        break;
                    }
                case "downloaderf":
                    {
                        this.State = States.Downloading;
                        this.Downloader.Run(this.comboBoxLanguages.SelectedItem as Language, true);

                        break;
                    }
                case "starter":
                    {
                        this.Worker.RunWorkerAsync(this.comboBoxLanguages.SelectedItem as Language);

                        break;
                    }
            }
        }

        public IEnumerable<string> GetComboBoxStringItems()
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
            if (Directory.Exists(Strings.FolderName.Backup))
            {
                if (Directory.GetFiles(Strings.FolderName.Backup, "*", SearchOption.AllDirectories).Length > 0)
                {
                    var result = MsgBox.Question(String.Format("Backup files found. Do you want to restore them now back in your client?\nExisting ones from your client will be swapped to the {0} translation.\nSelecting No will remove those backup files.", language.Lang));

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
            if (!Directory.Exists(Strings.FolderName.Backup))
                return;

            string backupFilePath = Path.Combine(Strings.FolderName.Backup, Strings.FileName.GameExe);
            if (File.Exists(backupFilePath))
            {
                string gameExePath = Path.Combine(UserSettings.GamePath, Strings.FileName.GameExe);
                string gameExePatchedPath = Path.Combine(UserSettings.PatcherPath, Strings.FileName.GameExe);

                File.Move(gameExePath, gameExePatchedPath);
                File.Move(backupFilePath, gameExePath);
            }

            string[] filePaths = Directory.GetFiles(Strings.FolderName.Backup, "*", SearchOption.AllDirectories);

            foreach (var file in filePaths)
            {
                string path = Path.Combine(UserSettings.GamePath, file.Substring(Strings.FolderName.Backup.Length + 1));

                File.Move(path, Path.Combine(language.Lang, path.Substring(UserSettings.GamePath.Length + 1)));
                File.Move(file, path);
            }
        }

        private static void DeleteTmpFiles(Language language)
        {
            string[] tmpFilePaths = Directory.GetFiles(language.Lang, "*.tmp", SearchOption.AllDirectories);

            foreach (var tmpFile in tmpFilePaths)
                File.Delete(tmpFile);
        }

        public static string GetSwPathFromRegistry()
        {
            using (var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\HanPurple\J_SW"))
            {
                if (key != null)
                    return Convert.ToString(key.GetValue("folder", String.Empty));
                else
                {
                    Error.Log("64-bit - Key not found");

                    using (var key32 = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\HanPurple\J_SW"))
                    {
                        if (key32 != null)
                            return Convert.ToString(key32.GetValue("folder", String.Empty));
                        else
                        {
                            Error.Log("32-bit - Key not found");
                            throw new Exception("Soulworker installation not found.");
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
                IniFile ini = new IniFile();
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
            using (var stream = File.OpenRead(filename))
            {
                return BitConverter.ToString(sha256.ComputeHash(stream)).Replace("-", "");
            }
        }

        private static bool IsTranslationOutdated(Language language, List<SWFile> swFiles)
        {
            string selectedTranslationPath = Path.Combine(language.Lang, Strings.IniName.Translation);
            if (!File.Exists(selectedTranslationPath))
                return true;

            IniFile ini = new IniFile();
            ini.Load(selectedTranslationPath);

            if (!ini.Sections[Strings.IniName.Patcher.Section].Keys.Contains(Strings.IniName.Patcher.KeyVer))
                throw new Exception("Error reading translation version, try to Menu -> Force Patch");

            Version translationVer = new Version(ini.Sections[Strings.IniName.Patcher.Section].Keys[Strings.IniName.Patcher.KeyVer].Value);
            ini.Sections.Clear();
            ini.Load(Path.Combine(UserSettings.GamePath, Strings.IniName.ClientVer));

            Version clientVer = new Version(ini.Sections[Strings.IniName.Ver.Section].Keys[Strings.IniName.Ver.Key].Value);
            if (clientVer > translationVer)
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

                if (!Directory.Exists(backupFileDirectory))
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

                if (!Directory.Exists(backupFileDirectory))
                    Directory.CreateDirectory(backupFileDirectory);

                File.Move(filePath, backupFilePath);
                File.Move(swFilePath, filePath);
            }
        }

        private static void HangameLogin(MyWebClient client)
        {
            var values = new NameValueCollection(2);
            values[Strings.Web.PostId] = UserSettings.GameId;
            values[Strings.Web.PostPw] = UserSettings.GamePw;
            var loginResponse = Encoding.GetEncoding("shift-jis").GetString(client.UploadValues(Urls.HangameLogin, values));
            try
            {
                string[] messages = GetVariableValue(loginResponse, Strings.Web.MessageVariable);
                if (messages[0].Length > 0)
                    throw new Exception("Incorrect ID or Password.", new Exception(String.Join("\n", messages)));
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
                    throw new Exception("To play the game you need to accept the Terms of Service.");
                else if (GetVariableValue(gameStartResponse, Strings.Web.MaintenanceVariable)[0] == "C")
                    throw new Exception("Game is under maintenance.");
            }
            catch (IndexOutOfRangeException)
            {
                var dialog = MsgBox.ErrorRetry("Validation failed.");
                if (dialog == DialogResult.Retry)
                    goto again;

                throw new Exception("Validation failed. Maybe your IP/Region is blocked?");
            }

            return gameStartResponse;
        }

        private static void StartReactorToUpdate()
        {
            again:
            using (var client = new MyWebClient())
            {
                HangameLogin(client);
                string gameStartResponse = GetGameStartResponse(client);

                PubPluginClass pubPluginClass = new PubPluginClass();
                IPubPlugin pubPlugin = null;
                try
                {
                    pubPlugin = pubPluginClass;
                }
                catch (InvalidCastException)
                {
                    throw new Exception("Run the game from the website first to install the plugin and reactor!");
                }
                if (pubPlugin.IsReactorInstalled() == 1)
                    try
                    {
                        pubPlugin.StartReactor(GetVariableValue(gameStartResponse, Strings.Web.ReactorStr)[0]);
                        throw new Exception("Update the game client using the game launcher.\nWhen it finished, close it and try 'Ready to Play' again.");
                    }
                    catch (IndexOutOfRangeException)
                    {
                        var dialog = MsgBox.ErrorRetry("Validation failed.");
                        if (dialog == DialogResult.Retry)
                            goto again;

                        throw new Exception("Validation failed. Maybe your IP/Region is blocked?");
                    }
                else
                    throw new Exception("Run the game from the website first to install the plugin and reactor!");
            }
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
                var dialog = MsgBox.ErrorRetry("Validation failed.");
                if (dialog == DialogResult.Retry)
                    goto again;

                var responseError = webEx.Response as HttpWebResponse;
                if (responseError.StatusCode == HttpStatusCode.NotFound)
                    throw new WebException("Validation failed. Maybe your IP/Region is blocked?", webEx);
                else
                    throw;
            }

            var reactorStartResponse = client.UploadData(Urls.SoulworkerReactorGameStart, new byte[] { });
            IniFile ini = new IniFile();
            ini.Load(Path.Combine(UserSettings.GamePath, Strings.IniName.GeneralClient));

            string[] gameStartArgs = new string[3];
            gameStartArgs[0] = GetVariableValue(Encoding.Default.GetString(reactorStartResponse), Strings.Web.GameStartArg)[0];
            gameStartArgs[1] = ini.Sections[Strings.IniName.General.SectionNetwork].Keys[Strings.IniName.General.KeyIP].Value;
            gameStartArgs[2] = ini.Sections[Strings.IniName.General.SectionNetwork].Keys[Strings.IniName.General.KeyPort].Value;

            return gameStartArgs;
        }

        internal static string[] GetVariableValue(string fullText, string variableName)
        {
            string result;
            int valueIndex = fullText.IndexOf(variableName);

            if (valueIndex == -1)
                throw new IndexOutOfRangeException();

            result = fullText.Substring(valueIndex + variableName.Length + 1);
            result = result.Substring(0, result.IndexOf('"'));

            return result.Split(' ');
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Language[] languages = GetAvailableLanguages();
            this.comboBoxLanguages.DataSource = languages.Length > 0 ? languages : null;

            var gamePath = UserSettings.GamePath;
            if (String.IsNullOrEmpty(gamePath) || !Methods.IsSwPath(gamePath))
                UserSettings.GamePath = GetSwPathFromRegistry();

            if (this.comboBoxLanguages.DataSource != null)
                if (String.IsNullOrEmpty(UserSettings.LanguageName))
                    UserSettings.LanguageName = (this.comboBoxLanguages.SelectedItem as Language).Lang;
                else
                {
                    int index = this.comboBoxLanguages.Items.IndexOf(new Language(UserSettings.LanguageName, DateTime.UtcNow));
                    this.comboBoxLanguages.SelectedIndex = index == -1 ? 0 : index;
                }

            StartupBackupCheck(this.comboBoxLanguages.SelectedItem as Language);

            if (!Methods.IsValidSwPatcherPath(Directory.GetCurrentDirectory()))
            {
                string error = "The program is in the same or in a sub folder as your game client.\nThis will cause malfunctions or data corruption on your game client.\nPlease move the patcher in another location or continue at your own risk.";

                Error.Log(error);
                MsgBox.Error(error);
            }
        }

        private void buttonDownload_Click(object sender, EventArgs e)
        {
            if (this.State == States.Downloading)
            {
                this.buttonDownload.Enabled = false;
                this.buttonDownload.Text = Strings.FormText.Cancelling;
                this.Downloader.Cancel();
            }
            else if (this.State == States.Patching)
            {
                this.buttonDownload.Enabled = false;
                this.buttonDownload.Text = Strings.FormText.Cancelling;
                this.Patcher.Cancel();
            }
            else if (this.State == States.RTPatching)
            {
                this.buttonDownload.Enabled = false;
                this.buttonDownload.Text = Strings.FormText.Cancelling;
                this.RTPatcher.Cancel();
            }
            else if (this.State == States.Idle)
            {
                this.State = States.RTPatching;
                this.RTPatcher.Run("Downloader");
            }
        }

        private void buttonPlay_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.State == States.WaitingClient)
            {
                this.buttonPlay.Enabled = false;
                this.buttonPlay.Text = Strings.FormText.Cancelling;
                this.Worker.CancelAsync();
            }
            else
            {
                this.State = States.RTPatching;
                RTPatcher.Run("Starter");
            }
        }

        private void toolStripMenuItemStartRaw_Click(object sender, EventArgs e)
        {
            if (this.State == States.Idle)
            {
                this.Worker.RunWorkerAsync();
            }
        }

        private void comboBoxLanguages_SelectedIndexChanged(object sender, EventArgs e)
        {
            Language language = this.comboBoxLanguages.SelectedItem as Language;

            if (language != null && Methods.HasNewTranslations(language))
                this.labelNewTranslations.Text = String.Format(Strings.FormText.NewTranslations, language.Lang, Methods.DateToString(language.LastUpdate));
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

        private void forceStripMenuItem_Click(object sender, EventArgs e)
        {
            Language language = this.comboBoxLanguages.SelectedItem as Language;

            DeleteTranslationIni(language);
            this.labelNewTranslations.Text = String.Format(Strings.FormText.NewTranslations, language.Lang, Methods.DateToString(language.LastUpdate));

            this.State = States.RTPatching;
            this.RTPatcher.Run("DownloaderF");
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
                MsgBox.Error("Log file does not exist.");

                return;
            }

            string logText = File.ReadAllText(Strings.FileName.Log);
            var client = new PasteBinClient(Strings.PasteBinDevKey);

            try
            {
                client.Login(Strings.PasteBinUsername, Strings.PasteBinPassword);
            }
            catch (Exception ex)
            {
                Error.Log(ex);
            }

            var entry = new PasteBinEntry
            {
                Title = String.Format("{0} ({1}) at {2}", AssemblyAccessor.Version, GetSHA256(Application.ExecutablePath).Substring(0, 12), Methods.DateToString(DateTime.UtcNow)),
                Text = logText,
                Expiration = PasteBinExpiration.OneHour,
                Private = true,
                Format = "csharp"
            };

            try
            {
                string pasteUrl = client.Paste(entry);

                Clipboard.SetText(pasteUrl);
                MsgBox.Success("Log file was uploaded to " + pasteUrl + "\nThe link was copied to clipboard.");
            }
            catch (Exception ex)
            {
                Error.Log(ex);
                MsgBox.Error("Failed to upload log file.");
            }
            finally
            {
                client.Logout();
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox aboutBox = new AboutBox();
            aboutBox.ShowDialog(this);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.State != States.Idle)
            {
                MsgBox.Error(AssemblyAccessor.Title + " is currently busy and cannot close.");

                e.Cancel = true;
            }
            else
            {
                UserSettings.LanguageName = this.comboBoxLanguages.SelectedIndex == -1 ? null : (this.comboBoxLanguages.SelectedItem as Language).Lang;
            }
        }

        private void exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
