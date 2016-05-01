using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Windows.Forms;
using SWPatcher.General;
using SWPatcher.Helpers;
using SWPatcher.Helpers.GlobalVars;
using System.IO;
using System.Collections.Generic;

namespace SWPatcher.Forms
{
    public partial class Main : Form
    {
        public enum States
        {
            Idle = 0,
            CheckingVersion,
            Downloading,
            Patching
        }

        private List<SWFile> FileList;
        private readonly BackgroundWorker Worker;
        private States _state;

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
                            buttonManage.Enabled = true;
                            buttonLastest.Enabled = true;
                            buttonLastest.Text = Strings.FormText.Download;
                            buttonPatch.Enabled = true;
                            buttonPatch.Text = Strings.FormText.Patch;
                            toolStripStatusLabel.Text = Strings.FormText.Status.Idle;
                            toolStripProgressBar.Value = toolStripProgressBar.Minimum;
                            break;
                        case States.CheckingVersion:
                            comboBoxLanguages.Enabled = false;
                            buttonManage.Enabled = false;
                            buttonLastest.Enabled = false;
                            buttonLastest.Text = Strings.FormText.Download;
                            buttonPatch.Enabled = false;
                            buttonPatch.Text = Strings.FormText.Patch;
                            toolStripStatusLabel.Text = Strings.FormText.Status.Check;
                            toolStripProgressBar.Value = toolStripProgressBar.Minimum;
                            break;
                        case States.Downloading:
                            comboBoxLanguages.Enabled = false;
                            buttonManage.Enabled = false;
                            buttonLastest.Enabled = true;
                            buttonLastest.Text = Strings.FormText.Cancel;
                            buttonPatch.Enabled = false;
                            buttonPatch.Text = Strings.FormText.Patch;
                            toolStripStatusLabel.Text = Strings.FormText.Status.Download;
                            toolStripProgressBar.Value = toolStripProgressBar.Minimum;
                            break;
                        case States.Patching:
                            comboBoxLanguages.Enabled = false;
                            buttonManage.Enabled = false;
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

        public Main()
        {
            InitializeComponent();
            Text = AssemblyAccessor.Title;
            buttonLastest.Text = Strings.FormText.Download;
            buttonPatch.Text = Strings.FormText.Patch;
            toolStripStatusLabel.Text = Strings.FormText.Status.Idle;
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings settings = new Settings();
            settings.ShowDialog(this);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox aboutBox = new AboutBox();
            aboutBox.ShowDialog(this);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            RestoreBackup();
            if (CheckForProgramUpdate())
                return;
            if (CheckForProgramFolderMalfunction(Path.GetDirectoryName(Paths.PatcherRoot)))
                return;
            if (CheckForSWPath())
                return;
            if (CheckForGameClientUpdate())
                return;
            if (PopulateList())
                return;
        }

        private void buttonManage_Click(object sender, EventArgs e)
        {
            ModsManager modsManager = new ModsManager();
            modsManager.ShowDialog(this);
        }

        private void buttonLastest_Click(object sender, EventArgs e)
        {

        }

        private void buttonPatch_Click(object sender, EventArgs e)
        {

        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
