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

using Ionic.Zip;
using MadMilkman.Ini;
using SWPatcher.General;
using SWPatcher.Helpers.GlobalVariables;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace SWPatcher.Helpers
{
    internal static class Methods
    {
        private static string DateFormat = "dd/MMM/yyyy h:mm tt";
        private static byte[] Entropy = Encoding.Unicode.GetBytes("C11699FC9EC2502027E0222999DA029D01DE3026");

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

            IniSection section = ini.Sections[Strings.IniName.Patcher.Section];
            if (!section.Keys.Contains(Strings.IniName.Pack.KeyDate))
                return true;

            string date = section.Keys[Strings.IniName.Pack.KeyDate].Value;

            return language.LastUpdate > ParseDate(date);
        }

        internal static bool IsSwPath(string path)
        {
            bool f1 = Directory.Exists(path);
            string dataPath = Path.Combine(path, Strings.FolderName.Data);
            bool f2 = Directory.Exists(dataPath);
            bool f3 = File.Exists(Path.Combine(path, Strings.FileName.GameExe));
            bool f4 = File.Exists(Path.Combine(path, Strings.IniName.ClientVer));
            bool f5 = File.Exists(Path.Combine(dataPath, Strings.FileName.Data12));
            bool f6 = File.Exists(Path.Combine(dataPath, Strings.FileName.Data14));

            return f1 && f2 && f3 && f4 && f5 && f6;
        }

        internal static bool IsValidSwPatcherPath(string path)
        {
            return String.IsNullOrEmpty(path) || !IsSwPath(path) && IsValidSwPatcherPath(Path.GetDirectoryName(path));
        }

        internal static IniFile GetServerIni()
        {
            using (var client = new WebClient())
            using (var zippedFile = new TempFile())
            {
                try
                {
                    client.DownloadFile(Urls.SoulworkerSettingsHome + Strings.IniName.ServerVer + ".zip", zippedFile.Path);
                }
                catch (WebException e)
                {
                    if (e.InnerException is SocketException)
                    {
                        var innerException = e.InnerException as SocketException;
                        if (innerException.SocketErrorCode == SocketError.ConnectionRefused)
                        {
                            Logger.Error(e);
                            MsgBox.Error(StringLoader.GetText("exception_hangame_refused_connection"));
                        }
                    }
                }

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

                    return ini;
                }
            }
        }

        internal static void PatchExeFile(string gameExePath)
        {
            Logger.Debug(Methods.MethodFullName(MethodBase.GetCurrentMethod(), gameExePath));

            using (var client = new WebClient())
            using (var file = new TempFile())
            {
                byte[] exeBytes = File.ReadAllBytes(gameExePath);
                string hexResult = BitConverter.ToString(exeBytes).Replace("-", "");

                client.DownloadFile(Urls.PatcherGitHubHome + Strings.IniName.BytesToPatch, file.Path);
                IniFile ini = new IniFile();
                ini.Load(file.Path);

                foreach (IniSection section in ini.Sections)
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

                foreach (IniSection section in ini.Sections)
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

        internal static string VersionToRTP(Version version)
        {
            return $"{version.Major}_{version.Minor}_{version.Build}_{version.Revision}.RTP";
        }

        internal static void RTPatchCleanup()
        {
            string[] filters = { "RT*", "*.RTP" };
            foreach (var filter in filters)
                foreach (var file in Directory.GetFiles(UserSettings.GamePath, filter, SearchOption.AllDirectories))
                {
                    Logger.Info($"Deleting file=[{file}]");
                    File.Delete(file);
                }
        }

        internal static void DoUnzipFile(string zipPath, string fileName, string extractDestination, string password)
        {
            using (var zip = ZipFile.Read(zipPath))
            {
                zip.Password = password;
                zip.FlattenFoldersOnExtract = true;
                zip[fileName].Extract(extractDestination, ExtractExistingFileAction.OverwriteSilently);
            }
        }

        internal static void DoZipFile(string zipPath, string fileName, string filePath, string password)
        {
            using (var zip = ZipFile.Read(zipPath))
            {
                zip.Password = password;
                zip.RemoveEntry(fileName);
                zip.AddFile(filePath, Path.GetDirectoryName(fileName));
                zip.Save();
            }
        }

        internal static void AddZipToZip(string zipPath, string destinationZipPath, string directoryInDestination, string password)
        {
            using (var zip = ZipFile.Read(zipPath))
            using (var destinationZip = ZipFile.Read(destinationZipPath))
            {
                zip.Password = password;
                var tempFileList = zip.Entries.Select(entry => new TempFile(Path.Combine(Path.GetTempPath(), Path.GetFileName(entry.FileName)))).ToList();
                zip.FlattenFoldersOnExtract = true;

                zip.ExtractAll(Path.GetTempPath(), ExtractExistingFileAction.OverwriteSilently);

                destinationZip.RemoveEntries(zip.Entries.Select(e => Path.Combine(directoryInDestination, e.FileName)).ToList());
                destinationZip.AddFiles(tempFileList.Select(tf => tf.Path), directoryInDestination);
                destinationZip.Save();

                tempFileList.ForEach(tf => tf.Dispose());
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
                throw new Exception(StringLoader.GetText("exception_read_translation_ini"));

            Version translationVer = new Version(ini.Sections[Strings.IniName.Patcher.Section].Keys[Strings.IniName.Patcher.KeyVer].Value);
            ini.Sections.Clear();
            ini.Load(Path.Combine(UserSettings.GamePath, Strings.IniName.ClientVer));

            Version clientVer = new Version(ini.Sections[Strings.IniName.Ver.Section].Keys[Strings.IniName.Ver.Key].Value);
            if (clientVer > translationVer)
                return true;

            return false;
        }

        internal static string EncryptString(SecureString input)
        {
            byte[] encryptedData = ProtectedData.Protect(
                Encoding.Unicode.GetBytes(ToInsecureString(input)),
                Entropy,
                DataProtectionScope.CurrentUser);

            return Convert.ToBase64String(encryptedData);
        }

        internal static SecureString DecryptString(string encryptedData)
        {
            try
            {
                byte[] decryptedData = ProtectedData.Unprotect(
                    Convert.FromBase64String(encryptedData),
                    Entropy,
                    DataProtectionScope.CurrentUser);

                return ToSecureString(Encoding.Unicode.GetString(decryptedData));
            }
            catch
            {
                return new SecureString();
            }
        }

        internal static SecureString ToSecureString(string input)
        {
            SecureString secure = new SecureString();
            foreach (char c in input)
            {
                secure.AppendChar(c);
            }
            secure.MakeReadOnly();

            return secure;
        }

        internal static string ToInsecureString(SecureString input)
        {
            string returnValue = string.Empty;
            IntPtr ptr = Marshal.SecureStringToBSTR(input);
            try
            {
                returnValue = Marshal.PtrToStringBSTR(ptr);
            }
            finally
            {
                Marshal.ZeroFreeBSTR(ptr);
            }

            return returnValue;
        }

        internal static bool In<T>(this T obj, params T[] values)
        {
            return values.Contains(obj);
        }

        internal static string MethodName(MethodBase method)
        {
            return $"{method.ReflectedType.FullName}.{method.Name}";
        }

        internal static string MethodParams(params string[] args)
        {
            return $"{(String.Join(", ", args))}";
        }

        internal static string MethodFullName(MethodBase method, params string[] args)
        {
            return $"{MethodName(method)}({MethodParams(args)})";
        }

        internal static string MethodFullName(string method, params string[] args)
        {
            return $"{method}({MethodParams(args)})";
        }

        internal static void CheckRunningPrograms()
        {
            string[] processes = Methods.GetRunningGameProcesses();
            if (processes.Length > 0)
                throw new Exception(String.Format(StringLoader.GetText("exception_game_already_open"), String.Join("/", processes)));
        }

        internal static string[] GetRunningGameProcesses()
        {
            string[] processNames = new[] { Strings.FileName.GameExe, Strings.FileName.PurpleExe, Strings.FileName.ReactorExe, Strings.FileName.OutboundExe };

            return processNames.SelectMany(pn => Process.GetProcessesByName(Path.GetFileNameWithoutExtension(pn))).Select(p => Path.GetFileName(Methods.GetProcessPath(p.Id))).Where(pn => processNames.Contains(pn)).ToArray();
        }

        private static string GetProcessPath(int processId)
        {
            var buffer = new StringBuilder(1024);
            IntPtr hprocess = NativeMethods.OpenProcess(NativeMethods.ProcessAccessFlags.QueryLimitedInformation, false, processId);
            if (hprocess != IntPtr.Zero)
            {
                try
                {
                    int size = buffer.Capacity;
                    if (NativeMethods.QueryFullProcessImageName(hprocess, 0, buffer, out size))
                    {
                        return buffer.ToString();
                    }
                }
                finally
                {
                    NativeMethods.CloseHandle(hprocess);
                }
            }
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }
    }
}
