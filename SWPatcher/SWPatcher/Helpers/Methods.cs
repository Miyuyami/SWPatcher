using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Ionic.Zip;
using MadMilkman.Ini;
using PubPluginLib;
using SWPatcher.General;
using SWPatcher.Helpers.GlobalVar;

namespace SWPatcher.Helpers
{
    internal static class Methods
    {
        private static string DateFormat = "dd/MMM/yyyy h:mm tt";

        internal static DateTime ParseDate(string date)
        {
            return DateTime.ParseExact(date, DateFormat, CultureInfo.InvariantCulture);
        }

        internal static string DateToString(DateTime date)
        {
            return date.ToString(DateFormat, CultureInfo.InvariantCulture);
        }

        internal static bool HasNewTranslations(Language language)
        {
            string directory = language.Lang;

            if (!Directory.Exists(directory))
                return true;

            string filePath = Path.Combine(directory, Strings.IniName.Translation);
            if (!File.Exists(filePath))
                return true;

            IniFile ini = new IniFile();
            ini.Load(filePath);

            if (!ini.Sections.Contains(Strings.IniName.Patcher.Section))
                return true;

            var section = ini.Sections[Strings.IniName.Patcher.Section];
            if (!section.Keys.Contains(Strings.IniName.Pack.KeyDate))
                return true;

            string date = section.Keys[Strings.IniName.Pack.KeyDate].Value;

            return language.LastUpdate > Methods.ParseDate(date);
        }

        internal static bool IsSwPath(string path)
        {
            return Directory.Exists(path) && Directory.Exists(Path.Combine(path, Strings.FolderName.Data)) && File.Exists(Path.Combine(path, Strings.FileName.GameExe)) && File.Exists(Path.Combine(path, Strings.IniName.ClientVer));
        }

        internal static void RestoreBackup()
        {
            if (Directory.Exists(Strings.FolderName.Backup))
            {
                string[] filePaths = Directory.GetFiles(Strings.FolderName.Backup, "*", SearchOption.AllDirectories);

                if (!String.IsNullOrEmpty(UserSettings.GamePath) && Methods.IsSwPath(UserSettings.GamePath))
                {
                    foreach (var s in filePaths)
                    {
                        string path = Path.Combine(UserSettings.GamePath, s.Substring(Strings.FolderName.Backup.Length + 1));

                        if (Directory.Exists(Path.GetDirectoryName(path)) && !File.Exists(path))
                            File.Move(s, path);
                        else
                            File.Delete(s);
                    }
                }
                else
                    foreach (var s in filePaths)
                        File.Delete(s);
            }
        }

        internal static void RestoreBackup(Language language)
        {
            if (!Directory.Exists(Strings.FolderName.Backup))
                return;

            string backupFilePath = Path.Combine(Strings.FolderName.Backup, Strings.FileName.GameExe);
            if (File.Exists(backupFilePath))
            {
                string gameExePath = Path.Combine(UserSettings.GamePath, Strings.FileName.GameExe);
                string gameExePatchedPath = Path.Combine(UserSettings.PatcherPath, Strings.FileName.GameExe);

                File.Move(gameExePath, gameExePatchedPath);
                File.Move(backupFilePath, gameExePath);
            }

            string[] filePaths = Directory.GetFiles(Strings.FolderName.Backup, "*", SearchOption.AllDirectories);

            foreach (var file in filePaths)
            {
                string path = Path.Combine(UserSettings.GamePath, file.Substring(Strings.FolderName.Backup.Length + 1));

                File.Move(path, Path.Combine(language.Lang, path.Substring(UserSettings.GamePath.Length + 1)));
                File.Move(file, path);
            }
        }

        internal static void DeleteTmpFiles(Language language)
        {
            string[] tmpFilePaths = Directory.GetFiles(language.Lang, "*.tmp", SearchOption.AllDirectories);

            foreach (var tmpFile in tmpFilePaths)
                File.Delete(tmpFile);
        }

