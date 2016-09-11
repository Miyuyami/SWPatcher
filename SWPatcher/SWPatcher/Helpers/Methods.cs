using Ionic.Zip;
using MadMilkman.Ini;
using SWPatcher.Downloading.GlobalVar;
using SWPatcher.General;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;

namespace SWPatcher.Downloading
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

            return language.LastUpdate > ParseDate(date);
        }

        internal static bool IsSwPath(string path)
        {
            return Directory.Exists(path) && Directory.Exists(Path.Combine(path, Strings.FolderName.Data)) && File.Exists(Path.Combine(path, Strings.FileName.GameExe)) && File.Exists(Path.Combine(path, Strings.IniName.ClientVer));
        }

        internal static bool IsValidSwPatcherPath(string path)
        {
            return String.IsNullOrEmpty(path) || !IsSwPath(path) && IsValidSwPatcherPath(Path.GetDirectoryName(path));
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

        private static string GetServerVersion()
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
                        Encoding = Encoding.Unicode
                    });
                    ini.Load(file.Path);

                    return ini.Sections[Strings.IniName.Ver.Section].Keys[Strings.IniName.Ver.Key].Value;
                }
            }
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
    }
}
