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
using System.Xml.Xsl;
using System.Xml;
using System.IO;
using avt.ActionForm.Utils;
using avt.ActionForm.Core.Validation;
using DotNetNuke.Entities.Modules;
using System.Text.RegularExpressions;
using avt.ActionForm.Core.Form;
using System.Text;
using System.Globalization;
using System.Web.Script.Serialization;

namespace avt.ActionForm.Templating
{
    public class XslTemplate : IActionFormTemplate
    {
        public ActionFormSettings Settings { get; set; }
        public bool IsDebug { get; set; }
        FormData InitData { get; set; }

        public XslTemplate(ActionFormSettings settings, bool isDebug, FormData initData)
        {
            this.IsDebug = isDebug;
            LoadForm(settings, initData);
        }

        public void LoadForm(ActionFormSettings settings, FormData initData)
        {
            this.Settings = settings;
            InitData = initData;
        }

        public string CancelUrl
        {
            get { return ""; }
            set { }
        }

        public void Error(string message)
        {
        }

        public void InitControls(ActionFormSettings settings)
        {
        }


        public void Render(Control inControl)
        {
            XslCompiledTransform transform = new XslCompiledTransform();
            XsltSettings xmlSettings = new XsltSettings();
            xmlSettings.EnableScript = true;

            var tplUrl = inControl.TemplateSourceDirectory + "/templates/Form/" + Settings.FormTemplate;
            var tplPath = HttpContext.Current.Server.MapPath(tplUrl);
            var currentCulture = System.Globalization.CultureInfo.CurrentCulture.ToString();

            // fallback to default template
            if (!Directory.Exists(tplPath)) {
                tplUrl = inControl.TemplateSourceDirectory + "/templates/Form/" + ActionFormSettings.DefaultFormTemplate;
                tplPath = HttpContext.Current.Server.MapPath(tplUrl);
            }

            if (File.Exists(Path.Combine(tplPath, "main." + currentCulture + ".xsl"))) {
                transform.Load(Path.Combine(tplPath, "main." + currentCulture + ".xsl"), xmlSettings, new XmlUrlResolver());
            } else {
                transform.Load(Path.Combine(tplPath, "main.xsl"), xmlSettings, new XmlUrlResolver());
            }

            XsltArgumentList args = new XsltArgumentList();
            args.AddExtensionObject("af:tokens", new XslUtils()); // obsolete
            args.AddExtensionObject("af:utils", new XslUtils(ActionFormController.GetLocaleFile()));

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(Settings.SerializeToXmlStr(InitData, true));

            // if we have a custom layout, tokenize it
            if (Settings.HasCustomLayout.Value) {
                var node = xmlDoc.DocumentElement["Settings"]["LayoutHtml"];
                node.InnerXml = xmlDoc.CreateCDataSection(Tokenize(Settings.LayoutHtml.Value)).OuterXml;
            }

            System.IO.StringWriter output = new System.IO.StringWriter();
            transform.Transform(xmlDoc, args, output);

            HtmlGenericControl c = new HtmlGenericControl();
            c.InnerHtml = output.ToString();
            inControl.Controls.Add(c);

            // also register styles and scripts
            var page = HttpContext.Current.Handler as CDefault;
            var key = string.Format("aformtpl-{0}", Settings.FormTemplate);
            if (!page.ClientScript.IsClientScriptIncludeRegistered(key))
                page.ClientScript.RegisterClientScriptInclude(key, tplUrl + "/script.js");
            //page.ClientScript.RegisterClientScriptBlock(GetType(), key, "afjQuery(function() { initForm(afjQuery('#dnn" + Settings.ModuleId + "root'), { appRoot: '" + ActionFormController.JsonEncode(inControl.TemplateSourceDirectory) + "' }); });", true);
            page.AddStyleSheet("avtSBStyle", Path.Combine(tplUrl, "styles.css").Replace('\\', '/'));

            if (IsDebug) {
                HtmlGenericControl cd = new HtmlGenericControl();
                cd.Style["margin"] = "20px 0";
                cd.InnerHtml = string.Format("<textarea style=\"width: 100%; height: 300px;\">{0}</textarea>", HttpUtility.HtmlEncode(xmlDoc.ToXmlString()));
                inControl.Controls.Add(cd);
            }
        }

        private string Tokenize(string template)
        {
            var xml = Settings.SerializeToXmlStr(InitData, true);

            var repl = new AfFieldTokenReplacer(Settings);
            var output = new StringBuilder();

            template.Replace("[Buttons:", "[Fields:");

            var iStart = template.IndexOf("[Fields:", StringComparison.OrdinalIgnoreCase);
            var iEnd = 0;
            while (iStart != -1) {
                output.Append(template.Substring(iEnd, iStart - iEnd));
                iStart += "[Fields:".Length;
                iEnd = template.IndexOf(']', iStart);
                if (iEnd == -1) {
                    output.Append(template.Substring(iStart));
                    break; // ? maybe some unfinished token?
                }

                var fieldName = template.Substring(iStart, iEnd - iStart).Trim();
                if (Settings.FieldsByName.ContainsKey(fieldName))
                    output.Append(repl.GetHtml(Settings.FieldsByName[fieldName].TitleCompacted, InitData, xml));

                if (iEnd == template.Length - 1)
                    break;

                iEnd++;
                iStart = template.IndexOf("[Fields:", iEnd, StringComparison.OrdinalIgnoreCase);
            }

            if (iEnd > 0)
                output.Append(template.Substring(iEnd));

            //if (iStart > 0) 
            //output.Append(template.Substring(iStart));


            //foreach (var field in Settings.Fields)
            //    template = Regex.Replace(template, "\\[\\s*Fields\\s*:\\s*" + field.TitleCompacted + "\\s*\\]", repl.GetHtml(field.TitleCompacted, InitData), RegexOptions.IgnoreCase);
            template = output.ToString();

            //// also replace buttons
            //template = Regex.Replace(template, "\\[\\s*Buttons\\s*:\\s*Submit\\s*\\]", repl.GetHtmlForTemplate("btn-submit"), RegexOptions.IgnoreCase);
            //template = Regex.Replace(template, "\\[\\s*Buttons\\s*:\\s*Cancel\\s*\\]", repl.GetHtmlForTemplate("btn-cancel"), RegexOptions.IgnoreCase);

            return TokenUtils.Tokenize(template, null, null, false, true);
        }

        public string InitScript(string baseRelPath)
        {
            // register group validation
            var script = @"
                ";

            JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
            string jsonLocalization = jsonSerializer.Serialize(new AdminApi().GetLocalization(CultureInfo.CurrentUICulture.Name));

            script += "initForm(afjQuery('#dnn" + Settings.ModuleId + "root'), { appRoot: '" + ActionFormController.JsonEncode(baseRelPath) + "' }, " + jsonLocalization + ");";
            return script;
        }



        public void RegisterClientValidator(ValidatorDef validDef, Page page, ModuleInfo module)
        {
            var script = string.Format(@"
                afjQuery(function() {{
                    afjQuery.validator.addMethod('{0}', function(value, element) {{
                        var isValid;
                        {1}
                        return this.optional(element) || isValid;
                    }}, '{2}');
                }});", validDef.JsValidatorName + module.ModuleID, validDef.GenerateJsValidationCode("isValid", "value", module), ActionFormController.JsonEncode(ActionFormController.Localize(validDef.ErrorMessageKey, validDef.ErrorMessageDefault)));

            page.ClientScript.RegisterClientScriptBlock(GetType(), "afjQueryValidator-" + validDef.JsValidatorName + module.ModuleID, script, true);

        }
    }
}
