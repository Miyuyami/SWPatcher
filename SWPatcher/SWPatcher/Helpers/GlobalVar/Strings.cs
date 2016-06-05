namespace SWPatcher.Helpers.GlobalVar
{
    public static class Strings
    {
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

            public const string LanguagePack = "LanguagePacksBeta.ini";
            public const string TranslationPackData = "TranslationPackDataBeta.ini";
            /*
            public const string LanguagePack = "LanguagePacks.ini";
            public const string TranslationPackData = "TranslationPackData.ini";
            */
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
            public const string NewTranslations = "There are new translations available for the selected language.";
            public const string Download = "Download Translations";
            public const string Play = "Ready To Play!";
            public const string Cancel = "Cancel";
            public const string Cancelling = "Cancelling...";

            public static class Status
            {
                public const string Idle = "";
                public const string Download = "Downloading...";
                public const string Patch = "Working files...";
                public const string Prepare = "Preparing files...";
                public const string WaitClient = "Waiting for client...";
                public const string ApplyFiles = "Applying files...";
                public const string WaitClose = "Waiting for client termination...";
            }
        }
    }
}
