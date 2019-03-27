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

using Ionic.Zip;
using MadMilkman.Ini;
using Microsoft.Win32;
using Newtonsoft.Json;
using SWPatcher.General;
using SWPatcher.Helpers.GlobalVariables;
using System;
using System.Collections.Specialized;
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
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;

namespace SWPatcher.Helpers
{
    internal static class Methods
    {
        private static string DateFormat = "d/MMM/yyyy h:mm tt";
        private static byte[] Entropy = Encoding.Unicode.GetBytes("C11699FC9EC2502027E0222999DA029D01DE3026");

        internal static DateTime ParseDate(string date)
        {
            return DateTime.ParseExact(date, DateFormat, CultureInfo.InvariantCulture);
        }

        internal static string DateToString(DateTime date)
        {
            return date.ToString(DateFormat, CultureInfo.InvariantCulture);
        }

        internal static string ExeptionParser(Exception ex)
        {
            string result = ex.Message;
            if (ex.InnerException != null)
            {
                result += "\n\n" + ex.InnerException.Message;
            }

            return result;
        }

        internal static bool HasNewTranslations(Language language)
        {
            string translationFolder = language.Path;

            if (!Directory.Exists(translationFolder))
            {
                return true;
            }

            string selectedTranslationIniPath = Path.Combine(translationFolder, Strings.IniName.Translation);
            if (!LoadPatcherIni(out IniFile translationIni, selectedTranslationIniPath))
            {
                return false;
            }
            IniSection translationPatcherSection = translationIni.Sections[Strings.IniName.Patcher.Section];
            string date = translationPatcherSection.Keys[Strings.IniName.Patcher.KeyDate].Value;

            return language.LastUpdate > ParseDate(date);
        }

        private static bool IsSwPath(string path)
        {
            bool f1 = Directory.Exists(path);
            string dataPath = Path.Combine(path, Strings.FolderName.Data);
            bool f2 = Directory.Exists(dataPath);
            bool f3 = File.Exists(Path.Combine(path, Strings.IniName.ClientVer));
            bool f4 = File.Exists(Path.Combine(dataPath, Strings.FileName.Data12));

            return f1 && f2 && f3 && f4;
        }

        internal static bool LoadIni(out IniFile iniFile, string iniPath)
        {
            return LoadIni(out iniFile, new IniOptions(), iniPath);
        }

        internal static bool LoadIni(out IniFile iniFile, IniOptions iniOptions, string iniPath)
        {
            if (!File.Exists(iniPath))
            {
                iniFile = null;
                return false;
            }

            iniFile = new IniFile(iniOptions);
            iniFile.Load(iniPath);

            return true;
        }

        internal static bool LoadVerIni(out IniFile verIni, string verIniPath)
        {
            return LoadVerIni(out verIni, new IniOptions(), verIniPath);
        }

        internal static bool LoadVerIni(out IniFile verIni, IniOptions verIniOptions, string verIniPath)
        {
            if (!LoadIni(out verIni, verIniOptions, verIniPath))
            {
                return false;
            }

            if (!verIni.Sections.Contains(Strings.IniName.Ver.Section))
            {
                return false;
            }

            IniSection clientVerSection = verIni.Sections[Strings.IniName.Ver.Section];
            if (!clientVerSection.Keys.Contains(Strings.IniName.Ver.Key))
            {
                return false;
            }

            return true;
        }

        internal static bool LoadPatcherIni(out IniFile patcherIni, string patcherIniPath)
        {
            return LoadPatcherIni(out patcherIni, new IniOptions(), patcherIniPath);
        }

