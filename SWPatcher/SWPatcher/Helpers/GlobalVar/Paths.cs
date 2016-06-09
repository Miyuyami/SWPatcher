using System;
using System.IO;
using System.Reflection;
using SWPatcher.Properties;

namespace SWPatcher.Helpers.GlobalVar
{
    public static class Paths
    {
        public static string PatcherRoot
        {
            get
            {
                return String.IsNullOrEmpty(Settings.Default.PatcherWorkingDirectory) ? Paths.PatcherRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Assembly.GetExecutingAssembly().GetName().Name) : Settings.Default.PatcherWorkingDirectory;
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    if (!Directory.Exists(value))
                        Directory.CreateDirectory(value);
                    Directory.SetCurrentDirectory(value);
                }
                Settings.Default.PatcherWorkingDirectory = value;
                Settings.Default.Save();
            }
        }
        public static string GameRoot
        {
            get
            {
                return Settings.Default.SoulworkerDirectory;
            }
            set
            {
                Settings.Default.SoulworkerDirectory = value;
                Settings.Default.Save();
            }
        }
    }
}
