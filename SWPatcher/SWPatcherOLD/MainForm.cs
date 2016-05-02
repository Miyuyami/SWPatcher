using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SWPatcher.Classes.Network;

namespace SWPatcher
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
        
        string _SourceFolder = string.Empty;
        Ini.IniFile PatcherSetting = null;
        Classes.TranslationOption theTranslationOption;

        BackgroundWorker bWorkerStartup;
        BackgroundWorker bWorkerBuildEnglishPatch;
        #region "Load"
        private void MainForm_Load(object sender, EventArgs e)
        {
            //Use same Icon
            this.Icon = SWPatcher.Properties.Resources.patcher;

            //because user may change windows user, so i think we shouldn't use %appdata% ....
            this._SourceFolder = System.IO.Directory.GetParent(Application.ExecutablePath).FullName;

            //i think we shouldn't keep OpenDialog in memory while user not using it much.
            this.PatcherSetting = new Ini.IniFile(_SourceFolder + "\\" + Classes.Patcher.Config.Setting);

            this.theTranslationOption = new Classes.TranslationOption(PatcherSetting.GetValue("Translation", "bSelectedTranslation", "true"));

            SettingThingUp();
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.PatcherSetting.GetValue(Classes.Patcher.Section.Patcher, Classes.Patcher.Section.SWPath, "")))
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
                    {
                        this.PatcherSetting.SetValue(Classes.Patcher.Section.Patcher, Classes.Patcher.Section.SWPath, System.IO.Directory.GetParent(Opener.FileName).FullName);
                        this.PatcherSetting.Save();
                    }
                    else
                    {
                        //How should we act when people click cancel this ............ ? Exit or warn them and re-open the OpenFileDialog ... ?
                        MessageBox.Show("Cannot found SoulWorker folder game", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        Application.Exit();
                    }
                }
            }
            var process = Module.Process.getRunning(this.PatcherSetting.GetValue(Classes.Patcher.Section.Patcher, Classes.Patcher.Section.SWPath, "") + "\\soulworker100.exe"); // Check if the game already running to make a easy life
            this.bWorkerStartup.RunWorkerAsync(process); //pass it even if it's null
        }
        #endregion
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
                {
                    PatcherSetting.SetValue(Classes.Patcher.Section.Patcher, Classes.Patcher.Section.SWPath, System.IO.Directory.GetParent(Opener.FileName).FullName);
                    PatcherSetting.Save();
                }
            }
        }

        private void languageList1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            PatcherSetting.SetValue(Classes.Patcher.Section.Patcher, Classes.Patcher.Section.TranslationLanguage, e.ClickedItem.Text);
            PatcherSetting.Save();
            listBoxTrasnaltionlanguage.Text = e.ClickedItem.Text;
        }

        private void MainForm_Closing(object sender, FormClosingEventArgs e)
        {
            Classes.Patcher.BWorkerCode progressID = isInProgress();
            if (progressID > Classes.Patcher.BWorkerCode.None)
            {
                // Promt user when doing progress
                if (MessageBox.Show("Are you sure you want to exit ?\n*Patcher is in progress*", "NOTICE", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
                {
                    if (progressID == Classes.Patcher.BWorkerCode.Patching)
                    {
                        cancelPatch(); // this kinda useless ....
                    }
                }
                else
                    e.Cancel = true;
            }
        }

        #region "Background Workers"

        private Classes.Patcher.BWorkerCode isInProgress()
        {
            if (this.bWorkerStartup.IsBusy)
                return Classes.Patcher.BWorkerCode.Startup;
            else if (this.bWorkerBuildEnglishPatch.IsBusy)
                return Classes.Patcher.BWorkerCode.Patching;
            return Classes.Patcher.BWorkerCode.None;
        }

        private void SettingThingUp()
        {
            this.bWorkerBuildEnglishPatch = new BackgroundWorker();
            this.bWorkerBuildEnglishPatch.WorkerReportsProgress = true;
            this.bWorkerBuildEnglishPatch.WorkerSupportsCancellation = true;
            this.bWorkerBuildEnglishPatch.DoWork += new DoWorkEventHandler(bWorkerBuildEnglishPatch_DoWork);
            this.bWorkerBuildEnglishPatch.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bWorkerBuildEnglishPatch_RunWorkerCompleted);
            this.bWorkerBuildEnglishPatch.ProgressChanged += new ProgressChangedEventHandler(bWorkerBuildEnglishPatch_ProgressChanged);

            this.bWorkerStartup = new BackgroundWorker();
            this.bWorkerStartup.WorkerReportsProgress = true;
            this.bWorkerStartup.WorkerSupportsCancellation = true;
            this.bWorkerStartup.DoWork += new DoWorkEventHandler(bWorkerStartup_DoWork);
            this.bWorkerStartup.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bWorkerStartup_RunWorkerCompleted);
            this.bWorkerStartup.ProgressChanged += new ProgressChangedEventHandler(bWorkerStartup_ProgressChanged);

        }

        #region "waiting for game ??? (This is not needed, i guess)"

        #endregion

        #region "checking at startup"
        private void bWorkerStartup_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker currentBWorker = (BackgroundWorker)sender;
            string selectedLanguage = PatcherSetting.GetValue(Classes.Patcher.Section.Patcher, Classes.Patcher.Section.TranslationLanguage, "English");
            System.Diagnostics.Process foundProcess = (System.Diagnostics.Process)e.Argument;
            currentBWorker.ReportProgress(0, "Checking at Startup");
            string swfolder = PatcherSetting.GetValue(Classes.Patcher.Section.Patcher, Classes.Patcher.Section.SWPath, "");

            SWWebClient theWebClient = new SWWebClient("https://raw.githubusercontent.com/Miyuyami/SWHQPatcher/master/");
            string tmpResult = theWebClient.DownloadString("LanguagePacks.ini");
            Ini.IniFile theIni = new Ini.IniFile(this._SourceFolder + "\\database\\version.ini");
            string currentPatchVersion = theIni.GetValue(selectedLanguage, "date", "");
            theIni.Close();
            string latestPatchVersion = "";
            if (string.IsNullOrEmpty(tmpResult) == false) // Double Check
            {
                using (System.IO.StringReader reader = new System.IO.StringReader(tmpResult))
                {
                    theIni = new Ini.IniFile(reader);
                    foreach (string sectionName in theIni.Sections)
                    {
                        currentBWorker.ReportProgress(1, sectionName);
                        if (sectionName.ToLower() == selectedLanguage.ToLower())
                            latestPatchVersion = theIni.GetValue(sectionName, "date", "22/Feb/2222 10:22 PM");
                    }
                    theIni.Close();
                }
            }

            if (foundProcess == null)
            {
                theIni = new Ini.IniFile(swfolder + "\\Ver.ini");
                string currentClientVersion = theIni.GetValue("client", "ver", "1.0.0.0");
                theIni.Close();
                string serverVer = Module.Hangame.getLatestClientVersion(theWebClient);
                if (currentClientVersion != serverVer)
                    SWPatcher.Module.InvokeMessageBox.Show(this, "Soul Worker client version mismatch with the server.\nClient is out-dated or corrupted. Please check or update the SW Client.\n" +
                        "Current: " + currentClientVersion + "\nLatest: " + serverVer, "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (currentPatchVersion != latestPatchVersion)
                    SWPatcher.Module.InvokeMessageBox.Show(this, "Found new version for " + selectedLanguage + " patch. Please click 'Get latest translation and Patch' to update and build it.\n" +
                        "Current: " + currentPatchVersion + "\nLatest: " + latestPatchVersion, "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                currentBWorker.ReportProgress(3, foundProcess);
            }
        }

        private void bWorkerStartup_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 0)
                this.progressbarText.Text = (string)e.UserState;
            else if (e.ProgressPercentage == 1)
                this.languageList1.Add((string)e.UserState);
            else if (e.ProgressPercentage == 2)
                this.languageList1.SelectedItem = (string)e.UserState;
            else if (e.ProgressPercentage == 3)
                MessageBox.Show("Game is running. Into the waiting for game exit mode right away.");
        }

        private void bWorkerStartup_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (languageList1.Items.Count > 0)
            {
                string selectedLanguage = PatcherSetting.GetValue(Classes.Patcher.Section.Patcher, Classes.Patcher.Section.TranslationLanguage, "English");
                if (string.IsNullOrEmpty(selectedLanguage))
                    this.languageList1.SelectedItem = "English";
                else
                    this.languageList1.SelectedItem = selectedLanguage;
            }
            progressbarText.Text = Classes.Patcher.Strings.ProgressBarReady;
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message + "\n" + e.Error.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region "Patches"
        private void bWorkerBuildEnglishPatch_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker currentBWorker = (BackgroundWorker)sender;
            string selectedLanguage = (string)e.Argument;
            currentBWorker.ReportProgress(3, "Cancel Patching");

            string swfolder = PatcherSetting.GetValue(Classes.Patcher.Section.Patcher, Classes.Patcher.Section.SWPath, "");
            Ini.IniFile theSWIni = new Ini.IniFile(swfolder + "\\Ver.ini");
            string currentClientVersion = theSWIni.GetValue("client", "ver", "1.0.0.0");
            theSWIni.Close();
            Ini.IniFile databaseIni = new Ini.IniFile(this._SourceFolder + "\\database\\version.ini");

            SWWebClient theWebClient = new SWWebClient("http://down.hangame.co.jp/jp/purple/plii/j_sw/");
            //just clone the headers, i was just .... well, for Hangame not doubt about exploiting....
            theWebClient.Headers.Add(System.Net.HttpRequestHeader.UserAgent, "purple");
            currentBWorker.ReportProgress(2, "Initializing");

            string serverVer = string.Empty;
            theWebClient.DownloadFile("ServerVer.ini.zip", this._SourceFolder + "\\ServerVer.ini.zip");
            /*if (!System.IO.File.Exists(this._SourceFolder + "\\ServerVer.ini"))
                throw new Exception("Fail to check game server version");*/
            serverVer = Module.Hangame.getLatestClientVersion(this._SourceFolder + "\\ServerVer.ini.zip");
            System.IO.File.Delete(this._SourceFolder + "\\ServerVer.ini.zip");
            theWebClient.Headers.Remove(System.Net.HttpRequestHeader.UserAgent);

            string pathLatestDate = "";
            theWebClient.BaseAddress = "https://raw.githubusercontent.com/Miyuyami/SWHQPatcher/master/";
            using (System.IO.StringReader sreader = new System.IO.StringReader(theWebClient.DownloadString("LanguagePacks.ini")))
            {
                theSWIni = new Ini.IniFile(sreader);
                pathLatestDate = theSWIni.GetValue(selectedLanguage, "date", "22/Feb/2222 10:22 PM");
                theSWIni.Close();
            }
            if (currentClientVersion != serverVer)
            {
                if (SWPatcher.Module.InvokeMessageBox.Show(this, "Soul Worker client version mismatch with the server.\nClient is out-dated or corrupted. Update or check the SW Client is recommended.\nContinue anyway ?", "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                    return;
            }
            else if (databaseIni.GetValue(selectedLanguage, "date", "") == pathLatestDate)
            {
                if (SWPatcher.Module.InvokeMessageBox.Show(this, "Your patch already in latest release date. Do you want to rebuild the patch ?", "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                    return;
            }

            //Get Data list
            string patcherDataString = string.Empty;
            PatchData currentNode = null;
            string tmpbuffer = null;
            System.Collections.Generic.List<PatchData> PatchDataList = new System.Collections.Generic.List<PatchData>();
            Classes.TranslationOption pickedTranslation = this.theTranslationOption;
            patcherDataString = theWebClient.DownloadString("TranslationPackData.ini");
            if (string.IsNullOrEmpty(patcherDataString) == false)
                using (System.IO.StringReader theStringReader = new System.IO.StringReader(patcherDataString))
                {
                    theSWIni = new Ini.IniFile(theStringReader);
                    foreach (string sectionName in theSWIni.Sections)
                    {
                        tmpbuffer = theSWIni.GetValue(sectionName, "path_d", "");
                        bool asdasd = pickedTranslation.GetValueDictionary(tmpbuffer);
                        if (pickedTranslation.GetValueDictionary(tmpbuffer))
                            PatchDataList.Add(new PatchData(tmpbuffer,
                                theSWIni.GetValue(sectionName, "path", ""),
                                theSWIni.GetValue(sectionName, "path_a", "..\\bin\\Table\\"),
                                theSWIni.GetValue(sectionName, "format", "")));

                    }
                    theSWIni.Close();
                }
            currentNode = null;
            patcherDataString = null;
            tmpbuffer = null;

            //Now we *may* have a full list of required thing, get work
            if (PatchDataList.Count > 0) //just to make sure we got the list .....
            {
                if (System.IO.Directory.Exists(swfolder))
                {
                    theWebClient.BaseAddress = "https://raw.githubusercontent.com/Miyuyami/SoulWorkerHQTranslations/master/" + selectedLanguage + "/";
                    currentBWorker.ReportProgress(2, "Get patch data");
                    //reuse currentNode above, "fake" calcutate the progressbar
                    currentBWorker.ReportProgress(0, PatchDataList.Count); // -1 will make it accurate but i intended it
                    for (int i = 0; i < PatchDataList.Count; i++)
                    {
                        currentNode = PatchDataList[i];
                        tmpbuffer = this._SourceFolder + "\\database\\" + selectedLanguage + "\\" + currentNode.FileName;
                        //Download files
                        //currentBWorker.ReportProgress(3, "https://raw.githubusercontent.com/Miyuyami/SoulWorkerHQTranslations/master/" + selectedLanguage + "/" + currentNode.FileName.Replace("\\", "/"));
                        System.IO.Directory.CreateDirectory(tmpbuffer.Remove(tmpbuffer.LastIndexOf("\\")));
                        theWebClient.DownloadFile(currentNode.FileName.Replace("\\", "/"), this._SourceFolder + "\\database\\" + selectedLanguage + "\\" + currentNode.FileName);
                        currentBWorker.ReportProgress(1, i);
                    }

                    //Walk throught it again, but this time, without zero length files
                    currentBWorker.ReportProgress(2, "Building data");

                    Classes.Patch.PatchBuilder theBuilder = new Classes.Patch.PatchBuilder(this._SourceFolder + "\\database\\" + selectedLanguage, swfolder, PatchDataList);
                    if (theBuilder.Build(currentBWorker))
                    {
                        currentBWorker.ReportProgress(3, "Databuild successfully.");
                        databaseIni.SetValue(selectedLanguage, "date", pathLatestDate);
                        databaseIni.Save();
                    }
                    theBuilder.Close();
                }
            }
            else
            {
                tmpbuffer = null;
                swfolder = null;
                PatchDataList.Clear();
                theSWIni = null;
                PatchDataList = null;
                theWebClient.Close();
                databaseIni.Close();
                throw new Exception("Failed to get patch data.");
            }
            databaseIni.Close();
            tmpbuffer = null;
            swfolder = null;
            PatchDataList.Clear();
            theSWIni = null;
            PatchDataList = null;
            theWebClient.Close();
        }

        public void cancelPatch()
        {
            this.bWorkerBuildEnglishPatch.CancelAsync();
            this.bWorkerBuildEnglishPatch.ReportProgress(2, "Cancelling");
            foreach (System.Diagnostics.Process huehuehue in Module.Process.getAllRunning(this._SourceFolder + "\\SWRFU.exe"))
            {
                try
                {
                    huehuehue.CloseMainWindow();
                }
                catch
                { }
            }
        }

        private void bWorkerBuildEnglishPatch_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 0)
                toolStripProgressBar1.Maximum = (int)e.UserState;
            else if (e.ProgressPercentage == 1)
                toolStripProgressBar1.Value = (int)e.UserState;
            else if (e.ProgressPercentage == 2)
                progressbarText.Text = (string)e.UserState;
            else if (e.ProgressPercentage == 3)
                buttonLastest.Text = (string)e.UserState;
        }

        private void bWorkerBuildEnglishPatch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            toolStripProgressBar1.Value = 0;
            buttonLastest.Text = "Get Latest Translations && Patch";
            progressbarText.Text = Classes.Patcher.Strings.ProgressBarReady;
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message + "\n" + e.Error.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //Perform cleanup if leftover ... ?
            }
            else if (e.Cancelled)
                MessageBox.Show("Clean things up", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); //this is just a placeholder
            else
            {

            }
        }
        #endregion
        #endregion

        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonLastest_Click(object sender, EventArgs e)
        {
            Classes.Patcher.BWorkerCode progressID = isInProgress();
            if (progressID == Classes.Patcher.BWorkerCode.None)
            {
                // Promt user when doing progress
                if (MessageBox.Show("Patcher will get latest translation and build patch for you.\nThis will take several minutes.\nDo you want to continue ?", "NOTICE", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.Yes)
                    this.bWorkerBuildEnglishPatch.RunWorkerAsync(PatcherSetting.GetValue(Classes.Patcher.Section.Patcher, Classes.Patcher.Section.TranslationLanguage, "English"));
            }
            else if (progressID == Classes.Patcher.BWorkerCode.Patching)
            {
                if (MessageBox.Show("Cancel patching progress ?", "NOTICE", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    cancelPatch();
            }
            else
                MessageBox.Show("Patcher is doing something", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void listBoxTrasnaltionlanguage_Click(object sender, EventArgs e)
        {
            using (TranslationOptionForm newTranslationOptionForm = new TranslationOptionForm(this.theTranslationOption))
                newTranslationOptionForm.ShowDialog(this);
        }

        private void optionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SWPatcherOptionForm theForm = new SWPatcherOptionForm(this.PatcherSetting))
                theForm.ShowDialog(this);
        }


    }
}
