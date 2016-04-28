
namespace SWPatcher.Helpers.GlobalVars
{
    public static class Paths
    {
        public static string PatcherRoot { get { return System.AppDomain.CurrentDomain.BaseDirectory; } }
        public static string GameRoot { get { return SWPatcher.Properties.Settings.Default.SoulWorkerDirectory; } }
    }
}
