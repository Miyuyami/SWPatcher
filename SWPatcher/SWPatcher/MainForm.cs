using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SWPatcher.Helpers;
using SWPatcher.Helpers.GlobalVars;
using System.Net;
using SWPatcher.General;
using System.Diagnostics;

namespace SWPatcher
{
    public partial class MainForm : Form
    {
        public enum States
        {
            Idle = 0,
            CheckingVersion,
            Downloading,
            Patching
        }

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

        public MainForm()
        {
            InitializeComponent();
            Text = AssemblyAccessor.Title;
            buttonLastest.Text = Strings.FormText.Download;
            buttonPatch.Text = Strings.FormText.Patch;
            toolStripStatusLabel.Text = Strings.FormText.Status.Idle;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox aboutBox = new AboutBox();
            aboutBox.ShowDialog();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            CheckForProgramUpdate();
            PopulateList();
        }

        private void CheckForProgramUpdate()
        {
            using (var client = new WebClient())
            using (var file = new TempFile())
            {
                client.DownloadFile(Uris.PatcherGitHubHome + Strings.IniName.PatcherVersion, file.Path);
                IniReader ini = new IniReader(file.Path);
                Version current = new Version(AssemblyAccessor.Version);
                Version read = new Version(ini.ReadString(Strings.IniName.Patcher.Section, Strings.IniName.Patcher.KeyVer));
                if (current.CompareTo(read) <= 0)
                {
                    string address = ini.ReadString(Strings.IniName.Patcher.Section, Strings.IniName.Patcher.KeyAddress);
                    DialogResult newVersionDialog = MsgBox.Question("There is a new patcher version available!\n\nYes - Application will close and redirect you to the patcher website.\nNo - Ignore");
                    if (newVersionDialog == DialogResult.Yes)
                    {
                        Process.Start(address);
                        this.Close();
                    }
                    else
                    {
                        DialogResult newVersionDialog2 = MsgBox.Question("Are you sure you want to ignore the update?\nIt might cause unknown problems!");
                        if (newVersionDialog2 == DialogResult.No)
                        {
                            Process.Start(address);
                        this.Close();
                        }
                    }
                }
            }
        }

        private void buttonManage_Click(object sender, EventArgs e)
        {
            ModsManagerForm modsManagerForm = new ModsManagerForm();
            modsManagerForm.ShowDialog();
        }

        private void buttonLastest_Click(object sender, EventArgs e)
        {
            
        }

        private void buttonPatch_Click(object sender, EventArgs e)
        {
            
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