        internal static bool LoadPatcherIni(out IniFile patcherIni, IniOptions patcherIniOptions, string patcherIniPath)
        {
            if (!LoadVerIni(out patcherIni, patcherIniOptions, patcherIniPath))
            {
                return false;
            }

            if (!patcherIni.Sections.Contains(Strings.IniName.Patcher.Section))
            {
                return false;
            }

            IniSection clientVerSection = patcherIni.Sections[Strings.IniName.Patcher.Section];
            if (!clientVerSection.Keys.Contains(Strings.IniName.Patcher.KeyDate))
            {
                return false;
            }

            return true;
        }

        internal static bool IsValidSwPatcherPath(string path)
        {
            return String.IsNullOrEmpty(path) || !IsSwPath(path) && IsValidSwPatcherPath(Path.GetDirectoryName(path));
        }

        internal static IniFile GetJPServerIni()
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                    byte[] iniBytes = client.DownloadData(Urls.SoulworkerSettingsHome + Strings.IniName.ServerVer);

                    IniFile ini = new IniFile();
                    using (MemoryStream ms = new MemoryStream(iniBytes))
                    {
                        ini.Load(ms);
                    }

                    return ini;
                }
                catch (WebException e)
                {
                    if (e.InnerException is SocketException innerException)
                    {
                        if (innerException.SocketErrorCode == SocketError.ConnectionRefused)
                        {
                            throw new Exception(StringLoader.GetText("exception_server_refused_connection"));
                        }
                        else
                        {
                            throw;
                        }
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        internal static int GetKRServerVersion()
        {
            using (WebClient client = new WebClient())
            {
                var apiJSON = new { message = "", result = "", value = new { pid = "", service_code = Int32.MaxValue, live_version = Int32.MinValue, live_project_url = "" } };

                NameValueCollection values = new NameValueCollection(2)
                {
                    [Strings.Web.KR.ServiceCode] = "11",
                    [Strings.Web.KR.LocalVersion] = "0"
                };
                byte[] byteResponse = client.UploadValues(Urls.SoulworkerKRAPI, values);
                string response = Encoding.UTF8.GetString(byteResponse);
                var jsonResponse = JsonConvert.DeserializeAnonymousType(response, apiJSON);

                // TODO: proper result handling
                switch (jsonResponse.result ?? throw new Exception("unexpected null result"))
                {
                    case "0":
                        return jsonResponse.value.live_version;
                    default:
                        throw new Exception($"result=[{jsonResponse.result}]\n{jsonResponse.message ?? "no error details"}");
                }
            }
        }

        internal static void PatchExeFile(byte[] exeFileBytes, string gameExePatchedPath, string patchInstructionFilePath)
        {
            Logger.Debug(Methods.MethodFullName(MethodBase.GetCurrentMethod(), exeFileBytes.Length.ToString(), gameExePatchedPath, patchInstructionFilePath));

            using (WebClient client = new WebClient())
            {
                string hexResult = BitConverter.ToString(exeFileBytes).Replace("-", "");
                string patchedHexResult = String.Copy(hexResult);

                byte[] fileBytes = client.DownloadData(patchInstructionFilePath);
                IniFile ini = new IniFile();
                using (MemoryStream ms = new MemoryStream(fileBytes))
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
                        MsgBox.Error(StringLoader.GetText("error_exe_patch_fail", section.Name));

                        if (File.Exists(gameExePatchedPath))
                        {
                            File.Delete(gameExePatchedPath);
                        }

                        UserSettings.WantToPatchExe = false;

                        return;
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

        /// <summary>
        /// Deletes related RTPatch files from the <c>SWPatcher.Helpers.GlobalVariables.UserSettings.GamePath</c>.
        /// </summary>
        /// <param name="filterFlag">if true, delete both "RT*" and "*.RTP", otherwise, delete only "RT*"</param>
        internal static void RTPatchCleanup(bool filterFlag)
        {
            string[] filters;
            if (filterFlag)
            {
                filters = new string[] { "RT*", "*.RTP" };
            }
            else
            {
                filters = new string[] { "RT*" };
            }

            foreach (string filter in filters)
            {
                foreach (string file in Directory.GetFiles(UserSettings.GamePath, filter, SearchOption.AllDirectories))
                {
                    Logger.Info($"Trying to delete file=[{file}]");
                    try
                    {
                        File.Delete(file);
                    }
                    catch (Exception e)
                    {
                        Logger.Debug($"Could not delete file=[{file}]\n\n{e.ToString()}");
                    }
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
            MemoryStream result = new MemoryStream();

            using (MemoryStream ms = new MemoryStream(zipData))
            using (var zip = ZipFile.Read(ms))
            {
                zip.Password = password;
                zip.FlattenFoldersOnExtract = true;
                zip[fileName].Extract(result);
                result.Position = 0;
            }

            return result;
        }

        internal static MemoryStream GetZippedFileStream(ZipFile zip, string fileName, string password)
        {
            MemoryStream result = new MemoryStream();

            zip.Password = password;
            zip.FlattenFoldersOnExtract = true;
            zip[fileName].Extract(result);
            result.Position = 0;

            return result;
        }

        internal static void ZipFileStream(ZipFile zip, string fileName, MemoryStream fileStream, string password)
        {
            fileStream.Position = 0;
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
        }

        internal static void AddZipToZip(ZipFile zip, string directoryInDestination, MemoryStream zipStream, string password)
        {
            zipStream.Position = 0;
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
            }
        }

        internal static bool IsTranslationOutdated(Language language)
        {
            string selectedTranslationIniPath = Path.Combine(language.Path, Strings.IniName.Translation);
            if (!LoadPatcherIni(out IniFile translationIni, selectedTranslationIniPath))
            {
                return true;
            }
            IniSection translationPatcherSection = translationIni.Sections[Strings.IniName.Patcher.Section];
            IniSection translationVerSection = translationIni.Sections[Strings.IniName.Ver.Section];

            string clientIniPath = Path.Combine(UserSettings.GamePath, Strings.IniName.ClientVer);
            if (!LoadVerIni(out IniFile clientIni, clientIniPath))
            {
                throw new Exception(StringLoader.GetText("exception_generic_read_error", clientIniPath));
            }
            IniSection clientVerSection = clientIni.Sections[Strings.IniName.Ver.Section];

            string translationVer = translationVerSection.Keys[Strings.IniName.Ver.Key].Value;
            string clientVer = clientVerSection.Keys[Strings.IniName.Ver.Key].Value;
            if (clientVer != translationVer)
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
            string returnValue = String.Empty;
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

        internal static void CheckRunningUpdaters()
        {
            string[] processes = Methods.GetRunningUpdaterProcesses();

            if (processes.Length > 0)
            {
                throw new Exception(StringLoader.GetText("exception_game_already_open", String.Join("/", processes)));
            }
        }

        internal static void CheckRunningGame(string regionId)
        {
            string[] processes = Methods.GetRunningGameProcesses(regionId);

            if (processes.Length > 0)
            {
                throw new Exception(StringLoader.GetText("exception_game_already_open", String.Join("/", processes)));
            }
        }

        internal static void CheckRunningProcesses(string regionId)
        {
            string[] processes = Methods.GetRunningGameProcesses(regionId).Union(Methods.GetRunningUpdaterProcesses()).ToArray();

            if (processes.Length > 0)
            {
                throw new Exception(StringLoader.GetText("exception_game_already_open", String.Join("/", processes)));
            }
        }

        private static string[] GetRunningUpdaterProcesses()
        {
            string[] processNames = { Strings.FileName.PurpleExe, Strings.FileName.ReactorExe, Strings.FileName.OutboundExe };

            return processNames.SelectMany(pn => Process.GetProcessesByName(Path.GetFileNameWithoutExtension(pn))).Select(p => Path.GetFileName(Methods.GetProcessPath(p.Id))).Where(pn => processNames.Contains(pn)).ToArray();
        }

        private static string[] GetRunningGameProcesses(string regionId)
        {
            string[] processNames = { Methods.GetGameExeName(regionId) };

            return processNames.SelectMany(pn => Process.GetProcessesByName(Path.GetFileNameWithoutExtension(pn))).Select(p => Path.GetFileName(Methods.GetProcessPath(p.Id))).Where(pn => processNames.Contains(pn)).ToArray();
        }

        private static string GetProcessPath(int processId)
        {
            StringBuilder buffer = new StringBuilder(1024);
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

        internal static void EnsureDirectoryRights(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                throw new DirectoryNotFoundException(StringLoader.GetText("exception_directory_not_exist", folderPath));
            }

            FileSystemRights rights = FileSystemRights.Modify;

            if (!DirectoryHasRights(folderPath, rights))
            {
                string securityExePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Strings.FileName.SecurityExe);

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    UseShellExecute = true,
                    Verb = "runas",
                    Arguments = $"\"{folderPath}\" \"{rights}\"",
                    FileName = securityExePath
                };

                Process process = Process.Start(startInfo);
                process.WaitForExit();

                int exitCode = process.ExitCode;

                if (exitCode != 0)
                {
                    if (exitCode == -2)
                    {
                        throw new DirectoryNotFoundException(StringLoader.GetText("exception_directory_not_exist", folderPath));
                    }
                    else
                    {
                        throw new Exception(StringLoader.GetText("exception_directory_rights", folderPath));
                    }
                }
            }
        }

        internal static bool DirectoryHasRights(string folderPath, FileSystemRights rights)
        {
            WindowsIdentity currentUserIdentity = WindowsIdentity.GetCurrent();
            SecurityIdentifier currentUserSID = currentUserIdentity.User;
            bool allow = false;
            bool deny = false;
            DirectorySecurity acl = Directory.GetAccessControl(folderPath);
            if (acl == null)
            {
                return false;
            }

            AuthorizationRuleCollection accessRules = acl.GetAccessRules(true, true, typeof(SecurityIdentifier));
            if (accessRules == null)
            {
                return false;
            }

            foreach (FileSystemAccessRule accessRule in accessRules)
            {
                if (currentUserSID.Equals(accessRule.IdentityReference))
                {
                    if ((rights & accessRule.FileSystemRights) != rights)
                    {
                        continue;
                    }

                    if (accessRule.AccessControlType == AccessControlType.Allow)
                    {
                        allow = true;
                    }
                    else if (accessRule.AccessControlType == AccessControlType.Deny)
                    {
                        deny = true;
                    }
                }
            }

            return allow && !deny;
        }

        internal static string GetRegistryValue(RegistryKey regKey, string path, string varName)
        {
            return GetRegistryValue(regKey, path, varName, String.Empty);
        }

        internal static string GetRegistryValue(RegistryKey regKey, string path, string varName, object defaultValue)
        {
            using (RegistryKey key = regKey.OpenSubKey(path))
            {
                if (key == null)
                {
                    return defaultValue.ToString();
                }

                return Convert.ToString(key.GetValue(varName, defaultValue));
            }
        }
        internal static string GetGameExeName(string regionId)
        {
            switch (regionId)
            {
                case "jp":
                    return Strings.FileName.GameExeJP;
                case "kr":
                case "nkr":
                    return Strings.FileName.GameExeKR;
                case "gf":
                    return Strings.FileName.GameExeGF;
                default:
                    throw new Exception(StringLoader.GetText("exception_region_unknown", regionId));
            }
        }

        internal static void RegionDoesNotSupportLogin()
        {
            UserSettings.WantToLogin = false;
            throw new Exception(StringLoader.GetText("exception_login_option_not_supported"));
        }

        internal static void RegionDoesNotSupportExePatch()
        {
            UserSettings.WantToPatchExe = false;
            throw new Exception(StringLoader.GetText("error_exe_region_not_supported"));
        }
    }
}
