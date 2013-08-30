using avt.ActionForm.Core.Fields;
using avt.ActionForm.Core.Form;
using DotNetNuke.Common.Lists;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace avt.ActionForm.Core.Services
{
    public class GetDnnList : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";

            try {

                var listName = context.Request.Params["listname"];
                var parentKey = context.Request.Params["parentkey"];
                if (string.IsNullOrEmpty(listName) || string.IsNullOrEmpty(parentKey)) {
                    context.Response.Write("{\"error\": \"Invalid list\"}");
                    return;
                }

                var sb = new StringBuilder();
                sb.Append("[");
                foreach (ListEntryInfo item in new ListController().GetListEntryInfoCollection(listName, parentKey)) {
                    sb.AppendFormat("{{\"key\":\"{0}\",\"value\":\"{1}\"}},", ActionFormController.JsonEncode(item.Value), ActionFormController.JsonEncode(item.Text));
                }
                if (sb[sb.Length - 1] == ',')
                    sb.Remove(sb.Length - 1, 1);
                sb.Append("]");

                context.Response.Write(sb.ToString());

            } catch (Exception ex) {
                context.Response.Write("{\"error\": \""+ ActionFormController.JsonEncode(ex.Message) +"\"}");
                Exceptions.LogException(ex);
            }

        }


        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
