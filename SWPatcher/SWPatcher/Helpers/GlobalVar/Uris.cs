namespace SWPatcher.Helpers.GlobalVar
{
    public static class Uris
    {
        public const string SWHQWebsite = "http://soulworkerhq.com/";
        //public const string PatcherGitHubHome = "https://raw.githubusercontent.com/Miyuyami/SWHQPatcher/master/";
        public static string PatcherGitHubHome = System.Environment.ExpandEnvironmentVariables(@"%userprofile%\Documents\GitHub\SWHQPatcher\");
        //public const string TranslationGitHubHome = "https://raw.githubusercontent.com/Miyuyami/SoulWorkerHQTranslations/master/";
        public static string TranslationGitHubHome = System.Environment.ExpandEnvironmentVariables(@"%userprofile%\Documents\GitHub\SoulWorkerHQTranslations\");
        //public const string SoulWorkerSettingsHome = "http://down.hangame.co.jp/jp/purple/plii/j_sw/";
        public static string SoulWorkerSettingsHome = System.Environment.ExpandEnvironmentVariables(@"%userprofile%\Documents\");
        public const string SoulWorkerHome = "http://soulworker.hangame.co.jp/";
    }
}
