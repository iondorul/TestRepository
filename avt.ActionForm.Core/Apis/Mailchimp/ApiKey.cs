using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace avt.ActionForm.Apis.MailChimp.Net
{
    public class ApiKey
    {
        internal const string ApiUrlFormat = "http://{0}.api.mailchimp.com/1.3/";

        public string StrApiKey { get; private set; }
        public string Url { get; private set; }

        public ApiKey(string strKey)
        {
            Load(strKey);
        }

        void Load(string strKey)
        {
            if (string.IsNullOrEmpty(strKey))
                throw new ArgumentNullException("Key is required");

            // TODO: some validation

            StrApiKey = strKey;
            if (strKey.IndexOf('-') == -1)
                throw new ArgumentException("API Key is invalid - could not determine endpoint.");

            var endPoint = strKey.Substring(strKey.IndexOf('-') + 1);
            if (string.IsNullOrEmpty(endPoint))
                throw new ArgumentException("API Key is invalid - could not determine endpoint.");

            Url = string.Format(ApiUrlFormat, endPoint);
        }

        public override string ToString()
        {
            return StrApiKey;
        }
    }
}
