using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;

namespace avt.ActionForm.Core.Form.Result
{
    public class RedirectToUrl : IFormEventResult
    {
        public string Url { get; set; }
        public eSubmitStatus Status { get; set; }

        public RedirectToUrl()
        {
            Status = eSubmitStatus.Submitted;
        }

        public void Execute(HttpContext context, Control baseControl)
        {
            if (string.IsNullOrEmpty(Url))
                Url = "/";
            context.Response.Redirect(Url);
        }

        public string ToJson()
        {
            if (string.IsNullOrEmpty(Url))
                Url = "/";
            return string.Format("{{\"redirect\":\"{0}\"}}", ActionFormController.JsonEncode(Url));
        }
    }
}
