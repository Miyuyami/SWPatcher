using SWPatcherTest.Properties;
using System;
using System.IO;
using System.Reflection;

namespace SWPatcherTest.Helpers.GlobalVariables
{
    public static class UserSettings
    {
        public static string PatcherPath
        {
            get
            {
                if (String.IsNullOrEmpty(Settings.Default.PatcherWorkingDirectory))
                {
                    return PatcherPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Assembly.GetExecutingAssembly().GetName().Name);
                }
                else
                {
                    return Settings.Default.PatcherWorkingDirectory;
                }
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    Directory.CreateDirectory(value);
                    Directory.SetCurrentDirectory(value);
                }

                Logger.Info($"Patcher path set to [{value}]");
                Settings.Default.PatcherWorkingDirectory = value;
                Settings.Default.Save();
            }
        }

        public static string GamePath
        {
            get
            {
                return Settings.Default.GameDirectory;
            }
            set
            {
                Logger.Info($"Soulworker path set to [{value}]");
                Settings.Default.GameDirectory = value;
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
                string gameExePatchedPath = Path.Combine(PatcherPath, Strings.FileName.GameExe);
                if (File.Exists(gameExePatchedPath))
                    File.Delete(gameExePatchedPath);

                Settings.Default.WantToPatchSoulworkerExe = value;
                Settings.Default.Save();
            }
        }

        public static string GameId
        {
            get
            {
                return Settings.Default.GameUserId;
            }
            set
            {
                Settings.Default.GameUserId = value;
                Settings.Default.Save();
            }
        }

        public static string GamePw
        {
            get
            {
                return Settings.Default.GameUserPassword;
            }
            set
            {
                Settings.Default.GameUserPassword = value;
                Settings.Default.Save();
            }
        }

        public static bool WantToLogin
        {
            get
            {
                return Settings.Default.WantToLoginWithPatcher;
            }
            set
            {
                Settings.Default.WantToLoginWithPatcher = value;
                Settings.Default.Save();
            }
        }

        public static string LanguageName
        {
            get
            {
                return Settings.Default.LanguageName;
            }
            set
            {
                Settings.Default.LanguageName = value;
                Settings.Default.Save();
            }
        }

        public static byte InterfaceMode
        {
            get
            {
                return Settings.Default.UIMode;
            }
            set
            {
                Logger.Info($"UI Mode set to [{value}]");
                Settings.Default.UIMode = value;
                Settings.Default.Save();
            }
        }

        public static string UILanguageCode
        {
            get
            {
                return Settings.Default.UILanguage;
            }
            set
            {
                Logger.Info($"UI Language set to [{value}]");
                Settings.Default.UILanguage = value;
                Settings.Default.Save();
            }
        }
    }
}
