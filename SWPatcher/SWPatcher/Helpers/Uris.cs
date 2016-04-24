using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SWPatcher.Helpers
{
    public static class Uris
    {
        public static Uri SWHQWebsite { get { return new Uri("http://soulworkerhq.com/"); } }
        public static Uri PatcherGitHubHome { get { return new Uri("https://raw.githubusercontent.com/Miyuyami/SWHQPatcher/master/"); } }
        public static Uri SoulWorkerSettingsHome { get { return new Uri("http://down.hangame.co.jp/jp/purple/plii/j_sw/"); } }
    }
}
