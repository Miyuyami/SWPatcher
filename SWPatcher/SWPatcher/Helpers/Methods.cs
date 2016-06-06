using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Ionic.Zip;
using MadMilkman.Ini;
using SWPatcher.General;
using SWPatcher.Helpers.GlobalVar;

namespace SWPatcher.Helpers
{
    public static class Methods
    {
        private static string DateFormat = "dd/MMM/yyyy h:mm tt";

        public static DateTime ParseDate(string date)
        {
            return DateTime.ParseExact(date, DateFormat, CultureInfo.InvariantCulture);
        }

        public static string DateToString(DateTime date)
        {
            return date.ToString(DateFormat, CultureInfo.InvariantCulture);
        }

        public static bool HasNewTranslations(Language language)
        {
            string directory = Path.Combine(Paths.PatcherRoot, language.Lang);

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
            if (language.LastUpdate > Methods.ParseDate(date))
                return true;

            return false;
        }

        public static bool IsSwPath(string path)
        {
            return Directory.Exists(path) && Directory.Exists(Path.Combine(path, Strings.FolderName.Data)) && File.Exists(Path.Combine(path, Strings.FileName.GameExe)) && File.Exists(Path.Combine(path, Strings.IniName.ClientVer));
        }

        public static void RestoreBackup()
        {
            if (Directory.Exists(Strings.FolderName.Backup))
            {
                string[] filePaths = Directory.GetFiles(Strings.FolderName.Backup, "*", SearchOption.AllDirectories);

                if (!String.IsNullOrEmpty(Paths.GameRoot) && Methods.IsSwPath(Paths.GameRoot))
                {
                    foreach (var s in filePaths)
                    {
                        string path = Path.Combine(Paths.GameRoot, s.Substring(Strings.FolderName.Backup.Length + 1));

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

        public static void RestoreBackup(Language language)
        {
            if (!Directory.Exists(Strings.FolderName.Backup))
                return;

            string[] filePaths = Directory.GetFiles(Strings.FolderName.Backup, "*", SearchOption.AllDirectories);

            foreach (var file in filePaths)
            {
                string path = Path.Combine(Paths.GameRoot, file.Substring(Strings.FolderName.Backup.Length + 1));
                File.Move(path, Path.Combine(Paths.PatcherRoot, language.Lang, path.Substring(Paths.GameRoot.Length + 1)));
                File.Move(file, path);
            }
        }

        public static void DeleteTmpFiles(Language language)
        {
            string[] tmpFilePaths = Directory.GetFiles(Path.Combine(Paths.PatcherRoot, language.Lang), "*.tmp", SearchOption.AllDirectories);

            foreach (var tmpFile in tmpFilePaths)
                File.Delete(tmpFile);
        }

        public static bool IsValidSwPatcherPath(string path)
        {
            return String.IsNullOrEmpty(path) || !Methods.IsSwPath(path) && Methods.IsValidSwPatcherPath(Path.GetDirectoryName(path));
        }

        public static string GetSwPathFromRegistry()
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

        public static Language[] GetAvailableLanguages()
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

        public static void DeleteTranslationIni(Language language)
        {
            string iniPath = Path.Combine(Paths.PatcherRoot, language.Lang, Strings.IniName.Translation);
            if (Directory.Exists(Path.GetDirectoryName(iniPath)))
                File.Delete(iniPath);
        }

        public static string GetArchivedSWFilePath(SWFile swFile, Language language)
        {
            string directory = Path.GetDirectoryName(Path.Combine(Paths.PatcherRoot, language.Lang, swFile.Path));
            string archiveName = Path.GetFileNameWithoutExtension(swFile.Path);
            string fileName = Path.GetFileName(swFile.PathD);

            return Path.Combine(directory, archiveName, fileName);
        }

        public static string GetMD5(string text)
        {
            var md5 = new MD5CryptoServiceProvider();
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));
            byte[] result = md5.Hash;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
                sb.Append(result[i].ToString("x2"));

            return sb.ToString();
        }

        public static void DoUnzipFile(string zipPath, string fileName, string extractDestination)
        {
            using (var zip = ZipFile.Read(zipPath))
            {
                zip.FlattenFoldersOnExtract = true;
                zip[fileName].Extract(extractDestination, ExtractExistingFileAction.OverwriteSilently);
            }
        }

        public static void DoZipFile(string zipPath, string fileName, string filePath)
        {
            using (var zip = ZipFile.Read(zipPath))
            {
                zip.RemoveEntry(fileName);
                zip.AddFile(filePath, Path.GetDirectoryName(fileName));
                zip.Save();
            }
        }

        public static void AddZipToZip(string zipPath, string destinationZipPath, string directoryInDestination)
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

        public static bool IsNewerGameClientVersion()
        {
            IniFile ini = new IniFile();
            ini.Load(Path.Combine(Paths.GameRoot, Strings.IniName.ClientVer));

            return VersionCompare(GetServerVersion(), ini.Sections[Strings.IniName.Ver.Section].Keys[Strings.IniName.Ver.Key].Value);
        }

        public static bool VersionCompare(string ver1, string ver2)
        {
            Version v1 = new Version(ver1);
            Version v2 = new Version(ver2);

            return v1 > v2;
        }

        public static string GetServerVersion()
        {
            using (var client = new WebClient())
            using (var zippedFile = new TempFile())
            {
                client.DownloadFile(Urls.SoulWorkerSettingsHome + Strings.IniName.ServerVer + ".zip", zippedFile.Path);

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

        public static bool IsTranslationOutdated(Language language)
        {
            string selectedTranslationPath = Path.Combine(Paths.PatcherRoot, language.Lang, Strings.IniName.Translation);
            if (!File.Exists(selectedTranslationPath))
                return true;

            IniFile ini = new IniFile();
            ini.Load(selectedTranslationPath);

            if (!ini.Sections[Strings.IniName.Patcher.Section].Keys.Contains(Strings.IniName.Patcher.KeyVer))
                throw new Exception("Error reading translation version, try to Menu -> Force Patch");

            string translationVer = ini.Sections[Strings.IniName.Patcher.Section].Keys[Strings.IniName.Patcher.KeyVer].Value;
            ini.Sections.Clear();
            ini.Load(Path.Combine(Paths.GameRoot, Strings.IniName.ClientVer));

            string clientVer = ini.Sections[Strings.IniName.Ver.Section].Keys[Strings.IniName.Ver.Key].Value;
            if (VersionCompare(clientVer, translationVer))
                return true;

            return false;
        }

        public static Process GetProcess(string name)
        {
            Process[] processesByName = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(name));

            if (processesByName.Length > 0)
                return processesByName[0];

            return null;
        }
    }
}
