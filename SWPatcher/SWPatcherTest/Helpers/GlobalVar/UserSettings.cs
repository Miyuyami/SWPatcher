using System;
using System.IO;
using System.Reflection;
using SWPatcher.Properties;
using System.Windows.Forms;
using MadMilkman.Ini;

namespace SWPatcher.Helpers.GlobalVar
{
    public static class UserSettings
    {
        #region "ConfigFile"
        private static IniFile theIni = null;
        private static string theIniPath = Path.Combine(new string[] { Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "SWPatcher", "Settings.ini" });
        public static IniFile ConfigInstance
        {
            get
            {
                if (theIni == null)
                {
                    theIni = new IniFile(new IniOptions()
                    {
                        CommentStarter = IniCommentStarter.Semicolon,
                        Compression = false,
                        KeyNameCaseSensitive = false,
                        Encoding = System.Text.Encoding.UTF8,
                        EncryptionPassword = ""
                    });
                    (new FileInfo(theIniPath).Directory).Create();

                    //Here come the config migration.
                    if (File.Exists(theIniPath))
                        theIni.Load(theIniPath);
                    else
                    {
                        SetValue(SettingName.PatcherPath, Settings.Default[SettingName.PatcherPath]);
                        SetValue(SettingName.GamePath, Settings.Default[SettingName.GamePath]);
                        SetValue(SettingName.WantToPatchExe, Settings.Default[SettingName.WantToPatchExe]);
                        SetValue(SettingName.LanguageName, Settings.Default[SettingName.LanguageName]);
                        theIni.Save(theIniPath);
                    }
                }
                return theIni;
            }
        }
        private static object GetValue(string SettingName, object DefaultValue)
        {
            if (ConfigInstance.Sections.Contains(SettingName))
            {
                if (!DefaultValue.GetType().Name.ToLower().Contains("string"))
                {
                    if (string.IsNullOrWhiteSpace(ConfigInstance.Sections[SettingName].Keys["Value"].Value))
                        return DefaultValue;
                    else
                    {
                        MethodInfo method = DefaultValue.GetType().GetMethod("Parse", BindingFlags.Static | BindingFlags.Public);
                        return method.Invoke(null, new object[] { ConfigInstance.Sections[SettingName].Keys["Value"].Value });
                    }
                }
                else
                {
                    return ConfigInstance.Sections[SettingName].Keys["Value"].Value;
                }
            }
            return DefaultValue;
        }

        /*
        private static object GetValueExact(string SettingName, System.Type Type)
        {
            if (Config.Sections.Contains(SettingName))
            {
                if (!Type.Name.ToLower().Contains("string"))
                {
                    if (string.IsNullOrWhiteSpace(Config.Sections[SettingName].Keys["Value"].Value))
                        return null;
                    else
                    {
                        MethodInfo method = Type.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public);
                        return method.Invoke(null, new object[] { Config.Sections[SettingName].Keys["Value"].Value });
                    }
                }
                else
                {
                    return Config.Sections[SettingName].Keys["Value"].Value;
                }
            }
            return null;
        }*/

        private static void SetValue(string SettingName, object Value)
        {
            if (!ConfigInstance.Sections.Contains(SettingName))
                ConfigInstance.Sections.Add(SettingName);
            if (!ConfigInstance.Sections[SettingName].Keys.Contains("Value"))
                ConfigInstance.Sections[SettingName].Keys.Add("Value", Value.ToString());
            else
                ConfigInstance.Sections[SettingName].Keys["Value"].Value = Value.ToString();
            ConfigInstance.Save(theIniPath);
        }
        #endregion

        private static class SettingName
        {
            public const string PatcherPath = "PatcherWorkingDirectory";
            public const string PatcherRunas = "PatcherRunas";
            public const string GamePath = "SoulworkerDirectory";
            public const string WantToPatchExe = "WantToPatchSoulworkerExe";
            public const string LanguageName = "LanguageName";
        }

        public static string PatcherPath
        {
            get
            {
                return (string)GetValue(SettingName.PatcherPath, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Assembly.GetExecutingAssembly().GetName().Name));
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    Directory.CreateDirectory(value);
                    Directory.SetCurrentDirectory(value);
                }
                SetValue(SettingName.PatcherPath, value);
            }
        }

        public static bool PatcherRunas
        {
            get
            {
                return (bool)GetValue(SettingName.PatcherRunas, false);
            }
            set
            {
                SetValue(SettingName.PatcherRunas, value);

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

        public static string GamePath
        {
            get
            {
                return (string)GetValue(SettingName.GamePath, string.Empty);
            }
            set
            {
                SetValue(SettingName.GamePath, value);
            }
        }

        public static bool WantToPatchExe
        {
            get
            {
                return (bool)GetValue(SettingName.WantToPatchExe, false);
            }
            set
            {
                File.Delete(Path.Combine(UserSettings.PatcherPath, Strings.FileName.GameExe));

                SetValue(SettingName.WantToPatchExe, value);
            }
        }

        public static string LanguageName
        {
            get
            {
                return (string)GetValue(SettingName.LanguageName, string.Empty);
            }
            set
            {
                SetValue(SettingName.LanguageName, value);
            }
        }
    }
}