        internal static bool IsValidSwPatcherPath(string path)
        {
            return String.IsNullOrEmpty(path) || !Methods.IsSwPath(path) && Methods.IsValidSwPatcherPath(Path.GetDirectoryName(path));
        }

        internal static string GetSwPathFromRegistry()
        {
            using (var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\HanPurple\J_SW"))
            {
                if (key != null)
                    return Convert.ToString(key.GetValue("folder", String.Empty));
                else
                {
                    Error.Log("64-bit - Key not found");

                    using (var key32 = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\HanPurple\J_SW"))
                    {
                        if (key32 != null)
                            return Convert.ToString(key32.GetValue("folder", String.Empty));
                        else
                        {
                            Error.Log("32-bit - Key not found");
                            throw new Exception("Soulworker installation not found.");
                        }
                    }
                }
            }
        }

        internal static Language[] GetAvailableLanguages()
        {
            List<Language> langs = new List<Language>();

            using (var client = new WebClient())
            using (var file = new TempFile())
            {
                client.DownloadFile(Urls.PatcherGitHubHome + Strings.IniName.LanguagePack, file.Path);
                IniFile ini = new IniFile();
                ini.Load(file.Path);

                foreach (var section in ini.Sections)
                    langs.Add(new Language(section.Name, Methods.ParseDate(section.Keys[Strings.IniName.Pack.KeyDate].Value)));
            }

            return langs.ToArray();
        }

        internal static void DeleteTranslationIni(Language language)
        {
            string iniPath = Path.Combine(language.Lang, Strings.IniName.Translation);
            if (Directory.Exists(Path.GetDirectoryName(iniPath)))
                File.Delete(iniPath);
        }

        internal static string GetArchivedSWFilePath(SWFile swFile, Language language)
        {
            string directory = Path.GetDirectoryName(Path.Combine(language.Lang, swFile.Path));
            string archiveName = Path.GetFileNameWithoutExtension(swFile.Path);
            string fileName = Path.GetFileName(swFile.PathD);

            return Path.Combine(directory, archiveName, fileName);
        }

        internal static string GetMD5(string text)
        {
            var md5 = new MD5CryptoServiceProvider();
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));
            byte[] result = md5.Hash;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
                sb.Append(result[i].ToString("x2"));

            return sb.ToString();
        }

        internal static void DoUnzipFile(string zipPath, string fileName, string extractDestination)
        {
            using (var zip = ZipFile.Read(zipPath))
            {
                zip.FlattenFoldersOnExtract = true;
                zip[fileName].Extract(extractDestination, ExtractExistingFileAction.OverwriteSilently);
            }
        }

        internal static void DoZipFile(string zipPath, string fileName, string filePath)
        {
            using (var zip = ZipFile.Read(zipPath))
            {
                zip.RemoveEntry(fileName);
                zip.AddFile(filePath, Path.GetDirectoryName(fileName));
                zip.Save();
            }
        }

        internal static void AddZipToZip(string zipPath, string destinationZipPath, string directoryInDestination)
        {
            using (var zip = ZipFile.Read(zipPath))
            using (var destinationZip = ZipFile.Read(destinationZipPath))
            {
                var tempFileList = zip.Entries.Select(entry => new TempFile(Path.Combine(Path.GetTempPath(), Path.GetFileName(entry.FileName)))).ToList();
                zip.FlattenFoldersOnExtract = true;

                zip.ExtractAll(Path.GetTempPath(), ExtractExistingFileAction.OverwriteSilently);

                destinationZip.RemoveEntries(zip.Entries.Select(e => Path.Combine(directoryInDestination, e.FileName)).ToList());
                destinationZip.AddFiles(tempFileList.Select(tf => tf.Path), directoryInDestination);
                destinationZip.Save();

                tempFileList.ForEach(tf => tf.Dispose());
            }
        }

