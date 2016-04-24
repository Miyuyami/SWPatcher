using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SWPatcher.Properties;

namespace SWPatcher.Helpers
{
    public static class Paths
    {
        public static string PatcherRoot { get { return AppDomain.CurrentDomain.BaseDirectory; } }
        public static string GameRoot { get { return Settings.Default.SoulWorkerDirectory; } }
        public static string GameExe { get { return Path.Combine(GameRoot, ""); } }
    }
}
