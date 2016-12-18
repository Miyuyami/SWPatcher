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

using MadMilkman.Ini;
using SWPatcher.Helpers;
using SWPatcher.Helpers.GlobalVariables;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;

namespace SWPatcher.RTPatch
{
    public delegate void RTPatcherDownloadProgressChangedEventHandler(object sender, RTPatcherDownloadProgressChangedEventArgs e);
    public delegate void RTPatcherProgressChangedEventHandler(object sender, RTPatcherProgressChangedEventArgs e);
    public delegate string RTPatchCallback(uint id, IntPtr ptr);

    public class RTPatcher : IDisposable
    {
        private readonly BackgroundWorker Worker;
        private readonly WebClient Client;
        private string CurrentLogFilePath;
        private string FileName;
        private string LastMessage;
        private string Url;
        private Version ClientVersion;
        private Version ServerVersion;
        private int FileCount;
        private int FileNumber;

        private bool disposedValue = false;

        public event RTPatcherDownloadProgressChangedEventHandler RTPatcherDownloadProgressChanged;
        public event RTPatcherProgressChangedEventHandler RTPatcherProgressChanged;
        public event AsyncCompletedEventHandler RTPatcherCompleted;

        public RTPatcher()
        {
            this.Worker = new BackgroundWorker()
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            this.Worker.DoWork += this.Worker_DoWork;
            this.Worker.ProgressChanged += this.Worker_ProgressChanged;
            this.Worker.RunWorkerCompleted += this.Worker_RunWorkerCompleted;
            this.Client = new WebClient();
            this.Client.DownloadProgressChanged += this.Client_DownloadProgressChanged;
            this.Client.DownloadFileCompleted += this.Client_DownloadFileCompleted;
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Logger.Debug(Methods.MethodFullName("RTPatch", Thread.CurrentThread.ManagedThreadId.ToString(), this.ClientVersion.ToString()));
            Methods.CheckRunningPrograms();

            if (this.Worker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            string gamePath = UserSettings.GamePath;
            string diffFilePath = Path.Combine(gamePath, Methods.VersionToRTP(this.ClientVersion));

            this.CurrentLogFilePath = Path.Combine(Strings.FolderName.RTPatchLogs, Path.GetFileName(diffFilePath) + ".log");
            string logDirectory = Path.GetDirectoryName(this.CurrentLogFilePath);
            Directory.CreateDirectory(logDirectory);
            File.WriteAllText(this.CurrentLogFilePath, string.Empty);

            Logger.Info($"RTPatch diffFile=[{diffFilePath}] path=[{gamePath}]");
            string command = $"/u /nos \"{gamePath}\" \"{diffFilePath}\"";
            ulong result = Environment.Is64BitProcess ? NativeMethods.RTPatchApply64(command, new RTPatchCallback(this.RTPatchMessage), true) : NativeMethods.RTPatchApply32(command, new RTPatchCallback(this.RTPatchMessage), true);
            File.Delete(diffFilePath);
            File.AppendAllText(this.CurrentLogFilePath, $"Result=[{result}]");

            if (result != 0)
                throw new ResultException(this.LastMessage, result, this.CurrentLogFilePath, this.FileName, this.ClientVersion);
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.RTPatcherProgressChanged?.Invoke(this, new RTPatcherProgressChangedEventArgs(this.FileNumber, this.FileCount, this.FileName, e.ProgressPercentage));
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled || e.Error != null)
                this.RTPatcherCompleted?.Invoke(sender, new AsyncCompletedEventArgs(e.Error, e.Cancelled, null));
            else
            {
                IniFile ini = new IniFile(new IniOptions
                {
                    KeyDuplicate = IniDuplication.Ignored,
                    SectionDuplicate = IniDuplication.Ignored
                });
                string iniPath = Path.Combine(UserSettings.GamePath, Strings.IniName.ClientVer);
                ini.Load(iniPath);
                string serverVer = ini.Sections[Strings.IniName.Ver.Section].Keys[Strings.IniName.Ver.Key].Value;
                string clientVer = this.ClientVersion.ToString();
                if (serverVer != clientVer)
                {
                    ini.Sections[Strings.IniName.Ver.Section].Keys[Strings.IniName.Ver.Key].Value = clientVer;
                    ini.Save(iniPath);
                }

                DownloadNext();
            }
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.RTPatcherDownloadProgressChanged?.Invoke(sender, new RTPatcherDownloadProgressChangedEventArgs(Methods.VersionToRTP(this.ClientVersion), e));
        }

        private void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled || e.Error != null)
                this.RTPatcherCompleted?.Invoke(sender, new AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            else
                this.Worker.RunWorkerAsync();
        }

