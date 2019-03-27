/*
 * This file is part of Soulworker Patcher.
 * Copyright (C) 2016-2017 Miyu, Dramiel Leayal
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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SWPatcher.General;
using SWPatcher.Helpers;
using SWPatcher.Helpers.GlobalVariables;
using SWPatcher.Helpers.Steam;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;

namespace SWPatcher.RTPatch
{
    delegate void RTPatcherDownloadProgressChangedEventHandler(object sender, RTPatcherDownloadProgressChangedEventArgs e);
    delegate void RTPatcherProgressChangedEventHandler(object sender, RTPatcherProgressChangedEventArgs e);
    delegate void RTPatcherCompletedEventHandler(object sender, RTPatcherCompletedEventArgs e);
    delegate string RTPatchCallback(uint id, IntPtr ptr);

    internal class RTPatcher
    {
        private const int DiffBytes = 10; // how many bytes to redownload on resume, just to be safe, why not?
        private readonly BackgroundWorker Worker;
        private string CurrentLogFilePath;
        private string FileName;
        private string LastMessage;
        private string Url;
        private Version ClientNextVersion;
        private Version ClientVersion;
        private Version ServerVersion;
        private int FileCount;
        private int FileNumber;
        private Language Language;

        internal event RTPatcherDownloadProgressChangedEventHandler RTPatcherDownloadProgressChanged;
        internal event RTPatcherProgressChangedEventHandler RTPatcherProgressChanged;
        internal event RTPatcherCompletedEventHandler RTPatcherCompleted;

        internal RTPatcher()
        {
            this.Worker = new BackgroundWorker()
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            this.Worker.DoWork += this.Worker_DoWork;
            this.Worker.ProgressChanged += this.Worker_ProgressChanged;
            this.Worker.RunWorkerCompleted += this.Worker_RunWorkerCompleted;
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            string regionId = this.Language.ApplyingRegionId;
            Methods.CheckRunningGame(regionId);
            switch (regionId)
            {
                case "jp":
                    LoadVersions();
                    Logger.Debug(Methods.MethodFullName("RTPatch", Thread.CurrentThread.ManagedThreadId.ToString(), this.ClientNextVersion.ToString()));

                    break;
                case "kr":
                    CheckKRVersion();
                    //Logger.Debug(Methods.MethodFullName("RTPatch", Thread.CurrentThread.ManagedThreadId.ToString()));
                    return;

                //break;
                case "nkr":
                    CheckNaverKRVersion();

                    return;
                case "gf":
                    CheckGFVersion();

                    return;
                default:
                    throw new Exception(StringLoader.GetText("exception_region_unknown", regionId));
            }

            while (this.ClientNextVersion < this.ServerVersion)
            {
                if (this.Worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                this.FileCount = 0;
                this.FileNumber = 0;
                this.FileName = "";
                this.LastMessage = "";
                this.ClientVersion = this.ClientNextVersion;
                this.ClientNextVersion = this.GetNextVersion(this.ClientNextVersion);
                string RTPFileName = VersionToRTP(this.ClientNextVersion);
                string url = this.Url + RTPFileName;
                string destination = Path.Combine(UserSettings.GamePath, RTPFileName);
                string gamePath = UserSettings.GamePath;
                string diffFilePath = Path.Combine(gamePath, VersionToRTP(this.ClientNextVersion));
                this.CurrentLogFilePath = Path.Combine(Strings.FolderName.RTPatchLogs, Path.GetFileName(diffFilePath) + ".log");
                string logDirectory = Path.GetDirectoryName(this.CurrentLogFilePath);
                Directory.CreateDirectory(logDirectory);

                Logger.Info($"Downloading url=[{url}] path=[{destination}]");
                #region Download Resumable File
                using (FileStream fs = File.OpenWrite(destination))
                {
                    long fileLength = fs.Length < DiffBytes ? 0 : fs.Length - DiffBytes;
                    fs.Position = fileLength;
                    HttpWebRequest request = WebRequest.CreateHttp(url);
                    request.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0;)";
                    request.Credentials = new NetworkCredential();
                    request.AddRange(fileLength);

                    using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                    {
                        long bytesToReceive = response.ContentLength;
                        using (Stream stream = response.GetResponseStream())
                        {
                            byte[] buffer = new byte[1000 * 1000]; // 1MB buffer
                            int bytesRead;
                            long totalFileBytes = fileLength + bytesToReceive;
                            long fileBytes = fileLength;
                            long receivedBytes = 0;

                            this.Worker.ReportProgress(0, 0L);
                            Stopwatch sw = new Stopwatch();
                            sw.Start();
                            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                            {
                                if (this.Worker.CancellationPending)
                                {
                                    e.Cancel = true;
                                    request.Abort();
                                    return;
                                }

                                fs.Write(buffer, 0, bytesRead);

                                fileBytes += bytesRead;
                                receivedBytes += bytesRead;
                                int progress = fileBytes >= totalFileBytes ? int.MaxValue : Convert.ToInt32(((double)fileBytes / totalFileBytes) * int.MaxValue);
                                double elapsedTicks = sw.ElapsedTicks;
                                long frequency = Stopwatch.Frequency;
                                long elapsedSeconds = Convert.ToInt64(elapsedTicks / frequency);
                                long bytesPerSecond;
                                if (elapsedSeconds > 0)
                                {
                                    bytesPerSecond = receivedBytes / elapsedSeconds;
                                }
                                else
                                {
                                    bytesPerSecond = receivedBytes;
                                }

                                this.Worker.ReportProgress(progress, bytesPerSecond);
                            }
                            sw.Stop();
                        }
                    }
                }
                #endregion

                Methods.CheckRunningProcesses(regionId);
                Logger.Info($"RTPatchApply diffFile=[{diffFilePath}] path=[{gamePath}]");
                this.Worker.ReportProgress(-1, 0L);
                #region Apply RTPatch
                File.Delete(this.CurrentLogFilePath);
                string command = $"/u /nos \"{gamePath}\" \"{diffFilePath}\"";
                ulong result = Environment.Is64BitProcess ? NativeMethods.RTPatchApply64(command, new RTPatchCallback(this.RTPatchMessage), true) : NativeMethods.RTPatchApply32(command, new RTPatchCallback(this.RTPatchMessage), true);
                Logger.Debug($"RTPatchApply finished with result=[{result}]");
                File.AppendAllText(this.CurrentLogFilePath, $"Result=[{result}]");

                if (result != 0)
                {
                    if (result > 10000)
                    {
                        Logger.Debug($"RTPatchApply cancelled Result=[{result}] IsNormal=[{result == 32769}]");
                        e.Cancel = true;

                        return; // RTPatch cancelled
                    }

                    throw new ResultException(this.LastMessage, result, this.CurrentLogFilePath, this.FileName, this.ClientVersion);
                }
                File.Delete(diffFilePath);

                IniFile ini = new IniFile(new IniOptions
                {
                    KeyDuplicate = IniDuplication.Ignored,
                    SectionDuplicate = IniDuplication.Ignored
                });
                string iniPath = Path.Combine(UserSettings.GamePath, Strings.IniName.ClientVer);
                ini.Load(iniPath);
                string serverVer = ini.Sections[Strings.IniName.Ver.Section].Keys[Strings.IniName.Ver.Key].Value;
                string clientVer = this.ClientNextVersion.ToString();
                if (serverVer != clientVer)
                {
                    ini.Sections[Strings.IniName.Ver.Section].Keys[Strings.IniName.Ver.Key].Value = clientVer;
                    ini.Save(iniPath);
                }
                #endregion
            }
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (this.FileCount == 0)
            {
                this.RTPatcherDownloadProgressChanged?.Invoke(sender, new RTPatcherDownloadProgressChangedEventArgs(VersionToRTP(this.ClientNextVersion), e.ProgressPercentage, (long)e.UserState));
            }
            else
            {
                this.RTPatcherProgressChanged?.Invoke(this, new RTPatcherProgressChangedEventArgs(this.FileNumber, this.FileCount, this.FileName, e.ProgressPercentage));
            }
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.RTPatcherCompleted?.Invoke(this, new RTPatcherCompletedEventArgs(e.Cancelled, e.Error, this.Language));
        }

        private void LoadVersions()
        {
            IniFile serverIni = Methods.GetJPServerIni();
            this.ServerVersion = new Version(serverIni.Sections[Strings.IniName.Ver.Section].Keys[Strings.IniName.Ver.Key].Value);
            string address = serverIni.Sections[Strings.IniName.ServerRepository.Section].Keys[Strings.IniName.ServerRepository.Key].Value;
            this.Url = address + Strings.IniName.ServerRepository.UpdateRepository;

            IniFile clientIni = new IniFile();
            clientIni.Load(Path.Combine(UserSettings.GamePath, Strings.IniName.ClientVer));
            this.ClientNextVersion = new Version(clientIni.Sections[Strings.IniName.Ver.Section].Keys[Strings.IniName.Ver.Key].Value);
        }

        private void CheckKRVersion()
        {
            int serverVersion = Methods.GetKRServerVersion();
            //int clientVersion = Convert.ToInt32(Methods.GetRegistryValue(Strings.Registry.KR.RegistryKey, Strings.Registry.KR.Key32Path, Strings.Registry.KR.Version, 0));
            string stovePath = Methods.GetRegistryValue(Strings.Registry.KR.RegistryKey, Strings.Registry.KR.StoveKeyPath, Strings.Registry.KR.StoveWorkingDir, String.Empty);
            string smilegatePath = Path.GetDirectoryName(stovePath);
            string soulworkerManifestPath = Path.Combine(smilegatePath, @"WebLauncher\gamemanifest\gamemanifest_11_live.upf");
            int clientVersion1 = -1;
            string soulworkerPath = Methods.GetRegistryValue(Strings.Registry.KR.RegistryKey, Strings.Registry.KR.Key32Path, Strings.Registry.KR.FolderName, String.Empty);
            string soulworkerManifest2Path = Path.Combine(soulworkerPath, @"combinedata_manifest\gamemanifest_11_live.upf");
            int clientVersion2 = -1;

            if (File.Exists(soulworkerManifestPath))
            {
                JObject manifestJson = (JObject)JsonConvert.DeserializeObject(File.ReadAllText(soulworkerManifestPath));
                clientVersion1 = Convert.ToInt32(manifestJson["gameinfo"]["version"].Value<string>());
            }

            if (File.Exists(soulworkerManifest2Path))
            {
                JObject manifestJson = (JObject)JsonConvert.DeserializeObject(File.ReadAllText(soulworkerManifest2Path));
                clientVersion2 = Convert.ToInt32(manifestJson["gameinfo"]["version"].Value<string>());
            }

            if (clientVersion1 == -1 && clientVersion2 == -1)
            {
                throw new Exception($"Could not find manifest file. Please contact Miyu#9404 on discord.\nManifest1 Path: \"{soulworkerManifestPath}\"\nManifest2 Path: \"{soulworkerManifest2Path}\"");
            }

            int clientVersion = clientVersion1 > clientVersion2 ? clientVersion1 : clientVersion2;

            if (clientVersion != serverVersion)
            {
                throw new Exception(StringLoader.GetText("exception_game_not_latest"));
            }
        }

        private void CheckNaverKRVersion()
        {
            int serverVersion = Methods.GetKRServerVersion();
            int clientVersion = Convert.ToInt32(Methods.GetRegistryValue(Strings.Registry.NaverKR.RegistryKey, Strings.Registry.NaverKR.Key32Path, Strings.Registry.NaverKR.Version, 0));

            if (clientVersion != serverVersion)
            {
                throw new Exception(StringLoader.GetText("exception_game_not_latest"));
            }
        }

        private void CheckGFVersion()
        {
            const string SteamGameId = "630100";
            string steamInstallPath;
            if (!Environment.Is64BitOperatingSystem)
            {
                steamInstallPath = Methods.GetRegistryValue(Strings.Registry.Steam.RegistryKey, Strings.Registry.Steam.Key32Path, Strings.Registry.Steam.InstallPath);
            }
            else
            {
                steamInstallPath = Methods.GetRegistryValue(Strings.Registry.Steam.RegistryKey, Strings.Registry.Steam.Key64Path, Strings.Registry.Steam.InstallPath);

                if (steamInstallPath == String.Empty)
                {
                    steamInstallPath = Methods.GetRegistryValue(Strings.Registry.Steam.RegistryKey, Strings.Registry.Steam.Key32Path, Strings.Registry.Steam.InstallPath);
                }
            }

            if (steamInstallPath == String.Empty)
            {
                throw new Exception(StringLoader.GetText("exception_game_not_latest"));
            }

            List<string> libraryPaths = new List<string>();
            string mainSteamLibrary = Path.Combine(steamInstallPath, "steamapps");
            libraryPaths.Add(mainSteamLibrary);
            string libraryFoldersFile = Path.Combine(mainSteamLibrary, "libraryfolders.vdf");

            SteamManifest libraryManifest = SteamManifest.Load(libraryFoldersFile);
            int i = 1;
            while (libraryManifest.Elements.TryGetValue((i++).ToString(), out SteamManifestElement sme))
            {
                libraryPaths.Add(Path.Combine(((SteamManifestEntry)sme).Value, "steamapps"));
            }

            bool success = false;
            foreach (string libraryPath in libraryPaths)
            {
                string acf = Path.Combine(libraryPath, $"appmanifest_{SteamGameId}.acf");
                if (File.Exists(acf))
                {
                    SteamManifest smacf = SteamManifest.Load(acf);
                    if (smacf.Elements.TryGetValue("StateFlags", out SteamManifestElement sme))
                    {
                        if (Int32.TryParse(((SteamManifestEntry)sme).Value, out int stateFlagInt))
                        {
                            AppState appState = (AppState)stateFlagInt;
                            if (appState == AppState.StateFullyInstalled)
                            {
                                success = true;
                            }
                            else
                            {
                                success = false;
                            }
                        }
                    }
                }
            }

            if (!success)
            {
                throw new Exception(StringLoader.GetText("exception_game_not_latest"));
            }
        }

        private static bool WebFileExists(string uri)
        {
            if (WebRequest.CreateHttp(uri) is HttpWebRequest request)
            {
                request.Method = "HEAD";

                try
                {
                    if (request.GetResponse() is HttpWebResponse response)
                    {
                        using (response)
                        {
                            return response.StatusCode == HttpStatusCode.OK;
                        }
                    }
                }
                catch (WebException ex)
                {
                    if (ex.Response is HttpWebResponse exResponse)
                    {
                        HttpStatusCode statusCode = exResponse.StatusCode;
                        if (statusCode != HttpStatusCode.NotFound)
                        {
                            throw;
                        }
                    }
                    else
                    {
                        throw;
                    }
                }

                request.Abort();
            }

            return false;
        }

        private Version GetNextVersion(Version clientVer)
        {
            Version result = new Version(clientVer.Major, clientVer.Minor, clientVer.Build, clientVer.Revision + 1);

            if (!WebFileExists(this.Url + VersionToRTP(result)))
            {
                result = new Version(clientVer.Major, clientVer.Minor, clientVer.Build + 1, 0);

                if (!WebFileExists(this.Url + VersionToRTP(result)))
                {
                    result = new Version(clientVer.Major, clientVer.Minor + 1, 0, 0);

                    if (!WebFileExists(this.Url + VersionToRTP(result)))
                    {
                        result = new Version(clientVer.Major + 1, 0, 0, 0);
                    }
                }
            }

            return result;
        }

        internal static string VersionToRTP(Version version)
        {
            return $"{version.Major}_{version.Minor}_{version.Build}_{version.Revision}.RTP";
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
                    string text = Marshal.PtrToStringAnsi(ptr);
                    this.LastMessage = text;
                    File.AppendAllText(this.CurrentLogFilePath, text);

                    break;
                case 14u:
                case 17u:
                case 18u: // abort on error

                    break;//return null;
                case 5u: // completion percentage
                    int readInt = Marshal.ReadInt32(ptr);
                    int percentage = readInt > short.MaxValue ? int.MaxValue : readInt * 0x10000;
                    this.Worker.ReportProgress(percentage);
                    percentage = readInt * 100 / 0x8000;
                    File.AppendAllText(this.CurrentLogFilePath, $"[{percentage}%]");

                    break;
                case 6u: // number of files in patch
                    int fileCount = Marshal.ReadInt32(ptr);
                    this.FileCount = fileCount;
                    File.AppendAllText(this.CurrentLogFilePath, $"File Count=[{fileCount}]\n");

                    break;
                case 7u: // current file
                    string fileName = Marshal.PtrToStringAnsi(ptr);
                    this.FileNumber++;
                    this.FileName = fileName;
                    this.Worker.ReportProgress(-1);
                    File.AppendAllText(this.CurrentLogFilePath, $"Patching=[{fileName}]\n");

                    break;
                default:
                    break; // ignore rest
            }

            if (this.Worker.CancellationPending)
            {
                return null;
            }

            return "";
        }

        internal void Cancel()
        {
            this.Worker.CancelAsync();
        }

        internal void Run(Language language)
        {
            if (this.Worker.IsBusy)
            {
                return;
            }

            this.Language = language;
            this.Worker.RunWorkerAsync();
        }
    }
}
