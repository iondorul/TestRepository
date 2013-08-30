using DotNetNuke.Entities.Modules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace avt.ActionForm.Core.Validation
{
    public class WebServiceValidator : IGroupValidator
    {
        public bool IsServerSideValidation { get { return true; } }

        public ValidationResult Validate(IDictionary<string, string> input, GroupValidatorDef validator)
        {
            var timeout = 10; // seconds
            
            string errorRegex, successRegex;
            validator.ValidatorParams.TryGetValue("ErrorRegex", out errorRegex);
            validator.ValidatorParams.TryGetValue("SuccessRegex", out successRegex);
            if (string.IsNullOrEmpty(errorRegex) && string.IsNullOrEmpty(successRegex))
                throw new Exception("Invalid regex check for web service validation " + validator.Title);

            string url;
            if (!validator.ValidatorParams.TryGetValue("url", out url))
                throw new Exception("Invalid URL for web service validation " + validator.Title);

            if (validator.ValidatorParams.ContainsKey("Get") && !string.IsNullOrEmpty(validator.ValidatorParams["Get"]))
                url += (url.IndexOf('?') != -1 ? "&" : "?") + validator.ValidatorParams["Get"];

            url = ActionFormController.ApplyAllTokens(input, url);

            string postData = null;
            if (validator.ValidatorParams.TryGetValue("Post", out postData))
                postData = ActionFormController.ApplyAllTokens(input, postData.Trim());

            var httpHeaders = new Dictionary<string, string>();
            string headersStr = null;
            validator.ValidatorParams.TryGetValue("Headers", out headersStr);
            if (!string.IsNullOrEmpty(headersStr)) {
                foreach (var pair in headersStr.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)) {
                    var isplit = pair.IndexOf(':');
                    if (isplit == -1)
                        continue;
                    httpHeaders[ActionFormController.ApplyAllTokens(input, pair.Substring(0, isplit).Trim())] =
                        ActionFormController.ApplyAllTokens(input, pair.Substring(isplit + 1).Trim());
                }
            }

            string strResponse = "";
            try {

                System.Net.ServicePointManager.Expect100Continue = false;

                HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Timeout = timeout * 1000;

                // append headers
                foreach (string hdr in httpHeaders.Keys) {
                    switch (hdr) {
                        case "Content-Type":
                            httpRequest.ContentType = httpHeaders[hdr];
                            break;
                        case "Accept":
                            httpRequest.Accept = httpHeaders[hdr];
                            break;
                        default:
                            httpRequest.Headers[hdr] = httpHeaders[hdr];
                            break;
                    }
                }

                if (!string.IsNullOrEmpty(postData)) {
                    httpRequest.Method = "POST";
                    if (!httpHeaders.ContainsKey("Content-Type"))
                        httpRequest.ContentType = "application/x-www-form-urlencoded";
                    httpRequest.ContentLength = postData.Length;
                    System.IO.Stream newStream = httpRequest.GetRequestStream();

                    // Send the data.
                    System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                    byte[] data = encoding.GetBytes(postData);

                    newStream.Write(data, 0, data.Length);
                    newStream.Close();
                }

                HttpWebResponse response = (HttpWebResponse)httpRequest.GetResponse();
                System.IO.StreamReader reader = new System.IO.StreamReader(response.GetResponseStream());
                strResponse = reader.ReadToEnd();
                response.Close();

                strResponse = strResponse.Trim();

            } catch (WebException ex) {
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream)) {
                    var err = reader.ReadToEnd();
                    throw new Exception("Error loading stream from " + url + " (inner exception: " + ex.Message + "): " + err, ex);
                }
            } catch (Exception ex) {
                throw new Exception("Error loading stream from " + url + " (inner exception: " + ex.Message + ")", ex);
            }

            return ParseResonse(validator, strResponse, successRegex, errorRegex);
        }

        ValidationResult ParseResonse(GroupValidatorDef validator, string response, string successRegex, string errorRegex)
        {
            var res = new ValidationResult();
            if (!string.IsNullOrEmpty(successRegex)) {
                var successMatch = Regex.Match(response, successRegex);
                res.IsValid = successMatch.Groups.Count >= 2;
            }

            if (!res.IsValid) {
                var errMatch = Regex.Match(response, errorRegex);
                res.IsValid = true;
                if (errMatch.Groups.Count >= 2) {
                    res.IsValid = false;
                    res.ErrMessage = string.IsNullOrEmpty(validator.ErrorMessageDefault) ? errMatch.Groups[1].Value : validator.ErrorMessageDefault;
                }
            }

            return res;
        }
    }
}
