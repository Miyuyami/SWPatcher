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
        private readonly List<SWFile> SWFiles;
        private Language Language;
        private int DownloadIndex;

        public static Downloader GetInstance(List<SWFile> swFiles)
        {
            if (_instance == null)
                _instance = new Downloader(swFiles);
            return _instance;
        }

        private Downloader(List<SWFile> swFiles)
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
            bool flag = false;
            e.Result = GetLastTranslationDate(this.Language.Lang);
            DirectoryInfo directory = new DirectoryInfo(Path.Combine(Paths.PatcherRoot, this.Language.Lang));
            if (directory.Exists)
            {
                string filePath = Path.Combine(directory.FullName, Strings.IniName.Translation);
                if (File.Exists(filePath))
                {
                    IniReader translationIni = new IniReader(Path.Combine(Paths.PatcherRoot, this.Language.Lang));
                    if (DateCompare(e.Result as string, translationIni.ReadString(this.Language.Lang, Strings.IniName.Pack.KeyDate)))
                        flag = true;
                }
                else
                    flag = true;
            }
            else
            {
                directory.Create();
                flag = true;
            }
            if (flag)
            {
                using (var client = new WebClient())
                using (var file = new TempFile())
                {
                    client.DownloadFile(Uris.PatcherGitHubHome + Strings.IniName.TranslationPackData, file.Path);
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
                        this.SWFiles.Add(new SWFile(name, path, pathA, pathD, format));
                        if (Worker.CancellationPending)
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
            if (e.Cancelled)
                return;
            if (e.Error != null)
            {
                string errorMessage = "Message:\n" + e.Error.Message;
                if (!string.IsNullOrEmpty(e.Error.StackTrace))
                    errorMessage += "\nStackTrace:\n" + e.Error.StackTrace;
                if (e.Error.InnerException != null)
                {
                    errorMessage += "\nInnerMessage:\n" + e.Error.InnerException.Message;
                    if (!string.IsNullOrEmpty(e.Error.InnerException.StackTrace))
                        errorMessage += "\nInnerStackTrace:\n" + e.Error.InnerException.StackTrace;
                }
                OnDownloaderComplete(sender, new DownloaderDownloadCompletedEventArgs(null));
                MsgBox.Error(errorMessage);
            }
            else
            {
                this.DownloadIndex = 0;
                if (SWFiles.Count > DownloadIndex)
                    DownloadNext();
                else
                    OnDownloaderComplete(sender, new DownloaderDownloadCompletedEventArgs(e.Result as string));
            }// create .ini with date somewhere here...........
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            OnDownloaderProgressChanged(sender, new DownloaderProgressChangedEventArgs(DownloadIndex + 1, SWFiles.Count, Path.GetFileNameWithoutExtension(SWFiles[DownloadIndex].Name), e));
        }

        private void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                OnDownloaderComplete(sender, new DownloaderDownloadCompletedEventArgs(null));
                return;
            }
            if (e.Error != null)
            {
                string errorMessage = "Message:\n" + e.Error.Message;
                if (!string.IsNullOrEmpty(e.Error.StackTrace))
                    errorMessage += "\nStackTrace:\n" + e.Error.StackTrace;
                if (e.Error.InnerException != null)
                {
                    errorMessage += "\nInnerMessage:\n" + e.Error.InnerException.Message;
                    if (!string.IsNullOrEmpty(e.Error.InnerException.StackTrace))
                        errorMessage += "\nInnerStackTrace:\n" + e.Error.InnerException.StackTrace;
                }
                OnDownloaderComplete(sender, new DownloaderDownloadCompletedEventArgs(null));
                MsgBox.Error(errorMessage);
            }
            else
            {
                this.DownloadIndex++;
                if (SWFiles.Count > DownloadIndex)
                    DownloadNext();
                else
                    OnDownloaderComplete(sender, new DownloaderDownloadCompletedEventArgs(null));
            }
        }

        protected virtual void OnDownloaderProgressChanged(object sender, DownloaderProgressChangedEventArgs e)
        {
            if (this.DownloaderProgressChanged != null)
                this.DownloaderProgressChanged(sender, e);
        }

        protected virtual void OnDownloaderComplete(object sender, DownloaderDownloadCompletedEventArgs e)
        {
            if (this.DownloaderCompleted != null)
                this.DownloaderCompleted(sender, e);
        }

        private void DownloadNext()
        {
            Uri uri = new Uri(Uris.TranslationGitHubHome + this.Language.Lang + '/' + SWFiles[DownloadIndex].PathD);
            DirectoryInfo folderDestination = new DirectoryInfo(Path.Combine(Path.GetDirectoryName(Path.Combine(Paths.PatcherRoot, this.Language.Lang, SWFiles[DownloadIndex].Path)), Path.GetFileNameWithoutExtension(SWFiles[DownloadIndex].Path)));
            if (!folderDestination.Exists)
                folderDestination.Create();
            string fileDestination = Path.Combine(folderDestination.FullName, Path.GetFileName(SWFiles[DownloadIndex].PathD));
            Client.DownloadFileAsync(uri, fileDestination);
        }

        private static bool DateCompare(string date1, string date2)
        {
            DateTime d1 = DateTime.ParseExact(date1, "dd/MMM/yyyy h:mm tt", CultureInfo.InvariantCulture);
            DateTime d2 = DateTime.ParseExact(date2, "dd/MMM/yyyy h:mm tt", CultureInfo.InvariantCulture);
            return d1 > d2;
        }

        private static string GetLastTranslationDate(string lang)
        {
            using (var client = new WebClient())
            using (var file = new TempFile())
            {
                client.DownloadFile(Uris.PatcherGitHubHome + Strings.IniName.LanguagePack, file.Path);
                IniReader translationIni = new IniReader(file.Path);
                return translationIni.ReadString(lang, Strings.IniName.Pack.KeyDate);
            }
        }

        public void Cancel()
        {
            Worker.CancelAsync();
            Client.CancelAsync();
        }

        public void Run(Language language)
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
            if (disposing)
            {
                _instance = null;
                if (this.Worker != null)
                    this.Worker.Dispose();
                if (this.Client != null)
                    this.Client.Dispose();
            }
        }

        ~Downloader()
        {
            Dispose(false);
        }
    }
}
