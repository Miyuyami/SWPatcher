using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SWPatcher
{
    public partial class Form1 : Form
    {
        BackgroundWorker BWorker_LoadEverything;
        #region "Some odd enum"
        enum DownloadCode : short
        {
            None = 0,
            Success,
            Failed
        }
        #endregion

        public Form1()
        {
            InitializeComponent();
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            this.BWorker_LoadEverything = new BackgroundWorker();
            this.BWorker_LoadEverything.WorkerReportsProgress = false;
            this.BWorker_LoadEverything.WorkerSupportsCancellation = true;
            this.BWorker_LoadEverything.DoWork += new DoWorkEventHandler(BWorker_LoadEverything_DoWork);
            this.BWorker_LoadEverything.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BWorker_LoadEverything_RunWorkerCompleted);
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            // should i run in Form_Load instead of Form_Shown ... ?
            this.BWorker_LoadEverything.RunWorkerAsync();
        }

        private void BWorker_LoadEverything_DoWork(object sender, DoWorkEventArgs e)
        {
            /* Now head to the English Patch checking .... and remind the user when
             * it has newer version
             * OR
             * the original data12.v has changed which need to be patched again. 
             */
            // Should i get response from another file ... ? or using the ini above ?

        }

        private void BWorker_LoadEverything_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
                MessageBox.Show(e.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {

            }
        }

        private DownloadCode[] HTTPDownloadList(string[] sUrlList, string sFolderDestination, string sUserAgent = "", bool bUseProgressBar = true)
        {
            System.Collections.Generic.List<DownloadCode> result = new List<DownloadCode>();
            using (System.Net.WebClient theWebClient = new System.Net.WebClient())
            {
                theWebClient.Proxy = null;
                if (string.IsNullOrWhiteSpace(sUserAgent) == false)
                    theWebClient.Headers.Add(System.Net.HttpRequestHeader.UserAgent, sUserAgent);
                long ByteProcessed = 0;
                byte[] BufferedByte = new byte[1025];
                string tmpStringPath = string.Empty;
                int bytesRead;
                for (short i = 0; i < sUrlList.Length; i++)
                {
                    try
                    {
                        tmpStringPath = sFolderDestination + "\\" + System.IO.Path.GetFileName(sUrlList[i]);
                        using (System.IO.Stream theWebClientStream = theWebClient.OpenRead(sUrlList[i]))
                        using (System.IO.FileStream localStream = new System.IO.FileStream(tmpStringPath + ".dtmp", System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.Read))
                        {
                            bytesRead = theWebClientStream.Read(BufferedByte, 0, 1024);
                            while (bytesRead > 0)
                            {
                                localStream.Write(BufferedByte, 0, bytesRead);
                                ByteProcessed += bytesRead;
                                bytesRead = theWebClientStream.Read(BufferedByte, 0, 1024);
                                if (bUseProgressBar == true)
                                    ProgressBarSetValue(progressBar1, Convert.ToInt32(ByteProcessed));
                            }
                        }
                        if (System.IO.File.Exists(tmpStringPath) == true)
                            System.IO.File.Delete(tmpStringPath);
                        System.IO.File.Move(tmpStringPath + ".dtmp", tmpStringPath);
                        result.Add(DownloadCode.Success);
                    }
                    catch (Exception ex)
                    {
                        result.Add(DownloadCode.Failed);
                    }
                }
                tmpStringPath = null;
                BufferedByte = null;
            }
            return result.ToArray();
        }

        private DownloadCode HTTPDownloadFile(string sUrlString, string sDestination, string sUserAgent = "Mozilla/4.0", int iTimeOut = 5000, bool bUseProgressBar = true)
        {
            DownloadCode result = DownloadCode.None; //i dunno if i should make a variable for this

            for (short i = 1; i <= 3; i++)
            {
                try
                {
                    System.Net.HttpWebRequest TheRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(sUrlString);
                    TheRequest.Timeout = iTimeOut;
                    TheRequest.UserAgent = sUserAgent;
                    TheRequest.AllowAutoRedirect = true;
                    TheRequest.KeepAlive = false;
                    TheRequest.Proxy = null;
                    System.Net.HttpWebResponse TheResponse = (System.Net.HttpWebResponse)TheRequest.GetResponse();
                    if (TheResponse != null)
                    {
                        long ByteProcessed = 0;
                        long FileSize = 1;
                        byte[] BufferedByte = new byte[1025];

                        if (TheResponse.ContentLength > 0)
                            FileSize = TheResponse.ContentLength;
                        ProgressBarSetMaximum(progressBar1, Convert.ToInt32(FileSize));
                        using (System.IO.Stream TheResponseStream = TheResponse.GetResponseStream())
                        using (System.IO.FileStream localStream = new System.IO.FileStream(sDestination + ".dtmp", System.IO.FileMode.Create, System.IO.FileAccess.ReadWrite))
                        {
                            int bytesRead;
                            bytesRead = TheResponseStream.Read(BufferedByte, 0, 1024);
                            while ((bytesRead > 0))
                            {
                                localStream.Write(BufferedByte, 0, bytesRead);
                                ByteProcessed += bytesRead;
                                bytesRead = TheResponseStream.Read(BufferedByte, 0, 1024);
                                if (bUseProgressBar == true)
                                    ProgressBarSetValue(progressBar1, Convert.ToInt32(ByteProcessed));
                            }
                        }
                        TheResponse.Close();
                        //double check the download
                        if (ByteProcessed == FileSize)
                        {
                            try
                            {
                                if (System.IO.File.Exists(sDestination) == true)
                                    System.IO.File.Delete(sDestination);
                                string MyLocalFile = System.IO.Path.GetFileName(sDestination);
                                System.IO.File.Move(sDestination + ".dtmp", sDestination);

                            }
                            finally
                            {
                                // if somehow failed to rename, delete the leftover. or .... perform a complicated check ... ?
                                if (System.IO.File.Exists(sDestination + ".dtmp") == true)
                                    System.IO.File.Delete(sDestination + ".dtmp");
                            }
                        }
                        else
                        {
                            Console.WriteLine(Debugging_GenerateString("Downloaded file but weird: Content-Length mismatch with downloaded bytes"));
                            // should I delete or ...... still accept the file ... ? but i guess this can't be happened....
                            if (System.IO.File.Exists(sDestination + ".dtmp") == true)
                                System.IO.File.Delete(sDestination + ".dtmp");
                        }
                        break;
                    }
                }
                catch (System.Net.WebException NetEx)
                {
                    if (NetEx.Response != null)
                    {
                        System.Net.HttpWebResponse TheResponse = (System.Net.HttpWebResponse)NetEx.Response;
                        Console.WriteLine(Debugging_GenerateString("Response Exception: " + NetEx.Message));
                        result = DownloadCode.Failed;
                        if (TheResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                            break;

                    }
                }
                catch (UriFormatException)
                {
                    Console.WriteLine(Debugging_GenerateString("Uri Format Exception: " + sUrlString));
                    result = DownloadCode.Failed;
                    break;
                }
                catch (Exception ex)
                {
                    result = DownloadCode.Failed;
                    Console.WriteLine(Debugging_GenerateString("Error: " + ex.Message));
                }
            }
            return result;
        }

        private delegate void _ProgressBarSetValue(ProgressBar target, int iValue);
        private void ProgressBarSetValue(ProgressBar target, int iValue)
        {
            if (target.InvokeRequired)
                target.Invoke(new _ProgressBarSetValue(ProgressBarSetValue), new object[] { target, iValue });
            else
                if (iValue <= target.Maximum)
                    target.Value = iValue;
        }

        private delegate void _ProgressBarSetMaximum(ProgressBar target, int iValue);
        private void ProgressBarSetMaximum(ProgressBar target, int iValue)
        {
            if (target.InvokeRequired)
                target.Invoke(new _ProgressBarSetMaximum(ProgressBarSetMaximum), new object[] { target, iValue });
            else
                target.Maximum = iValue;
        }

        private delegate void _TextBox_SetFocus(TextBox Target);
        private void TextBox_SetFocus(TextBox Target)
        {
            if (Target.InvokeRequired)
            {
                Target.Invoke(new _TextBox_SetFocus(TextBox_SetFocus), new object[] { Target });
            }
            else
            {
                Target.Focus();
            }
        }

        private delegate DialogResult _MessageBoxInvokeShow(string sText, string sCaption, MessageBoxButtons iMessageBoxButtons, MessageBoxIcon iMessageBoxIcon);
        private DialogResult MessageBoxInvokeShow(string sText, string sCaption, MessageBoxButtons iMessageBoxButtons, MessageBoxIcon iMessageBoxIcon)
        {
            if (this.InvokeRequired)
            {
                return (DialogResult)this.Invoke(new _MessageBoxInvokeShow(MessageBoxInvokeShow), new object[] { sText, sCaption, iMessageBoxButtons, iMessageBoxIcon });
            }
            else
            {
                return MessageBox.Show(sText, sCaption, iMessageBoxButtons, iMessageBoxIcon);
            }
        }

        



        private string HTTPDoSimpleRequest(string sUrlString, string sUserAgent = "Mozilla/4.0", int iTimeOut = 5000)
        {
            string result = string.Empty;
            for (short i = 1; i <= 3; i++)
            {
                try
                {
                    System.Net.HttpWebRequest TheRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(sUrlString);
                    TheRequest.Timeout = iTimeOut;
                    TheRequest.UserAgent = sUserAgent;
                    TheRequest.AllowAutoRedirect = true;
                    TheRequest.KeepAlive = false;
                    TheRequest.Proxy = null;
                    System.Net.HttpWebResponse TheResponse = (System.Net.HttpWebResponse)TheRequest.GetResponse();
                    if (TheResponse != null)
                    {
                        using (System.IO.Stream TheResponseStream = TheResponse.GetResponseStream())
                        using (System.IO.StreamReader TheStreamReader = new System.IO.StreamReader(TheResponseStream, Encoding.UTF8))
                        {
                            result = TheStreamReader.ReadToEnd();
                        }
                        TheResponse.Close();
                        break;
                    }
                }
                catch (System.Net.WebException NetEx)
                {
                    if (NetEx.Response != null)
                    {
                        System.Net.HttpWebResponse TheResponse = (System.Net.HttpWebResponse)NetEx.Response;
                        Console.WriteLine(Debugging_GenerateString("Response Exception: " + NetEx.Message));
                        if (TheResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                            break;
                        
                    }
                }
                catch (UriFormatException)
                {
                    Console.WriteLine(Debugging_GenerateString("Uri Format Exception: " + sUrlString));
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(Debugging_GenerateString("Error: " + ex.Message));
                }
            }
            return result;
        }

        private string Debugging_GenerateString(string sMessage)
        {
            DateTime CurrentDateTime = DateTime.Now;
            return ("[" + CurrentDateTime.ToShortDateString() + " " + CurrentDateTime.ToShortTimeString() + "] " + sMessage);
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

        }
    }
}
