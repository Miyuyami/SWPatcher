using System;
using System.IO;
using System.Reflection;
using SWPatcher.Properties;
using System.Windows.Forms;

namespace SWPatcher.Helpers.GlobalVar
{
    public static class UserSettings
    {
        public static string PatcherPath
        {
            get
            {
                return String.IsNullOrEmpty(Settings.Default.PatcherWorkingDirectory) ? UserSettings.PatcherPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Assembly.GetExecutingAssembly().GetName().Name) : Settings.Default.PatcherWorkingDirectory;
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

        public static string GamePath
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

        public static bool WantToPatchExe
        {
            get
            {
                return Settings.Default.WantToPatchSoulworkerExe;
            }
            set
            {
                string gameExePatchedPath = Path.Combine(UserSettings.PatcherPath, Strings.FileName.GameExe);
                if (File.Exists(gameExePatchedPath))
                    File.Delete(gameExePatchedPath);

                Settings.Default.WantToPatchSoulworkerExe = value;
                Settings.Default.Save();
            }
        }

        public static bool PatcherRunas
        {
            get
            {
                return Settings.Default.PatcherRunasAdmin;
            }
            set
            {
                Settings.Default.PatcherRunasAdmin = value;
                Settings.Default.Save();

                if (value)
                {
                    DialogResult result = MsgBox.Question("To apply some of the settings you must restart the patcher.\nDo you want to do this now?");
                    if (result == DialogResult.Yes)
                        Methods.RestartAsAdmin();
                }
                else
                {
                    MsgBox.Success("To apply some of the settings you must manually restart the patcher.");
                }
            }
        }
    }
}
