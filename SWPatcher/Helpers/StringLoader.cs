/*
 * This file is part of Soulworker Patcher.
 * Copyright (C) 2016-2017 Miyu, Dramiel Leayal
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

using SWPatcher.Helpers.GlobalVariables;
using System;
using System.Globalization;

namespace SWPatcher.Helpers
{
    public static class StringLoader
    {
        public static string GetText(string name)
        {
            return GetText(name, UserSettings.UILanguageCode);
        }
        public static string GetText(string name, params object[] args)
        {
            return String.Format(GetText(name, UserSettings.UILanguageCode), args);
        }

        private static string GetText(string name, string languageCode)
        {
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
