using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SWPatcher.General;
using SWPatcher.Helpers.GlobalVar;

namespace SWPatcher.Downloading
{
    public class DownloaderCompletedEventArgs : EventArgs
    {
        public Language Language { get; private set; }
        public bool IsSame { get; private set; }
        public bool Cancelled { get; private set; }
        public Exception Error { get; private set; }

        public DownloaderCompletedEventArgs(bool cancelled, Exception error)
        {
            this.Language = null;
            this.IsSame = false;
            this.Cancelled = cancelled;
            this.Error = error;
        }

        public DownloaderCompletedEventArgs(Language language, bool isSame, bool cancelled, Exception error)
        {
            this.Language = language;
            this.IsSame = isSame;
            this.Cancelled = cancelled;
            this.Error = error;
        }
    }
}
