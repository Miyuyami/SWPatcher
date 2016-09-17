using SWPatcher.Helpers.GlobalVar;
using System;

namespace SWPatcher.Helpers
{
    public static class StringLoader
    {
        public static string GetText(string name)
        {
            var languageCode = UserSettings.UILanguageCode;
            switch (languageCode)
            {
                case "en":
                    return Resources.en.ResourceManager.GetString(name);
                case "kr":
                    return Resources.kr.ResourceManager.GetString(name);
                case "vi":
                    return Resources.vi.ResourceManager.GetString(name);
                default:
                    throw new Exception($"unknown code=[{languageCode}]");
            }
        }
    }
}
