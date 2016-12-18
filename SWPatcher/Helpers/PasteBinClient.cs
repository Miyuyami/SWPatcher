/*
 * This file is part of Soulworker Patcher.
 * Copyright (C) 2016 Miyu
 * 
 * Soulworker Patcher is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * Soulworker Patcher is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Soulworker Patcher. If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Runtime.Serialization;

namespace SWPatcher.Helpers
{
    public enum PasteBinExpiration
    {
        Never,
        TenMinutes,
        OneHour,
        OneDay,
        OneWeek,
        OneMonth
    }

    public class PasteBinEntry
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public string Format { get; set; }
        public bool Private { get; set; }
        public PasteBinExpiration Expiration { get; set; }
    }

    class PasteBinClient
    {
        private const string ApiPostUrl = "http://pastebin.com/api/api_post.php";
        private const string ApiLoginUrl = "http://pastebin.com/api/api_login.php";

        private readonly string ApiDevKey;
        public string UserName { get; private set; }
        public string ApiUserKey { get; private set; }

        public PasteBinClient(string apiDevKey)
        {
            if (String.IsNullOrEmpty(apiDevKey))
                throw new ArgumentNullException("apiDevKey");
            this.ApiDevKey = apiDevKey;
        }

        public void Login(string userName, string password)
        {
            if (String.IsNullOrEmpty(userName))
                throw new ArgumentNullException("userName");
            if (String.IsNullOrEmpty(password))
                throw new ArgumentNullException("password");

            NameValueCollection parameters = GetBaseParameters();
            parameters[ApiParameters.UserName] = userName;
            parameters[ApiParameters.UserPassword] = password;

            byte[] bytes;
            using (var client = new WebClient())
                bytes = client.UploadValues(ApiLoginUrl, parameters);
            string resp = GetResponseText(bytes);
            if (resp.StartsWith("Bad API request"))
                throw new PasteBinApiException(resp);

            this.UserName = userName;
            this.ApiUserKey = resp;
        }

        public void Logout()
        {
            this.UserName = null;
            this.ApiUserKey = null;
        }

        public string Paste(PasteBinEntry entry)
        {
            if (entry == null)
                throw new ArgumentNullException("entry");
            if (String.IsNullOrEmpty(entry.Text))
                throw new ArgumentException("The paste text must be set", "entry");

            NameValueCollection parameters = GetBaseParameters();
            parameters[ApiParameters.Option] = "paste";
            parameters[ApiParameters.PasteCode] = entry.Text;
            SetIfNotEmpty(parameters, ApiParameters.PasteName, entry.Title);
            SetIfNotEmpty(parameters, ApiParameters.PasteFormat, entry.Format);
            SetIfNotEmpty(parameters, ApiParameters.PastePrivate, entry.Private ? "2" : "1");
            SetIfNotEmpty(parameters, ApiParameters.PasteExpireDate, FormatExpireDate(entry.Expiration));
            SetIfNotEmpty(parameters, ApiParameters.UserKey, this.ApiUserKey);

            byte[] bytes;
            using (var client = new WebClient())
                bytes = client.UploadValues(ApiPostUrl, parameters);
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
            if (!String.IsNullOrEmpty(value))
            {
                parameters[name] = value;
            }
        }

        private NameValueCollection GetBaseParameters()
        {
            var parameters = new NameValueCollection
            {
                [ApiParameters.DevKey] = this.ApiDevKey
            };
            return parameters;
        }

        private static string GetResponseText(byte[] bytes)
        {
            using (var reader = new StreamReader(new MemoryStream(bytes)))
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

    [Serializable]
    public class PasteBinApiException : Exception
    {
        public PasteBinApiException()
        {

        }

        public PasteBinApiException(string message) : base(message)
        {

        }

        public PasteBinApiException(string message, Exception innerException) : base(message, innerException)
        {

        }

        protected PasteBinApiException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
    }
}