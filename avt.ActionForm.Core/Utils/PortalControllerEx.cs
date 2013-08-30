using DotNetNuke.Common;
using DotNetNuke.Entities.Portals;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace avt.ActionForm.Utils
{
    static public class PortalControllerEx
    {

        public static PortalSettings GetCurrentPortal(HttpContext context)
        {
            var alias = GetCurrentPortalAlias(context);
            if (alias != null)
                return new PortalSettings(-1, alias);

            // try to match a portalid parameter
            int portalId;
            if (!string.IsNullOrEmpty(context.Request.QueryString["portalid"]) 
                && Int32.TryParse(context.Request.QueryString["portalid"], out portalId)) {
                // TODO: what about the alias?
                    return new PortalSettings(-1, new PortalAliasController().GetPortalAliasArrayByPortalID(portalId)[0] as PortalAliasInfo);
            }

            // if we didn't figure it out, let DNN do it
            return PortalController.GetCurrentPortalSettings();
        }

        public static PortalAliasInfo GetCurrentPortalAlias(HttpContext context)
        {
            // check for the alias parameter
            if (!string.IsNullOrEmpty(context.Request.QueryString["alias"])) {
                var strAlias = context.Request.QueryString["alias"];
                var alias = GetAlias(strAlias);
                if (alias == null) {
                    // also check portal from tabId
                    int tabId;
                    if (!string.IsNullOrEmpty(context.Request.QueryString["tabid"]) && Int32.TryParse(context.Request.QueryString["tabid"], out tabId))
                        alias = PortalAliasController.GetPortalAliasInfo(PortalAliasController.GetPortalAliasByTab(tabId, strAlias));
                }

                if (alias != null)
                    return alias;
            }

            // no hints in the URL, let's determine portal alias looking for longest possible match
            return GetCurrentPortalAlias(context.Request.Url);
        }

        public static string SanitizePortalAlias(Uri currentUrl, PortalAliasInfo portalAlias)
        {
            // the idea is that portalAlias may or may not include www but DNN/IIS would serve both anyway
            // so we need to convert the DB portal alias back to the format that is actually used
            
            if (currentUrl.Host.IndexOf("www.") == 0 && portalAlias.HTTPAlias.IndexOf("www.") != 0)
                return "www." + portalAlias.HTTPAlias;

            if (currentUrl.Host.IndexOf("www.") == -1 && portalAlias.HTTPAlias.IndexOf("www.") == 0)
                return portalAlias.HTTPAlias.Substring("www.".Length);

            return portalAlias.HTTPAlias;
        }


        public static PortalSettings GetCurrentPortal(string strAlias)
        {
            return new PortalSettings(-1, GetAlias(strAlias));
        }

        public static PortalSettings GetCurrentPortal(Uri uri)
        {
            // try to get the portal as it is
            var alias = GetCurrentPortalAlias(uri);
            if (alias != null)
                return new PortalSettings(-1, alias);

            return PortalController.GetCurrentPortalSettings();
        }

        public static PortalAliasInfo GetCurrentPortalAlias(Uri uri)
        {
            // try to get the portal as it is
            var alias = GetAliasAndCheckAllForms(uri.Host, uri.Port, uri.AbsolutePath);
            if (alias != null)
                return alias;

            // start from largest URL path and try to find an alias
            var aliases = PortalAliasController.GetPortalAliasLookup();
            var path = uri.AbsolutePath.Substring(0, uri.AbsolutePath.LastIndexOf('/')).TrimEnd('/');
            while (path != "") {

                alias = GetAliasAndCheckAllForms(uri.Host, uri.Port, path);
                if (alias != null)
                    return alias;

                path = path.Substring(0, path.LastIndexOf('/'));
            }

            // do it once more for the root
            return GetAliasAndCheckAllForms(uri.Host, uri.Port, path);
        }

        static PortalAliasInfo GetAliasAndCheckAllForms(string host, int port, string path)
        {
            path = path.TrimEnd('/');
            var alias = GetAlias(host + path);
            if (alias != null)
                return alias;

            // also try it with ending slash
            alias = GetAlias(host + path + '/');
            if (alias != null)
                return alias;

            // also try with port
            alias = GetAlias(host + ":" + port + path);
            if (alias != null)
                return alias;

            // also try with port and ending slash
            alias = GetAlias(host + ":" + port + path + '/');
            if (alias != null)
                return alias;

            return null;
        }

        static PortalAliasInfo GetAlias(string host)
        {
            // get as it is
            var alias = PortalAliasController.GetPortalAliasInfo(host);
            if (alias != null)
                return alias;

            // check www/ variant as well
            if (host.IndexOf("www.") != 0) {
                alias = PortalAliasController.GetPortalAliasInfo("www." + host);
                if (alias != null)
                    return alias;
            }

            // check non-www variant as well
            if (host.IndexOf("www.") == 0) {
                alias = PortalAliasController.GetPortalAliasInfo(host.Substring("www.".Length));
                if (alias != null)
                    return alias;
            }

            return null;
        }


        public static string GetCurrentPortalAlias(int portalId)
        {
            // see if we can get it from HTTP Alias
            var portal = new PortalController().GetPortal(portalId);
            PortalSettings portalSettings = new PortalSettings(portal.HomeTabId, portal);
            if (portalSettings != null && portalSettings.PortalAlias != null)
                return portalSettings.PortalAlias.HTTPAlias;

            PortalAliasController pac = new PortalAliasController();
            var aliases = pac.GetPortalAliasArrayByPortalID(portalId);
            if (HttpContext.Current == null)  // we can't determine from URL, there's no URL
                return (aliases[0] as PortalAliasInfo).HTTPAlias;

            var request = HttpContext.Current.Request;
            var host = request.Url.Host;
            if (!request.Url.IsDefaultPort)
                host += ":" + request.Url.Port;

            var alias = GetAlias(host, aliases);
            if (!string.IsNullOrEmpty(alias))
                return alias;

            // if we got here, also check the www variant
            if (host.IndexOf("www") == 0) {
                host = host.Substring("www.".Length);
                alias = GetAlias(host, aliases);
                if (!string.IsNullOrEmpty(alias))
                    return "www." + alias;
            }

            // just return the first alias
            return (aliases[0] as PortalAliasInfo).HTTPAlias;
        }

        public static string MakeRelativeUrl(string absoluteUrl, int portalId)
        {
            if (absoluteUrl.IndexOf("http") != 0)
                return absoluteUrl; // already realtive

            var currentAlias = GetCurrentPortalAlias(portalId);
            if (absoluteUrl.IndexOf(currentAlias, StringComparison.OrdinalIgnoreCase) == 0)
                return "/" + absoluteUrl.Substring(currentAlias.Length).TrimStart('/');

            return UriUtils.ToRelativeUrl(absoluteUrl);
        }

        static string GetAlias(string host, IEnumerable aliases)
        {
            var request = HttpContext.Current.Request;
            foreach (PortalAliasInfo alias in aliases) {
                if (alias.HTTPAlias.IndexOf(host) == 0) {
                    // this is a candidate, but remember that portal alias can also be domain.com/folder
                    if (alias.HTTPAlias.Equals(host, StringComparison.OrdinalIgnoreCase))
                        return alias.HTTPAlias;

                    if (request.RawUrl.IndexOf('/', 1) > 0) {
                        var firstPart = request.RawUrl.Substring(0, request.RawUrl.IndexOf('/', 1));
                        if (firstPart.Equals("/DesktopModules", StringComparison.OrdinalIgnoreCase))
                            continue;
                        if (alias.HTTPAlias.IndexOf(host + firstPart) == 0)
                            return alias.HTTPAlias;
                    }
                }
            }
            return null;
        }

        public static string GetDnnTabsCacheKey(int portalId)
        {
            // here comes the mess
            string tabCacheKey = null; // string.Format(DataCache.TabCacheKey, portalId.ToString());
            if (HttpRuntime.Cache.Get("Tabs" + portalId.ToString()) != null) {
                tabCacheKey = "Tabs" + portalId.ToString();
            } else if (HttpRuntime.Cache.Get("DNN_Tab_Tabs" + portalId.ToString()) != null) {
                tabCacheKey = "DNN_Tab_Tabs" + portalId.ToString();
            } else if (HttpRuntime.Cache.Get("DNN_Tabs" + portalId.ToString()) != null) {
                tabCacheKey = "DNN_Tabs" + portalId.ToString();
            }

            return tabCacheKey;
        }
    }

}
