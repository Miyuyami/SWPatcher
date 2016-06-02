using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Ionic.Zip;
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
            WaitingClose = 6,
            //Disabled = 7,
        }

        private States _state;
        private readonly Downloader Downloader;
        private readonly Patcher Patcher;
        private readonly BackgroundWorker Worker;
        private readonly BackgroundWorker Checker;
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
                        /*case States.Disabled:
                            comboBoxLanguages.Enabled = false;
                            buttonDownload.Enabled = false;
                            buttonDownload.Text = Strings.FormText.Download;
                            buttonPlay.Enabled = false;
                            buttonPlay.Text = Strings.FormText.Play;
                            buttonExit.Enabled = true;
                            forceStripMenuItem.Enabled = false;
                            refreshToolStripMenuItem.Enabled = true;
                            settingsToolStripMenuItem.Enabled = true;
                            toolStripStatusLabel.Text = Strings.FormText.Status.Idle;
                            toolStripProgressBar.Value = toolStripProgressBar.Minimum;
                            toolStripProgressBar.Style = ProgressBarStyle.Blocks;
                            break;*/
                    }
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
                IniFile ini = new IniFile(new IniOptions
                {
                    KeyDuplicate = IniDuplication.Ignored,
                    SectionDuplicate = IniDuplication.Ignored
                });

                string iniPath = Path.Combine(Paths.PatcherRoot, e.Language.Lang, Strings.IniName.Translation);
                if (!File.Exists(iniPath))
                    File.Create(iniPath).Dispose();

                ini.Load(iniPath);
                ini.Sections.Add(Strings.IniName.Patcher.Section);
                ini.Sections[Strings.IniName.Patcher.Section].Keys.Add(Strings.IniName.Pack.KeyDate);
                ini.Sections[Strings.IniName.Patcher.Section].Keys[Strings.IniName.Pack.KeyDate].Value = Methods.DateToString(e.Language.LastUpdate);
                ini.Save(iniPath);

                this.State = 0;
                //this.State = States.Patching;
                //this.Patcher.Run(e.Language);

                return;
            }

            this.State = 0;
        }

        private void Patcher_PatcherProgressChanged(object sender, PatcherProgressChangedEventArgs e)
        {
            if (this.State == States.Patching)
            {
                this.toolStripStatusLabel.Text = String.Format("{0} Step {1}/{2}", Strings.FormText.Status.Patch, e.FileNumber, e.FileCount);
                this.toolStripProgressBar.Value = e.Progress;
            }
        }

        private void Patcher_PatcherCompleted(object sender, PatcherCompletedEventArgs e)
        {
            if (e.Cancelled) { }
            else if (e.Error != null)
            {
                Error.Log(e.Error);
                MsgBox.Error(Error.ExeptionParser(e.Error));
            }
            else
            {
                IniFile ini = new IniFile(new IniOptions { KeyDuplicate = IniDuplication.Ignored, SectionDuplicate = IniDuplication.Ignored });
                ini.Load(Path.Combine(Paths.GameRoot, Strings.IniName.ClientVer));
                string clientVer = ini.Sections[Strings.IniName.Ver.Section].Keys[Strings.IniName.Ver.Key].Value;
                string iniPath = Path.Combine(Paths.PatcherRoot, e.Language.Lang, Strings.IniName.Translation);
                if (!File.Exists(iniPath))
                    File.Create(iniPath).Dispose();
                ini.Sections.Clear();
                ini.Load(iniPath);
                ini.Sections.Add(Strings.IniName.Patcher.Section);
                ini.Sections[Strings.IniName.Patcher.Section].Keys.Add(Strings.IniName.Patcher.KeyVer);
                ini.Sections[Strings.IniName.Patcher.Section].Keys[Strings.IniName.Patcher.KeyVer].Value = clientVer;
                ini.Save(iniPath);
                this.labelNewTranslations.Text = string.Empty;
            }
            this.State = 0;
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Language language = e.Argument as Language;
            if (IsNewerGameClientVersion())
                throw new Exception("Game client is not updated to the latest version.");

            if (IsTranslationOutdated(language))
            {
                e.Result = true; // force patch = true
                return;
            }

            this.SWFiles.Clear();
            using (var client = new System.Net.WebClient())
            using (var file = new TempFile())
            {
                client.DownloadFile(Uris.PatcherGitHubHome + Strings.IniName.TranslationPackData, file.Path);
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
                }
            }

            bool isClientClosed = true;
            Process clientProcess = null;

            this.Worker.ReportProgress((int)States.WaitingClient);
            while (isClientClosed)
            {
                clientProcess = GetProcess(Path.GetFileNameWithoutExtension(Strings.FileName.GameExe));
                if (clientProcess != null)
                    isClientClosed = false;

                Thread.Sleep(100);

                if (this.Worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
            }

            this.Worker.ReportProgress((int)States.Applying);
            foreach (var archive in SWFiles.Select(f => f.Path).Distinct().Where(s => !string.IsNullOrEmpty(s))) // backup and place translated .v's
            {
                string archivePath = Path.Combine(Paths.GameRoot, archive);
                string backupFilePath = Path.Combine(Paths.PatcherRoot, Strings.FolderName.Backup, archive);
                string backupFileDirectory = Path.GetDirectoryName(backupFilePath);
                if (!Directory.Exists(backupFileDirectory))
                    Directory.CreateDirectory(backupFileDirectory);
                File.Move(archivePath, backupFilePath);
                File.Move(Path.Combine(Paths.PatcherRoot, language.Lang, archive), archivePath);
            }
            foreach (var file in SWFiles.Where(f => string.IsNullOrEmpty(f.PathA))) // other files(.zip) that weren't patched
            {
                string filePath = Path.Combine(Paths.PatcherRoot, language.Lang, file.PathD);
                string destination = Path.Combine(Paths.GameRoot, file.Path);
                using (var zip = ZipFile.Read(filePath))
                    zip.ExtractAll(destination, ExtractExistingFileAction.OverwriteSilently);
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

            notifyIcon_DoubleClick(null, null);
            RestoreBackup(this.comboBoxLanguages.SelectedItem as Language);
            this.State = 0;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                RestoreBackup();
                CheckForProgramFolderMalfunction(Path.GetDirectoryName(Paths.PatcherRoot));
                comboBoxLanguages.DataSource = GetAllAvailableLanguages();
            }
            catch (Exception ex)
            {
                Error.Log(ex);
                this.Close();
            }
        }

        private void buttonDownload_Click(object sender, EventArgs e)
        {
            if (this.State == States.Downloading)
            {
                this.Downloader.Cancel();
                // button cancelling.... + disable
                this.State = 0;
            }
            else if (this.State == States.Patching)
            {
                this.Patcher.Cancel();
                this.State = 0;
            }
            else if (this.State == 0)
            {
                try
                {
                    CheckForSWPath();
                    CheckForGameClientUpdate();
                    this.State = States.Downloading;
                    this.Downloader.Run(this.comboBoxLanguages.SelectedItem as Language, false);
                }
                catch (Exception ex)
                {
                    Error.Log(ex);
                    this.Close();
                }
            }
        }

        private void buttonPlay_Click(object sender, EventArgs e)
        {
            if (this.State == States.WaitingClient)
            {
                this.Worker.CancelAsync();
                this.State = 0;
            }
            else
            {
                try
                {
                    CheckForSWPath();
                    CheckForGameClientUpdate();
                    this.State = States.Preparing;
                    this.Worker.RunWorkerAsync(this.comboBoxLanguages.SelectedItem as Language);
                }
                catch (Exception ex)
                {
                    Error.Log(ex);
                    this.Close();
                }
            }
        }

        private void comboBoxLanguages_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Methods.HasNewTranslations(this.comboBoxLanguages.SelectedItem as Language))
                this.labelNewTranslations.Text = Strings.FormText.NewTranslations;
            else
                this.labelNewTranslations.Text = string.Empty;
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
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            this.Show();
            notifyIcon.Visible = false;
        }

        private void forceStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Language language = this.comboBoxLanguages.SelectedItem as Language;
                string iniPath = Path.Combine(Paths.PatcherRoot, language.Lang, Strings.IniName.Translation);
                if (Directory.Exists(Path.GetDirectoryName(iniPath)))
                    File.Delete(iniPath);
                this.labelNewTranslations.Text = Strings.FormText.NewTranslations;
                this.State = States.Downloading;
                this.Downloader.Run(language, true);
            }
            catch (Exception ex)
            {
                Error.Log(ex);
                this.Close();
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsForm settingsForm = new SettingsForm();
            settingsForm.ShowDialog(this);
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Language language = this.comboBoxLanguages.SelectedItem as Language;
                comboBoxLanguages.DataSource = GetAllAvailableLanguages();

                int index = comboBoxLanguages.Items.IndexOf(language);
                comboBoxLanguages.SelectedIndex = index == -1 ? 0 : index;
            }
            catch (Exception ex)
            {
                Error.Log(ex);
                this.Close();
            }
        }

        private void openSWWebpageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("iexplore.exe", Uris.SoulWorkerHome);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox aboutBox = new AboutBox();
            aboutBox.ShowDialog(this);
        }

        private void exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
