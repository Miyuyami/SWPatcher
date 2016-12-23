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
        private BackgroundWorker Worker;
        private WebClient Client;
        private Language Language;

        public Downloader()
        {
            this.Worker = new BackgroundWorker
            {
                WorkerSupportsCancellation = true
            };
            this.Worker.DoWork += this.Worker_DoWork;
            this.Worker.RunWorkerCompleted += this.Worker_RunWorkerCompleted;
            this.Client = new WebClient();
            this.Client.DownloadProgressChanged += this.Client_DownloadProgressChanged;
            this.Client.DownloadDataCompleted += this.Client_DownloadDataCompleted;
        }

        public event DownloaderProgressChangedEventHandler DownloaderProgressChanged;
        public event DownloaderCompletedEventHandler DownloaderCompleted;

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Logger.Debug(Methods.MethodFullName("Downloader", Thread.CurrentThread.ManagedThreadId.ToString(), this.Language.ToString()));

            if (Methods.HasNewTranslations(this.Language) || Methods.IsTranslationOutdated(this.Language))
            {
                SWFileManager.LoadFileConfiguration();
            }
            else
            {
                throw new Exception(String.Format(StringLoader.GetText("exception_already_latest_translation"), Methods.DateToString(this.Language.LastUpdate)));
            }
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled || e.Error != null)
            {
                this.DownloaderCompleted?.Invoke(sender, new DownloaderCompletedEventArgs(e.Cancelled, e.Error));
            }
            else
            {
                this.DownloadNext(0);
            }
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.DownloaderProgressChanged?.Invoke(sender, new DownloaderProgressChangedEventArgs((int)e.UserState + 1, SWFileManager.Count, Path.GetFileNameWithoutExtension(SWFileManager.GetElementAt((int)e.UserState).Name), e));
        }

        private void Client_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            if (e.Cancelled || e.Error != null)
            {
                this.DownloaderCompleted?.Invoke(sender, new DownloaderCompletedEventArgs(e.Cancelled, e.Error));
            }
            else
            {
                var index = (int)e.UserState;
                SWFile swFile = SWFileManager.GetElementAt(index);
                if (swFile is ArchivedSWFile archivedSWFile)
                {
                    archivedSWFile.Data = e.Result;
                }
                else
                {
                    string swFilePath = Path.Combine(this.Language.Name, swFile.Path, Path.GetFileName(swFile.PathD));
                    string swFileDirectory = Path.GetDirectoryName(swFilePath);

                    Directory.CreateDirectory(swFileDirectory);
                    File.WriteAllBytes(swFilePath, e.Result);
                }

                if (SWFileManager.Count > ++index)
                {
                    this.DownloadNext(index);
                }
                else
                {
                    this.DownloaderCompleted?.Invoke(sender, new DownloaderCompletedEventArgs(this.Language, e.Cancelled, e.Error));
                }
            }
        }

        private void DownloadNext(int index)
        {
            Uri uri = new Uri(Urls.TranslationGitHubHome + this.Language.Name + '/' + SWFileManager.GetElementAt(index).PathD);

            this.Client.DownloadDataAsync(uri, index);

            Logger.Debug(Methods.MethodFullName(System.Reflection.MethodBase.GetCurrentMethod(), uri.AbsoluteUri));
        }

        public void Cancel()
        {
            this.Worker.CancelAsync();
            this.Client.CancelAsync();
        }

        public void Run(Language language)
        {
            if (this.Worker.IsBusy || this.Client.IsBusy)
            {
                return;
            }

            this.Language = language;
            this.Worker.RunWorkerAsync();
        }
    }
}
