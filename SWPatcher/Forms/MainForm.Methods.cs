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
using Microsoft.Win32;
using SWPatcher.General;
using SWPatcher.Helpers;
using SWPatcher.Helpers.GlobalVariables;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Windows.Forms;

namespace SWPatcher.Forms
{
    public partial class MainForm
    {
        public IEnumerable<string> GetComboBoxItemsAsString()
        {
            return this.comboBoxLanguages.Items.Cast<Language>().Select(s => s.Name);
        }

        public void RestoreFromTray()
        {
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            this.Show();

            this.notifyIcon.Visible = false;
        }

        private static void StartupBackupCheck(Language language)
        {
            if (Directory.Exists(Strings.FolderName.Backup))
            {
                if (Directory.GetFiles(Strings.FolderName.Backup, "*", SearchOption.AllDirectories).Length > 0)
                {
                    DialogResult result = MsgBox.Question(String.Format(StringLoader.GetText("question_backup_files_found"), language.Name));

                    if (result == DialogResult.Yes)
                        RestoreBackup(language);
                    else
                        Directory.Delete(Strings.FolderName.Backup, true);
                }
            }
            else
            {
                Directory.CreateDirectory(Strings.FolderName.Backup);
            }
        }

        private static void RestoreBackup(Language language)
        {
            if (!Directory.Exists(Strings.FolderName.Backup))
                return;

            string backupFilePath = Path.Combine(Strings.FolderName.Backup, Strings.FileName.GameExe);
            if (File.Exists(backupFilePath))
            {
                string gameExePath = Path.Combine(UserSettings.GamePath, Strings.FileName.GameExe);
                string gameExePatchedPath = Path.Combine(UserSettings.PatcherPath, Strings.FileName.GameExe);
                Logger.Info($"Restoring .exe original=[{gameExePath}] backup=[{gameExePatchedPath}]");

                if (File.Exists(gameExePath))
                    File.Move(gameExePath, gameExePatchedPath);
                File.Move(backupFilePath, gameExePath);
            }

            string[] filePaths = Directory.GetFiles(Strings.FolderName.Backup, "*", SearchOption.AllDirectories);

            foreach (var file in filePaths)
            {
                string path = Path.Combine(UserSettings.GamePath, file.Substring(Strings.FolderName.Backup.Length + 1));
                Logger.Info($"Restoring files original=[{path}] backup=[{file}]");

                if (File.Exists(path))
                {
                    string langPath = Path.Combine(language.Name, path.Substring(UserSettings.GamePath.Length + 1));
                    if (File.Exists(langPath))
                        File.Delete(langPath);

                    File.Move(path, langPath);
                }

                try
                {
                    File.Move(file, path);
                }
                catch (DirectoryNotFoundException)
                {
                    MsgBox.Error(String.Format(StringLoader.GetText("exception_cannot_restore_file"), Path.GetFullPath(file)));
                    Logger.Error($"Cannot restore file=[{file}]");
                    File.Delete(file);
                }
            }
        }

        private static void DeleteTmpFiles(Language language)
        {
            string[] tmpFilePaths = Directory.GetFiles(language.Name, "*.tmp", SearchOption.AllDirectories);

            foreach (var tmpFile in tmpFilePaths)
            {
                File.Delete(tmpFile);
                Logger.Info($"Deleting tmp file=[{tmpFile}]");
            }
        }

