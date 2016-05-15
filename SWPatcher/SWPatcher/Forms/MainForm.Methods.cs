using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows.Forms;
using Ionic.Zip;
using SWPatcher.General;
using SWPatcher.Helpers;
using SWPatcher.Helpers.GlobalVar;

namespace SWPatcher.Forms
{
    partial class MainForm
    {
        private void CheckForProgramUpdate()
        {
            try
            {
                CheckProgramUpdate();
            }
            catch (WebException)
            {
                DialogResult result = MsgBox.ErrorRetry("Could not connect to download server.\nTry again later.");
                if (result == DialogResult.Retry)
                    CheckForProgramUpdate();
                else
                {
                    MsgBox.Error("The program cannot run without an internet connection and will now close.");
                    throw new Exception("0x0000001 - Connection to download server failed");
                }
            }
            catch
            {
                throw;
            }
        }

        private void CheckProgramUpdate()
        {
            using (var client = new WebClient())
            using (var file = new TempFile())
            {
                client.DownloadFile(Uris.PatcherGitHubHome + Strings.IniName.PatcherVersion, file.Path);
                IniReader ini = new IniReader(file.Path);
                Version current = new Version(AssemblyAccessor.Version);
                Version read = new Version(ini.ReadString(Strings.IniName.Patcher.Section, Strings.IniName.Patcher.KeyVer, "0.0.0.0"));
                if (current.CompareTo(read) < 0)
                {
                    string address = ini.ReadString(Strings.IniName.Patcher.Section, Strings.IniName.Patcher.KeyAddress);
                    DialogResult newVersionDialog = MsgBox.Question("There is a new patcher version available!\n\nYes - Application will close and redirect you to the patcher website.\nNo - Ignore");
                    if (newVersionDialog == DialogResult.Yes)
                    {
                        Process.Start(address);
                        throw new Exception("0x0000000 - Closing app to update");
                    }
                    else
                    {
                        DialogResult newVersionDialog2 = MsgBox.Question("Are you sure you want to ignore the update?\nIt might cause unknown problems!");
                        if (newVersionDialog2 == DialogResult.No)
                        {
                            Process.Start(address);
                            throw new Exception("0x0000000 - Closing app to update");
                        }
                    }
                }
            }
        }

        private void CheckForProgramFolderMalfunction(string path)
        {
            while (!string.IsNullOrEmpty(path))
            {
                if (IsSWPath(path))
                {
                    MsgBox.Error("The program is in the same or in a sub folder as your game client.\nThis will cause malfunctions or data corruption on your game client.\nPlease move it in another location.");
                    throw new Exception("0x00000002 - Illegal patcher path");
                }
                path = Path.GetDirectoryName(path);
            }
        }

        private void CheckForSWPath()
        {
            if (string.IsNullOrEmpty(Paths.GameRoot))
                SetSWPath();
            if (!IsSWPath(Paths.GameRoot))
            {
                MsgBox.Error("The saved Soul Worker game client folder is invalid.");
                SetSWPath();
            }
        }

        private void SetSWPath()
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                using (var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\HanPurple\J_SW"))
                {
                    if (key != null)
                        folderDialog.SelectedPath = Convert.ToString(key.GetValue("folder", ""));
                    else
                    {
                        Error.Log("0x0000010 - WOW6432Node - Key not found");
                        using (var key32 = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\HanPurple\J_SW"))
                        {
                            if (key32 != null)
                                folderDialog.SelectedPath = Convert.ToString(key32.GetValue("folder", ""));
                            else
                                Error.Log("0x0000020 - StandardNode - Key not found");
                        }
                    }
                }
                folderDialog.ShowNewFolderButton = false;
                folderDialog.Description = "Select your Soul Worker game client folder.";
                DialogResult result = folderDialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    if (IsSWPath(folderDialog.SelectedPath))
                        Paths.GameRoot = folderDialog.SelectedPath;
                    else
                    {
                        MsgBox.Error("The selected folder is not a Soul Worker game client folder.");
                        SetSWPath();
                    }
                }
                else
                {
                    MsgBox.Error("You cannot run the patcher without selecting a valid game client folder.");
                    throw new Exception("0x0000012 - No valid path selected");
                }
            }
        }

        private void CheckForGameClientUpdate()
        {
            if (IsNewerGameClientVersion())
            {
                DialogResult result = MsgBox.ErrorRetry("Your game client is not updated to the latest version.\nRun the game client launcher first and retry.");
                if (result == DialogResult.Retry)
                    CheckForGameClientUpdate();
                else
                {
                    MsgBox.Error("Patching old files will lead to certain data corruption.\nThe program will now close.");
                    throw new Exception("0x0000003 - Old translation files");
                }
            }
        }

        private bool IsNewerGameClientVersion()
        {
            try
            {
                IniReader clientIni = new IniReader(Path.Combine(Paths.GameRoot, Strings.IniName.ClientVer));
                return VersionCompare(GetServerVersion(), clientIni.ReadString(Strings.IniName.Ver.Section, Strings.IniName.Ver.Key, "0.0.0.0"));
            }
            catch (WebException)
            {
                DialogResult result = MsgBox.ErrorRetry("Could not connect to Hangame server.\nTry again later.");
                if (result == DialogResult.Retry)
                    return IsNewerGameClientVersion();
                else
                {
                    MsgBox.Error("The program cannot run without an internet connection and will now close.");
                    throw new Exception("0x0000011 - Connection to Hangame server failed");
                }
            }
        }

