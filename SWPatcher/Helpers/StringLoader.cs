using SWPatcher.Helpers.GlobalVar;
using System;
using System.Globalization;

namespace SWPatcher.Helpers
{
    public static class StringLoader
    {
        public static string GetText(string name)
        {
            var languageCode = UserSettings.UILanguageCode;
            switchagain:
            switch (languageCode)
            {
                case "default":
                    languageCode = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
                    goto switchagain;
                case "en":
                    return Resources.en.ResourceManager.GetString(name);
                case "ko":
                    return Resources.ko.ResourceManager.GetString(name);
                case "vi":
                    return Resources.vi.ResourceManager.GetString(name);
                default:
                    return Resources.en.ResourceManager.GetString(name);
            }
        }
    }
}
