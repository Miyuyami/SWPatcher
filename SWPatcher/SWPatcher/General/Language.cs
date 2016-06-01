using System;

namespace SWPatcher.General
{
    public class Language
    {
        public string Lang { get; set; }
        public DateTime LastUpdate { get; private set; }

        public Language(string lang, DateTime lastUpdate)
        {
            this.Lang = lang;
            this.LastUpdate = lastUpdate;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType())
                return false;
            Language language = obj as Language;
            return Lang == language.Lang && LastUpdate == language.LastUpdate;
        }

        public override int GetHashCode()
        {
            return Lang.GetHashCode();
        }

        public override string ToString()
        {
            return Lang;
        }
    }
}
