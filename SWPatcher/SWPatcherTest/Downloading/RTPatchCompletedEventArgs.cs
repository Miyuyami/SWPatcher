using System;

namespace SWPatcherTest.Helpers
{
    public class RTPatchCompletedEventArgs : EventArgs
    {
        public bool Cancelled { get; private set; }
        public Exception Error { get; private set; }

        public RTPatchCompletedEventArgs()
        {
            this.Cancelled = false;
            this.Error = null;
        }

        public RTPatchCompletedEventArgs(bool cancelled, Exception error)
        {
            this.Cancelled = cancelled;
            this.Error = error;
        }
    }
}
