using avt.ActionForm.Core.Fields;
using avt.ActionForm.Core.Form;
using avt.ActionForm.Utils;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;

namespace avt.ActionForm.Core.Services
{
    public class SubmitData : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";

            // load language if it exists in query string
            if (!string.IsNullOrEmpty(context.Request.QueryString["language"]))
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(context.Request.QueryString["language"]);

            try {

                var module = GetModule(context);
                var settings = new ActionFormSettings(module.ModuleID);

                settings.Debug("Parsing submitted data");

                var reportKey = context.Request.QueryString["submission"];
                var eventName = context.Request.QueryString["event"];
                var button = DetermineSubmitButton(context, settings);

                var report = ReportEntry.Get(reportKey);
                FormData data = report != null ?  report.ToFormData() : new FormData(settings, context);

                settings.Debug("Executing form actions...");
                var result = settings.Execute(data, report == null ? -1 : report.ReportEntryId, button, eventName);

                settings.Debug("Serializing form result...");
                context.Response.Write(result.ToJson());

                settings.Debug("All done.");

            } catch (Exception ex) {
                context.Response.Write("{\"error\": \""+ ActionFormController.JsonEncode(ex.Message) +"\"}");
                Exceptions.LogException(ex);
            }

        }

        FormField DetermineSubmitButton(HttpContext context, ActionFormSettings settings)
        {
            var buttonId = context.Request.QueryString["b"].ToInt(null);
            if (!buttonId.HasValue)
                throw new Exception("Invalid submit button");
            
            var button = settings.Fields.SingleOrDefault(x => x.FormFieldId == buttonId.Value);
            if (button == null || !button.IsVisibile)
                throw new Exception("Invalid submit button");

            return button;
        }

        ModuleInfo GetModule(HttpContext context)
        {
            int moduleId = -1;
            if (!int.TryParse(context.Request.QueryString["mid"], out moduleId))
                throw new ArgumentException("Invalid form module");

            var module = new ModuleController().GetModule(moduleId);
            if (module == null || module.ModuleID <= 0 || module.DesktopModule.FriendlyName != "avt.ActionForm")
                throw new ArgumentException("Invalid form module");

            return module;
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
