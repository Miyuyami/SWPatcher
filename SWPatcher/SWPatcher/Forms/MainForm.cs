using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Linq;
using SWPatcher.Downloading;
using SWPatcher.General;
using SWPatcher.Helpers;
using SWPatcher.Helpers.GlobalVar;
using SWPatcher.Patching;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using Ionic.Zip;

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
            Playing = 4
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
                            toolStripStatusLabel.Text = Strings.FormText.Status.Prepare;
                            toolStripProgressBar.Value = toolStripProgressBar.Minimum;
                            toolStripProgressBar.Style = ProgressBarStyle.Marquee;
                            break;
                        case States.Playing:
                            comboBoxLanguages.Enabled = false;
                            buttonDownload.Enabled = false;
                            buttonDownload.Text = Strings.FormText.Download;
                            buttonPlay.Enabled = true;
                            buttonPlay.Text = Strings.FormText.Cancel;
                            buttonExit.Enabled = false;
                            forceStripMenuItem.Enabled = false;
                            toolStripStatusLabel.Text = Strings.FormText.Status.Play;
                            toolStripProgressBar.Value = toolStripProgressBar.Minimum;
                            toolStripProgressBar.Style = ProgressBarStyle.Blocks;
                            break;
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
                this.toolStripStatusLabel.Text = string.Format("{0} {1} ({2}/{3})", Strings.FormText.Status.Download, e.FileName, e.FileNumber, e.FileCount);
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
                IniReader translationIni = new IniReader(Path.Combine(Paths.PatcherRoot, e.Language.Lang, Strings.IniName.Translation));
                translationIni.Write(Strings.IniName.Patcher.Section, Strings.IniName.Pack.KeyDate, Strings.DateToString(e.Language.LastUpdate));

                this.State = States.Patching;
                this.Patcher.Run(e.Language);
            }
        }

        private void Patcher_PatcherProgressChanged(object sender, PatcherProgressChangedEventArgs e)
        {
            if (this.State == States.Patching)
            {
                this.toolStripStatusLabel.Text = string.Format("{0} Step {1}/{2}", Strings.FormText.Status.Patch, e.FileNumber, e.FileCount);
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
                IniReader clientIni = new IniReader(Path.Combine(Paths.GameRoot, Strings.IniName.ClientVer));
                IniReader translationIni = new IniReader(Path.Combine(Paths.PatcherRoot, e.Language.Lang, Strings.IniName.Translation));
                translationIni.Write(Strings.IniName.Patcher.Section, Strings.IniName.Patcher.KeyVer, clientIni.ReadString(Strings.IniName.Ver.Section, Strings.IniName.Ver.Key));
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
                IniReader dataIni = new IniReader(file.Path);
                var array = dataIni.GetSectionNames();
                foreach (var fileName in array)
                {
                    string name = fileName as string;
                    dataIni.Section = name;
                    string path = dataIni.ReadString(Strings.IniName.Pack.KeyPath);
                    string pathA = dataIni.ReadString(Strings.IniName.Pack.KeyPathInArchive);
                    string pathD = dataIni.ReadString(Strings.IniName.Pack.KeyPathOfDownload);
                    string format = dataIni.ReadString(Strings.IniName.Pack.KeyFormat);
                    this.SWFiles.Add(new SWFile(name, path, pathA, pathD, format));
                }
            }

            this.Worker.ReportProgress(4); // States.Playing;
            bool isClientClosed = true;
            Process clientProcess = null;
            while (isClientClosed)
            {
                if (this.Worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                clientProcess = GetProcess(Path.GetFileNameWithoutExtension(Strings.FileName.GameExe));
                Thread.Sleep(1000);
                if (clientProcess != null)
                    isClientClosed = false;
            }

            this.Worker.ReportProgress(3); // States.Preparing;
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

            this.WindowState = FormWindowState.Minimized;
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
            {
                this.WindowState = FormWindowState.Normal;
            }
            RestoreBackup(this.comboBoxLanguages.SelectedItem as Language);
            this.State = 0;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                RestoreBackup();
                CheckForProgramUpdate();
                CheckForProgramFolderMalfunction(Path.GetDirectoryName(Paths.PatcherRoot));
                CheckForSWPath();
                CheckForGameClientUpdate();
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
                this.State = 0;
            }
            else if (this.State == States.Patching)
            {
                this.Patcher.Cancel();
                this.State = 0;
            }
            else if (this.State == 0)
            {
                this.State = States.Downloading;
                this.Downloader.Run(this.comboBoxLanguages.SelectedItem as Language, false);
            }
        }

        private void buttonPlay_Click(object sender, EventArgs e)
        {
            if (this.State == States.Playing)
            {
                this.Worker.CancelAsync();
                this.State = 0;
            }
            else
            {
                this.State = States.Preparing;
                this.Worker.RunWorkerAsync(this.comboBoxLanguages.SelectedItem as Language);
            }
        }

        private void forceStripMenuItem_Click(object sender, EventArgs e)
        {
            Language language = this.comboBoxLanguages.SelectedItem as Language;
            File.Delete(Path.Combine(Paths.PatcherRoot, language.Lang, Strings.IniName.Translation));
            this.State = States.Downloading;
            this.Downloader.Run(language, true);
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
