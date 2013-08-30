using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace avt.ActionForm.Apis.MailChimp.Net
{
    public static class HttpUtils
    {
        const int timeout = 10;

        public static string Send(string url, Dictionary<string, object> getData = null, Dictionary<string, object> postData = null, Dictionary<string, string> httpHeaders = null)
        {
            string strResponse = "";

            try {

                //System.Net.ServicePointManager.Expect100Continue = false;

                // append get data
                if (getData != null && getData.Count > 0) {
                    url += url.TrimEnd('?', '&').IndexOf('?') > 0 ? '&' : '?';
                    foreach (var key in getData.Keys)
                        url += string.Format("{0}={1}&", key, getData[key]);
                    url = url.TrimEnd('&');
                }

                HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Timeout = timeout * 1000;

                // append headers
                if (httpHeaders != null && httpHeaders.Count > 0) {
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
                }

                // append post data
                if (postData != null && postData.Count > 0) {
                    httpRequest.Method = "POST";
                    if (!httpHeaders.ContainsKey("Content-Type"))
                        httpRequest.ContentType = "application/x-www-form-urlencoded";

                    StringBuilder sbPostData = new StringBuilder();
                    foreach (var key in postData.Keys) {
                        //logger.Debug("    post {0}: {1}", key, postData[key]);
                        sbPostData.AppendFormat("{0}={1}&", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(postData[key].ToString()));
                    }
                    sbPostData.Remove(sbPostData.Length - 1, 1);

                    httpRequest.ContentLength = sbPostData.Length;
                    System.IO.Stream newStream = httpRequest.GetRequestStream();

                    // Send the data.
                    System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                    byte[] data = encoding.GetBytes(sbPostData.ToString());

                    newStream.Write(data, 0, data.Length);
                    newStream.Close();
                }

                HttpWebResponse response = (HttpWebResponse)httpRequest.GetResponse();
                System.IO.StreamReader reader = new System.IO.StreamReader(response.GetResponseStream());
                strResponse = reader.ReadToEnd();
                response.Close();

                strResponse = strResponse.Trim();
                //logger.Debug("Response: {0}", strResponse);

            } catch (WebException ex) {
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream)) {
                    var err = reader.ReadToEnd();
                    //logger.Error(err);
                    throw new Exception("Error loading stream from " + url + " (inner exception: " + ex.Message + "): " + err, ex);
                }
            } catch (Exception ex) {
                //logger.Error(ex);
                throw new Exception("Error loading stream from " + url + " (inner exception: " + ex.Message + ")", ex);
            }

            return strResponse;
        }

    }
}
