using System;
using System.Diagnostics;
using System.Net;
using System.Windows.Forms;
using SWPatcher.General;
using SWPatcher.Helpers;
using SWPatcher.Helpers.GlobalVars;
using System.IO;
using Ionic.Zip;
using System.Collections.Generic;

namespace SWPatcher.Forms
{
    partial class Main
    {
        private bool CheckForProgramUpdate()
        {
            try
            {
                return CheckProgramUpdate();
            }
            catch (WebException)
            {
                DialogResult result = MsgBox.ErrorRetry("Could not connect to download server.\nTry again later.");
                if (result == DialogResult.Retry)
                    return CheckForProgramUpdate();
                else
                {
                    MsgBox.Error("The program cannot run without an internet connection and will now close.");
                    this.Close();
                }
                return true;
            }
        }

        private bool CheckProgramUpdate()
        {
            using (var client = new WebClient())
            using (var file = new TempFile())
            {
                client.DownloadFile(Uris.PatcherGitHubHome + Strings.IniName.PatcherVersion, file.Path);
                IniReader ini = new IniReader(file.Path);
                Version current = new Version(AssemblyAccessor.Version);
                Version read = new Version(ini.ReadString(Strings.IniName.Patcher.Section, Strings.IniName.Patcher.KeyVer));
                if (current.CompareTo(read) < 0)
                {
                    string address = ini.ReadString(Strings.IniName.Patcher.Section, Strings.IniName.Patcher.KeyAddress);
                    DialogResult newVersionDialog = MsgBox.Question("There is a new patcher version available!\n\nYes - Application will close and redirect you to the patcher website.\nNo - Ignore");
                    if (newVersionDialog == DialogResult.Yes)
                    {
                        Process.Start(address);
                        this.Close();
                        return true;
                    }
                    else
                    {
                        DialogResult newVersionDialog2 = MsgBox.Question("Are you sure you want to ignore the update?\nIt might cause unknown problems!");
                        if (newVersionDialog2 == DialogResult.No)
                        {
                            Process.Start(address);
                            this.Close();
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool CheckForProgramFolderMalfunction(string path)
        {
            while (!string.IsNullOrEmpty(path))
            {
                if (IsSWPath(path))
                {
                    MsgBox.Error("The program is in the same or in a sub folder as your game client.\nThis will cause malfunctions or data corruption on your game client.\nPlease move it in another location.");
                    this.Close();
                    return true;
                }
                path = Path.GetDirectoryName(path);
            }
            return false;
        }

        private bool CheckForSWPath()
        {
            if (string.IsNullOrEmpty(Paths.GameRoot))
                return SetSWPath();
            if (!IsSWPath(Paths.GameRoot))
            {
                MsgBox.Error("The saved Soul Worker game client folder is invalid.");
                return SetSWPath();
            }
            return false;
        }

        private bool SetSWPath()
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.ShowNewFolderButton = false;
                folderDialog.Description = "Select your Soul Worker game client folder";
                DialogResult result = folderDialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    if (IsSWPath(folderDialog.SelectedPath))
                    {
                        Paths.GameRoot = folderDialog.SelectedPath;
                        return false;
                    }
                    else
                    {
                        MsgBox.Error("The selected folder is not a Soul Worker game client folder");
                        return SetSWPath();
                    }
                }
                else
                {
                    MsgBox.Error("The patcher cannot run without a valid game client folder.");
                    this.Close();
                    return true;
                }
            }
        }

        private bool CheckForGameClientUpdate()
        {
            if (IsNewerGameClientVersion())
            {
                DialogResult result = MsgBox.ErrorRetry("Your game client is not updated to the latest version.\nRun the game client launcher first and retry.");
                if (result == DialogResult.Cancel)
                {
                    MsgBox.Error("Patching old files will lead to certain data corruption.\nThe program will now close.");
                    this.Close();
                    return true;
                }
                return CheckForGameClientUpdate();
            }
            return false;
        }

        private bool IsNewerGameClientVersion()
        {
            try
            {
                Version serverVersion = GetServerVersion();
                IniReader clientIni = new IniReader(Path.Combine(Paths.GameRoot, Strings.IniName.ClientVer));
                Version clientVersion = new Version(clientIni.ReadString(Strings.IniName.Ver.Section, Strings.IniName.Ver.Key));
                return serverVersion > clientVersion;
            }
            catch (WebException)
            {
                DialogResult result = MsgBox.ErrorRetry("Could not connect to Hangame server.\nTry again later.");
                if (result == DialogResult.Cancel)
                {
                    MsgBox.Error("The program cannot run without an internet connection and will now close.");
                    this.Close();
                    return false;
                }
                return IsNewerGameClientVersion();
            }
        }

        private Version GetServerVersion()
        {
            using (var client = new WebClient())
            using (var zippedFile = new TempFile())
            {
                client.DownloadFile(Uris.SoulWorkerSettingsHome + Strings.IniName.ServerVer + Strings.FileExtentionName.Zip, zippedFile.Path);
                using (var file = new TempFile())
                {
                    using (ZipFile zip = ZipFile.Read(zippedFile.Path))
                    {
                        ZipEntry entry = zip[0];
                        entry.FileName = Path.GetFileName(file.Path);
                        entry.Extract(Path.GetDirectoryName(file.Path), ExtractExistingFileAction.OverwriteSilently);
                    }
                    IniReader serverIni = new IniReader(file.Path);
                    return new Version(serverIni.ReadString(Strings.IniName.Ver.Section, Strings.IniName.Ver.Key));
                }
            }
        }

        private bool PopulateList()
        {
            this.FileList = new List<SWFile>();
            try
            {
                bool flag1 = AddTranslationPackDataToList(this.FileList);
                bool flag2 = AddOtherTranslationPackDataToList(this.FileList);
                return flag1 || flag2;
            }
            catch (WebException)
            {
                DialogResult result = MsgBox.ErrorRetry("Could not connect to download server.\nTry again later.");
                if (result == DialogResult.Retry)
                    return PopulateList();
                else
                {
                    MsgBox.Error("The program cannot run without an internet connection and will now close.");
                    this.Close();
                }
                return true;
            }
        }

        private bool AddTranslationPackDataToList(List<SWFile> list)
        {
            throw new NotImplementedException();
        }

        private bool AddOtherTranslationPackDataToList(List<SWFile> list)
        {
            throw new NotImplementedException();
        }

        private bool IsSWPath(string path)
        {
            return Directory.Exists(path) && Directory.Exists(Path.Combine(path, Strings.FolderName.Data)) && File.Exists(Path.Combine(path, Strings.FileName.GameExe)) && File.Exists(Path.Combine(path, Strings.IniName.ClientVer));
        }

        private void RestoreBackup()
        {
            DirectoryInfo backup = new DirectoryInfo(Strings.FolderName.Backup);
            if (backup.Exists)
            {
                FileInfo[] files = backup.GetFiles("*", SearchOption.AllDirectories);
                if (string.IsNullOrEmpty(Paths.GameRoot) && IsSWPath(Paths.GameRoot))
                    foreach (var s in files)
                    {
                        FileInfo 
                    }
                else
                    foreach (var s in filePaths)
                        File.Delete(s.FullName);
            }
            else
                backup.Create();
        }
    }
}
