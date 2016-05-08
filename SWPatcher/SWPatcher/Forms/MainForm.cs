using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
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
            Downloading,
            Patching,
            WaitingForGame
        }

        private States _state;
        private Downloader Downloader;
        private Patcher Patcher;
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
                            buttonLastest.Enabled = true;
                            buttonLastest.Text = Strings.FormText.Download;
                            buttonPlay.Enabled = true;
                            buttonPlay.Text = Strings.FormText.Play;
                            forceStripMenuItem.Enabled = true;
                            forceStripMenuItem.Text = Strings.FormText.ForcePatch;
                            toolStripStatusLabel.Text = Strings.FormText.Status.Idle;
                            toolStripProgressBar.Value = toolStripProgressBar.Minimum;
                            break;
                        case States.Downloading:
                            comboBoxLanguages.Enabled = false;
                            buttonLastest.Enabled = true;
                            buttonLastest.Text = Strings.FormText.Cancel;
                            buttonPlay.Enabled = false;
                            buttonPlay.Text = Strings.FormText.Play;
                            forceStripMenuItem.Enabled = false;
                            forceStripMenuItem.Text = Strings.FormText.ForcePatch;
                            toolStripStatusLabel.Text = Strings.FormText.Status.Download;
                            toolStripProgressBar.Value = toolStripProgressBar.Minimum;
                            break;
                        case States.Patching:
                            comboBoxLanguages.Enabled = false;
                            buttonLastest.Enabled = false;
                            buttonLastest.Text = Strings.FormText.Download;
                            buttonPlay.Enabled = false;
                            buttonPlay.Text = Strings.FormText.Play;
                            forceStripMenuItem.Enabled = true;
                            forceStripMenuItem.Text = Strings.FormText.Cancel;
                            toolStripStatusLabel.Text = Strings.FormText.Status.Patch;
                            toolStripProgressBar.Value = toolStripProgressBar.Minimum;
                            break;
                        case States.WaitingForGame:
                            comboBoxLanguages.Enabled = false;
                            buttonLastest.Enabled = false;
                            buttonLastest.Text = Strings.FormText.Download;
                            buttonPlay.Enabled = true;
                            buttonPlay.Text = Strings.FormText.Cancel;
                            forceStripMenuItem.Enabled = false;
                            forceStripMenuItem.Text = Strings.FormText.ForcePatch;
                            toolStripStatusLabel.Text = Strings.FormText.Status.WaitingForGame;
                            toolStripProgressBar.Value = toolStripProgressBar.Minimum;
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
            InitializeComponent();
            this.Text = AssemblyAccessor.Title + " " + AssemblyAccessor.Version;
        }

        private void Downloader_DownloaderProgressChanged(object sender, DownloaderProgressChangedEventArgs e)
        {
            if (this.State == States.Downloading)
            {
                this.toolStripStatusLabel.Text = string.Format("{0} {1} ({2}/{3})", Strings.FormText.Status.Download, e.FileName, e.FileNumber, e.TotalFileCount);
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
            }
            this.State = 0;
        }

        private void Patcher_PatcherProgressChanged(object sender, PatcherProgressChangedEventArgs e)
        {
            if (this.State == States.Patching)
            {
                this.toolStripStatusLabel.Text = string.Format("{0} {1} ({2}/{3})", Strings.FormText.Status.Download, e.FileName, e.FileNumber, e.TotalFileCount);
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
                
                // start patcher
            }
            this.State = 0;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox aboutBox = new AboutBox();
            aboutBox.ShowDialog(this);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //if (CheckForProgramUpdate() || CheckForProgramFolderMalfunction(Path.GetDirectoryName(Paths.PatcherRoot)) || CheckForSWPath() || CheckForGameClientUpdate() || (comboBoxLanguages.DataSource = GetAllAvailableLanguages()) == null)
            //    return;
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

        private void buttonLastest_Click(object sender, EventArgs e)
        {
            if (this.State == States.Downloading)
            {
                this.Downloader.Cancel();
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

        }

        private void forceStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.State == States.Downloading)
            {
                this.Downloader.Cancel();
                this.State = 0;
            }
            else if (this.State == 0)
            {
                this.State = States.Downloading;
                this.Downloader.Run(this.comboBoxLanguages.SelectedItem as Language, true);
            }
        }

        private void exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
