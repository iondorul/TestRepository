using avt.ActionForm.Core.Form;
using avt.ActionForm.Core.Form.Result;
using avt.ActionForm.Core.Utils;
using avt.ActionForm.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace avt.ActionForm.Core.Actions
{
    public class SubmitAnyway : IAction
    {
        public string Message { get; set; }
        public ActionInfo ActionInfo { get; set; }

        public void Init(ActionInfo actionInfo, SettingsDictionary settings)
        {
            Message = settings.GetValue("Message", "");
            ActionInfo = actionInfo;
        }

        public IFormEventResult Execute(ActionFormSettings settings, FormData data, eActionContext context)
        {
            //var report = ReportEntry.Get(int.Parse(data.Data["ReportEntryId"].Value));
            //report.SkipValidation = true;

            //if (data.Report != null)
            //    data.Report.SkipValidation = true;

            data["SkipValidation"] = "true";

            return new Skip();
        }
    }
}