        internal static bool IsNewerGameClientVersion()
        {
            IniFile ini = new IniFile();
            ini.Load(Path.Combine(UserSettings.GamePath, Strings.IniName.ClientVer));

            return VersionCompare(GetServerVersion(), ini.Sections[Strings.IniName.Ver.Section].Keys[Strings.IniName.Ver.Key].Value);
        }

        internal static bool VersionCompare(string ver1, string ver2)
        {
            Version v1 = new Version(ver1);
            Version v2 = new Version(ver2);

            return v1 > v2;
        }

        internal static string GetServerVersion()
        {
            using (var client = new WebClient())
            using (var zippedFile = new TempFile())
            {
                client.DownloadFile(Urls.SoulworkerSettingsHome + Strings.IniName.ServerVer + ".zip", zippedFile.Path);

                using (var file = new TempFile())
                {
                    using (ZipFile zip = ZipFile.Read(zippedFile.Path))
                    {
                        ZipEntry entry = zip[0];
                        entry.FileName = Path.GetFileName(file.Path);
                        entry.Extract(Path.GetDirectoryName(file.Path), ExtractExistingFileAction.OverwriteSilently);
                    }

                    IniFile ini = new IniFile(new IniOptions
                    {
                        Encoding = System.Text.Encoding.Unicode
                    });
                    ini.Load(file.Path);

                    return ini.Sections[Strings.IniName.Ver.Section].Keys[Strings.IniName.Ver.Key].Value;
                }
            }
        }

        internal static bool IsTranslationOutdated(Language language)
        {
            string selectedTranslationPath = Path.Combine(language.Lang, Strings.IniName.Translation);
            if (!File.Exists(selectedTranslationPath))
                return true;

            IniFile ini = new IniFile();
            ini.Load(selectedTranslationPath);

            if (!ini.Sections[Strings.IniName.Patcher.Section].Keys.Contains(Strings.IniName.Patcher.KeyVer))
                throw new Exception("Error reading translation version, try to Menu -> Force Patch");

            string translationVer = ini.Sections[Strings.IniName.Patcher.Section].Keys[Strings.IniName.Patcher.KeyVer].Value;
            ini.Sections.Clear();
            ini.Load(Path.Combine(UserSettings.GamePath, Strings.IniName.ClientVer));

            string clientVer = ini.Sections[Strings.IniName.Ver.Section].Keys[Strings.IniName.Ver.Key].Value;
            if (VersionCompare(clientVer, translationVer))
                return true;

            return false;
        }

        internal static Process GetProcess(string name)
        {
            Process[] processesByName = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(name));

            if (processesByName.Length > 0)
                return processesByName[0];

            return null;
        }

        internal static void MoveOldPatcherFolder(string oldPath, string newPath, IEnumerable<string> translationFolders)
        {
            string[] movingFolders = translationFolders.Where(s => Directory.Exists(s)).ToArray();
            string backupDirectory = Path.Combine(oldPath, Strings.FolderName.Backup);
            string logFilePath = Path.Combine(oldPath, Strings.FileName.Log);
            string gameExePath = Path.Combine(oldPath, Strings.FileName.GameExe);

            foreach (var folder in movingFolders)
            {
                string folderPath = Path.Combine(oldPath, folder);
                string destinationPath = Path.Combine(newPath, folder);

                foreach (var dirPath in Directory.GetDirectories(folderPath, "*", SearchOption.AllDirectories))
                    Directory.CreateDirectory(dirPath.Replace(folderPath, destinationPath));

                foreach (var filePath in Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories))
                    File.Move(filePath, filePath.Replace(folderPath, destinationPath));

                Directory.Delete(folder, true);
            }

