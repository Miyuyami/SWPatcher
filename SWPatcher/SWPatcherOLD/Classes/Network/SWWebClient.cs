using System;
using System.Net;

namespace SWPatcher.Classes.Network
{
    class SWWebClient
    {
        WebClient _manager;
        bool _isbusy;
        bool _cancel;
        bool _isdispose;

        #region "Constructor"
        public SWWebClient(string sBaseAddress, IWebProxy oProxy, System.Net.Cache.RequestCachePolicy oCachePolicy)
        {
            this._manager = new WebClient();
            this._manager.BaseAddress = sBaseAddress;
            this._manager.Proxy = oProxy;
            this._manager.CachePolicy = oCachePolicy;
            //this._manager.Headers.Add( HttpRequestHeader.UserAgent, "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
            this._isbusy = false;
            this._cancel = false;
            this._isdispose = false;
        }

        public SWWebClient(string sBaseAddress) : this(sBaseAddress, null, new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore)) { }

        public SWWebClient() : this(string.Empty, null, new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore)) { }
        #endregion

        #region "Properties"
        public string BaseAddress
        {
            get { return this._manager.BaseAddress; }
            set { this._manager.BaseAddress = value; }
        }

        public WebHeaderCollection Headers
        {
            get { return this._manager.Headers; }
        }

        public IWebProxy Proxy
        {
            get { return this._manager.Proxy; }
            set { this._manager.Proxy = value; }
        }

        public System.Net.Cache.RequestCachePolicy CachePolicy
        {
            get { return this._manager.CachePolicy; }
            set { this._manager.CachePolicy = value; }
        }

        public bool IsBusy
        {
            get { return (this._isbusy || this._manager.IsBusy); }
        }

        public bool IsDisposed
        {
            get { return this._isdispose; }
        }
        #endregion

        #region "Methods"

        public string DownloadString(string sUrl, short retry = 3)
        {
            if (this._isdispose)
                return null;
            this._isbusy = true;
            string result = string.Empty;
            for (short cou = 0; cou <= retry; cou++)
            {
                try
                {
                    result = this._manager.DownloadString(sUrl);
                    break;
                }
                catch (System.Net.WebException webEx)
                {
                    if (webEx.Response != null)
                        if (((System.Net.HttpWebResponse)webEx.Response).StatusCode == HttpStatusCode.NotFound)
                            break;
                }
            }
            this._isbusy = false;
            this._cancel = false;
            return result;
        }

        //sync
        public bool DownloadFile(string sUrl, string sDestination, short retry = 3, System.Windows.Forms.ProgressBar oProgressBar = null)
        {
            if (this._isdispose)
                return false;
            this._isbusy = true;
            this._cancel = false;
            bool result = false;
            System.IO.Stream networkStream = null;
            System.IO.FileStream localStream = null;
            long FileSize = 1;
            long processed = 0;
            int reading = 0;
            byte[] byteAray = new byte[1025];
            for (short cou = 0; cou <= retry; cou++)
            {
                try
                {
                    localStream = new System.IO.FileStream(sDestination + ".dtmp", System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.Read);
                    networkStream = this._manager.OpenRead(sUrl);

                    if (string.IsNullOrEmpty(this._manager.ResponseHeaders[HttpResponseHeader.ContentLength]) == false)
                        if (this._manager.ResponseHeaders[HttpResponseHeader.ContentLength] == "0")
                        {
                            result = false;
                            break;
                        }
                        else
                            FileSize = long.Parse(this._manager.ResponseHeaders[HttpResponseHeader.ContentLength]);

                    reading = networkStream.Read(byteAray, 0, 1024);
                    while ((reading > 0))
                    {
                        localStream.Write(byteAray, 0, reading);
                        processed += reading;
                        reading = networkStream.Read(byteAray, 0, 1024);
                        if (oProgressBar != null)
                            WeirdInokeProgressBarMaxmimum(oProgressBar, (int)((processed / FileSize) * 100));
                    }
                    
                    result = true;
                    break;
                }
                catch (System.Net.WebException webEx)
                {
                    result = false;
                    if (webEx.Response != null)
                        if (((System.Net.HttpWebResponse)webEx.Response).StatusCode == HttpStatusCode.NotFound)
                            break;
                }
                catch (Exception)
                {
                    result = false;
                }
                finally
                {
                    networkStream.Close();
                    localStream.Close();
                }
            }
            networkStream.Close();
            byteAray = null;
            localStream.Close();
            if (this._cancel == true)
            {
                System.IO.File.Delete(sDestination + ".dtmp");
            }
            else
            {
                System.IO.File.Delete(sDestination);
                System.IO.File.Move(sDestination + ".dtmp", sDestination);
            }
            this._isbusy = false;
            return result;
        }

        public void CancelOperation()
        {
            this._cancel = true;
        }

        public void Close()
        {
            if (this._manager.IsBusy)
                this._manager.CancelAsync();
            if (this._isbusy)
                this.CancelOperation();
            this._manager.Dispose();
            this._isdispose = true;
        }
        #endregion

        #region "Private"

        private delegate void _WeirdInokeProgressBarMaxmimum(System.Windows.Forms.ProgressBar oProgressBar, int iValue);
        private void WeirdInokeProgressBarMaxmimum(System.Windows.Forms.ProgressBar oProgressBar, int iValue)
        {
            if (oProgressBar.InvokeRequired)
                oProgressBar.Invoke(new _WeirdInokeProgressBarMaxmimum(WeirdInokeProgressBarMaxmimum), new object[] { oProgressBar, iValue });
            else
                oProgressBar.Maximum = iValue;
        }

        private delegate void _WeirdInokeProgressBarValue(System.Windows.Forms.ProgressBar oProgressBar, int iValue);
        private void WeirdInokeProgressBarValue(System.Windows.Forms.ProgressBar oProgressBar, int iValue)
        {
            if (oProgressBar.InvokeRequired)
                oProgressBar.Invoke(new _WeirdInokeProgressBarValue(WeirdInokeProgressBarValue), new object[] { oProgressBar, iValue });
            else
                if (iValue <= oProgressBar.Maximum)
                    oProgressBar.Value = iValue;
        }

        #endregion

        #region "Events"

        #endregion
    }
}