        private static bool VersionCompare(string ver1, string ver2)
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
                client.DownloadFile(Uris.SoulWorkerSettingsHome + Strings.IniName.ServerVer + ".zip", zippedFile.Path);
                using (var file = new TempFile())
                {
                    using (ZipFile zip = ZipFile.Read(zippedFile.Path))
                    {
                        ZipEntry entry = zip[0];
                        entry.FileName = Path.GetFileName(file.Path);
                        entry.Extract(Path.GetDirectoryName(file.Path), ExtractExistingFileAction.OverwriteSilently);
                    }
                    IniReader serverIni = new IniReader(file.Path);
                    return serverIni.ReadString(Strings.IniName.Ver.Section, Strings.IniName.Ver.Key);
                }
            }
        }

        private static bool IsSWPath(string path)
        {
            return Directory.Exists(path) && Directory.Exists(Path.Combine(path, Strings.FolderName.Data)) && File.Exists(Path.Combine(path, Strings.FileName.GameExe)) && File.Exists(Path.Combine(path, Strings.IniName.ClientVer));
        }

        private static void RestoreBackup()
        {
            if (Directory.Exists(Strings.FolderName.Backup))
            {
                string[] filePaths = Directory.GetFiles(Strings.FolderName.Backup, "*", SearchOption.AllDirectories);
                if (!string.IsNullOrEmpty(Paths.GameRoot) && IsSWPath(Paths.GameRoot))
                    foreach (var s in filePaths)
                    {
                        string path = Path.Combine(Paths.GameRoot, s.Substring(Strings.FolderName.Backup.Length + 1));
                        if (File.Exists(path))
                            File.Delete(path);
                        File.Move(s, path);
                    }
                else
                    foreach (var s in filePaths)
                        File.Delete(s);
            }
            else
                Directory.CreateDirectory(Strings.FolderName.Backup);
        }

        private static void RestoreBackup(Language language)
        {
            string[] filePaths = Directory.GetFiles(Strings.FolderName.Backup, "*.v", SearchOption.AllDirectories);
            foreach (var s in filePaths)
            {
                string path = Path.Combine(Paths.GameRoot, s.Substring(Strings.FolderName.Backup.Length + 1));
                File.Move(path, Path.Combine(Paths.PatcherRoot, language.Lang, path.Substring(Paths.GameRoot.Length + 1)));
                File.Move(s, path);
            }
        }

        private Language[] GetAllAvailableLanguages()
        {
            try
            {
                return GetAvailableLanguages();
            }
            catch (WebException)
            {
                DialogResult result = MsgBox.ErrorRetry("Could not connect to download server.\nTry again later.");
                if (result == DialogResult.Retry)
                    return GetAllAvailableLanguages();
                else
                {
                    MsgBox.Error("The program cannot run without an internet connection and will now close.");
                    throw new Exception("0x0000001 - Connection to download server failed");
                }
            }
        }

        private static Language[] GetAvailableLanguages()
        {
            List<Language> langs = new List<Language>();
            using (var client = new WebClient())
            using (var file = new TempFile())
            {
                client.DownloadFile(Uris.PatcherGitHubHome + Strings.IniName.LanguagePack, file.Path);
                IniReader langIni = new IniReader(file.Path);
                foreach (var s in langIni.GetSectionNames())
                    langs.Add(new Language(s.ToString(), Strings.ParseDate(langIni.ReadString(s.ToString(), Strings.IniName.Pack.KeyDate, Strings.DateToString(DateTime.MinValue)))));
            }
            return langs.ToArray();
        }

        private bool IsTranslationOutdated(Language language)
        {
            string selectedTranslationPath = Path.Combine(Paths.PatcherRoot, language.Lang, Strings.IniName.Translation);
            if (!File.Exists(selectedTranslationPath))
                return true;
            IniReader translationIni = new IniReader(selectedTranslationPath);
            string translationVer = translationIni.ReadString(Strings.IniName.Patcher.Section, Strings.IniName.Patcher.KeyVer);
            bool isEmpty = string.IsNullOrEmpty(translationVer);
            if (isEmpty)
                throw new Exception("0x0000004 - Error reading translation version: " + (isEmpty ? "try to force patch" : translationVer));
            IniReader clientIni = new IniReader(Path.Combine(Paths.GameRoot, Strings.IniName.ClientVer));
            string clientVer = clientIni.ReadString(Strings.IniName.Ver.Section, Strings.IniName.Ver.Key);
            if (VersionCompare(clientVer, translationVer))
                return true;
            return false;
        }

        private Process GetProcess(string name)
        {
            Process[] processesByName = Process.GetProcessesByName(name);
            if (processesByName.Length == 0)
                return null;
            foreach (Process p in processesByName)
                return p;
            return null;
        }

        private static bool HasNewTranslations(Language language)
        {
            string directory = Path.Combine(Paths.PatcherRoot, language.Lang);
            if (Directory.Exists(directory))
            {
                string filePath = Path.Combine(directory, Strings.IniName.Translation);
                if (File.Exists(filePath))
                {
                    IniReader translationIni = new IniReader(filePath);
                    string date = translationIni.ReadString(Strings.IniName.Patcher.Section, Strings.IniName.Pack.KeyDate, Strings.DateToString(DateTime.MinValue));
                    if (language.LastUpdate > Strings.ParseDate(date))
                        return true;
                }
                else
                    return true;
            }
            else
                return true;
            return false;
        }
    }
}
