using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using SWPatcher.Helpers;
using System.Net;
using SWPatcher.General;
using SWPatcher.Helpers.GlobalVar;
using SWPatcher.Downloading;

namespace SWPatcher.Forms
{
    partial class Main
    {
        private void workerCheck_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void workerCheck_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.toolStripProgressBar.Increment(e.ProgressPercentage);
        }

        private void workerCheck_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        private void workerPatch_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void workerPatch_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.toolStripProgressBar.Increment(e.ProgressPercentage);
        }

        private void workerPatch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.State = States.Idle;
        }
    }
}
