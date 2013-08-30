using avt.ActionForm.Core.Form;
using avt.ActionForm.Core.Form.Result;
using avt.ActionForm.Core.Utils;
using DotNetNuke.Services.Log.EventLog;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace avt.ActionForm.Core.Actions
{
    public class PostData : IAction
    {
        public string Url { get; set; }
        public string Data { get; set; }
        public string SaveToken { get; set; }
        
        public void Init(ActionInfo actionInfo, SettingsDictionary settings)
        {
            Url = settings.GetValue("Url", "");
            Data = settings.GetValue("Data", "");
            SaveToken = settings.GetValue("SaveToken", "");
        }

        public IFormEventResult Execute(ActionFormSettings settings, FormData data, eActionContext context)
        {
            var url = data.ApplyAllTokens(Url);
            if (string.IsNullOrEmpty(url))
                return null;

            // initialize logging
            HttpRequest req = HttpContext.Current.Request;
            string logFolder = Path.Combine(HttpRuntime.AppDomainAppPath, "Portals\\_default\\Logs\\ActionForm");
            if (req.Params["afDebug"] == "true" && !Directory.Exists(logFolder)) {
                Directory.CreateDirectory(logFolder);
            }

            string postParams = data.ApplyAllTokens(Data);

            // also replace cookies (if My Tokens was installed this is already done)
            // TODO: why is this here?
            if (Regex.IsMatch(postParams, @"\[\s*Cookie\s*:", RegexOptions.IgnoreCase)) {
                foreach (string cookieName in HttpContext.Current.Request.Cookies.Keys) {
                    postParams = Regex.Replace(postParams, @"\[\s*Cookie\s*:\s*" + cookieName + @"\s*\]", HttpContext.Current.Request.Cookies[cookieName].Value, RegexOptions.IgnoreCase);
                }
            }

            StringBuilder sbData = new StringBuilder();
            foreach (string pLine in postParams.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries)) {
                if (sbData.Length > 0)
                    sbData.Append('&');
                string key = pLine.Substring(0, pLine.IndexOf("=")).Trim();
                string val = pLine.Substring(pLine.IndexOf("=") + 1).Trim();

                sbData.AppendFormat("{0}={1}", key, Uri.EscapeDataString(val));
            }

            string response;

            try {
                WebClient apiClient = new WebClient();

                Uri currentUrl = HttpContext.Current.Request.Url;
                string port = currentUrl.IsDefaultPort ? string.Empty : (":" + currentUrl.Port);

                apiClient.Headers.Add("Referer",
                    string.Format("{0}://{1}{2}", currentUrl.Scheme, currentUrl.Host, port));

                // apiClient.Headers.Add("Referer", "http://c5insight.com");
                if (sbData.Length > 0) {
                    byte[] postArray = Encoding.UTF8.GetBytes(sbData.ToString());
                    apiClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                    // UploadData implicitly sets HTTP POST as the request method.
                    byte[] responseArray = apiClient.UploadData(url, postArray);

                    // Decode and return the response.
                    response = Encoding.UTF8.GetString(responseArray);
                    if (req.Params["afDebug"] == "true") {
                        // spit respons to file on disk
                        string lofFile = Path.Combine(logFolder, "post-to-url-" + (DateTime.Now.ToString("yyyy-MM-dd")) + ".txt");
                        File.WriteAllText(lofFile, string.Format("{0}\n{1}\n\n------------------------------------------------------------\n\n{2}", url, sbData.ToString(), response));
                    }
                } else {
                    response = apiClient.DownloadString(url);
                }

              
            } catch (Exception ex) {
                response = ex.Message;
                ExceptionLogController objEventLog = new ExceptionLogController();
                Exception cex = new Exception("ActionForm failed to post to url " + url, ex);
                objEventLog.AddLog(cex, DotNetNuke.Services.Log.EventLog.ExceptionLogController.ExceptionLogType.GENERAL_EXCEPTION);
            }

            // this is obsolete
            data["PostResponse"] = response;

            // this is the new configurable preferred way
            if (!string.IsNullOrEmpty(SaveToken))
                data[SaveToken.Trim('[', ']')] = response;


            return null;
        }

    }
}
