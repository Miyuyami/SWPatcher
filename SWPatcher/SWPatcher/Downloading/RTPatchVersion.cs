using System;

namespace SWPatcher.Helpers
{
    public class RTPatchVersion
    {
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Build { get; set; }
        public int Revision { get; set; }

        public RTPatchVersion(Version version)
        {
            this.Major = version.Major;
            this.Minor = version.Minor;
            this.Build = version.Build;
            this.Revision = version.Revision;
        }

        public override string ToString()
        {
            return $"{this.Major}_{this.Minor}_{this.Build}_{this.Revision}.RTP";
        }
    }
}
