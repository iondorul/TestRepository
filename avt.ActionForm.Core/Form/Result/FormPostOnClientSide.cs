using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;

namespace avt.ActionForm.Core.Form.Result
{
    public class FormPostOnClientSide : IFormEventResult
    {
        public string FormHtml { get; set; }
        public eSubmitStatus Status { get; set; }

        public FormPostOnClientSide()
        {
            Status = eSubmitStatus.Submitted;
        }

        public void Execute(HttpContext context, Control baseControl)
        {
            var response = context.Response;
            response.Clear();
            response.ClearContent();
            response.ClearHeaders();

            StringBuilder sbForm = new StringBuilder();

            sbForm.Append("<html><body>");
            sbForm.Append(FormHtml);
            sbForm.Append("</body></html>");
            response.Write(sbForm.ToString());
        }

        public string ToJson()
        {
            return string.Format("{{\"appendHtml\":\"{0}\", \"appendTo\":\"body\"}}", ActionFormController.JsonEncode(FormHtml));
        }
    }
}
