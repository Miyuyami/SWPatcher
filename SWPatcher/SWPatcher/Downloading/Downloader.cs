using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Net;

namespace SWPatcher.Downloading
{
    public class Downloader : IDisposable
    {
        private readonly BackgroundWorker Worker;
        private readonly WebClient Client;
        private List<string> DownloadList;
        private bool disposed = false;

        public Downloader(List<Uri> uriList)
        {
            
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;
            if (disposing)
            {
                if (Client != null)
                    Client.Dispose();
            }
            disposed = true;
        }

        ~Downloader()
        {
            Dispose(false);
        }
    }
}