        private void LoadVersions()
        {
            IniFile serverIni = Methods.GetServerIni();
            this.ServerVersion = new Version(serverIni.Sections[Strings.IniName.Ver.Section].Keys[Strings.IniName.Ver.Key].Value);
            string address = serverIni.Sections[Strings.IniName.ServerRepository.Section].Keys[Strings.IniName.ServerRepository.Key].Value;
            this.Url = address + Strings.IniName.ServerRepository.UpdateRepository;

            IniFile clientIni = new IniFile();
            clientIni.Load(Path.Combine(UserSettings.GamePath, Strings.IniName.ClientVer));
            this.ClientVersion = new Version(clientIni.Sections[Strings.IniName.Ver.Section].Keys[Strings.IniName.Ver.Key].Value);
        }

        private void DownloadNext()
        {
            if (this.ClientVersion < this.ServerVersion)
            {
                this.ClientVersion = GetNextVersion(this.ClientVersion, this.ServerVersion);
                string RTPFileName = Methods.VersionToRTP(this.ClientVersion);
                Uri uri = new Uri(this.Url + '/' + RTPFileName);
                string destination = Path.Combine(UserSettings.GamePath, RTPFileName);
                Logger.Info($"Downloading url=[{uri.AbsoluteUri}] path=[{destination}]");
                this.Client.DownloadFileAsync(uri, destination);
            }
            else
            {
                RTPatcherCompleted?.Invoke(this, new AsyncCompletedEventArgs(null, false, null));
            }
        }

        private static Version GetNextVersion(Version clientVer, Version serverVer)
        {
            var result = new Version(clientVer.Major + 1, 0, 0, 0);

            if (result > serverVer)
            {
                result = new Version(clientVer.Major, clientVer.Minor + 1, 0, 0);

                if (result > serverVer)
                {
                    result = new Version(clientVer.Major, clientVer.Minor, clientVer.Build + 1, 0);

                    if (result > serverVer)
                    {
                        result = new Version(clientVer.Major, clientVer.Minor, clientVer.Build, clientVer.Revision + 1);
                    }
                }
            }

            return result;
        }

        private string RTPatchMessage(uint id, IntPtr ptr)
        {
            switch (id)
            {
                case 1u:
                case 2u:
                case 3u:
                case 4u:
                case 8u:
                case 9u:
                case 10u:
                case 11u:
                case 12u: // outputs
                    {
                        string text = Marshal.PtrToStringAnsi(ptr);
                        this.LastMessage = text;
                        File.AppendAllText(this.CurrentLogFilePath, text);

                        break;
                    }
                case 14u:
                case 17u:
                case 18u: // idk
                    return null;
                case 5u: // completion percentage
                    {
                        int readInt = Marshal.ReadInt32(ptr);
                        int percentage = readInt > short.MaxValue ? int.MaxValue : readInt * 0x10000;
                        this.Worker.ReportProgress(percentage);
                        percentage = readInt * 100 / 0x8000;
                        File.AppendAllText(this.CurrentLogFilePath, $"[{percentage}%]");

                        break;
                    }
                case 6u: // number of files in patch
                    {
                        int fileCount = Marshal.ReadInt32(ptr);
                        this.FileNumber = 0;
                        this.FileCount = fileCount;
                        File.AppendAllText(this.CurrentLogFilePath, $"File Count=[{fileCount}]\n");

                        break;
                    }
                case 7u: // current file
                    {
                        string fileName = Marshal.PtrToStringAnsi(ptr);
                        this.FileNumber++;
                        this.FileName = fileName;
                        File.AppendAllText(this.CurrentLogFilePath, $"Patching=[{fileName}]\n");

                        break;
                    }
                case 32u:
                case 33u: // idk
                    {
                        int[] numbers = new int[2];
                        Marshal.Copy(ptr, numbers, 0, 2);
                        File.AppendAllText(this.CurrentLogFilePath, $"number1=[{numbers[0]}] number2=[{numbers[1]}]\n");

                        break;
                    }
                default: break; // ignore rest
            }
            return "";
        }

        public void Cancel()
        {
            this.Client.CancelAsync();
            this.Worker.CancelAsync();
        }

        public void Run()
        {
            if (this.Client.IsBusy || this.Worker.IsBusy)
                return;

            LoadVersions();
            DownloadNext();
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

                this.CurrentLogFilePath = null;
                this.FileName = null;
                this.LastMessage = null;
                this.Url = null;
                this.ClientVersion = null;
                this.ServerVersion = null;

                this.disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
