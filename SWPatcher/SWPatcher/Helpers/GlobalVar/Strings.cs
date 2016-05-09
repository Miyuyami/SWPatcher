using System;
using System.Globalization;

namespace SWPatcher.Helpers.GlobalVar
{
    public static class Strings
    {
        public static class FileExtentionName
        {
            public const string V = ".v";
            public const string Txt = ".txt";
            public const string Res = ".res";
            public const string Exe = ".exe";
            public const string Ini = ".ini";
            public const string Zip = ".zip";
        }

        public static class FileName
        {
            public const string GameExe = "SoulWorker100.exe";
            public const string Log = "log.txt";
        }

        public static class FolderName
        {
            public const string Data = "datas";
            public const string Backup = "backup";
        }

        public static class IniName
        {
            public const string PatcherVersion = "version.ini";
            public const string ServerVer = "ServerVer.ini";
            public const string ClientVer = "Ver.ini";
            public const string Translation = "Translation.ini";
            public const string LanguagePack = "LanguagePacks.ini";
            public const string TranslationPackData = "TranslationPackData.ini";
            public const string OtherTranslationPackData = "OtherTranslationPackData.ini";
            public static class Ver
            {
                public const string Section = "Client";
                public const string Key = "ver";
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
        }

        public static class FormText
        {
            public const string Download = "Download Translations";
            public const string Play = "Ready To Play!";
            public const string Cancel = "Cancel";

            public static class Status
            {
                public const string Idle = "";
                public const string Download = "Downloading...";
                public const string Patch = "Working files...";
                public const string Play = "Waiting for game...";
            }
        }

        public static DateTime ParseExact(string date)
        {
            return DateTime.ParseExact(date, "dd/MMM/yyyy h:mm tt", CultureInfo.InvariantCulture);
        }

        public static string DateToString(DateTime date)
        {
            return date.ToString("dd/MMM/yyyy h:mm tt", CultureInfo.InvariantCulture);
        }
    }
}
