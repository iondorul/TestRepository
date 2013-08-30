using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace avt.ActionForm.Utils
{
    public class XslUtils
    {
        public string ResourceFile { get; set; }

        public XslUtils()
        {

        }

        public XslUtils(string localeFile)
        {
            ResourceFile = localeFile;
        }

        public string Replace(string content)
        {
            return TokenUtils.Tokenize(content, null, null, false, true);
        }

        /// <summary>
        /// Also have it lowercase so it's most like XSL standards
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public string replace(string content)
        {
            return TokenUtils.Tokenize(content, null, null, false, true);
        }

        public string Tokenize(string content)
        {
            return TokenUtils.Tokenize(content, null, null, false, true);
        }

        /// <summary>
        /// Also have it lowercase so it's most like XSL standards
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public string tokenize(string content)
        {
            return TokenUtils.Tokenize(content, null, null, false, true);
        }


        public string Localize(string key, string defaultText = null)
        {
            var text = DotNetNuke.Services.Localization.Localization.GetString(key, ResourceFile);
            if (string.IsNullOrEmpty(text))
                text = defaultText;
            return text;
        }

        /// <summary>
        /// Also have it lowercase so it's most like XSL standards
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public string localize(string key, string defaultText = null)
        {
            return Localize(key, defaultText);
        }

    }
}
