using SWPatcher.Properties;

namespace SWPatcher.Helpers.GlobalVar
{
    public static class Paths
    {
        public static string PatcherRoot { get { return System.AppDomain.CurrentDomain.BaseDirectory; } }
        public static string GameRoot
        {
            get
            {
                return Settings.Default.SoulWorkerDirectory;
            }
            set
            {
                Settings.Default.SoulWorkerDirectory = value;
                Settings.Default.Save();
            }
        }

        public static string ExclusiveTempFolder
        {
            get
            {
                return (System.IO.Path.GetTempPath() + "SWPatcher\\" + ((System.Runtime.InteropServices.GuidAttribute)System.Reflection.Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(System.Runtime.InteropServices.GuidAttribute), false).GetValue(0)).Value.ToString());
            }
        }
    }
}
