using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using MadMilkman.Ini;
using SWPatcher.Downloading;
using SWPatcher.General;
using SWPatcher.Helpers;
using SWPatcher.Helpers.GlobalVar;
using SWPatcher.Patching;
using PubPluginLib;

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
            WaitingClose = 6
        }

        private States _state;
        private readonly Downloader Downloader;
        private readonly Patcher Patcher;
        private readonly BackgroundWorker Worker;
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
                            buttonExit.Enabled = true;
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
                            buttonExit.Enabled = false;
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
                            buttonExit.Enabled = false;
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
                            buttonExit.Enabled = false;
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
                            buttonExit.Enabled = false;
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
                            buttonExit.Enabled = false;
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
                            buttonExit.Enabled = false;
                            forceStripMenuItem.Enabled = false;
                            refreshToolStripMenuItem.Enabled = false;
                            toolStripStatusLabel.Text = Strings.FormText.Status.WaitClose;
                            toolStripProgressBar.Value = toolStripProgressBar.Minimum;
                            toolStripProgressBar.Style = ProgressBarStyle.Marquee;
                            WindowState = FormWindowState.Minimized;
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
            this.Worker.DoWork += new DoWorkEventHandler(Worker_DoWork);
            this.Worker.ProgressChanged += new ProgressChangedEventHandler(Worker_ProgressChanged);
            this.Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Worker_RunWorkerCompleted);
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

            this.State = 0;
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
                Methods.DeleteTmpFiles(e.Language);
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

            this.State = 0;
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Language language = e.Argument as Language;

            if (Methods.IsNewerGameClientVersion())
            {
                if (UserSettings.WantToLogin)
                {
                    using (var client = new MyWebClient())
                    {
                        var values = new NameValueCollection(2);
                        values[Strings.Web.PostId] = UserSettings.GameId;
                        values[Strings.Web.PostPw] = UserSettings.GamePw;
                        client.UploadValues(Urls.HangameLogin, values);
                        var response = client.DownloadString(Urls.SoulworkerGameStart);
                        PubPluginClass pubPlugin = new PubPluginClass();
                        if (pubPlugin.IsReactorInstalled() == 1)
                            try
                            {
                                pubPlugin.StartReactor(Methods.GetVariableValue(response, Strings.Web.ReactorStr)[0]);
                                throw new Exception("Update the game client then try again.");
                            }
                            catch (IndexOutOfRangeException)
                            {
                                throw new Exception("Validation failed. Maybe your IP/Region is blocked?");
                            }
                        else
                            throw new Exception("Reactor not installed!");
                    }
                }
                else
                    throw new Exception("Game client is not updated to the latest version.");
            }

            if (Methods.IsTranslationOutdated(language))
            {
                e.Result = true; // force patch = true
                return;
            }

            if (this.SWFiles.Count == 0)
            {
                this.SWFiles.Clear();
                using (var client = new WebClient())
                using (var file = new TempFile())
                {
                    client.DownloadFile(Urls.PatcherGitHubHome + Strings.IniName.TranslationPackData, file.Path);
                    IniFile ini = new IniFile();
                    ini.Load(file.Path);

                    foreach (var section in ini.Sections)
                    {
                        string name = section.Name;
                        string path = section.Keys[Strings.IniName.Pack.KeyPath].Value;
                        string pathA = section.Keys[Strings.IniName.Pack.KeyPathInArchive].Value;
                        string pathD = section.Keys[Strings.IniName.Pack.KeyPathOfDownload].Value;
                        string format = section.Keys[Strings.IniName.Pack.KeyFormat].Value;
                        this.SWFiles.Add(new SWFile(name, path, pathA, pathD, format));

                        if (this.Worker.CancellationPending)
                        {
                            e.Cancel = true;
                            return;
                        }
                    }
                }
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
            if (UserSettings.WantToLogin)
            {
                using (var client = new MyWebClient())
                {
                    var values = new NameValueCollection(2);
                    values[Strings.Web.PostId] = UserSettings.GameId;
                    values[Strings.Web.PostPw] = UserSettings.GamePw;
                    client.UploadValues(Urls.HangameLogin, values);

                    client.DownloadString(Urls.SoulworkerGameStart);
                    try
                    {
                        client.UploadData(Urls.SoulworkerRegistCheck, new byte[] { });
                    }
                    catch (WebException webEx)
                    {
                        var responseError = webEx.Response as HttpWebResponse;
                        if (responseError.StatusCode == HttpStatusCode.NotFound)
                            throw new WebException("Validation failed. Maybe your IP/Region is blocked?", webEx);
                        else
                            throw;
                    }

                    var response = client.UploadData(Urls.SoulworkerReactorGameStart, new byte[] { });
                    string[] gameStartArgs = new string[] { Methods.GetVariableValue(Encoding.Default.GetString(response), Strings.Web.GameStartArg)[0], "", "" };

                    IniFile ini = new IniFile();
                    ini.Load(Path.Combine(UserSettings.GamePath, Strings.IniName.GeneralFile));
                    gameStartArgs[1] = ini.Sections[Strings.IniName.General.SectionNetwork].Keys[Strings.IniName.General.KeyIP].Value;
                    gameStartArgs[2] = ini.Sections[Strings.IniName.General.SectionNetwork].Keys[Strings.IniName.General.KeyPort].Value;
                    var startInfo = new ProcessStartInfo
                    {
                        UseShellExecute = true,
                        Verb = "runas",
                        Arguments = String.Join(" ", gameStartArgs.Select(s => "\"" + s + "\"")),
                        WorkingDirectory = UserSettings.GamePath,
                        FileName = Strings.FileName.GameExe
                    };
                    clientProcess = Process.Start(startInfo);
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

                    clientProcess = Methods.GetProcess(Strings.FileName.GameExe);

                    if (clientProcess == null)
                        Thread.Sleep(1000);
                    else
                        break;
                }
            }

            this.Worker.ReportProgress((int)States.Applying);
            var archives = this.SWFiles.Where(f => !String.IsNullOrEmpty(f.PathA)).Select(f => f.Path).Distinct();

            foreach (var archive in archives) // backup and place *.v files
            {
                string archivePath = Path.Combine(UserSettings.GamePath, archive);
                string backupFilePath = Path.Combine(Strings.FolderName.Backup, archive);
                string backupFileDirectory = Path.GetDirectoryName(backupFilePath);

                if (!Directory.Exists(backupFileDirectory))
                    Directory.CreateDirectory(backupFileDirectory);

                File.Move(archivePath, backupFilePath);
                File.Move(Path.Combine(language.Lang, archive), archivePath);
            }

            var swFiles = this.SWFiles.Where(f => String.IsNullOrEmpty(f.PathA));

            foreach (var swFile in swFiles) // other files that weren't patched
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

            this.Worker.ReportProgress((int)States.WaitingClose);
            clientProcess.WaitForExit();
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
                MsgBox.Error(Error.ExeptionParser(e.Error));
            }
            else if (e.Result != null && Convert.ToBoolean(e.Result))
            {
                MsgBox.Error("Your translation files are outdated, force patching will now commence.");
                forceStripMenuItem_Click(null, null);

                return;
            }
            else
                this.RestoreFromTray();

            try
            {
                Methods.RestoreBackup(this.comboBoxLanguages.SelectedItem as Language);
            }
            finally
            {
                this.State = 0;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Methods.RestoreBackup();
            Language[] languages = Methods.GetAvailableLanguages();
            //Language[] languages = new Language[0];
            this.comboBoxLanguages.DataSource = languages.Length > 0 ? languages : null;

            if (String.IsNullOrEmpty(UserSettings.GamePath))
                UserSettings.GamePath = Methods.GetSwPathFromRegistry();
            else if (!Methods.IsSwPath(UserSettings.GamePath))
                UserSettings.GamePath = null;

            if (this.comboBoxLanguages.DataSource != null)
                if (String.IsNullOrEmpty(Strings.LanguageName))
                    Strings.LanguageName = (this.comboBoxLanguages.SelectedItem as Language).Lang;
                else
                {
                    int index = this.comboBoxLanguages.Items.IndexOf(new Language(Strings.LanguageName, DateTime.UtcNow));
                    this.comboBoxLanguages.SelectedIndex = index == -1 ? 0 : index;
                }

            if (!Methods.IsValidSwPatcherPath(Directory.GetCurrentDirectory()))
            {
                string error = "The program is in the same or in a sub folder as your game client.\nThis will cause malfunctions or data corruption on your game client.\nPlease move the patcher in another location or continue at your own risk.";

                Error.Log(error);
                MsgBox.Error(error);
            }
        }

        public IEnumerable<string> GetComboBoxStringItems()
        {
            return this.comboBoxLanguages.Items.Cast<Language>().Select(s => s.Lang);
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
            else if (this.State == States.Idle)
            {
                this.State = States.Downloading;
                this.Downloader.Run(this.comboBoxLanguages.SelectedItem as Language, false);
            }
        }

        private void buttonPlay_Click(object sender, EventArgs e)
        {
            if (this.State == States.WaitingClient)
            {
                this.buttonPlay.Enabled = false;
                this.buttonPlay.Text = Strings.FormText.Cancelling;
                this.Worker.CancelAsync();
            }
            else
            {
                this.State = States.Preparing;
                this.Worker.RunWorkerAsync(this.comboBoxLanguages.SelectedItem as Language);
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

        public void RestoreFromTray()
        {
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            this.Show();

            notifyIcon.Visible = false;
        }

        private void forceStripMenuItem_Click(object sender, EventArgs e)
        {
            Language language = this.comboBoxLanguages.SelectedItem as Language;

            Methods.DeleteTranslationIni(language);
            this.labelNewTranslations.Text = String.Format(Strings.FormText.NewTranslations, language.Lang, Methods.DateToString(language.LastUpdate));

            this.State = States.Downloading;
            this.Downloader.Run(language, true);
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsForm settingsForm = new SettingsForm();
            settingsForm.ShowDialog(this);
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Language language = this.comboBoxLanguages.SelectedItem as Language;
            Language[] languages = Methods.GetAvailableLanguages();
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
                Title = this.Text + " at " + Methods.DateToString(DateTime.UtcNow),
                Text = logText,
                Expiration = PasteBinExpiration.OneWeek,
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

        private void exit_Click(object sender, EventArgs e)
        {
            Strings.LanguageName = this.comboBoxLanguages.SelectedIndex == -1 ? null : (this.comboBoxLanguages.SelectedItem as Language).Lang;
            this.Close();
        }
    }
}
