using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Text.RegularExpressions;
using System.Collections.Specialized;

namespace avt.ActionForm.Utils
{
    public static class UriUtils
    {
        public static string ToAbsoulteUri(string baseUri, string relUri)
        {
            if (HttpRuntime.AppDomainAppVirtualPath != "/")
                baseUri = Regex.Replace(baseUri, HttpRuntime.AppDomainAppVirtualPath, "", RegexOptions.IgnoreCase);
            relUri = relUri.TrimEnd('&');

            if (relUri.IndexOf("http") == 0)
                return relUri;

            if (baseUri.IndexOf("http") != 0)
                baseUri = "http://" + baseUri;

            if (relUri.StartsWith("~/"))
                relUri = HttpRuntime.AppDomainAppVirtualPath.TrimEnd('/') + "/" + relUri.Substring(2); // VirtualPathUtility.ToAbsolute(relUri);

            Uri uri = new Uri(baseUri);
            return "http://" + uri.Host + (uri.IsDefaultPort ? "" : ":" + uri.Port.ToString()) + "/" + relUri.Trim('/');
        }

        //public static string ToAbsoluteUrl(string relativeUrl)
        //{
        //    if (string.IsNullOrEmpty(relativeUrl))
        //        return relativeUrl;

        //    if (relativeUrl.IndexOf("http") == 0)
        //        return relativeUrl;

        //    if (HttpContext.Current == null)
        //        return relativeUrl;

        //    if (relativeUrl.StartsWith("/"))
        //        relativeUrl = relativeUrl.Insert(0, "~");

        //    if (!relativeUrl.StartsWith("~/"))
        //        relativeUrl = relativeUrl.Insert(0, "~/");

        //    relativeUrl = HttpRuntime.AppDomainAppVirtualPath.TrimEnd('/') + "/" + relativeUrl.Substring(2); // VirtualPathUtility.ToAbsolute(relUri);

        //    var url = HttpContext.Current.Request.Url;
        //    var port = url.Port != 80 ? (":" + url.Port) : String.Empty;

        //    return String.Format("{0}://{1}{2}{3}", url.Scheme, url.Host, port, relativeUrl);
        //}

        public static string ToAbsoluteUrl(string relativeUrl)
        {
            if (string.IsNullOrEmpty(relativeUrl))
                return relativeUrl;

            if (relativeUrl.IndexOf("http") == 0)
                return relativeUrl;

            if (HttpContext.Current == null)
                return relativeUrl;

            if (relativeUrl.StartsWith("/"))
                relativeUrl = relativeUrl.Insert(0, "~");
            if (!relativeUrl.StartsWith("~/"))
                relativeUrl = relativeUrl.Insert(0, "~/");

            Uri url = HttpContext.Current.Request.Url;
            string port = url.Port != 80 ? (":" + url.Port) : string.Empty;
            string query = relativeUrl.IndexOf("?") != -1 ? relativeUrl.Substring(relativeUrl.IndexOf("?")) : "";
            relativeUrl = relativeUrl.IndexOf("?") != -1 ? relativeUrl.Substring(0, relativeUrl.IndexOf("?")) : relativeUrl;

            return string.Format("{0}://{1}{2}{3}{4}", url.Scheme, url.Host, port, VirtualPathUtility.ToAbsolute(relativeUrl), query);
        }

        public static string ToRelativeUrl(string absoluteUrl)
        {
            if (absoluteUrl.IndexOf("http") != 0)
                return absoluteUrl;

            // return after the first slash, but skip the https:// slashes
            return absoluteUrl.Substring(absoluteUrl.IndexOf('/', 10));
        }

        public static string DropExtension(string url)
        {
            var ext = VirtualPathUtility.GetExtension(url);
            if (!string.IsNullOrEmpty(ext))
                url = url.Substring(0, url.LastIndexOf(ext));
            return url;
        }

        public static string Sanitize(string url, bool allowAbsolute)
        {
            if (string.IsNullOrEmpty(url))
                return "/";

            if (url.IndexOf("http://", StringComparison.OrdinalIgnoreCase) == 0 || url.IndexOf("https://", StringComparison.OrdinalIgnoreCase) == 0) {
                
                // absolute URLs only allowed for redirects - we can't rewrite to a different address
                if (allowAbsolute) 
                    return url;
                
                var iOfRootSlash = url.IndexOf('/', 8);
                return iOfRootSlash == -1 ? "/" : url.Substring(iOfRootSlash);
            }

            // should begin with a slash
            if (url[0] != '/')
                return '/' + url;

            return url;
        }

        public static bool IsIp(string host)
        {
            // TODO: match anywhere or not?
            return Regex.IsMatch(host, ".*\\d+\\.\\d+\\.\\d+\\.\\d+.*");
        }

        public static string BuildQueryString(NameValueCollection queryParams, bool prependQuestionMark = false)
        {
            if (queryParams.Count == 0)
                return "";

            var sb = new StringBuilder();
            if (prependQuestionMark)
                sb.Append('?');

            foreach (var name in queryParams.AllKeys) {
                sb.AppendFormat("{0}={1}&", name, queryParams[name]);
            }

            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        public static string ResolvePortalUrl(string appPath, string portalRelativeUrl)
        {
            return (appPath.TrimEnd('/') + "/" + portalRelativeUrl).TrimEnd('/');
        }

        public static string GetPath(string url)
        {
            if (url.IndexOf('?') > 0)
                return url.Substring(0, url.IndexOf('?'));
            return url;
        }

        public static string GetQuery(string url)
        {
            if (url.IndexOf('?') > 0)
                return url.Substring(url.IndexOf('?') + 1);
            return "";
        }
    }
}