        private static string GetSwPathFromRegistry()
        {
            if (!Environment.Is64BitOperatingSystem)
            {
                using (RegistryKey key32 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\HanPurple\J_SW"))
                {
                    if (key32 != null)
                    {
                        return Convert.ToString(key32.GetValue("folder", String.Empty)).Replace("\\\\", "\\");
                    }
                    else
                    {
                        throw new Exception(StringLoader.GetText("exception_game_install_not_found"));
                    }
                }
            }
            else
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\HanPurple\J_SW"))
                {
                    if (key != null)
                    {
                        return Convert.ToString(key.GetValue("folder", String.Empty));
                    }
                    else
                    {
                        using (RegistryKey key32 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\HanPurple\J_SW"))
                        {
                            if (key32 != null)
                            {
                                return Convert.ToString(key32.GetValue("folder", String.Empty)).Replace("\\\\", "\\");
                            }
                            else
                            {
                                throw new Exception(StringLoader.GetText("exception_game_install_not_found"));
                            }
                        }
                    }
                }
            }
        }

        private static Language[] GetAvailableLanguages()
        {
            List<Language> langs = new List<Language>();

            using (var client = new WebClient())
            {
                byte[] fileBytes = client.DownloadData(Urls.TranslationGitHubHome + Strings.IniName.LanguagePack);
                IniFile ini = new IniFile(new IniOptions
                {
                    Encoding = Encoding.UTF8
                });
                using (var ms = new MemoryStream(fileBytes))
                {
                    ini.Load(ms);
                }

                foreach (IniSection section in ini.Sections)
                {
                    langs.Add(new Language(section.Name, Methods.ParseDate(section.Keys[Strings.IniName.Pack.KeyDate].Value)));
                }
            }

            return langs.ToArray();
        }

        private static void DeleteTranslationIni(Language language)
        {
            string iniPath = Path.Combine(language.Name, Strings.IniName.Translation);
            if (Directory.Exists(Path.GetDirectoryName(iniPath)))
                File.Delete(iniPath);
        }

        private static string GetSHA256(string filename)
        {
            using (var sha256 = SHA256.Create())
            using (FileStream fs = File.OpenRead(filename))
            {
                return BitConverter.ToString(sha256.ComputeHash(fs)).Replace("-", "");
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
            IEnumerable<string> translationFilePaths = archivesPaths.Distinct().Union(otherSWFilesPaths).Select(f => Path.Combine(language.Name, f));

            foreach (var path in translationFilePaths)
            {
                if (!File.Exists(path))
                {
                    return true;
                }
            }

            return false;
        }

        private static Process GetProcess(string name)
        {
            Process[] processesByName = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(name));

            if (processesByName.Length > 0)
                return processesByName[0];

            return null;
        }

        private static void BackupAndPlaceFiles(Language language)
        {
            ReadOnlyCollection<SWFile> swFiles = SWFileManager.GetFiles();
            ILookup<Type, SWFile> swFileTypeLookup = swFiles.ToLookup(f => f.GetType());
            IEnumerable<string> archives = swFileTypeLookup[typeof(ArchivedSWFile)].Select(f => f.Path).Union(swFileTypeLookup[typeof(PatchedSWFile)].Select(f => f.Path));
            IEnumerable<string> otherSWFiles = swFileTypeLookup[typeof(SWFile)].Select(f => f.Path + Path.GetFileName(f.PathD));
            IEnumerable<string> translationFiles = archives.Distinct().Union(otherSWFiles).Select(f => Path.Combine(language.Name, f));

            foreach (var path in translationFiles)
            {
                string originalFilePath = Path.Combine(UserSettings.GamePath, path);
                string translationFilePath = Path.Combine(language.Name, path);
                string backupFilePath = Path.Combine(Strings.FolderName.Backup, path);

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

        private static void HangameLogin(MyWebClient client)
        {
            var values = new NameValueCollection(2);
            string id = HttpUtility.UrlEncode(UserSettings.GameId);

            values[Strings.Web.PostEncodeId] = id;
            values[Strings.Web.PostEncodeFlag] = Strings.Web.PostEncodeFlagDefaultValue;
            if (String.IsNullOrEmpty(values[Strings.Web.PostId] = id))
            {
                throw new Exception(StringLoader.GetText("exception_empty_id"));
            }
            using (System.Security.SecureString secure = Methods.DecryptString(UserSettings.GamePw))
            {
                if (String.IsNullOrEmpty(values[Strings.Web.PostPw] = HttpUtility.UrlEncode(Methods.ToInsecureString(secure))))
                {
                    throw new Exception(StringLoader.GetText("exception_empty_pw"));
                }
            }
            values[Strings.Web.PostClearFlag] = Strings.Web.PostClearFlagDefaultValue;
            values[Strings.Web.PostNextUrl] = Strings.Web.PostNextUrlDefaultValue;

            var loginResponse = Encoding.GetEncoding("shift-jis").GetString(client.UploadValues(Urls.HangameLogin, values));
            if (loginResponse.Contains(Strings.Web.CaptchaValidationText))
            {
                Process.Start(Strings.Web.CaptchaUrl);
                throw new Exception(StringLoader.GetText("exception_captcha_validation"));
            }
            try
            {
                string[] messages = GetVariableValue(loginResponse, Strings.Web.MessageVariable);
                if (messages[0].Length > 0)
                    throw new Exception(StringLoader.GetText("exception_incorrect_id_pw"));
            }
            catch (IndexOutOfRangeException)
            {

            }
        }

        private static string GetGameStartResponse(MyWebClient client)
        {
            again:
            string gameStartResponse = client.DownloadString(Urls.SoulworkerGameStart);
            try
            {
                if (GetVariableValue(gameStartResponse, Strings.Web.ErrorCodeVariable)[0] == "03")
                {
                    throw new Exception(StringLoader.GetText("exception_not_tos"));
                }
                else if (GetVariableValue(gameStartResponse, Strings.Web.MaintenanceVariable)[0] == "C")
                {
                    throw new Exception(StringLoader.GetText("exception_game_maintenance"));
                }
            }
            catch (IndexOutOfRangeException)
            {
                DialogResult dialog = MsgBox.ErrorRetry(StringLoader.GetText("exception_retry_validation_failed"));
                if (dialog == DialogResult.Retry)
                {
                    goto again;
                }

                throw new Exception(StringLoader.GetText("exception_validation_failed"));
            }

            return gameStartResponse;
        }

        private static string[] GetGameStartArguments(MyWebClient client)
        {
            again:
            try
            {
                client.UploadData(Urls.SoulworkerRegistCheck, new byte[] { });

                string reactorStartResponse = Encoding.Default.GetString(client.UploadData(Urls.SoulworkerReactorGameStart, new byte[] { }));
                
                if (GetVariableValue(reactorStartResponse, Strings.Web.ErrorCodeArg)[0] == "10")
                {
                    throw new Exception(StringLoader.GetText("exception_game_maintenance"));
                }

                string[] gameStartArgs = new string[3];
                gameStartArgs[0] = GetVariableValue(reactorStartResponse, Strings.Web.GameStartArg)[0];
                gameStartArgs[1] = Strings.Server.IP;
                gameStartArgs[2] = Strings.Server.Port;

                return gameStartArgs;
            }
            catch (WebException webEx)
            {
                DialogResult dialog = MsgBox.ErrorRetry(StringLoader.GetText("exception_retry_validation_failed"));
                if (dialog == DialogResult.Retry)
                {
                    goto again;
                }

                var responseError = webEx.Response as HttpWebResponse;
                if (responseError.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new WebException(StringLoader.GetText("exception_validation_failed"), webEx);
                }
                else
                {
                    throw;
                }
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

        private string UploadToPasteBin(string title, string text, PasteBinExpiration expiration, bool isPrivate, string format)
        {
            var client = new PasteBinClient(Strings.PasteBin.DevKey);

            try
            {
                client.Login(Strings.PasteBin.Username, Strings.PasteBin.Password);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            var entry = new PasteBinEntry
            {
                Title = title,
                Text = text,
                Expiration = expiration,
                Private = isPrivate,
                Format = format
            };

            try
            {
                return client.Paste(entry);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                MsgBox.Error(StringLoader.GetText("exception_log_file_failed"));
            }
            finally
            {
                client.Logout();
            }

            return null;
        }

        private static byte[] TrimArrayIfNecessary(byte[] array)
        {
            int limit = 512000 / 2;

            if (array.Length > limit)
            {
                byte[] trimmedArray = new byte[limit];
                Array.Copy(array, array.Length - limit, trimmedArray, 0, limit);

                return trimmedArray;
            }

            return array;
        }
    }
}
