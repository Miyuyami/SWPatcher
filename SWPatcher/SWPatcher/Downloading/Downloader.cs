using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using MadMilkman.Ini;
using SWPatcher.General;
using SWPatcher.Helpers;
using SWPatcher.Helpers.GlobalVar;

namespace SWPatcher.Downloading
{
    public class Downloader
    {
        private readonly BackgroundWorker Worker;
        private readonly WebClient Client;
        private readonly List<SWFile> SWFiles;
        private Language Language;
        private int DownloadIndex;

        public Downloader(List<SWFile> swFiles)
        {
            this.SWFiles = swFiles;
            this.Worker = new BackgroundWorker
            {
                WorkerSupportsCancellation = true
            };
            this.Worker.DoWork += new DoWorkEventHandler(Worker_DoWork);
            this.Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Worker_RunWorkerCompleted);
            this.Client = new WebClient();
            this.Client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(Client_DownloadProgressChanged);
            this.Client.DownloadFileCompleted += new AsyncCompletedEventHandler(Client_DownloadFileCompleted);
        }

        public event DownloaderProgressChangedEventHandler DownloaderProgressChanged;
        public event DownloaderCompletedEventHandler DownloaderCompleted;

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (!(bool)e.Argument && !Methods.HasNewTranslations(this.Language))
                throw new Exception(String.Format("You already have the latest({0} JST) translation files for this language!", Methods.DateToString(this.Language.LastUpdate)));

            if (Methods.IsNewerGameClientVersion())
                throw new Exception("Game client is not updated to the latest version.");

            if (this.SWFiles.Count == 0)
            {
                this.SWFiles.Clear();
                using (var client = new WebClient())
                using (var file = new TempFile())
                {
                    client.DownloadFile(Urls.PatcherGitHubHome + Strings.IniName.TranslationPackData, file.Path);
                    IniFile ini = new IniFile();
                    ini.Load(file.Path);

                    foreach (var section in ini.Sections)
                    {
                        string name = section.Name;
                        string path = section.Keys[Strings.IniName.Pack.KeyPath].Value;
                        string pathA = section.Keys[Strings.IniName.Pack.KeyPathInArchive].Value;
                        string pathD = section.Keys[Strings.IniName.Pack.KeyPathOfDownload].Value;
                        string format = section.Keys[Strings.IniName.Pack.KeyFormat].Value;
                        this.SWFiles.Add(new SWFile(name, path, pathA, pathD, format));

                        if (this.Worker.CancellationPending)
                        {
                            e.Cancel = true;
                            return;
                        }
                    }
                }
            }
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled || e.Error != null)
                this.OnDownloaderComplete(sender, new DownloaderCompletedEventArgs(e.Cancelled, e.Error));
            else
            {
                this.DownloadIndex = 0;
                this.DownloadNext();
            }
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.OnDownloaderProgressChanged(sender, new DownloaderProgressChangedEventArgs(this.DownloadIndex + 1, this.SWFiles.Count, Path.GetFileNameWithoutExtension(this.SWFiles[this.DownloadIndex].Name), e));
        }

        private void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled || e.Error != null)
                this.OnDownloaderComplete(sender, new DownloaderCompletedEventArgs(e.Cancelled, e.Error));
            else
            {
                if (SWFiles.Count > ++this.DownloadIndex)
                    DownloadNext();
                else
                    this.OnDownloaderComplete(sender, new DownloaderCompletedEventArgs(this.Language, e.Cancelled, e.Error));
            }
        }

        private void OnDownloaderProgressChanged(object sender, DownloaderProgressChangedEventArgs e)
        {
            if (this.DownloaderProgressChanged != null)
                this.DownloaderProgressChanged(sender, e);
        }

        private void OnDownloaderComplete(object sender, DownloaderCompletedEventArgs e)
        {
            if (this.DownloaderCompleted != null)
                this.DownloaderCompleted(sender, e);
        }

        private void DownloadNext()
        {
            Uri uri = new Uri(Urls.TranslationGitHubHome + this.Language.Lang + '/' + this.SWFiles[this.DownloadIndex].PathD);
            string path = "";

            if (String.IsNullOrEmpty(this.SWFiles[this.DownloadIndex].PathA))
                path = Path.Combine(Paths.PatcherRoot, this.Language.Lang, this.SWFiles[this.DownloadIndex].Path);
            else
                path = Path.Combine(Path.GetDirectoryName(Path.Combine(Paths.PatcherRoot, this.Language.Lang, this.SWFiles[this.DownloadIndex].Path)), Path.GetFileNameWithoutExtension(this.SWFiles[this.DownloadIndex].Path));

            string directoryDestionation = path;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string fileDestination = Path.Combine(directoryDestionation, Path.GetFileName(this.SWFiles[this.DownloadIndex].PathD));
            this.Client.DownloadFileAsync(uri, fileDestination);
        }

        public void Cancel()
        {
            this.Worker.CancelAsync();
            this.Client.CancelAsync();
        }

        public void Run(Language language, bool isForced)
        {
            if (this.Worker.IsBusy || this.Client.IsBusy)
                return;

            this.Language = language;
            this.Worker.RunWorkerAsync(isForced);
        }
    }
}
