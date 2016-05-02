using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Net;
using SWPatcher.Helpers;
using SWPatcher.General;
using SWPatcher.Helpers.GlobalVar;
using System.IO;
using System.Globalization;

namespace SWPatcher.Downloading
{
    public class Downloader : IDisposable
    {
        private static Downloader _instance;
        private readonly BackgroundWorker Worker;
        private readonly WebClient Client;
        private string Language;
        private bool disposed = false;
        public List<string> DownloadList { get; private set; }

        public static Downloader Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Downloader();
                return _instance;
            }
        }
        private Downloader()
        {
            this.Worker = new BackgroundWorker
            {
                WorkerSupportsCancellation = true
            };
            this.Worker.DoWork += new DoWorkEventHandler(Worker_DoWork);
            this.Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Worker_RunWorkerCompleted);
            this.Client = new WebClient();
            this.Client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(Client_DownloadProgressChanged);
            this.Client.DownloadFileCompleted += new AsyncCompletedEventHandler(Client_DownloadFileCompleted);
            this.DownloadList = new List<string>();
        }

        public event DownloaderProgressChangedEventHandler DownloaderProgressChanged;
        public event DownloaderCompletedEventHandler DownloaderCompleted;

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (IsNewerTranslationVersion(this.Language))
            {
                List<SWFile> SWFiles = new List<SWFile>();
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
                    IniReader dataIni = new IniReader(file.Path);
                    var array = dataIni.GetSectionNames();
                    foreach (var fileName in array)
                    {
                        string name = fileName as string;
                        dataIni.Section = name;
                        string path = dataIni.ReadString(Strings.IniName.Pack.KeyPath);
                        string pathA = dataIni.ReadString(Strings.IniName.Pack.KeyPathInArchive);
                        string pathD = dataIni.ReadString(Strings.IniName.Pack.KeyPathOfDownload);
                        string format = dataIni.ReadString(Strings.IniName.Pack.KeyFormat);
                        SWFiles.Add(new SWFile(name, path, pathA, pathD, format));
                        if (Worker.CancellationPending)
                        {
                            e.Cancel = true;
                            return;
                        }
                    }
                }
                e.Result = SWFiles;
            }
            else
                e.Result = null;
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
                MsgBox.Error(e.Error.Message + "\n\n" + e.Error.StackTrace);
            if (e.Cancelled)
                return;
            if (e.Result != null)
                this.DownloadList = (e.Result as List<SWFile>).Select(f => Uris.TranslationGitHubHome + (this.Language) + '/' + f.PathD).ToList<string>();
            //Client.DownloadFileAsync(); send downloadList via userToken
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            throw new NotImplementedException();//send SWList with event back to main
        }

        protected virtual void OnDownloaderComplete(object sender, AsyncCompletedEventArgs e)
        {
            if (this.DownloaderCompleted != null)
                this.DownloaderCompleted(sender, e);
        }

        private bool IsNewerTranslationVersion(string lang)
        {
            string directoryPath = Path.Combine(Paths.PatcherRoot, lang);
            if (!Directory.Exists(directoryPath))
                return true;
            string filePath = Path.Combine(directoryPath, Strings.IniName.TranslationVer);
            if (!File.Exists(filePath))
                return true;
            IniReader translationIni = new IniReader(Path.Combine(Paths.PatcherRoot, lang));
            if (DateCompare(GetTranslationDate(lang), translationIni.ReadString(lang, Strings.IniName.Pack.KeyDate)))
                return true;
            return false;
        }

        private static bool DateCompare(string date1, string date2)
        {
            DateTime d1 = DateTime.ParseExact(date1, "dd/MMM/yyyy h:mm tt", CultureInfo.InvariantCulture);
            DateTime d2 = DateTime.ParseExact(date2, "dd/MMM/yyyy h:mm tt", CultureInfo.InvariantCulture);
            return d1 > d2;
        }

        private static string GetTranslationDate(string lang)
        {
            using (var client = new WebClient())
            using (var file = new TempFile())
            {
                client.DownloadFile(Uris.PatcherGitHubHome + lang + Strings.IniName.TranslationVer, file.Path);
                IniReader translationIni = new IniReader(file.Path);
                return translationIni.ReadString(lang, Strings.IniName.Pack.KeyDate);
            }
        }

        public void Cancel()
        {
            Worker.CancelAsync();
            Client.CancelAsync();
        }

        public void Run(string language)
        {
            this.Language = language;
            Worker.RunWorkerAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;
            if (disposing)
            {
                if (this.Worker != null)
                    this.Worker.Dispose();
                if (this.Client != null)
                    this.Client.Dispose();
            }
            _instance = null;
            this.DownloadList = null;
            this.disposed = true;
        }

        ~Downloader()
        {
            Dispose(false);
        }
    }
}
