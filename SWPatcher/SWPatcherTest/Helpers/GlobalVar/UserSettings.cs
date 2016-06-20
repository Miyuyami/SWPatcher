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
        public static IniFile Config
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
                        EncryptionPassword = "swhq"
                    });
                    (new FileInfo(theIniPath).Directory).Create();
                    if (File.Exists(theIniPath))
                        theIni.Load(theIniPath);
                }
                return theIni;
            }
        }
        private static object GetValue(string SettingName, object DefaultValue)
        {
            if (Config.Sections.Contains(SettingName))
            {
                if (!DefaultValue.GetType().Name.ToLower().Contains("string"))
                {
                    if (string.IsNullOrWhiteSpace(Config.Sections[SettingName].Keys["Value"].Value))
                        return DefaultValue;
                    else
                    {
                        MethodInfo method = DefaultValue.GetType().GetMethod("Parse", BindingFlags.Static | BindingFlags.Public);
                        return method.Invoke(null, new object[] { Config.Sections[SettingName].Keys["Value"].Value });
                    }
                }
                else
                {
                    return Config.Sections[SettingName].Keys["Value"].Value;
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
            if (!Config.Sections.Contains(SettingName))
                Config.Sections.Add(SettingName);
            if (!Config.Sections[SettingName].Keys.Contains("Value"))
                Config.Sections[SettingName].Keys.Add("Value", Value.ToString());
            else
                Config.Sections[SettingName].Keys["Value"].Value = Value.ToString();
            Config.Save(theIniPath);
        }
        #endregion

        public static string PatcherPath
        {
            get
            {
                return (string)GetValue("PatcherPath", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Assembly.GetExecutingAssembly().GetName().Name));
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    Directory.CreateDirectory(value);
                    Directory.SetCurrentDirectory(value);
                }
                SetValue("PatcherPath", value);
            }
        }

        public static bool PatcherRunas
        {
            get
            {
                return (bool)GetValue("PatcherRunas", false);
            }
            set
            {
                SetValue("PatcherRunas", value);

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
                return (string)GetValue("GamePath", string.Empty);
            }
            set
            {
                SetValue("GamePath", value);
            }
        }

        public static bool WantToPatchExe
        {
            get
            {
                return (bool)GetValue("WantToPatchExe", false);
            }
            set
            {
                File.Delete(Path.Combine(UserSettings.PatcherPath, Strings.FileName.GameExe));

                SetValue("WantToPatchExe", value);
            }
        }
    }
}
