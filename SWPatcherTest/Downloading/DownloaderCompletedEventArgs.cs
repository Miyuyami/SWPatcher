using System;
using SWPatcherTest.General;

namespace SWPatcherTest.Helpers
{
    public class DownloaderCompletedEventArgs : EventArgs
    {
        public Language Language { get; private set; }
        public bool Cancelled { get; private set; }
        public Exception Error { get; private set; }

        public DownloaderCompletedEventArgs(bool cancelled, Exception error)
        {
            this.Language = null;
            this.Cancelled = cancelled;
            this.Error = error;
        }

        public DownloaderCompletedEventArgs(Language language, bool cancelled, Exception error)
        {
            this.Language = language;
            this.Cancelled = cancelled;
            this.Error = error;
        }
    }
}
