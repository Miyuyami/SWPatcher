using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using MadMilkman.Ini;
using SWPatcher.Downloading;
using SWPatcher.General;
using SWPatcher.Helpers;
using SWPatcher.Helpers.GlobalVar;
using SWPatcher.Patching;

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

            this.State = States.Idle;
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            this.Worker.ReportProgress((int)States.Preparing);

            if (e.Argument != null)
            {
                Language language = e.Argument as Language;

                if (Methods.IsNewerGameClientVersion())
                {
                    if (UserSettings.WantToLogin)
                    {
                        Methods.StartReactorToUpdate();
                    }
                    else
                        throw new Exception("Game client is not updated to the latest version.");
                }

                Methods.SetSWFiles(this.SWFiles);

                if (Methods.IsTranslationOutdated(language, this.SWFiles))
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
                        Methods.HangameLogin(client);
                        Methods.GetGameStartResponse(client);
                        string[] gameStartArgs = Methods.GetGameStartArguments(client);

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

                        clientProcess = Methods.GetProcess(Strings.FileName.GameExe);

                        if (clientProcess == null)
                            Thread.Sleep(1000);
                        else
                            break;
                    }
                }

                this.Worker.ReportProgress((int)States.Applying);
                Methods.BackupAndPlaceDataFiles(this.SWFiles, language);
                Methods.BackupAndPlaceOtherFiles(this.SWFiles, language);

                if (startInfo != null)
                    clientProcess = Process.Start(startInfo);

                this.Worker.ReportProgress((int)States.WaitingClose);
                clientProcess.WaitForExit();
            }
            else
            {
                if (UserSettings.WantToLogin)
                {
                    if (Methods.IsNewerGameClientVersion())
                    {
                        Methods.StartReactorToUpdate();
                    }
                    else
                    {
                        ProcessStartInfo startInfo = null;
                        using (var client = new MyWebClient())
                        {
                            Methods.HangameLogin(client);
                            Methods.GetGameStartResponse(client);
                            string[] gameStartArgs = Methods.GetGameStartArguments(client);

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
                MsgBox.Error("Your translation files are outdated, force patching will now commence.");
                forceStripMenuItem_Click(sender, e);

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
                this.State = States.Idle;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Language[] languages = Methods.GetAvailableLanguages();
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

            Methods.StartupBackupCheck(this.comboBoxLanguages.SelectedItem as Language);

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
                this.Worker.RunWorkerAsync(this.comboBoxLanguages.SelectedItem as Language);
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

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.State != States.Idle)
            {
                MsgBox.Error(AssemblyAccessor.Title + " is currently busy and cannot close.");

                e.Cancel = true;
            }
            else
            {
                Strings.LanguageName = this.comboBoxLanguages.SelectedIndex == -1 ? null : (this.comboBoxLanguages.SelectedItem as Language).Lang;
            }
        }

        private void exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
