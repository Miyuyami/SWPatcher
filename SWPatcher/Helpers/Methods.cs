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
using SWPatcher.Patching;
using System;
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
            string directory = language.Name;

            if (!Directory.Exists(directory))
            {
                return true;
            }

            string filePath = Path.Combine(directory, Strings.IniName.Translation);
            if (!File.Exists(filePath))
            {
                return true;
            }

            IniFile ini = new IniFile();
            ini.Load(filePath);

            if (!ini.Sections.Contains(Strings.IniName.Patcher.Section))
            {
                return true;
            }

            IniSection section = ini.Sections[Strings.IniName.Patcher.Section];
            if (!section.Keys.Contains(Strings.IniName.Pack.KeyDate))
            {
                return true;
            }

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
            {
                try
                {
                    byte[] zipData = client.DownloadData(Urls.SoulworkerSettingsHome + Strings.IniName.ServerVer + ".zip");

                    IniFile ini = new IniFile(new IniOptions
                    {
                        Encoding = Encoding.Unicode
                    });
                    using (MemoryStream ms = Methods.GetZippedFileStream(zipData, Strings.IniName.ServerVer, null))
                    {
                        ini.Load(ms);
                    }

                    return ini;
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

                return null;
            }
        }

        internal static void PatchExeFile(byte[] exeFileBytes, string gameExePatchedPath)
        {
            Logger.Debug(Methods.MethodFullName(MethodBase.GetCurrentMethod(), exeFileBytes.Length.ToString(), gameExePatchedPath));

            using (var client = new WebClient())
            {
                string hexResult = BitConverter.ToString(exeFileBytes).Replace("-", "");
                string patchedHexResult = String.Copy(hexResult);

                byte[] fileBytes = client.DownloadData(Urls.PatcherGitHubHome + Strings.IniName.BytesToPatch);
                IniFile ini = new IniFile();
                using (var ms = new MemoryStream(fileBytes))
                {
                    ini.Load(ms);
                }

                foreach (IniSection section in ini.Sections)
                {
                    string original = section.Keys[Strings.IniName.PatchBytes.KeyOriginal].Value;
                    string patch = section.Keys[Strings.IniName.PatchBytes.KeyPatch].Value;

                    patchedHexResult = patchedHexResult.Replace(original, patch);

                    if (hexResult == patchedHexResult)
                    {
                        Logger.Info($"Failed .exe patch=[{section.Name}]");
                        MsgBox.Error(".exe patch \"{0}\" was not applied because ");
                    }
                }

                int charCount = hexResult.Length;
                byte[] resultBytes = new byte[charCount / 2];

                for (int i = 0; i < charCount; i += 2)
                {
                    resultBytes[i / 2] = Convert.ToByte(patchedHexResult.Substring(i, 2), 16);
                }

                File.WriteAllBytes(gameExePatchedPath, resultBytes);
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
            {
                foreach (var file in Directory.GetFiles(UserSettings.GamePath, filter, SearchOption.AllDirectories))
                {
                    Logger.Info($"Deleting file=[{file}]");
                    File.Delete(file);
                }
            }
        }

        private static void CopyZipEntryAttributes(ZipEntry zipEntry, ZipEntry zipEntryToCopy)
        {
            zipEntryToCopy.Attributes = zipEntry.Attributes;
            zipEntryToCopy.AccessedTime = zipEntry.AccessedTime;
            zipEntryToCopy.CreationTime = zipEntry.CreationTime;
            zipEntryToCopy.LastModified = zipEntry.LastModified;
            zipEntryToCopy.ModifiedTime = zipEntry.ModifiedTime;
            zipEntryToCopy.CompressionLevel = zipEntry.CompressionLevel;
            zipEntryToCopy.CompressionMethod = zipEntry.CompressionMethod;
            zipEntryToCopy.Encryption = zipEntry.Encryption;
        }

        internal static MemoryStream GetZippedFileStream(byte[] zipData, string fileName, string password)
        {
            var result = new MemoryStream();

            using (var ms = new MemoryStream(zipData))
            using (var zip = ZipFile.Read(ms))
            {
                zip.Password = password;
                zip.FlattenFoldersOnExtract = true;
                zip[fileName].Extract(result);
                result.Position = 0;
            }

            return result;
        }

        internal static MemoryStream GetZippedFileStream(XorMemoryStream stream, string fileName, string password)
        {
            var result = new MemoryStream();
            stream.Position = 0;
            
            using (var zip = ZipFile.Read(stream))
            {
                zip.Password = password;
                zip.FlattenFoldersOnExtract = true;
                zip[fileName].Extract(result);
                result.Position = 0;
            }

            return result;
        }

        internal static byte[] ZipFileStream(byte[] zipData, string fileName, MemoryStream fileStream, string password)
        {
            fileStream.Position = 0;
            using (var ms = new MemoryStream(zipData))
            using (var zip = ZipFile.Read(ms))
            {
                zip.Password = password;

                if (zip.ContainsEntry(fileName))
                {
                    ZipEntry zipEntry = zip[fileName];
                    fileName = zipEntry.FileName;
                    zip.RemoveEntry(fileName);
                    zip.AddEntry(fileName, fileStream);

                    ZipEntry modifiedZipEntry = zip[fileName];
                    CopyZipEntryAttributes(zipEntry, modifiedZipEntry);
                }
                else
                {
                    Logger.Debug($"ZipFileStream does not contain fileName=[{fileName}]");
                }

                var msDst = new MemoryStream();
                zip.Save(msDst);
                return msDst.ToArray();
            }
        }

        internal static XorMemoryStream ZipFileStream(XorMemoryStream stream, string fileName, MemoryStream fileStream, string password)
        {
            stream.Position = 0;
            fileStream.Position = 0;
            using (var zip = ZipFile.Read(stream))
            {
                zip.Password = password;

                if (zip.ContainsEntry(fileName))
                {
                    ZipEntry zipEntry = zip[fileName];
                    fileName = zipEntry.FileName;
                    zip.RemoveEntry(fileName);
                    zip.AddEntry(fileName, fileStream);

                    ZipEntry modifiedZipEntry = zip[fileName];
                    CopyZipEntryAttributes(zipEntry, modifiedZipEntry);
                }
                else
                {
                    Logger.Debug($"ZipFileStream does not contain fileName=[{fileName}]");
                }

                var msDst = new XorMemoryStream(stream.XorByte);
                zip.Save(msDst);
                return msDst;
            }
        }

        internal static byte[] AddZipToZip(byte[] zipData, string directoryInDestination, MemoryStream zipStream, string password)
        {
            zipStream.Position = 0;
            using (var ms = new MemoryStream(zipData))
            using (var zip = ZipFile.Read(ms))
            using (var zipToAdd = ZipFile.Read(zipStream))
            {
                zip.Password = password;

                int zipToAddEntriesCount = zipToAdd.Entries.Count;
                MemoryStream[] msArray = new MemoryStream[zipToAddEntriesCount];
                for (int i = 0; i < zipToAddEntriesCount; i++)
                {
                    ZipEntry zipToAddEntry = zipToAdd[i];
                    MemoryStream zipMemoryStream = msArray[i] = new MemoryStream();
                    string filePathInZip = Path.Combine(directoryInDestination, zipToAddEntry.FileName);

                    if (zip.ContainsEntry(filePathInZip))
                    {
                        ZipEntry zipEntry = zip[filePathInZip];
                        filePathInZip = zipEntry.FileName;
                        zip.RemoveEntry(filePathInZip);
                        zipToAddEntry.Extract(zipMemoryStream);
                        zipMemoryStream.Position = 0;
                        zip.AddEntry(filePathInZip, zipMemoryStream);

                        ZipEntry modifiedZipEntry = zip[filePathInZip];
                        CopyZipEntryAttributes(zipEntry, modifiedZipEntry);
                    }
                    else
                    {
                        Logger.Debug($"AddZipToZip does not contain filePathInZip=[{filePathInZip}]");
                    }
                }

                var msDst = new MemoryStream();
                zip.Save(msDst);
                return msDst.ToArray();
            }
        }

        internal static XorMemoryStream AddZipToZip(XorMemoryStream stream, string directoryInDestination, MemoryStream zipStream, string password)
        {
            stream.Position = 0;
            zipStream.Position = 0;
            using (var zip = ZipFile.Read(stream))
            using (var zipToAdd = ZipFile.Read(zipStream))
            {
                zip.Password = password;

                int zipToAddEntriesCount = zipToAdd.Entries.Count;
                MemoryStream[] msArray = new MemoryStream[zipToAddEntriesCount];
                for (int i = 0; i < zipToAddEntriesCount; i++)
                {
                    ZipEntry zipToAddEntry = zipToAdd[i];
                    MemoryStream zipMemoryStream = msArray[i] = new MemoryStream();
                    string filePathInZip = Path.Combine(directoryInDestination, zipToAddEntry.FileName);

                    if (zip.ContainsEntry(filePathInZip))
                    {
                        ZipEntry zipEntry = zip[filePathInZip];
                        filePathInZip = zipEntry.FileName;
                        zip.RemoveEntry(filePathInZip);
                        zipToAddEntry.Extract(zipMemoryStream);
                        zipMemoryStream.Position = 0;
                        zip.AddEntry(filePathInZip, zipMemoryStream);

                        ZipEntry modifiedZipEntry = zip[filePathInZip];
                        CopyZipEntryAttributes(zipEntry, modifiedZipEntry);
                    }
                    else
                    {
                        Logger.Debug($"AddZipToZip does not contain filePathInZip=[{filePathInZip}]");
                    }
                }

                var msDst = new XorMemoryStream(stream.XorByte);
                zip.Save(msDst);
                return msDst;
            }
        }

        internal static bool IsTranslationOutdated(Language language)
        {
            string selectedTranslationPath = Path.Combine(language.Name, Strings.IniName.Translation);
            if (!File.Exists(selectedTranslationPath))
            {
                return true;
            }

            IniFile ini = new IniFile();
            ini.Load(selectedTranslationPath);

            if (!ini.Sections[Strings.IniName.Patcher.Section].Keys.Contains(Strings.IniName.Patcher.KeyVer))
            {
                throw new Exception(StringLoader.GetText("exception_read_translation_ini"));
            }

            Version translationVer = new Version(ini.Sections[Strings.IniName.Patcher.Section].Keys[Strings.IniName.Patcher.KeyVer].Value);
            ini.Sections.Clear();
            ini.Load(Path.Combine(UserSettings.GamePath, Strings.IniName.ClientVer));

            Version clientVer = new Version(ini.Sections[Strings.IniName.Ver.Section].Keys[Strings.IniName.Ver.Key].Value);
            if (clientVer > translationVer)
            {
                return true;
            }

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
