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

        public static void RestoreBackup()
        {
            if (Directory.Exists(Strings.FolderName.Backup))
            {
                string[] filePaths = Directory.GetFiles(Strings.FolderName.Backup, "*", SearchOption.AllDirectories);

                if (!String.IsNullOrEmpty(Paths.GameRoot) && Methods.IsSwPath(Paths.GameRoot))
                    foreach (var s in filePaths)
                    {
                        string path = Path.Combine(Paths.GameRoot, s.Substring(Strings.FolderName.Backup.Length + 1));
                        if (File.Exists(path))
                            File.Delete(path);

                        File.Move(s, path);
                    }
                else
                    foreach (var s in filePaths)
                        File.Delete(s);
            }
            else
                Directory.CreateDirectory(Strings.FolderName.Backup);
        }

        public static bool IsValidSwPatcherPath(string path)
        {
            while (!String.IsNullOrEmpty(path))
            {
                if (Methods.IsSwPath(path))
                    return false;

                path = Path.GetDirectoryName(path);
            }

            return true;
        }

        public static String GetSwPathFromRegistry()
        {
            using (var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\HanPurple\J_SW"))
            {
                if (key != null)
                    return Convert.ToString(key.GetValue("folder", String.Empty));
                else
                {
                    Error.Log("64-bit - Key not found");

                    using (var key32 = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\HanPurple\J_SW"))
                    {
                        if (key32 != null)
                            return Convert.ToString(key32.GetValue("folder", String.Empty));
                        else
                        {
                            Error.Log("32-bit - Key not found");
                            return String.Empty;
                        }
                    }
                }
            }
        }
    }
}
