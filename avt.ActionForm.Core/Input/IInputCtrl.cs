using avt.ActionForm.Core.Fields;
using avt.ActionForm.Core.Form;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;

namespace avt.ActionForm.Core.Input
{
    public interface IInputCtrl
    {
        IList<ListItem> GetOptions(InputTypeDef typeDef, FormField field);
        NameValueCollection GetData(InputTypeDef typeDef, FormField field);
        bool IsValid(FormData formData, FieldData field, out string[] errMessages);

        /// <summary>
        /// Gives control a chance to register more tokens based on submitted value
        /// </summary>
        /// <param name="formData"></param>
        /// <param name="fieldData"></param>
        void ExpandTokens(FormData formData, FieldData fieldData);
    }
}
