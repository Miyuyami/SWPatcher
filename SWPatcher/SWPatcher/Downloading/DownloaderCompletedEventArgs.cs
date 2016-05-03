using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SWPatcher.General;
using SWPatcher.Helpers.GlobalVar;

namespace SWPatcher.Downloading
{
    public class DownloaderDownloadCompletedEventArgs : EventArgs
    {
        public Language Language { get; private set; }
        public bool IsSame { get; private set; }

        public DownloaderDownloadCompletedEventArgs(Language language)
        {
            this.Language = language;
            this.IsSame = false;
        }

        public DownloaderDownloadCompletedEventArgs(Language language, bool isSame)
        {
            this.Language = language;
            this.IsSame = isSame;
        }
    }
}
