using SWPatcher.General;
using SWPatcher.Helpers;
using SWPatcher.Helpers.GlobalVariables;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading;

namespace SWPatcher.Downloading
{
    public delegate void DownloaderProgressChangedEventHandler(object sender, DownloaderProgressChangedEventArgs e);
    public delegate void DownloaderCompletedEventHandler(object sender, DownloaderCompletedEventArgs e);

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
            this.Worker.DoWork += Worker_DoWork;
            this.Worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            this.Client = new WebClient();
            this.Client.DownloadProgressChanged += Client_DownloadProgressChanged;
            this.Client.DownloadFileCompleted += Client_DownloadFileCompleted;
        }

        public event DownloaderProgressChangedEventHandler DownloaderProgressChanged;
        public event DownloaderCompletedEventHandler DownloaderCompleted;

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Logger.Debug(Methods.MethodFullName("Downloader", Thread.CurrentThread.ManagedThreadId.ToString(), this.Language.ToString()));

            if (Methods.HasNewTranslations(this.Language) || Methods.IsTranslationOutdated(this.Language))
            {
                Methods.SetSWFiles(this.SWFiles);
            }
            else
                throw new Exception(String.Format(StringLoader.GetText("exception_already_latest_translation"), Methods.DateToString(this.Language.LastUpdate)));
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled || e.Error != null)
                this.DownloaderCompleted?.Invoke(sender, new DownloaderCompletedEventArgs(e.Cancelled, e.Error));
            else
            {
                this.DownloadIndex = 0;
                this.DownloadNext();
            }
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.DownloaderProgressChanged?.Invoke(sender, new DownloaderProgressChangedEventArgs(this.DownloadIndex + 1, this.SWFiles.Count, Path.GetFileNameWithoutExtension(this.SWFiles[this.DownloadIndex].Name), e));
        }

        private void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled || e.Error != null)
                this.DownloaderCompleted?.Invoke(sender, new DownloaderCompletedEventArgs(e.Cancelled, e.Error));
            else
            {
                if (SWFiles.Count > ++this.DownloadIndex)
                    DownloadNext();
                else
                    this.DownloaderCompleted?.Invoke(sender, new DownloaderCompletedEventArgs(this.Language, e.Cancelled, e.Error));
            }
        }

        private void DownloadNext()
        {
            Uri uri = new Uri(Urls.TranslationGitHubHome + this.Language.Lang + '/' + this.SWFiles[this.DownloadIndex].PathD);
            string path = "";

            if (String.IsNullOrEmpty(this.SWFiles[this.DownloadIndex].PathA))
                path = Path.Combine(this.Language.Lang, this.SWFiles[this.DownloadIndex].Path);
            else
                path = Path.Combine(Path.GetDirectoryName(Path.Combine(this.Language.Lang, this.SWFiles[this.DownloadIndex].Path)), Path.GetFileNameWithoutExtension(this.SWFiles[this.DownloadIndex].Path));

            Directory.CreateDirectory(path);

            string fileDestination = Path.Combine(path, Path.GetFileName(this.SWFiles[this.DownloadIndex].PathD));
            this.Client.DownloadFileAsync(uri, fileDestination);
            
            Logger.Debug(Methods.MethodFullName(System.Reflection.MethodBase.GetCurrentMethod(), uri.AbsoluteUri, path));
        }

        public void Cancel()
        {
            this.Worker.CancelAsync();
            this.Client.CancelAsync();
        }

        public void Run(Language language)
        {
            if (this.Worker.IsBusy || this.Client.IsBusy)
                return;
            
            this.Language = language;
            this.Worker.RunWorkerAsync();
        }
    }
}
