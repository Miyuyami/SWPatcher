
namespace SWPatcher.Helpers.GlobalVars
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
        }

        public static class FolderName
        {
            public const string Patcher = "SWPatcher";
            public const string Data = "datas";
            public const string Backup = "backup";
        }

        public static class IniName
        {
            public const string PatcherVersion = "version.ini";
            public const string ServerVer = "ServerVer.ini";
            public const string ClientVer = "Ver.ini";
            public const string TranslationVer = "TranslationVer.ini";
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
                public const string KeyName = "name";
                public const string KeyPathA = "path_a";
                public const string KeyPath = "path";
                public const string KeyFormat = "format";
            }
        }

        public static class FormText
        {
            public const string Download = "Download Translations";
            public const string Patch = "Ready To Play!";
            public const string Cancel = "Cancel";

            public static class Status
            {
                public const string Idle = "Ready";
                public const string Check = "Checking version...";
                public const string Download = "Downloading...";
                public const string Patch = "Patching...";
            }
        }
    }
}
