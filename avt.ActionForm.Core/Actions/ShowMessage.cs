using avt.ActionForm.Core.Form;
using avt.ActionForm.Core.Form.Result;
using avt.ActionForm.Core.Utils;
using avt.ActionForm.Data;
using avt.ActionForm.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace avt.ActionForm.Core.Actions
{
    public class ShowMessage : IAction
    {
        public string Message { get; set; }

        /// <summary>
        /// ArrayList is the underlaying type of the list of settings as deserialized by JavaScriptSerializer
        /// </summary>
        public ArrayList Buttons { get; set; }

        public ActionInfo ActionInfo { get; set; }

        public void Init(ActionInfo actionInfo, SettingsDictionary settings)
        {
            Message = settings.GetValue("Message", "");
            Buttons = settings.GetAs<ArrayList>("ButtonsList", null);
            ActionInfo = actionInfo;
        }

        public IFormEventResult Execute(ActionFormSettings settings, FormData data, eActionContext context)
        {
            // don't show buttons during AJAX validation?
            var btnClass = context == eActionContext.AjaxValidation ? "btn-mini" : "";
            //if (context == eActionContext.AjaxValidation)
            //    return new MessageHtml() { Html = data.ApplyAllTokens(Message) };

            var sbMessage = new StringBuilder();
            sbMessage.Append(data.ApplyAllTokens(Message));

            if (Buttons != null && Buttons.Count > 0) {
                sbMessage.Append("<div>");
                var repl = new AfFieldTokenReplacer(settings);
                foreach (string btnId in Buttons) {
                    if (!settings.FieldsByName.ContainsKey(btnId))
                        continue;

                    var field = settings.FieldsByName[btnId];
                    var xml = settings.SerializeToXmlStr(null, true);
                    sbMessage.Append(repl.GetHtml(field.TitleCompacted, null, xml));
                }
                sbMessage.Append("</div>");
            }

            return new MessageHtml() {
                Html = sbMessage.ToString(),
                Type = context == eActionContext.Validation ? "warning" : ""
            };

        }
    }
}