            if (Directory.Exists(backupDirectory))
            {
                foreach (var dirPath in Directory.GetDirectories(backupDirectory, "*", SearchOption.AllDirectories))
                    Directory.CreateDirectory(dirPath.Replace(backupDirectory, Path.Combine(newPath, Strings.FolderName.Backup)));

                foreach (var filePath in Directory.GetFiles(backupDirectory, "*", SearchOption.AllDirectories))
                    File.Move(filePath, filePath.Replace(backupDirectory, Path.Combine(newPath, Strings.FolderName.Backup)));

                Directory.Delete(backupDirectory, true);
            }

            if (File.Exists(logFilePath))
                File.Move(logFilePath, Path.Combine(newPath, Strings.FileName.Log));

            string newGameExePath = Path.Combine(newPath, Strings.FileName.GameExe);
            if (File.Exists(gameExePath))
                File.Move(gameExePath, newGameExePath);
            else if (File.Exists(newGameExePath))
                File.Delete(newGameExePath);
        }

        internal static void PatchExeFile(string gameExePath)
        {
            using (var client = new WebClient())
            using (var file = new TempFile())
            {
                var exeBytes = File.ReadAllBytes(gameExePath);
                string hexResult = BitConverter.ToString(exeBytes).Replace("-", "");

                client.DownloadFile(Urls.PatcherGitHubHome + Strings.IniName.BytesToPatch, file.Path);
                IniFile ini = new IniFile();
                ini.Load(file.Path);

                foreach (var section in ini.Sections)
                {
                    string original = section.Keys[Strings.IniName.PatchBytes.KeyOriginal].Value;
                    string patch = section.Keys[Strings.IniName.PatchBytes.KeyPatch].Value;

                    hexResult = hexResult.Replace(original, patch);
                }

                int charCount = hexResult.Length;
                byte[] resultBytes = new byte[charCount / 2];

                for (int i = 0; i < charCount; i += 2)
                    resultBytes[i / 2] = Convert.ToByte(hexResult.Substring(i, 2), 16);

                File.WriteAllBytes(gameExePath, resultBytes);
            }
        }

        internal static string[] GetVariableValue(string fullText, string variableName)
        {
            string result;
            int valueIndex = fullText.IndexOf(variableName);

            if (valueIndex == -1)
                throw new IndexOutOfRangeException();

            result = fullText.Substring(valueIndex + variableName.Length + 1);
            result = result.Substring(0, result.IndexOf('"'));

            return result.Split(' ');
        }

        internal static void SetSWFiles(List<SWFile> swfiles)
        {
            if (swfiles.Count > 0)
                return;

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
                    swfiles.Add(new SWFile(name, path, pathA, pathD, format));
                }
            }
        }

        internal static void HangameLogin(MyWebClient client)
        {
            var values = new NameValueCollection(2);
            values[Strings.Web.PostId] = UserSettings.GameId;
            values[Strings.Web.PostPw] = UserSettings.GamePw;
            var loginResponse = Encoding.GetEncoding("shift-jis").GetString(client.UploadValues(Urls.HangameLogin, values));
            try
            {
                if (Methods.GetVariableValue(loginResponse, Strings.Web.MessageVariable)[0].Length > 0)
                    throw new Exception("Incorrect ID or Password.");
            }
            catch (IndexOutOfRangeException)
            {

            }
        }

        internal static string GetGameStartResponse(MyWebClient client)
        {
            string gameStartResponse = client.DownloadString(Urls.SoulworkerGameStart);
            try
            {
                if (Methods.GetVariableValue(gameStartResponse, Strings.Web.ErrorCodeVariable)[0] == "03")
                    throw new Exception("To play the game you need to accept the Terms of Service.");
                else if (Methods.GetVariableValue(gameStartResponse, Strings.Web.MaintenanceVariable)[0] == "C")
                    throw new Exception("Game is under maintenance.");
            }
            catch (IndexOutOfRangeException)
            {
                throw new Exception("Validation failed. Maybe your IP/Region is blocked?");
            }

            return gameStartResponse;
        }

        internal static void StartReactorToUpdate()
        {
            using (var client = new MyWebClient())
            {
                Methods.HangameLogin(client);
                string gameStartResponse = Methods.GetGameStartResponse(client);

                PubPluginClass pubPluginClass = new PubPluginClass();
                IPubPlugin pubPlugin = null;
                try
                {
                    pubPlugin = (IPubPlugin)pubPluginClass;
                }
                catch (InvalidCastException)
                {
                    throw new Exception("Run the game from the website first to install the plugin and reactor!");
                }
                if (pubPlugin.IsReactorInstalled() == 1)
                    try
                    {
                        pubPlugin.StartReactor(Methods.GetVariableValue(gameStartResponse, Strings.Web.ReactorStr)[0]);
                        throw new Exception("Update the game client using the game launcher.\nWhen it finished, close it and try 'Ready to Play' again.");
                    }
                    catch (IndexOutOfRangeException)
                    {
                        throw new Exception("Validation failed. Maybe your IP/Region is blocked?");
                    }
                else
                    throw new Exception("Run the game from the website first to install the plugin and reactor!");
            }
        }

        internal static string[] GetGameStartArguments(MyWebClient client)
        {
            try
            {
                client.UploadData(Urls.SoulworkerRegistCheck, new byte[] { });
            }
            catch (WebException webEx)
            {
                var responseError = webEx.Response as HttpWebResponse;
                if (responseError.StatusCode == HttpStatusCode.NotFound)
                    throw new WebException("Validation failed. Maybe your IP/Region is blocked?", webEx);
                else
                    throw;
            }

            var reactorStartResponse = client.UploadData(Urls.SoulworkerReactorGameStart, new byte[] { });
            IniFile ini = new IniFile();
            ini.Load(Path.Combine(UserSettings.GamePath, Strings.IniName.GeneralFile));

            string[] gameStartArgs = new string[3];
            gameStartArgs[0] = Methods.GetVariableValue(Encoding.Default.GetString(reactorStartResponse), Strings.Web.GameStartArg)[0];
            gameStartArgs[1] = ini.Sections[Strings.IniName.General.SectionNetwork].Keys[Strings.IniName.General.KeyIP].Value;
            gameStartArgs[2] = ini.Sections[Strings.IniName.General.SectionNetwork].Keys[Strings.IniName.General.KeyPort].Value;

            return gameStartArgs;
        }

        internal static void BackupAndPlaceDataFiles(List<SWFile> swfiles, Language language)
        {
            var archives = swfiles.Where(f => !String.IsNullOrEmpty(f.PathA)).Select(f => f.Path).Distinct();
            foreach (var archive in archives)
            {
                string archivePath = Path.Combine(UserSettings.GamePath, archive);
                string backupFilePath = Path.Combine(Strings.FolderName.Backup, archive);
                string backupFileDirectory = Path.GetDirectoryName(backupFilePath);

                if (!Directory.Exists(backupFileDirectory))
                    Directory.CreateDirectory(backupFileDirectory);

                File.Move(archivePath, backupFilePath);
                File.Move(Path.Combine(language.Lang, archive), archivePath);
            }
        }

        internal static void BackupAndPlaceOtherFiles(List<SWFile> swfiles, Language language)
        {
            var swFiles = swfiles.Where(f => String.IsNullOrEmpty(f.PathA));
            foreach (var swFile in swFiles)
            {
                string swFileName = Path.Combine(swFile.Path, Path.GetFileName(swFile.PathD));
                string swFilePath = Path.Combine(language.Lang, swFileName);
                string filePath = Path.Combine(UserSettings.GamePath, swFileName);
                string backupFilePath = Path.Combine(Strings.FolderName.Backup, swFileName);
                string backupFileDirectory = Path.GetDirectoryName(backupFilePath);

                if (!Directory.Exists(backupFileDirectory))
                    Directory.CreateDirectory(backupFileDirectory);

                File.Move(filePath, backupFilePath);
                File.Move(swFilePath, filePath);
            }
        }
    }
}
