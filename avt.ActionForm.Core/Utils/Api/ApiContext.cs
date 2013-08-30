using DotNetNuke.Entities.Modules;
using DotNetNuke.Security.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace avt.ActionForm.Utils.Api
{
    public class ApiContext
    {
        public LiteContainer Container { get; set; }

        public ApiContext()
        {
            Container = new LiteContainer();
        }

        public void Execute(IHttpHandler handler, HttpContext httpContext)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            if (httpContext == null)
                throw new ArgumentNullException("httpContext");

            // first, determine which method to call
            var method = handler.GetType().GetMethod(httpContext.Request.Params["method"] ?? "", BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
            if (method == null)
                throw new ArgumentException("API Method");

            // determine either this is a web method
            WebMethodAttribute attr = method.GetCustomAttributes(false).FirstOrDefault(x => x is WebMethodAttribute) as WebMethodAttribute;
            if (attr == null)
                throw new UnauthorizedAccessException(method.Name + " is not allowed");

            if (attr.RequiredEditPermissions) {
                ModuleInfo moduleInfo = Container.ResolveProperty("moduleInfo") as ModuleInfo;
                if (moduleInfo == null || moduleInfo.ModuleID == -1 || !ModulePermissionController.CanEditModuleContent(moduleInfo))
                    throw new UnauthorizedAccessException("Access denied!");
            }

            // try to fill the parameters
            var parameterDefs = method.GetParameters();
            object[] parameters = new object[parameterDefs.Length];

            for (var i = 0; i < parameterDefs.Length; i++)
                parameters[i] = GetParamValue(parameterDefs[i], httpContext);

            httpContext.Response.Cache.SetNoStore();

            var result = method.Invoke(handler, parameters);
            SerializeResult(result, httpContext, attr);

        }

        object GetParamValue(ParameterInfo pInfo, HttpContext httpContext)
        {
            // first, check in the container, it has priority
            if (Container.HasProperty(pInfo.Name))
                return Container.ResolveProperty(pInfo.Name);

            if (httpContext.Request.Params[pInfo.Name] != null)
                return ConvertUtils.Cast(httpContext.Request.Params[pInfo.Name], pInfo.ParameterType, pInfo.ParameterType.GetDefault());

            if (!pInfo.IsOptional)
                throw new ArgumentException(pInfo.Name);

            if (pInfo.DefaultValue is Missing)
                return pInfo.ParameterType.GetDefault();

            return pInfo.DefaultValue;
        }

        void SerializeResult(object result, HttpContext httpContext, WebMethodAttribute attr)
        {
            switch (attr.DefaultResponseType) {
                case eResponseType.Text:
                    httpContext.Response.ContentType = "text/plain";
                    httpContext.Response.Write(result);
                    break;
                case eResponseType.Html:
                    httpContext.Response.ContentType = "text/html";
                    httpContext.Response.Write(result);
                    break;
                case eResponseType.Json:
                    httpContext.Response.ContentType = "application/json";
                    JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
                    string json = jsonSerializer.Serialize(result);
                    httpContext.Response.Write(json);
                    break;
                case eResponseType.Xml:
                    httpContext.Response.ContentType = "application/xml";
                    System.Xml.Serialization.XmlSerializer xmlSerializer = new System.Xml.Serialization.XmlSerializer(result.GetType());
                    StringBuilder xmlSb = new StringBuilder();
                    System.Xml.XmlWriter xmlWriter = System.Xml.XmlWriter.Create(xmlSb);
                    xmlSerializer.Serialize(xmlWriter, result);
                    httpContext.Response.Write(xmlSb.ToString());
                    break;
            }

        }

    }
}
