using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SWPatcher.Helpers
{
    public static class Strings
    {
        public static class FileExtentionNames
        {
            public static string V { get { return ".v"; } }
            public static string Txt { get { return ".txt"; } }
            public static string Res { get { return ".res"; } }
            public static string Exe { get { return ".exe"; } }
            public static string Ini { get { return ".ini"; } }
            public static string Zip { get { return ".zip"; } }
        }

        public static class FileNames
        {
            public static string GameExe { get { return "SoulWorker100" + FileExtentionNames.Exe; } }
            public static string ServerVer { get { return "ServerVer" + FileExtentionNames.Ini; } }
            public static string ClientVer { get { return "Ver" + FileExtentionNames.Ini; } }
            public static string TranslationVer { get { return "TranslationVer" + FileExtentionNames.Ini; } }
            public static string PatcherData { get { return "PatcherData"; } }
            public static string Languages { get { return "Languages"; } }
            public static string PatcherVersion { get { return "version"; } }
        }

        public static class FolderNames
        {
            public static string Patcher { get { return "SWPatcher"; } }
            public static string Datas { get { return "datas"; } }
            public static string Back { get { return "backup"; } }
        }

        public static class IniNames
        {
            public static string Section { get { return "Client"; } }
            public static string Key { get { return "ver"; } }
        }
    }
}
