using avt.ActionForm.Core.Fields;
using avt.ActionForm.Core.Form;
using avt.ActionForm.Utils;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;

namespace avt.ActionForm.Core.Input
{
    public class BaseControl : IInputCtrl
    {
        public virtual IList<ListItem> GetOptions(InputTypeDef typeDef, FormField field)
        {
            return null;
        }

        public virtual NameValueCollection GetData(InputTypeDef typeDef, FormField field)
        {
            return null;
        }

        public virtual bool IsValid(Form.FormData formData, Form.FieldData field, out string[] errMessages)
        {
            errMessages = null;
            return true;
        }

        public virtual void ExpandTokens(FormData formData, FieldData fieldData)
        {

        }
    }
}
