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


namespace SWPatcher.Helpers.GlobalVariables
{
    internal static class Strings
    {
        internal static class PasteBin
        {
            internal const string DevKey = "2e5bee04f7455774443dd399934494bd";
            internal const string Username = "SWPatcher";
            internal const string Password = "pIIrwSL8lNJOjPhW";
        }

        internal static class FileName
        {
            internal const string GameExeJP = "SoulWorker100.exe";
            internal const string GameExeKR = "SoulWorker.exe";
            internal const string GameExeGF = "SoulWorker.exe";
            internal const string PurpleExe = "PLauncher.exe";
            internal const string ReactorExe = "reactor.exe";
            internal const string OutboundExe = "Outbound.exe";
            internal const string OptionExe = "Option.exe";
            internal const string SecurityExe = "DirectoryRights.exe";
            internal const string Log = ".log";
            internal const string Data12 = "data12.v";
            //public const string Data14 = "data14.v";
        }

        internal static class FolderName
        {
            internal const string Data = "datas";
            internal const string Backup = "backup";
            internal const string RTPatchLogs = "RTPatchLogs";
        }

        internal static class IniName
        {
            internal const string ServerVer = "ServerVer.ini";
            internal const string ClientVer = "Ver.ini";
            internal const string Translation = "Translation.ini";
            internal const string LanguagePack = "LanguagePacks.xml";
            internal const string TranslationPackData = "TranslationPackData.ini";
            internal const string BytesToPatch = "BytePatch.ini";
            internal const string DatasArchives = "datas.ini";

            internal static class Ver
            {
                internal const string Section = "Client";
                internal const string Key = "ver";
            }

            internal static class ServerRepository
            {
                internal const string Section = "FTP";
                internal const string Key = "address";
                internal const string UpdateRepository = "update/";
            }

            internal static class Patcher
            {
                internal const string Section = "Patcher";
                internal const string KeyDate = "date";
            }

            internal static class Pack
            {
                internal const string KeyPath = "path";
                internal const string KeyPathInArchive = "path_a";
                internal const string KeyPathOfDownload = "path_d";
                internal const string KeyFormat = "format";
                internal const string KeyBaseValue = "__base__";
            }

            internal static class PatchBytes
            {
                internal const string KeyOriginal = "original";
                internal const string KeyPatch = "patch";
            }

            internal static class Datas
            {
                internal const string SectionZipPassword = "Zip Passwords";
                internal const string Data12 = "data12";
            }
        }

        internal static class Server
        {
            internal const string IP = "sw-auth.hangame.co.jp";
            internal const string Port = "10000";
        }

        internal static class Web
        {
            internal const string PostEncodeId = "encodeId";
            internal const string PostEncodeFlag = "encodeFlg";
            internal const string PostEncodeFlagDefaultValue = "true";
            internal const string PostId = "strmemberid";
            internal const string PostPw = "strpassword";
            internal const string PostClearFlag = "clFlg";
            internal const string PostClearFlagDefaultValue = "y";
            internal const string PostNextUrl = "nxtURL";
            internal const string PostNextUrlDefaultValue = "http://www.hangame.co.jp";
            /*public const string ReactorStr = "	reactorStr = ";
            internal const string GameStartArg = "\"gs\":";
            internal const string ErrorCodeVariable = "var errCode = ";
            internal const string ErrorCodeArg = "\"errCode\":";
            internal const string MaintenanceVariable = "var openCloseTypeCd = ";*/
            internal const string MessageVariable = "var msg = ";
            internal const string CaptchaValidationText = "画像認証";
            internal const string CaptchaValidationText2 = "認証に連続";
            internal const string CaptchaUrl = "http://top.hangame.co.jp/login/loginfailed.nhn?type=dlf";

            internal static class KR
            {
                internal const string ServiceCode = "service_code";
                internal const string LocalVersion = "local_version";

                internal const string PostId = "user_id";
                internal const string PostPw = "user_pwd";
                internal const string KeepForever = "forever";
                internal const string KeepForeverDefaultValue = "false";
            }
        }

        internal static class Registry
        {
            internal static class JP
            {
                internal static Microsoft.Win32.RegistryKey RegistryKey = Microsoft.Win32.Registry.LocalMachine;
                internal const string Key32Path = @"SOFTWARE\WeMade Online\Soulworker";
                internal const string Key64Path = @"SOFTWARE\WOW6432Node\WeMade Online\Soulworker";
                internal const string FolderName = "InstallPath";
            }

            internal static class KR
            {
                internal static Microsoft.Win32.RegistryKey RegistryKey = Microsoft.Win32.Registry.CurrentUser;
                internal const string Key32Path = @"SOFTWARE\SGUP\Apps\11";
                //public const string Key64Path = @"SOFTWARE\SGUP\Apps\11";
                internal const string FolderName = "GamePath";
                internal const string Version = "Version";

                internal const string StoveKeyPath = @"SOFTWARE\SGUP\ActiveProcess";
                internal const string StoveWorkingDir = "WorkingDir";
            }

            internal static class NaverKR
            {
                internal static Microsoft.Win32.RegistryKey RegistryKey = Microsoft.Win32.Registry.CurrentUser;
                internal const string Key32Path = @"SOFTWARE\SGSWOSCHANNEL\Apps\11";
                //public const string Key64Path = @"SOFTWARE\SGSWOSCHANNEL\Apps\11";
                internal const string FolderName = "GamePath";
                internal const string Version = "Version";
            }

            internal static class Steam
            {
                internal static Microsoft.Win32.RegistryKey RegistryKey = Microsoft.Win32.Registry.LocalMachine;
                internal const string Key32Path = @"SOFTWARE\Valve\Steam";
                internal const string Key64Path = @"SOFTWARE\Wow6432Node\Valve\Steam";
                internal const string InstallPath = "InstallPath";
            }
        }

        internal static class Xml
        {
            internal const string Value = "value";
            internal const string Regions = "regions";
            internal const string Languages = "languages";
            //internal const string Supports = "supports";

            internal static class Attributes
            {
                internal const string Name = "name";
            }
        }
    }
}
