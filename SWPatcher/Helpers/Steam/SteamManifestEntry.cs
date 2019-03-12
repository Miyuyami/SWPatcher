namespace SWPatcher.Helpers.Steam
{
    public class SteamManifestEntry : SteamManifestElement
    {
        public string Value { get; set; }

        public SteamManifestEntry(string name, string value) : base(name)
        {
            this.Value = value;
        }
    }
}
