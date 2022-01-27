using System.Collections.Generic;
using System.Linq;

namespace SWPatcher.Helpers.Steam
{
    public class SteamManifestSection : SteamManifestElement
    {
        public Dictionary<string, SteamManifestElement> Elements { get; set; }

        public SteamManifestSection(string name, Dictionary<string, SteamManifestElement> elements = null) : base(name)
        {
            this.Elements = elements ?? new Dictionary<string, SteamManifestElement>();
        }

        public IEnumerable<SteamManifestEntry> EnumerateEntries()
        {
            return this.Elements.Values.SelectMany(GetElementEntries);
        }

        private static IEnumerable<SteamManifestEntry> GetElementEntries(SteamManifestElement element)
        {
            if (element is SteamManifestEntry entry)
            {
                yield return entry;
            }
            else if (element is SteamManifestSection deepSection)
            {
                foreach (var x in deepSection.EnumerateEntries())
                {
                    yield return x;
                }
            }
        }
    }
}
