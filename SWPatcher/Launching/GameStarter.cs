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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using Newtonsoft.Json;
using SWPatcher.General;
using SWPatcher.Helpers;
using SWPatcher.Helpers.GlobalVariables;
using static SWPatcher.Forms.MainForm;

namespace SWPatcher.Launching
{
    delegate void GameStarterProgressChangedEventHandler(object sender, GameStarterProgressChangedEventArgs e);
    delegate void GameStarterCompletedEventHandler(object sender, GameStarterCompletedEventArgs e);

    internal class GameStarter
    {
        private readonly BackgroundWorker Worker;
        private Language Language;
        private bool PlaceTranslations;

        internal GameStarter()
        {
            this.Worker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            this.Worker.DoWork += this.Worker_DoWork;
            this.Worker.ProgressChanged += this.Worker_ProgressChanged;
            this.Worker.RunWorkerCompleted += this.Worker_RunWorkerCompleted;
        }

        internal event GameStarterProgressChangedEventHandler GameStarterProgressChanged;
        internal event GameStarterCompletedEventHandler GameStarterCompleted;

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            this.Worker.ReportProgress((int)State.Prepare);

            if (this.PlaceTranslations)
            {
                Logger.Debug(Methods.MethodFullName("GameStart", Thread.CurrentThread.ManagedThreadId.ToString(), this.Language.ApplyingRegionId, this.Language.ToString()));

                SWFileManager.LoadFileConfiguration(this.Language);

                if (IsTranslationOutdatedOrMissing(this.Language))
                {
                    e.Result = true; // call force patch in completed event
                    return;
                }

                if (UserSettings.WantToPatchExe)
                {
                    string regionId = this.Language.ApplyingRegionId;
                    string regionFolder = this.Language.ApplyingRegionFolder;
                    string gameExePath = Path.Combine(UserSettings.GamePath, Methods.GetGameExeName(regionId));
                    string gameExePatchedPath = Path.Combine(UserSettings.PatcherPath, regionId, Methods.GetGameExeName(regionId));
                    string backupFilePath = Path.Combine(this.Language.BackupPath, Methods.GetGameExeName(regionId));

                    if (!File.Exists(gameExePatchedPath))
                    {
                        byte[] gameExeBytes = File.ReadAllBytes(gameExePath);

                        Methods.PatchExeFile(gameExeBytes, gameExePatchedPath, Urls.TranslationGitHubHome + regionFolder + '/' + Strings.IniName.BytesToPatch);
                    }

                    BackupAndPlaceFile(gameExePath, gameExePatchedPath, backupFilePath);
                }

                Process clientProcess = null;
                if (UserSettings.WantToLogin)
                {
                    string regionId = this.Language.ApplyingRegionId;
                    switch (regionId)
                    {
                        case "jp":
                            StartHangeJP(() => BackupAndPlaceFiles(this.Language));

                            this.Worker.ReportProgress((int)State.WaitClient);
                            while (true)
                            {
                                if (this.Worker.CancellationPending)
                                {
                                    e.Cancel = true;
                                    return;
                                }

                                clientProcess = GetProcess(Methods.GetGameExeName(regionId));

                                if (clientProcess == null)
                                {
                                    Thread.Sleep(1000);
                                }
                                else
                                {
                                    break;
                                }
                            }

                            break;
                        case "gjp":
                            StartGamecomJP(() => BackupAndPlaceFiles(this.Language));

                            this.Worker.ReportProgress((int)State.WaitClient);
                            while (true)
                            {
                                if (this.Worker.CancellationPending)
                                {
                                    e.Cancel = true;
                                    return;
                                }

                                clientProcess = GetProcess(Methods.GetGameExeName(regionId));

                                if (clientProcess == null)
                                {
                                    Thread.Sleep(1000);
                                }
                                else
                                {
                                    break;
                                }
                            }

                            break;
                        case "kr":
                            StartStoveKR(() => BackupAndPlaceFiles(this.Language));

                            this.Worker.ReportProgress((int)State.WaitClient);
                            while (true)
                            {
                                if (this.Worker.CancellationPending)
                                {
                                    e.Cancel = true;
                                    return;
                                }

                                clientProcess = GetProcess(Methods.GetGameExeName(regionId));

                                if (clientProcess == null)
                                {
                                    Thread.Sleep(1000);
                                }
                                else
                                {
                                    break;
                                }
                            }

                            break;
                        case "nkr":
                            Methods.RegionDoesNotSupportLogin();

                            break;
                        case "gl":
                            Methods.RegionDoesNotSupportLogin();

                            break;
                        default:
                            throw new Exception(StringLoader.GetText("exception_region_unknown", regionId));
                    }
                }
                else
                {
                    BackupAndPlaceFiles(this.Language);

                    this.Worker.ReportProgress((int)State.WaitClient);
                    while (true)
                    {
                        if (this.Worker.CancellationPending)
                        {
                            e.Cancel = true;
                            return;
                        }

                        clientProcess = GetProcess(Methods.GetGameExeName(this.Language.ApplyingRegionId));

                        if (clientProcess == null)
                        {
                            Thread.Sleep(1000);
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                this.Worker.ReportProgress((int)State.WaitClose);
                clientProcess.WaitForExit();
            }
            else
            {
                Logger.Debug(Methods.MethodFullName("GameStart", Thread.CurrentThread.ManagedThreadId.ToString(), this.Language.ApplyingRegionId));

                if (UserSettings.WantToLogin)
                {
                    string regionId = this.Language.ApplyingRegionId;
                    switch (regionId)
                    {
                        case "jp":
                            StartHangeJP();
                            e.Cancel = true;

                            break;
                        case "gjp":
                            StartGamecomJP();
                            e.Cancel = true;

                            break;
                        case "kr":
                            StartStoveKR();
                            e.Cancel = true;

                            break;
                        case "nkr":
                            Methods.RegionDoesNotSupportLogin();

                            break;
                        case "gl":
                            Methods.RegionDoesNotSupportLogin();

                            break;
                        default:
                            throw new Exception(StringLoader.GetText("exception_region_unknown", regionId));
                    }
                }
                else
                {
                    throw new Exception(StringLoader.GetText("exception_not_login_option"));
                }
            }
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.GameStarterProgressChanged?.Invoke(sender, new GameStarterProgressChangedEventArgs((State)e.ProgressPercentage));
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled || e.Error != null)
            {
                this.GameStarterCompleted?.Invoke(sender, new GameStarterCompletedEventArgs(e.Cancelled, e.Error, this.Language));
            }
            else
            {
                this.GameStarterCompleted?.Invoke(sender, new GameStarterCompletedEventArgs(e.Cancelled, e.Error, this.Language, e.Result == null ? false : (bool)e.Result));
            }
        }

        private static bool IsTranslationOutdatedOrMissing(Language language)
        {
            if (Methods.IsTranslationOutdated(language))
            {
                return true;
            }

            ReadOnlyCollection<SWFile> swFiles = SWFileManager.GetFiles();
            ILookup<Type, SWFile> things = swFiles.ToLookup(f => f.GetType());
            IEnumerable<string> archivesPaths = things[typeof(ArchivedSWFile)].Select(f => f.Path).Union(things[typeof(PatchedSWFile)].Select(f => f.Path));
            IEnumerable<string> otherSWFilesPaths = things[typeof(SWFile)].Select(f => f.Path + Path.GetFileName(f.PathD));
            IEnumerable<string> translationFilePaths = archivesPaths.Distinct().Union(otherSWFilesPaths).Select(f => Path.Combine(language.Path, f));

            foreach (string path in translationFilePaths)
            {
                if (!File.Exists(path))
                {
                    return true;
                }
            }

            return false;
        }

        private static void BackupAndPlaceFiles(Language language)
        {
            ReadOnlyCollection<SWFile> swFiles = SWFileManager.GetFiles();
            ILookup<Type, SWFile> swFileTypeLookup = swFiles.ToLookup(f => f.GetType());
            IEnumerable<string> archives = swFileTypeLookup[typeof(ArchivedSWFile)].Select(f => f.Path).Union(swFileTypeLookup[typeof(PatchedSWFile)].Select(f => f.Path));
            IEnumerable<string> otherSWFiles = swFileTypeLookup[typeof(SWFile)].Select(f => f.Path + Path.GetFileName(f.PathD));
            IEnumerable<string> translationFiles = archives.Distinct().Union(otherSWFiles);

            foreach (string path in translationFiles)
            {
                string originalFilePath = Path.Combine(UserSettings.GamePath, path);
                string translationFilePath = Path.Combine(language.Path, path);
                string backupFilePath = Path.Combine(language.BackupPath, path);

                BackupAndPlaceFile(originalFilePath, translationFilePath, backupFilePath);
            }
        }

        private static void BackupAndPlaceFile(string originalFilePath, string translationFilePath, string backupFilePath)
        {
            string backupFileDirectory = Path.GetDirectoryName(backupFilePath);
            Directory.CreateDirectory(backupFileDirectory);

            Logger.Info($"Swapping file original=[{originalFilePath}] backup=[{backupFilePath}] translation=[{translationFilePath}]");
            File.Move(originalFilePath, backupFilePath);
            File.Move(translationFilePath, originalFilePath);
        }

        private static MyWebClient HangeLogin(MyWebClient client)
        {
            string id = UserSettings.GameId;
            string pw = UserSettings.GamePw;

            if (String.IsNullOrEmpty(id))
            {
                throw new Exception(StringLoader.GetText("exception_empty_id"));
            }

            var values = new NameValueCollection(5)
            {
                [Strings.Web.JP.Hange.PostEncodeId] = HttpUtility.UrlEncode(id),
                [Strings.Web.JP.Hange.PostEncodeFlag] = Strings.Web.JP.Hange.PostEncodeFlagDefaultValue,
                [Strings.Web.JP.Hange.PostId] = id,
            };
            using (System.Security.SecureString secure = Methods.DecryptString(pw))
            {
                if (String.IsNullOrEmpty(values[Strings.Web.JP.Hange.PostPw] = Methods.ToInsecureString(secure)))
                {
                    throw new Exception(StringLoader.GetText("exception_empty_pw"));
                }
            }
            values[Strings.Web.JP.Hange.PostClearFlag] = Strings.Web.JP.Hange.PostClearFlagDefaultValue;
            values[Strings.Web.JP.Hange.PostNextUrl] = Strings.Web.JP.Hange.PostNextUrlDefaultValue;

            byte[] byteResponse = client.UploadValues(Urls.HangeLogin, values);
            string loginResponse = Encoding.GetEncoding("shift-jis").GetString(byteResponse);
            if (loginResponse.Contains(Strings.Web.JP.Hange.CaptchaValidationText) || loginResponse.Contains(Strings.Web.JP.Hange.CaptchaValidationText2))
            {
                Process.Start(Strings.Web.JP.Hange.CaptchaUrl);
                throw new Exception(StringLoader.GetText("exception_captcha_validation"));
            }
            try
            {
                string[] messages = GetVariableValue(loginResponse, Strings.Web.JP.Hange.MessageVariable);

                if (messages[0].Length > 0)
                {
                    throw new Exception(StringLoader.GetText("exception_incorrect_id_pw"));
                }
            }
            catch (IndexOutOfRangeException)
            {

            }

            return client;
        }

        private static MyWebClient GamecomLogin(MyWebClient client)
        {
            string id = UserSettings.GameId;
            string pw = UserSettings.GamePw;

            if (String.IsNullOrEmpty(id))
            {
                throw new Exception(StringLoader.GetText("exception_empty_id"));
            }

            var values = new NameValueCollection(2)
            {
                [Strings.Web.JP.Gamecom.PostId] = HttpUtility.UrlEncode(id),
            };
            using (System.Security.SecureString secure = Methods.DecryptString(pw))
            {
                if (String.IsNullOrEmpty(values[Strings.Web.JP.Gamecom.PostPw] = Methods.ToInsecureString(secure)))
                {
                    throw new Exception(StringLoader.GetText("exception_empty_pw"));
                }
            }
            byte[] byteResponse = client.UploadValues(Urls.GamecomLogin, values);
            string loginResponse = Encoding.GetEncoding("shift-jis").GetString(byteResponse);
            client.DownloadString(Urls.SoulworkerJPGamecomHome);

            return client;
        }

        private static void StoveLogin(MyWebClient client)
        {
            client.DownloadData("https://member.onstove.com/auth/login");

            string id = UserSettings.GameId;
            string pw = UserSettings.GamePw;

            if (String.IsNullOrEmpty(id))
            {
                throw new Exception(StringLoader.GetText("exception_empty_id"));
            }

            var values = new NameValueCollection(3)
            {
                [Strings.Web.KR.PostId] = id
            };
            using (System.Security.SecureString secure = Methods.DecryptString(pw))
            {
                if (String.IsNullOrEmpty(values[Strings.Web.KR.PostPw] = Methods.ToInsecureString(secure)))
                {
                    throw new Exception(StringLoader.GetText("exception_empty_pw"));
                }
            }
            values[Strings.Web.KR.KeepForever] = Strings.Web.KR.KeepForeverDefaultValue;

            client.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");
            byte[] byteResponse = client.UploadValues(Urls.StoveLogin, values);
            string loginResponse = Encoding.UTF8.GetString(byteResponse);

            var loginJSON = new { value = "", message = "", result = "" };
            var jsonResponse = JsonConvert.DeserializeAnonymousType(loginResponse, loginJSON);

            switch (jsonResponse.result ?? throw new Exception("unexpected null result"))
            {
                case "000":

                    break;
                case "589":
                    Process.Start("https://member.onstove.com/auth/login");
                    throw new Exception(StringLoader.GetText("exception_captcha_required"));
                case "551":
                case "552":
                case "553":
                case "554":
                    Process.Start($"https://member.onstove.com/block?user_id={id}");
                    throw new Exception(StringLoader.GetText("exception_follow_instruction_webpage"));
                case "569":
                    Process.Start($"https://member.onstove.com/register/ok?user_id={id}");
                    throw new Exception(StringLoader.GetText("exception_follow_instruction_webpage"));
                case "556":
                    throw new Exception(StringLoader.GetText("exception_incorrect_id_pw"));
                case "610":
                    throw new Exception(StringLoader.GetText("exception_account_to_be_deleted"));
                case "550":
                    var onlyIdValues = new NameValueCollection(1)
                    {
                        [Strings.Web.KR.PostId] = id
                    };
                    client.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");
                    byte[] byteWakeUpResponse = client.UploadValues("https://member.onstove.com/member/wake/up", onlyIdValues);
                    string wakeUpResponse = Encoding.UTF8.GetString(byteWakeUpResponse);
                    var jsonWakeUpResponse = JsonConvert.DeserializeAnonymousType(wakeUpResponse, loginJSON);
                    switch (jsonWakeUpResponse.result ?? throw new Exception("unexpected null result"))
                    {
                        case "000":
                            StoveLogin(client);

                            break;
                        default:
                            throw new Exception($"result=[{jsonResponse.result}]\n{jsonResponse.message ?? "no error details"}");
                    }

                    break;
                default:
                    throw new Exception($"result=[{jsonResponse.result}]\n{jsonResponse.message ?? "no error details"}");
            }
        }

        private static string GetGameStartArgumentHangeJP(MyWebClient client)
        {
            client.DownloadString(Urls.SoulworkerJPHangeExternalGameStartMiddleware);
            string response = client.DownloadString(Urls.SoulworkerJPHangeGameStart);

            if (response.Length > 3)
            {
                return response.Trim('"').Replace("\\", "");
            }
            else
            {
                switch (response)
                {
                    case "110":
                        throw new Exception("error login (110)");
                    case "320":
                        throw new Exception(StringLoader.GetText("exception_game_maintenance"));
                    case "340":
                        throw new Exception("error block (340)");
                    default:
                        throw new Exception($"errCode=[{response}");
                }
            }
        }

        private static string GetGameStartArgumentGamecomJP(MyWebClient client)
        {
            string response = client.DownloadString(Urls.SoulworkerJPGamecomGameStart);

            if (response.Length > 3)
            {
                return response.Trim('"').Replace("\\", "");
            }
            else
            {
                switch (response)
                {
                    case "110":
                        throw new Exception("error login (110)");
                    case "320":
                        throw new Exception(StringLoader.GetText("exception_game_maintenance"));
                    case "340":
                        throw new Exception("error block (340)");
                    default:
                        throw new Exception($"errCode=[{response}");
                }
            }
        }

        private static string GetKRGameStartProtocol(MyWebClient client)
        {
            string response = Encoding.UTF8.GetString(client.UploadData(Urls.SoulworkerKRGameStart, new byte[] { }));
            var gameStartJSON = new { code = Int32.MaxValue, message = "", memberNo = Int64.MaxValue, gameAuthToken = "", maintenenceType = "", endTime = "", maintenenceTime = "" };
            var jsonResponse = JsonConvert.DeserializeAnonymousType(response, gameStartJSON);

            switch (jsonResponse.code)
            {
                case 0:
                    return $"sgup://run/11/{jsonResponse.memberNo}/{jsonResponse.gameAuthToken ?? throw new Exception("unexpected null gameAuthToken")}";

                case -1:
                    throw new Exception($"You are not logged in.");

                case -3:
                    string maintType = jsonResponse.maintenenceType ?? throw new Exception("unexpected null maintenenceType");
                    string maintTime = jsonResponse.maintenenceTime ?? throw new Exception("unexpected null maintenenceTime");
                    string maintEndTime = jsonResponse.endTime ?? throw new Exception("unexpected null endTime");
                    string message = jsonResponse.message ?? throw new Exception("unexpected null message");

                    throw new Exception(StringLoader.GetText("exception_game_stove_maintenance", maintType, maintTime, maintEndTime, message.Replace("<p>", "").Replace("</p>", "\n")));

                case -4:
                case -5:

                    throw new Exception(StringLoader.GetText("exception_account_not_validated"));
                default:
                    throw new Exception($"code=[{jsonResponse.code}]");
            }
        }

        private static string[] GetVariableValue(string fullText, string variableName)
        {
            string result;
            int valueIndex = fullText.IndexOf(variableName);

            if (valueIndex == -1)
            {
                throw new IndexOutOfRangeException();
            }

            result = fullText.Substring(valueIndex + variableName.Length + 1);
            result = result.Substring(0, result.IndexOf('"'));

            return result.Split(' ');
        }

        private static Process StartHangeJP(Action onSuccess = null)
        {
            using (var client = new MyWebClient())
            {
                string gameStartArg = GetGameStartArgumentHangeJP(HangeLogin(client));

                onSuccess?.Invoke();

                return Process.Start(gameStartArg);
            }
        }

        private static Process StartGamecomJP(Action onSuccess = null)
        {
            using (var client = new MyWebClient())
            {
                string gameStartArg = GetGameStartArgumentGamecomJP(GamecomLogin(client));

                onSuccess?.Invoke();

                return Process.Start(gameStartArg);
            }
        }

        private static void StartStoveKR(Action onSuccess = null)
        {
            using (var client = new MyWebClient())
            {
                StoveLogin(client);
                string stoveProtocol = GetKRGameStartProtocol(client);

                onSuccess?.Invoke();

                Process.Start(stoveProtocol);
            }
        }

        private static Process GetProcess(string name)
        {
            Process[] processesByName = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(name));

            if (processesByName.Length > 0)
            {
                return processesByName[0];
            }

            return null;
        }

        internal void Cancel()
        {
            this.Worker.CancelAsync();
        }

        internal void Run(Language language, bool placeTranslations)
        {
            if (this.Worker.IsBusy)
            {
                return;
            }

            this.PlaceTranslations = placeTranslations;
            this.Language = language;
            this.Worker.RunWorkerAsync();
        }
    }
}
