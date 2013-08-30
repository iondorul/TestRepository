using avt.ActionForm.Core.Form;
using avt.ActionForm.Core.Form.Result;
using avt.ActionForm.Core.Utils;
using avt.ActionForm.Data;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Membership;
using DotNetNuke.Services.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace avt.ActionForm.Core.Actions
{
    public class RepostData : IAction
    {
        public string Url { get; set; }
        public string PostData { get; set; }
        public bool RepostEverything { get; set; }

        public void Init(ActionInfo actionInfo, SettingsDictionary settings)
        {
            Url = settings.GetValue("Url", "");
            PostData = settings.GetValue("PostData", "");
            RepostEverything = settings.GetValue("RepostEverything", true);
        }

        public IFormEventResult Execute(ActionFormSettings settings, FormData data, eActionContext context)
        {
            StringBuilder sbForm = new StringBuilder();

            sbForm.AppendFormat("<form id='form' name='form' method='post' action='{0}'>", Url);
           
            HashSet<string> dataKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var line in PostData.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)) {
                if (line.IndexOf('=') == -1)
                    continue; // ??
                var key = data.ApplyAllTokens(line.Substring(0, line.IndexOf('=')).Trim());
                var val = data.ApplyAllTokens(line.Substring(line.IndexOf('=') + 1).Trim());
                sbForm.AppendFormat("<input type='hidden' name='{0}' value='{1}'>", key, HttpUtility.HtmlEncode(val));
                dataKeys.Add(key);
            }

            if (RepostEverything) {
                foreach (var key in data.Data.Keys) {
                    if (dataKeys.Contains(key))
                        continue; // already added from manual post data
                    sbForm.AppendFormat("<input type='hidden' name='{0}' value='{1}'>", key, HttpUtility.HtmlEncode(data[key]));
                }
            }

            sbForm.Append("</form>");
            sbForm.Append("<script type = 'text/javascript'>");
            sbForm.Append("document.forms['form'].submit();");
            sbForm.Append("</script>");

            return new FormPostOnClientSide() { FormHtml = sbForm.ToString() };
        }


    }
}
