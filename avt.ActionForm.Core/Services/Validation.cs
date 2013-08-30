using avt.ActionForm.Core.Actions;
using avt.ActionForm.Core.Config;
using avt.ActionForm.Core.Fields;
using avt.ActionForm.Core.Form;
using avt.ActionForm.Core.Validation;
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
    public class Validation : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";

            if (!string.IsNullOrEmpty(context.Request.QueryString["language"]))
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(context.Request.QueryString["language"]);

            try {

                var module = GetModule(context);
                var settings = new ActionFormSettings(module.ModuleID);
                var input = context.Request.Form["value"];

                string configFolder = ActionFormController.AppPath.Trim('\\').Trim('/') + "\\Config\\Validators";
                var validationDefitions = ItemsFromXmlConfig<ValidatorDef>.GetConfig(configFolder);
                if (!validationDefitions.ItemsHash.ContainsKey(context.Request.Form["validator"]))
                    throw new ArgumentException("Invalid validator");

                var validator = validationDefitions.ItemsHash[context.Request.Form["validator"]];
                var err = validator.Validate(ref input);
                if (err == null) {
                    context.Response.Write("{\"success\": true}");
                    return;
                }

                // we have an error, but do we have custom actions to execute?
                var field = GetField(context, settings);
                if (field == null) {
                    // no custom actions, just return the error message
                    context.Response.Write("{\"error\": \"" + ActionFormController.JsonEncode(err) + "\"}");
                    return;
                }

                var eventName = string.Format("{0}-{1}", field.FormFieldId, validator.Title);
                var actions = ActionInfo.GetAllByProperty("OrderIndex",
                    new KeyValuePair<string, object>("ModuleId", module.ModuleID),
                    new KeyValuePair<string, object>("EventName", eventName));

                if (actions == null || actions.Count == 0) {
                    // no cusotm actions, revert to "any validation failed" event
                    actions = ActionInfo.GetAllByProperty("OrderIndex",
                        new KeyValuePair<string, object>("ModuleId", module.ModuleID),
                        new KeyValuePair<string, object>("EventName", "validation-failed"));
                }

                if (actions == null || actions.Count == 0) {
                    // no custom actions, just return the error message
                    context.Response.Write("{\"error\": \"" + ActionFormController.JsonEncode(err) + "\"}");
                    return;
                }

                var formData = new FormData(settings);
                formData["ValidationErros"] = err;
                formData[field.TitleCompacted] = input;
                var result = settings.ExecuteActions(actions, formData, eActionContext.AjaxValidation);
                context.Response.Write(result.ToJson());

            } catch (Exception ex) {
                context.Response.Write("{\"error\": \"" + ActionFormController.JsonEncode(ex.Message) + "\"}");
                Exceptions.LogException(ex);
            }

        }

        FormField GetField(HttpContext context, ActionFormSettings settings)
        {
            var fieldId = ConvertUtils.Cast(context.Request.Form["fieldid"], -1);
            if (fieldId == -1)
                return null;

            return settings.Fields.FirstOrDefault(x => x.FormFieldId == fieldId);
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
