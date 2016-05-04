using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public override string ToString()
        {
            return Lang;
        }
    }
}
