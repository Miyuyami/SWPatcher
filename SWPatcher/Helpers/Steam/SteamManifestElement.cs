namespace SWPatcher.Helpers.Steam
{
    public abstract class SteamManifestElement
    {
        public string Name { get; protected set; }

        public SteamManifestElement(string name)
        {
            this.Name = name;
        }
    }
}
