using System;
using SWPatcherTest.General;

namespace SWPatcherTest.Patching
{
    public class PatcherCompletedEventArgs : EventArgs
    {
        public Language Language { get; private set; }
        public bool Cancelled { get; private set; }
        public Exception Error { get; private set; }

        public PatcherCompletedEventArgs(bool cancelled, Exception error)
        {
            this.Language = null;
            this.Cancelled = cancelled;
            this.Error = error;
        }

        public PatcherCompletedEventArgs(Language language, bool cancelled, Exception error)
        {
            this.Language = language;
            this.Cancelled = cancelled;
            this.Error = error;
        }
    }
}
