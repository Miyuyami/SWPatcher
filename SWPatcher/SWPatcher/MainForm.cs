using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SWPatcher.Helpers;

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

        private readonly ModsManagerForm managerForm;
        
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
            managerForm = new ModsManagerForm();
            InitializeComponent();
            Text = AssemblyAccessor.AssemblyTitle;
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

        }

        private void buttonManage_Click(object sender, EventArgs e)
        {
        
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
