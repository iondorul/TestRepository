using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;

namespace avt.ActionForm.Core.Form.Result
{
    public class KeepOnPage : IFormEventResult
    {
        public eSubmitStatus Status { get; set; }

        public KeepOnPage()
        {
            Status = eSubmitStatus.Submitted;
        }

        public void Execute(HttpContext context, Control baseControl)
        {
            context.Response.Redirect(context.Request.RawUrl);
        }

        public string ToJson()
        {
            return string.Format("{{\"keepOnPage\":true}}");
        }
    }
}
