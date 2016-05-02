namespace SWPatcher.Classes.Patcher
{
    enum BWorkerCode :short
    {
        None = 0,
        Startup = 1,
        Patching
    }

    public struct Strings
    {
        public static string ProgressBarReady { get { return "Ready your soul"; } }
    }

    public struct Config
    {
        public static string Setting { get { return "Settings.ini"; } }
        public static string ModMeta { get { return "Mod.ini"; } }
    }

    public struct Section
    {
        public static string Patcher { get { return "patcher"; } }
        public static string SWPath { get { return "folder"; } }
        public static string TranslationLanguage { get { return "s_translationlanguage"; } }
    }
}
