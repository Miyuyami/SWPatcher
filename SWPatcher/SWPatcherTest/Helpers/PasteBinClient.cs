using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;

namespace SWPatcher.Helpers
{
    class PasteBinClient
    {
        private const string _apiPostUrl = "http://pastebin.com/api/api_post.php";
        private const string _apiLoginUrl = "http://pastebin.com/api/api_login.php";


        private WebClient _client;
        private readonly string _apiDevKey;
        private string _userName;
        private string _apiUserKey;

        public PasteBinClient(string apiDevKey)
        {
            if (string.IsNullOrEmpty(apiDevKey))
                throw new ArgumentNullException("apiDevKey");
            _apiDevKey = apiDevKey;
            _client = new WebClient();
        }

        public string UserName
        {
            get { return _userName; }
        }

        public void Login(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException("userName");
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException("password");

            var parameters = GetBaseParameters();
            parameters[ApiParameters.UserName] = userName;
            parameters[ApiParameters.UserPassword] = password;
            
            byte[] bytes = _client.UploadValues(_apiLoginUrl, parameters);
            string resp = GetResponseText(bytes);
            if (resp.StartsWith("Bad API request"))
                throw new PasteBinApiException(resp);

            _userName = userName;
            _apiUserKey = resp;
        }

        public void Logout()
        {
            _userName = null;
            _apiUserKey = null;
            _client.Dispose();
        }

        public string Paste(PasteBinEntry entry)
        {
            if (entry == null)
                throw new ArgumentNullException("entry");
            if (string.IsNullOrEmpty(entry.Text))
                throw new ArgumentException("The paste text must be set", "entry");

            var parameters = GetBaseParameters();
            parameters[ApiParameters.Option] = "paste";
            parameters[ApiParameters.PasteCode] = entry.Text;
            SetIfNotEmpty(parameters, ApiParameters.PasteName, entry.Title);
            SetIfNotEmpty(parameters, ApiParameters.PasteFormat, entry.Format);
            SetIfNotEmpty(parameters, ApiParameters.PastePrivate, entry.Private ? "2" : "1");
            SetIfNotEmpty(parameters, ApiParameters.PasteExpireDate, FormatExpireDate(entry.Expiration));
            SetIfNotEmpty(parameters, ApiParameters.UserKey, _apiUserKey);
            
            byte[] bytes = _client.UploadValues(_apiPostUrl, parameters);
            string resp = GetResponseText(bytes);
            if (resp.StartsWith("Bad API request"))
                throw new PasteBinApiException(resp);

            return resp;
        }

        private static string FormatExpireDate(PasteBinExpiration expiration)
        {
            switch (expiration)
            {
                case PasteBinExpiration.Never:
                    return "N";
                case PasteBinExpiration.TenMinutes:
                    return "10M";
                case PasteBinExpiration.OneHour:
                    return "1H";
                case PasteBinExpiration.OneDay:
                    return "1D";
                case PasteBinExpiration.OneWeek:
                    return "1W";
                case PasteBinExpiration.OneMonth:
                    return "1M";
                default:
                    throw new ArgumentException("Invalid expiration date");
            }
        }

        private static void SetIfNotEmpty(NameValueCollection parameters, string name, string value)
        {
            if (!string.IsNullOrEmpty(value))
                parameters[name] = value;
        }

        private NameValueCollection GetBaseParameters()
        {
            var parameters = new NameValueCollection();
            parameters[ApiParameters.DevKey] = _apiDevKey;

            return parameters;
        }

        private static string GetResponseText(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            using (var reader = new StreamReader(ms))
            {
                return reader.ReadToEnd();
            }
        }

        private static class ApiParameters
        {
            public const string DevKey = "api_dev_key";
            public const string UserKey = "api_user_key";
            public const string Option = "api_option";
            public const string UserName = "api_user_name";
            public const string UserPassword = "api_user_password";
            public const string PasteCode = "api_paste_code";
            public const string PasteName = "api_paste_name";
            public const string PastePrivate = "api_paste_private";
            public const string PasteFormat = "api_paste_format";
            public const string PasteExpireDate = "api_paste_expire_date";
        }
    }

    public class PasteBinApiException : Exception
    {
        public PasteBinApiException(string message)
            : base(message)
        {
        }
    }

    public class PasteBinEntry
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public string Format { get; set; }
        public bool Private { get; set; }
        public PasteBinExpiration Expiration { get; set; }
    }

    public enum PasteBinExpiration
    {
        Never,
        TenMinutes,
        OneHour,
        OneDay,
        OneWeek,
        OneMonth
    }
}