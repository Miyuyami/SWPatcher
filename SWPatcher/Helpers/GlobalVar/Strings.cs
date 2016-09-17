namespace SWPatcher.Helpers.GlobalVar
{
    public static class Strings
    {
        public const string PasteBinDevKey = "2e5bee04f7455774443dd399934494bd";
        public const string PasteBinUsername = "SWPatcher";
        public const string PasteBinPassword = "pIIrwSL8lNJOjPhW";

        public static class FileName
        {
            public const string GameExe = "SoulWorker100.exe";
            public const string Log = "log.txt";
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

            public static class General
            {
                public const string SectionNetwork = "Network Info";
                public const string KeyIP = "IP";
                public const string KeyPort = "PORT";
            }

            public static class Datas
            {
                public const string SectionZipPassword = "Zip Passwords";
                public const string Data12 = "data12";
            }
        }

        public static class Web
        {
            public const string PostId = "strmemberid";
            public const string PostPw = "strpassword";
            public const string ReactorStr = "	reactorStr = ";
            public const string GameStartArg = "\"gs\":";
            public const string ErrorCodeVariable = "var errCode = ";
            public const string MaintenanceVariable = "var openCloseTypeCd = ";
            public const string MessageVariable = "var msg = ";
        }
    }
}
