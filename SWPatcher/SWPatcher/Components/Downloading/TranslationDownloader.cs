using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Net;

namespace SWPatcher.Components.Downloading
{
    public class FileDownloader : Component
    {
        private readonly WebClient webClient;
        private readonly BackgroundWorker worker;
        private Queue<Uri> downloadQueue;

        public FileDownloader(List<string> downloadQueueList)
        {
            
        }
    }
}
