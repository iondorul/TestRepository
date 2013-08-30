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
using avt.ActionForm.Core.Form;
using DotNetNuke.Entities.Portals;

namespace avt.ActionForm.Core.Input
{
   

    public class FileUpload : BaseControl
    {
        /// <summary>
        /// FieldData contains the filename on the server only.
        /// We'll register a few other tokens: [FieldName:RelativeUrl], [FieldName:AbsoluteUrl], [FieldName:FilePath]
        /// </summary>
        /// <param name="formData"></param>
        /// <param name="fieldData"></param>
        public override void ExpandTokens(FormData formData, FieldData fieldData)
        {
            var context = HttpContext.Current;
            var portal = PortalController.GetCurrentPortalSettings();
            string folder = fieldData.Field.Parameters["Folder"].ToString();
            var relativePath = string.Format("{0}/{1}/{2}", HttpRuntime.AppDomainAppVirtualPath.TrimEnd('/'), folder.Trim('/'), fieldData.Value);

            formData[string.Format("{0}-RelativeUrl", fieldData.Field.TitleCompacted)] = relativePath;

            formData[string.Format("{0}-AbsoluteUrl", fieldData.Field.TitleCompacted)] =
                string.Format("{0}://{1}/{2}/{3}/{4}",
                context.Request.Url.Scheme,
                portal.PortalAlias.HTTPAlias.TrimEnd('/'),
                portal.HomeDirectory.Trim('/'),
                folder.Trim('/'), fieldData.Value);

            formData[string.Format("{0}-FilePath", fieldData.Field.TitleCompacted)] = context.Server.MapPath(relativePath);
        }
    }
}
