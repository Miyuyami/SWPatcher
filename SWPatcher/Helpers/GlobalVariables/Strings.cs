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

namespace SWPatcher.Helpers.GlobalVariables
{
    public static class Strings
    {
        public static class PasteBin
        {
            public const string DevKey = "2e5bee04f7455774443dd399934494bd";
            public const string Username = "SWPatcher";
            public const string Password = "pIIrwSL8lNJOjPhW";
        }

        public static class FileName
        {
            public const string GameExe = "SoulWorker100.exe";
            public const string PurpleExe = "PLauncher.exe";
            public const string ReactorExe = "reactor.exe";
            public const string OutboundExe = "Outbound.exe";
            public const string Log = ".log";
            public const string Data12 = "data12.v";
            public const string Data14 = "data14.v";
        }

        public static class FolderName
        {
            public const string Data = "datas";
            public const string Backup = "backup";
            public const string RTPatchLogs = "RTPatchLogs";
        }

        public static class IniName
        {
            public const string PatcherVersion = "version.ini";
            public const string ServerVer = "ServerVer.ini";
            public const string ClientVer = "Ver.ini";
            public const string Translation = "Translation.ini";
            public const string LanguagePack = "LanguagePacks.ini";
            public const string TranslationPackData = "TranslationPackData.ini";
            public const string BytesToPatch = "Sw100BytesToPatch.ini";
            public const string GeneralClient = "General.ini";
            public const string DatasArchives = "datas.ini";

            public static class Ver
            {
                public const string Section = "Client";
                public const string Key = "ver";
            }

            public static class ServerRepository
            {
                public const string Section = "FTP";
                public const string Key = "address";
                public const string UpdateRepository = "update";
            }

            public static class Patcher
            {
                public const string Section = "Patcher";
                public const string KeyVer = "ver";
                public const string KeyAddress = "address";
            }

            public static class Pack
            {
                public const string KeyDate = "date";
                public const string KeyPath = "path";
                public const string KeyPathInArchive = "path_a";
                public const string KeyPathOfDownload = "path_d";
                public const string KeyFormat = "format";
            }

            public static class PatchBytes
            {
                public const string KeyOriginal = "original";
                public const string KeyPatch = "patch";
            }

            public static class Datas
            {
                public const string SectionZipPassword = "Zip Passwords";
                public const string Data12 = "data12";
            }
        }

        public static class Server
        {
            public const string IP = "sw-auth.hangame.co.jp";
            public const string Port = "10000";
        }

        public static class Web
        {
            public const string PostEncodeId = "encodeId";
            public const string PostEncodeFlag = "encodeFlg";
            public const string PostEncodeFlagDefaultValue = "true";
            public const string PostId = "strmemberid";
            public const string PostPw = "strpassword";
            public const string PostClearFlag = "clFlg";
            public const string PostClearFlagDefaultValue = "y";
            public const string PostNextUrl = "nxtURL";
            public const string PostNextUrlDefaultValue = "http://www.hangame.co.jp";
            public const string ReactorStr = "	reactorStr = ";
            public const string GameStartArg = "\"gs\":";
            public const string ErrorCodeVariable = "var errCode = ";
            public const string MaintenanceVariable = "var openCloseTypeCd = ";
            public const string MessageVariable = "var msg = ";
            public const string CaptchaValidationText = "画像認証";
            public const string CaptchaUrl = "http://top.hangame.co.jp/login/loginfailed.nhn?type=dlf";
        }
    }
}
