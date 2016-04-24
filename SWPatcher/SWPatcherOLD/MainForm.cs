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

        const string progressStep = "Ready your soul"; //um....i don't know .....
        string _SourceFolder = string.Empty;
        IniFile PatcherSetting = null;

        BackgroundWorker BWorkerCheckAndInstallEnglishPatch;

        private void MainForm_Load(object sender, EventArgs e)
        {
            //because user may change windows user, so i think we shouldn't use %appdata% ....
            this._SourceFolder = System.IO.Directory.GetParent(Application.ExecutablePath).FullName;

            //i think we shouldn't keep OpenDialog in memory while user not using it much.
            PatcherSetting = new IniFile(_SourceFolder + "\\Settings.ini");

            SettingThingUp();

        }

        private void MainForm_Shown(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(PatcherSetting.IniReadValue("patcher", "folder")))
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
                        PatcherSetting.IniWriteValue("patcher", "folder", System.IO.Directory.GetParent(Opener.FileName).FullName);
                    else
                    {
                        //How should we act when people click cancel this ............ ? Exit or warn them and re-open the OpenFileDialog ... ?
                        MessageBox.Show("Cannot found SoulWorker folder game", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        Application.Exit();
                    }
                }
            }
            System.Net.WebClient theWebClient = new System.Net.WebClient();
            theWebClient.BaseAddress = "https://raw.githubusercontent.com/Miyuyami/SoulWorkerHQTranslations/master/";
            theWebClient.Proxy = null;
            string tmpResult = string.Empty;
            for (short i = 0; i <= 2; i++)
            {
                try
                {
                    tmpResult = theWebClient.DownloadString("Languages");
                }
                catch (System.Net.WebException webEx)
                {
                    if (((System.Net.HttpWebResponse)webEx.Response).StatusCode == System.Net.HttpStatusCode.NotFound)
                        break;
                }
                if (string.IsNullOrEmpty(tmpResult) == false)
                    break;
            }

            if (string.IsNullOrEmpty(tmpResult) == false) // Double Check
            {
                foreach (string supportLanguage in tmpResult.Split('\n'))
                    comboBoxLanguages.Items.Add(supportLanguage);
                if (comboBoxLanguages.Items.Count > 0)
                {
                    string lastchosenLanguage = PatcherSetting.IniReadValue("patcher", "s_translationlanguage");
                    if (string.IsNullOrEmpty(lastchosenLanguage))
                        comboBoxLanguages.SelectedIndex = 0;
                    else
                        comboBoxLanguages.SelectedItem = lastchosenLanguage;
                    lastchosenLanguage = null;
                }
            }
            progressbarText.Text = progressStep;
            theWebClient.Dispose(); // just in case (i'll always leave this at the end of method)
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
                    PatcherSetting.IniWriteValue("patcher", "folder", System.IO.Directory.GetParent(Opener.FileName).FullName);
            }
        }

        private void comboBoxLanguages_SelectedIndexChanged(object sender, EventArgs e)
        {
            PatcherSetting.IniWriteValue("patcher", "s_translationlanguage", (string)comboBoxLanguages.SelectedItem);
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MainForm_Closing(object sender, FormClosingEventArgs e)
        {
            int progressID = isInProgress();
            if (progressID > 0)
            {
                // Promt user when doing progress
                if (MessageBox.Show("Are you sure you want to exit ?\n*Patcher is in progress*", "NOTICE", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
                {
                    // Stop progress and clean up according to progress's ID ... ?
                }
                else
                    e.Cancel = true;
            }
        }

        private void buttonLastest_Click(object sender, EventArgs e)
        {
            int progressID = isInProgress();
            if (progressID == 0)
            {
                // Promt user when doing progress
                if (MessageBox.Show("Patcher will get latest translation and build patch for you.\nThis will take several minutes.\nDo you want to continue ?", "NOTICE", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
                {
                    this.BWorkerCheckAndInstallEnglishPatch.RunWorkerAsync(comboBoxLanguages.SelectedItem);
                }
            }
            else if (progressID == 1)
                MessageBox.Show("Patcher already in progress...", "Notice",  MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("Patcher is doing something", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #region "Background Workers"

        private int isInProgress()
        {
            if (this.BWorkerCheckAndInstallEnglishPatch.IsBusy)
                return 1;
            return 0;
        }

        private void SettingThingUp()
        {
            this.BWorkerCheckAndInstallEnglishPatch = new BackgroundWorker();
            this.BWorkerCheckAndInstallEnglishPatch.WorkerReportsProgress = true;
            this.BWorkerCheckAndInstallEnglishPatch.WorkerSupportsCancellation = true;
            this.BWorkerCheckAndInstallEnglishPatch.DoWork += new DoWorkEventHandler(BWorkerCheckAndInstallEnglishPatch_DoWork);
            this.BWorkerCheckAndInstallEnglishPatch.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BWorkerCheckAndInstallEnglishPatch_RunWorkerCompleted);
            this.BWorkerCheckAndInstallEnglishPatch.ProgressChanged += new ProgressChangedEventHandler(BWorkerCheckAndInstallEnglishPatch_ProgressChanged);

            

        }

        private void BWorkerCheckAndInstallEnglishPatch_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker currentBWorker = (BackgroundWorker)sender;
            string selectedLanguage = (string)e.Argument;
            currentBWorker.ReportProgress(2, "Initializing");
            using (System.Net.WebClient theWebClient = new System.Net.WebClient())
            {
                //Get Data list
                string patcherDataString = string.Empty;
                string bufferedLine = null;
                PatchData currentNode = null;
                string[] tmpSplit = null;
                System.Collections.Generic.List<PatchData> PatchDataList = new System.Collections.Generic.List<PatchData>();
                theWebClient.Proxy = null;
                theWebClient.BaseAddress = "https://raw.githubusercontent.com/Miyuyami/SoulWorkerHQTranslations/master/";                
                for (short i = 0; i <= 2; i++)
                {
                    try
                    {
                        patcherDataString = theWebClient.DownloadString("PatcherData");
                        if (string.IsNullOrEmpty(patcherDataString) == false)
                        {
                            using (System.IO.StringReader theStringReader = new System.IO.StringReader(patcherDataString))
                                while (theStringReader.Peek() > 0)
                                {
                                    bufferedLine = theStringReader.ReadLine();
                                    if (bufferedLine.IndexOf(" ") > -1)
                                    {
                                        tmpSplit = bufferedLine.Split(' ');
                                        if (tmpSplit[0].EndsWith(".txt") || tmpSplit[0].EndsWith(".html"))
                                        {
                                            currentNode = new PatchData(tmpSplit[0], tmpSplit[1]);
                                            PatchDataList.Add(currentNode);
                                        }
                                        else
                                            currentNode.Param = bufferedLine;
                                    }
                                }
                        }
                        break;
                    }
                    catch (System.Net.WebException webEx)
                    {
                        if (((System.Net.HttpWebResponse)webEx.Response).StatusCode == System.Net.HttpStatusCode.NotFound)
                            break;
                    }
                }
                tmpSplit = null;
                currentNode = null;
                bufferedLine = null;
                patcherDataString = null;

                //Now we *may* have a full list of required thing, get work
                if (PatchDataList.Count > 0) //just to make sure we got the list .....
                {
                    currentBWorker.ReportProgress(2, "Get patch data");
                    //reuse currentNode above
                    currentBWorker.ReportProgress(0, PatchDataList.Count - 1);
                    for (int i = 0; i < PatchDataList.Count; i++)
                    {
                        currentNode = PatchDataList[i];
                        //Download files
                        if (System.IO.Directory.Exists(_SourceFolder + "\\" + selectedLanguage + "\\" + currentNode.TargetData) == false)
                            System.IO.Directory.CreateDirectory(_SourceFolder + "\\" + selectedLanguage + "\\" + currentNode.TargetData);
                        theWebClient.DownloadFile(selectedLanguage + "/" + currentNode.TargetData + "/" + currentNode.FileName, _SourceFolder + "\\" + selectedLanguage + "\\" + currentNode.TargetData + "\\" + currentNode.FileName);
                        currentBWorker.ReportProgress(1, i);
                    }
                }
                else
                    throw new Exception("Failed to get patch data.");

                //Now we build the patch.....but......i don't know how so ................ sorry ;w;
            }
        }

        private void BWorkerCheckAndInstallEnglishPatch_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 0)
                toolStripProgressBar1.Maximum = (int)e.UserState;
            else if (e.ProgressPercentage == 1)
                toolStripProgressBar1.Value = (int)e.UserState;
            else if (e.ProgressPercentage == 2)
                progressbarText.Text = (string)e.UserState;
        }

        private void BWorkerCheckAndInstallEnglishPatch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressbarText.Text = progressStep;
            if (e.Error != null)
                MessageBox.Show(e.Error.Message, "Error",   MessageBoxButtons.OK,  MessageBoxIcon.Error);
                //Perform cleanup if leftover ... ?
            else if (e.Cancelled)
                MessageBox.Show("Clean things up", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); //this is just a placeholder
            else
            {

            }
        }

        #endregion

    }
}
