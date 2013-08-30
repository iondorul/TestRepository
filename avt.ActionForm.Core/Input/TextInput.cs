using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using avt.ActionForm.Core.Fields;
using System.Web.UI;
using System.Web;
using avt.ActionForm.Core.Validation;
using avt.ActionForm.Core.Config;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using avt.ActionForm.Utils;

namespace avt.ActionForm.Core.Input
{
    public class TextInput : BaseControl
    {
        public override bool IsValid(Form.FormData formData, Form.FieldData field, out string[] errMessages)
        {
            base.IsValid(formData, field, out errMessages);
            
            // check confirmation for password fields
            if (field.Field != null && field.Field.InputTypeStr == "open-password" && field.Field.Parameters.ContainsKey("ConfirmationOf")) {
                var otherField = formData.GetValue(field.Field.Parameters["ConfirmationOf"].ToString());
                if (otherField != null && otherField != field.Value) {
                    errMessages = new string[] {
                        "Password fields do not match."
                    };
                    return false;
                }
            }
            return true;
        }
    }
}
