using System.Collections.Generic;

namespace SWPatcher.Helpers.Steam
{
    public class SteamManifestSection : SteamManifestElement
    {
        public Dictionary<string, SteamManifestElement> Elements { get; set; }

        public SteamManifestSection(string name, Dictionary<string, SteamManifestElement> elements = null) : base(name)
        {
            this.Elements = elements ?? new Dictionary<string, SteamManifestElement>();
        }
    }
}
