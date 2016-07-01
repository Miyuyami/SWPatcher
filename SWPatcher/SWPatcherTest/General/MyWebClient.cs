using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace SWPatcherTEST.General
{
    public class MyWebClient : WebClient
    {
        private readonly CookieContainer _container = new CookieContainer();
        public CookieCollection ResponseCookies { get; private set; }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = (HttpWebRequest)base.GetWebRequest(address);
            request.CookieContainer = _container;
            return request;
        }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            var response = (HttpWebResponse)base.GetWebResponse(request);
            if (this.ResponseCookies != null)
                this.ResponseCookies.Add(response.Cookies);
            else
                this.ResponseCookies = response.Cookies;

            return response;
        }
    }
}
