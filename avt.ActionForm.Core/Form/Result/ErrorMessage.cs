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
    public class ErrorMessage : IFormEventResult
    {
        public eSubmitStatus Status { get; set; }
        public string Message { get; set; }

        public ErrorMessage()
        {
            Status = eSubmitStatus.Draft;
        }

        public void Execute(HttpContext context, Control baseControl)
        {
            ControlFinder.FindControl<HtmlGenericControl>(baseControl, "pnlMessage").Visible = true;
            ControlFinder.FindControl<HtmlGenericControl>(baseControl, "phFormTemplate").Visible = false;
            ControlFinder.FindControl<Literal>(baseControl, "lblMessage").Text = Message;
        }

        public string ToJson()
        {
            return string.Format("{{\"error\":\"{0}\"}}", ActionFormController.JsonEncode(Message));
        }


    }
}
