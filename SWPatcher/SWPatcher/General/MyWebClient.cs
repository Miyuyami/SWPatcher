using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace SWPatcher.General
{
    public class MyWebClient : WebClient
    {
        private readonly CookieContainer _container = new CookieContainer();
        public CookieCollection ResponseCookies { get; private set; }

        protected override WebRequest GetWebRequest(Uri address)
        {
            HttpWebRequest request = (HttpWebRequest)base.GetWebRequest(address);
            request.ClientCertificates.Add(new X509Certificate());
            request.CookieContainer = _container;
            return request;
        }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            HttpWebResponse response = (HttpWebResponse)base.GetWebResponse(request);
            if (this.ResponseCookies != null)
                this.ResponseCookies.Add(response.Cookies);
            else
                this.ResponseCookies = response.Cookies;

            return response;
        }
    }
}
