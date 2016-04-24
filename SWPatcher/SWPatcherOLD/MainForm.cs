using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SWPatcher
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        string _SourceFolder = string.Empty;
        IniFile PatcherSetting = null;

        private void MainForm_Shown(object sender, EventArgs e)
        {
            //because user may change windows user, so i think we shouldn't use %appdata% ....
            this._SourceFolder = System.IO.Directory.GetParent(Application.ExecutablePath).FullName;

            //i think we shouldn't keep OpenDialog in memory while user not using it much.
            PatcherSetting = new IniFile(_SourceFolder + "\\Settings.ini");

            if (string.IsNullOrWhiteSpace(PatcherSetting.IniReadValue("sw", "folder")))
            {
                using (OpenFileDialog Opener = new OpenFileDialog())
                {
                    Opener.Multiselect = false;
                    Opener.CheckFileExists = true;
                    Opener.CheckPathExists = true;
                    Opener.Title = "Select game executable file";
                    Opener.FileName = "soulworker100";
                    Opener.Filter = "soulworker100.exe|soulworker100.exe"; // hard coded or *.exe ?
                    Opener.DefaultExt = "exe";
                    if (Opener.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                        PatcherSetting.IniWriteValue("sw", "folder", System.IO.Directory.GetParent(Opener.FileName).FullName);
                    else
                    {
                        //How should we act when people click cancel this ............ ? Exit or warn them and re-open the OpenFileDialog ... ?
                        MessageBox.Show("Cannot found SoulWorker folder game", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        Application.Exit();
                    }
                }
            }
        }

        private void buttonResetSWFolder_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog Opener = new OpenFileDialog())
            {
                Opener.Multiselect = false;
                Opener.CheckFileExists = true;
                Opener.CheckPathExists = true;
                Opener.Title = "Select game executable file";
                Opener.FileName = "soulworker100";
                Opener.Filter = "soulworker100.exe|soulworker100.exe";
                Opener.DefaultExt = "exe";
                if (Opener.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                    PatcherSetting.IniWriteValue("sw", "folder", System.IO.Directory.GetParent(Opener.FileName).FullName);
            }
        }
    }
}
