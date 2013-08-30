using avt.ActionForm.Core.Config;
using avt.ActionForm.Core.Form;
using avt.ActionForm.Core.Validation;
using avt.ActionForm.Templating;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Framework;
using DotNetNuke.Services.Exceptions;
using System;
using System.IO;
using System.Web;
using System.Web.UI;



namespace avt.ActionForm
{
    public partial class FormPage : PortalModuleBase //, IActionable
    {
        protected IActionFormTemplate FormTemplate { get; set; }

        ActionFormSettings _Settings = null;
        protected ActionFormSettings AfSettings {
            get {
                if (_Settings == null) {
                    _Settings = new ActionFormSettings();
                    _Settings.Load(ModuleId);
                }
                return _Settings;
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            //ModuleConfiguration.ModuleTitle = AfSettings.FormTitle;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!AfSettings.IsInitialized.Value) {
                phTemplate.InnerHtml = "This <i>Action Form</i> is not configured...";
                return;
            }

            FormData initData = null;
            try {
                initData = AfSettings.Init();
            } catch (Exception ex) {
                pnlMessage.Visible = true;
                pnlMessage.InnerHtml = ex.Message;
                return;
            }

            LoadClientDeps(AfSettings);
            pnlMessage.Attributes["class"] = AfSettings.jQueryTheme.Value;

            // init javascripts
            lblInitJs.Text = AfSettings.InitJs.Value;

            FormTemplate = TemplateFactory.New(Path.Combine(TemplateSourceDirectory, "templates/Form"), AfSettings, initData);
            
            // register validators
            var validationDefitions = ItemsFromXmlConfig<ValidatorDef>.GetConfig(Server.MapPath(TemplateSourceDirectory + "/Config/Validators"));
            foreach (var validDef in validationDefitions.Items) {
                FormTemplate.RegisterClientValidator(validDef, Page, ModuleConfiguration);
            }

            var groupValidationDefs = ItemsFromXmlConfig<GroupValidatorDef>.GetConfig(Server.MapPath(TemplateSourceDirectory + "/Config/GroupValidators"));
            foreach (var validDef in groupValidationDefs.Items) {
                if (!string.IsNullOrEmpty(validDef.JsValidationCode))
                    Page.ClientScript.RegisterClientScriptBlock(GetType(), "afjQueryValidator-" + validDef.JsValidatorName, validDef.JsValidationCode, true);
            }

            FormTemplate.CancelUrl = "window.location='" + DotNetNuke.Common.Globals.NavigateURL(TabId) + "'";
            //FormTemplate.LoadForm(AfSettings, initData);

            FormTemplate.Render(phTemplate);
            phTemplate.Attributes["class"] = AfSettings.jQueryTheme.Value;
        }

        void LoadClientDeps(ActionFormSettings settings)
        {

            if (!Page.ClientScript.IsClientScriptIncludeRegistered("afJquery")) {
                if (DotNetNuke.Common.Globals.DataBaseVersion.Major < 7) {
                    Page.ClientScript.RegisterClientScriptInclude("afJquery", TemplateSourceDirectory + "/js/jquery-1.8.3.js?v=" + avt.ActionForm.ActionFormController.Build);
                } else {
                    jQuery.RequestRegistration();
                    HttpContext.Current.Items["jQueryDnnPluginsRequested"] = true;
                    Page.ClientScript.RegisterClientScriptBlock(GetType(), "afJquery", "if (window.jQuery) { afjQuery = jQuery; } else { document.write('<script src=\"" + TemplateSourceDirectory + "/js/jquery-1.8.3.js?v=" + avt.ActionForm.ActionFormController.Build + "\"><\\/script>'); }", true);
                }
            }

            // also include the popup scripts
            if (!Page.ClientScript.IsClientScriptIncludeRegistered("afJqueryUi")) {
                if (DotNetNuke.Common.Globals.DataBaseVersion.Major < 7) {
                    Page.ClientScript.RegisterClientScriptInclude("afJqueryUi", TemplateSourceDirectory + "/js/jquery-ui-1.9.2.js?v=" + avt.ActionForm.ActionFormController.Build);
                } else {
                    Page.ClientScript.RegisterClientScriptBlock(GetType(), "afJqueryUi", "if (!afjQuery.ui) { document.write('<script src=\"" + TemplateSourceDirectory + "/js/jquery-ui-1.9.2.js?v=" + avt.ActionForm.ActionFormController.Build + "\"><\\/script>'); }", true);
                }
            }

            // now register client script blocks
            Page.ClientScript.RegisterClientScriptBlock(GetType(), "initClientScripts-" + ModuleId, ";" + string.Join(";", settings.ClientScripts.ToArray()), true);

            //if (DotNetNuke.Common.Globals.DataBaseVersion.Major < 7) {
            CDefault defaultPage = (CDefault)Page;
            if (!string.IsNullOrEmpty(settings.jQueryTheme.Value)) {
                defaultPage.AddStyleSheet("AFormJQueryTheme" + settings.jQueryTheme, TemplateSourceDirectory + "/templates/jQuery/" + settings.jQueryTheme + "/jquery-ui.css");
            }
            //}
        }


    }

}
