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
    public class Skip : IFormEventResult
    {
        public eSubmitStatus Status { get; set; }

        public Skip()
        {
            Status = eSubmitStatus.Submitted;
        }

        public void Execute(HttpContext context, Control baseControl)
        {
        }

        public string ToJson()
        {
            return "";
        }


    }
}
