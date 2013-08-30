using avt.ActionForm.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace avt.ActionForm.Core.Form.Result
{
    public class MessageHtml : IFormEventResult
    {
        public eSubmitStatus Status { get; set; }
        public string Html { get; set; }
        public string Type { get; set; }

        public MessageHtml()
        {
            Status = eSubmitStatus.Submitted;
        }

        public void Execute(HttpContext context, Control baseControl)
        {
            ControlFinder.FindControl<HtmlGenericControl>(baseControl, "pnlMessage").Visible = true;
            ControlFinder.FindControl<HtmlGenericControl>(baseControl, "phFormTemplate").Visible = false;
            ControlFinder.FindControl<Literal>(baseControl, "lblMessage").Text = Html;
        }

        public string ToJson()
        {
            return string.Format("{{\"messageHtml\":\"{0}\", \"type\":\"{1}\"}}",
                ActionFormController.JsonEncode(Html), ActionFormController.JsonEncode(Type ?? ""));
        }


    }
}
