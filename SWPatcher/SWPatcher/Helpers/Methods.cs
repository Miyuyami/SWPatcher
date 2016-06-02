using System.IO;
using MadMilkman.Ini;
using SWPatcher.General;
using SWPatcher.Helpers.GlobalVar;
using System;
using System.Globalization;
using System.Windows.Forms;

namespace SWPatcher.Helpers
{
    public static class Methods
    {
        private static string DateFormat = "dd/MMM/yyyy h:mm tt";

        public static DateTime ParseDate(string date)
        {
            return DateTime.ParseExact(date, DateFormat, CultureInfo.InvariantCulture);
        }

        public static string DateToString(DateTime date)
        {
            return date.ToString(DateFormat, CultureInfo.InvariantCulture);
        }

        public static bool HasNewTranslations(Language language)
        {
            string directory = Path.Combine(Paths.PatcherRoot, language.Lang);

            if (Directory.Exists(directory))
            {
                string filePath = Path.Combine(directory, Strings.IniName.Translation);

                if (File.Exists(filePath))
                {
                    IniFile ini = new IniFile();
                    ini.Load(filePath);

                    if (!ini.Sections.Contains(Strings.IniName.Patcher.Section))
                        return true;

                    var section = ini.Sections[Strings.IniName.Patcher.Section];
                    if (!section.Keys.Contains(Strings.IniName.Pack.KeyDate))
                        return true;

                    string date = section.Keys[Strings.IniName.Pack.KeyDate].Value;
                    if (language.LastUpdate > Methods.ParseDate(date))
                        return true;
                    else
                        return false;
                }
                else
                    return true;
            }
            else
                return true;
        }

        public static bool IsSwPath(string path)
        {
            return Directory.Exists(path) && Directory.Exists(Path.Combine(path, Strings.FolderName.Data)) && File.Exists(Path.Combine(path, Strings.FileName.GameExe)) && File.Exists(Path.Combine(path, Strings.IniName.ClientVer));
        }
    }
}
