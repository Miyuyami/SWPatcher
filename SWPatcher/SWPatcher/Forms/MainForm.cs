using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            Patching
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
                            buttonPatch.Enabled = true;
                            buttonPatch.Text = Strings.FormText.Patch;
                            toolStripStatusLabel.Text = Strings.FormText.Status.Idle;
                            toolStripProgressBar.Value = toolStripProgressBar.Minimum;
                            break;
                        case States.Downloading:
                            comboBoxLanguages.Enabled = false;
                            buttonLastest.Enabled = true;
                            buttonLastest.Text = Strings.FormText.Cancel;
                            buttonPatch.Enabled = false;
                            buttonPatch.Text = Strings.FormText.Patch;
                            toolStripStatusLabel.Text = Strings.FormText.Status.Download;
                            toolStripProgressBar.Value = toolStripProgressBar.Minimum;
                            break;
                        case States.Patching:
                            comboBoxLanguages.Enabled = false;
                            buttonLastest.Enabled = false;
                            buttonLastest.Text = Strings.FormText.Download;
                            buttonPatch.Enabled = true;
                            buttonPatch.Text = Strings.FormText.Cancel;
                            toolStripStatusLabel.Text = Strings.FormText.Status.Patch;
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
            //this.Patcher.PatcherProgressChanged +=
            //this.Patcher.PatcherCompleted +=
            InitializeComponent();
            this.Text = AssemblyAccessor.Title + " " + AssemblyAccessor.Version;
            this.buttonLastest.Text = Strings.FormText.Download;
            this.buttonPatch.Text = Strings.FormText.Patch;
            this.toolStripStatusLabel.Text = Strings.FormText.Status.Idle;
        }

        private void Downloader_DownloaderProgressChanged(object sender, DownloaderProgressChangedEventArgs e)
        {
            this.toolStripStatusLabel.Text = string.Format("{0} {1} ({2}/{3})", Strings.FormText.Status.Download, e.FileName, e.FileNumber, e.TotalFileCount);
            this.toolStripProgressBar.Value = e.ProgressPercentage;
        }

        private void Downloader_DownloaderCompleted(object sender, DownloaderDownloadCompletedEventArgs e)
        {
            if (e.IsSame)
                MsgBox.Success(string.Format("You already have the latest({0} JST) translation files for this language!", Strings.DateToString(e.Language.LastUpdate)));
            else
            {
                IniReader translationIni = new IniReader(Path.Combine(Paths.PatcherRoot, e.Language.Lang, Strings.IniName.Translation));
                translationIni.Write(Strings.IniName.Patcher.Section, Strings.IniName.Pack.KeyDate, Strings.DateToString(e.Language.LastUpdate));
            }
            this.State = 0;
            OfferPatchNow();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox aboutBox = new AboutBox();
            aboutBox.ShowDialog(this);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            RestoreBackup();
            if (CheckForProgramUpdate() || CheckForProgramFolderMalfunction(Path.GetDirectoryName(Paths.PatcherRoot)) || CheckForSWPath() || CheckForGameClientUpdate() || (comboBoxLanguages.DataSource = GetAllAvailableLanguages()) == null)
                return;
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
                this.Downloader.Run(this.comboBoxLanguages.SelectedItem as Language);
            }
        }

        private void buttonPatch_Click(object sender, EventArgs e)
        {
            if (this.State == States.Patching)
            {
                this.Patcher.Cancel();
                this.State = 0;
            }
            else if (this.State == 0)
            {
                this.State = States.Patching;
                this.Patcher.Run(this.comboBoxLanguages.SelectedItem as Language);
            }
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
