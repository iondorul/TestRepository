using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using avt.ActionForm.Core.Fields;
using avt.ActionForm.Core.Input;
using DotNetNuke.Framework;
using DotNetNuke.UI.WebControls;
using System.IO;
using avt.ActionForm.Core.Form;

namespace avt.ActionForm.Templating
{
    public static class TemplateFactory
    {
        public static IActionFormTemplate New(string templateBaseDir, ActionFormSettings settings, FormData initData)
        {
            string template = settings.FormTemplate.Value;
            if (string.IsNullOrEmpty(template))
                template = "bootstrap";

            var templateUrl = Path.Combine(templateBaseDir, settings.FormTemplate.Value);
            var templatePath = HttpContext.Current.Server.MapPath(templateUrl);
            if (!Directory.Exists(templatePath))
                throw new IOException("Template folder " + templateUrl + " does not exist");

            if (File.Exists(Path.Combine(templatePath, "main.xsl")))
                return new XslTemplate(settings, settings.IsDebug.Value, initData);

            throw new IOException("Template main.xsl does not exist");
        }

    }
}
