/*
 * This file is part of Soulworker Patcher.
 * Copyright (C) 2016 Miyu
 * 
 * Soulworker Patcher is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * Soulworker Patcher is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Soulworker Patcher. If not, see <http://www.gnu.org/licenses/>.
 */

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

    /// <summary>
    /// Handles the downloading of translation files.
    /// </summary>
    public class Downloader : IDisposable
    {
        private readonly BackgroundWorker Worker;
        private readonly WebClient Client;
        private List<SWFile> SWFiles;
        private Language Language;
        private int DownloadIndex;

        private bool disposedValue = false;

        /// <summary>
        /// Creates a new instance of <c>Downloader</c>.
        /// </summary>
        /// <param name="swFiles">is a list of the file that need to be downloaded</param>
        public Downloader(List<SWFile> swFiles)
        {
            this.SWFiles = swFiles;
            this.Worker = new BackgroundWorker
            {
                WorkerSupportsCancellation = true
            };
            this.Worker.DoWork += this.Worker_DoWork;
            this.Worker.RunWorkerCompleted += this.Worker_RunWorkerCompleted;
            this.Client = new WebClient();
            this.Client.DownloadProgressChanged += this.Client_DownloadProgressChanged;
            this.Client.DownloadFileCompleted += this.Client_DownloadFileCompleted;
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
                if (this.SWFiles.Count > ++this.DownloadIndex)
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

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    this.Worker.Dispose();
                    this.Client.Dispose();
                }
                
                this.SWFiles = null;
                this.Language = null;

                this.disposedValue = true;
            }
        }
        
        public void Dispose()
        {
            Dispose(true);
        }
    }
}
