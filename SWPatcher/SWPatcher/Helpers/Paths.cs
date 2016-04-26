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
        public static string PatcherRoot = AppDomain.CurrentDomain.BaseDirectory;
        public static string GameRoot = Settings.Default.SoulWorkerDirectory;
        public static string GameExe = Path.Combine(GameRoot, "");
    }
}
