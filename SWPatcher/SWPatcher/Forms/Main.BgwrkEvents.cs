using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using SWPatcher.Helpers;
using System.Net;
using SWPatcher.General;
using SWPatcher.Helpers.GlobalVar;

namespace SWPatcher.Forms
{
    partial class Main
    {
        private void workerLatest_DoWork(object sender, DoWorkEventArgs e)
        {
            string lang = e.Argument as string;
            this.WorkerLatest.ReportProgress(5);
            if (IsNewerTranslationVersion(lang))
            {
                using (var client = new WebClient())
                using (var file = new TempFile())
                {
                    try
                    {
                        client.DownloadFile(Uris.PatcherGitHubHome + Strings.IniName.TranslationPackData, file.Path);
                    }
                    catch (WebException)
                    {
                        e.Cancel = true;
                        e.Result = null;
                        MsgBox.Error("Could not connect to download server.\nTry again later.");
                    }
                    this.WorkerLatest.ReportProgress(10);
                    IniReader dataIni = new IniReader(file.Path);
                    foreach (var fileName in dataIni.GetSectionNames())
                    {
                        string name = fileName as string;
                        dataIni.Section = name;
                        string path = dataIni.ReadString(Strings.IniName.Pack.KeyPath);
                        string pathA = dataIni.ReadString(Strings.IniName.Pack.KeyPathInArchive);
                        string pathD = dataIni.ReadString(Strings.IniName.Pack.KeyPathOfDownload);
                        string format = dataIni.ReadString(Strings.IniName.Pack.KeyFormat);
                        SWFiles.Add(new SWFile(name, path, pathA, pathD, format));
                    }
                }
            }
        }

        private void workerLatest_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.toolStripProgressBar.Value = e.ProgressPercentage;
        }

        private void workerLatest_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.State = States.Idle;
        }

        private void workerPatch_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void workerPatch_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.toolStripProgressBar.Value = e.ProgressPercentage;
        }

        private void workerPatch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.State = States.Idle;
        }
    }
}
