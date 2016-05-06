using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SWPatcher.General;

namespace SWPatcher.Patching
{
    public class PatcherCompletedEventArgs : EventArgs
    {
        public Language Language { get; private set; }
        public bool Cancelled { get; private set; }
        public Exception Error { get; private set; }

        public PatcherCompletedEventArgs(Language language, bool cancelled, Exception error)
        {
            this.Language = language;
            this.Cancelled = cancelled;
            this.Error = error; 
        }
    }
}
