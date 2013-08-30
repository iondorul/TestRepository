using avt.ActionForm.Core.Config;
using avt.ActionForm.Core.Form;
using avt.ActionForm.Core.Validation;
using avt.ActionForm.RegCore;
using avt.ActionForm.Templating;
using avt.ActionForm.Utils;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Framework;
using DotNetNuke.Security;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Exceptions;
using System;
using System.IO;
using System.Web;
using System.Web.UI;

namespace avt.ActionForm
{
    public partial class Main : PortalModuleBase, IActionable
    {
        ActionFormSettings _Settings = null;
        protected ActionFormSettings AfSettings
        {
            get
            {
                if (_Settings == null) {
                    _Settings = new ActionFormSettings();
                    _Settings.Load(ModuleId);
                }
                return _Settings;
            }
        }

        protected IActionFormTemplate FormTemplate { get; set; }

        protected bool IsAdmin
        {
            get
            {
                bool isAdmin = false;
                if (PortalSettings.UserMode == DotNetNuke.Entities.Portals.PortalSettings.Mode.Edit &&
                    ((ModuleId != -1 && ModulePermissionController.CanAdminModule(ModuleConfiguration)) || PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName))) {
                    isAdmin = true;
                }

                return isAdmin;
            }
        }

        protected string JsonEncode(string jsonStr)
        {
            return ActionFormController.JsonEncode(jsonStr);
        }


        //protected void Page_Init(object sender, EventArgs e)
        //{
        //    //ListController cc = new ListController();
        //    //ListEntryInfoCollection ec = cc.GetListEntryInfoCollection("Region", "Country.US");
        //    //Response.Write(ec.Count);

            
        //}

        protected void Page_Load(object sender, EventArgs e)
        {
            ModuleConfiguration.ModuleControl.ControlTitle = "Test";

            Control ctrlAct = LoadControl(TemplateSourceDirectory.TrimEnd('/') + "/RegCore/QuickStatusAndLink.ascx");
            (ctrlAct as IRegCoreComponent).InitRegCore(IsAdmin, ActionFormController.RegCoreServer, ActionFormController.ProductName, ActionFormController.ProductCode, ActionFormController.ProductKey, ActionFormController.Version, TemplateSourceDirectory.TrimEnd('/') + "/RegCore/", typeof(ActionFormController));
            this.cFormTemplate.Controls.Add(ctrlAct);
            pnlMessage.Attributes["class"] = "pnlMessage " + AfSettings.jQueryTheme;

            //if (PortalSettings.UserMode == DotNetNuke.Entities.Portals.PortalSettings.Mode.Edit && 
            //    (PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName) || PortalSecurity.HasNecessaryPermission(SecurityAccessLevel.Edit, PortalSettings, ModuleConfiguration))) {
            //    pnlAdmin.Visible = true;
            //} else {
            //    pnlAdmin.Visible = false;
            //}

            // TODO: ASPX script for paypal
            //if (Request.QueryString["paypal-continue"] != null) {
            //    PayPalController payPal = new PayPalController();
            //    payPal.ProcessPayment();
            //    return;
            //}

            BindData();

            
        }

        void BindData()
        {
            ActionFormController afCtrl = new ActionFormController();
            if (!afCtrl.IsActivated() || afCtrl.IsTrialExpired()) {
                return;
            }

            if (!AfSettings.IsInitialized.Value) {
                lblContent.Text = "This <i>Action Form</i> is not configured!<br/> Proceed to <a href='" + EditUrl("Manage") + "'>Manage Form</a> screen...";
                return;
            }

            //if (!string.IsNullOrEmpty(Request.QueryString["afdl"]) && Request.QueryString["afdl"] == ModuleId.ToString() && AfSettings.ResourceAccess == ResourceOpenMode.ForceDownload) {

            //    string filePath = HttpContext.Current.Server.MapPath(AfSettings.TargetUrl());

            //    if (!File.Exists(filePath)) {
            //        Response.Write("Invalid file!");
            //        return;
            //    }

            //    string friendlyFileName = Path.GetFileName(filePath);
            //    if (Path.GetExtension(friendlyFileName).ToLower() == ".resources") {
            //        friendlyFileName = friendlyFileName.Substring(0, friendlyFileName.Length - ".resources".Length);
            //    }

            //    Response.Clear();
            //    Response.ContentType = "application/octet-stream";
            //    Response.AppendHeader("Content-Disposition", "attachment; filename=\"" + friendlyFileName + "\"");
            //    Response.TransmitFile(filePath);
            //    Response.End();
            //    return;
            //}

            FormData initData = null;
            try {
                initData = AfSettings.Init();
            } catch (Exception ex) {
                pnlMessage.Visible = true;
                pnlMessage.InnerHtml = ex.Message;
                return;
            }

            if (AfSettings.OpenFormMode.Value != FormOpenMode.Always)
                lblContent.Text = AfSettings.FrontEndTemplateTokenized(TabId);

            pnlDialogContainer.Attributes["class"] = AfSettings.jQueryTheme.Value;

            if (AfSettings.OpenFormMode.Value != FormOpenMode.Page) {

                LoadClientDeps(AfSettings);

                // init javascripts
                lblInitJs.Text = TokenUtils.Tokenize(AfSettings.InitJs.Value);

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

                if (AfSettings.OpenFormMode.Value == FormOpenMode.Inline) {
                    pnlScriptInline.Visible = true;
                    FormTemplate.CancelUrl = "hideFormInline" + ModuleId + "();";
                } else if (AfSettings.OpenFormMode.Value == FormOpenMode.Popup) {
                    pnlScriptPopup.Visible = true;
                    FormTemplate.CancelUrl = "hideFormPopup" + ModuleId + "();";
                } else {
                    pnlScriptAlways.Visible = true;
                }

                FormTemplate.Render(phFormTemplate);

                if (AfSettings.OpenFormMode.Value == FormOpenMode.Inline || AfSettings.OpenFormMode.Value == FormOpenMode.Popup) {
                    phFormTemplate.Style["display"] = "none";
                }
                phFormTemplate.Attributes["class"] = "phFormTemplate " + AfSettings.jQueryTheme;

                //scriptDlgSubmitImage.Visible = AfSettings.SubmitType.Value == "image";
                //scriptDlgCancelImage.Visible = AfSettings.CancelType.Value == "image";

                // init controls
                FormTemplate.InitControls(AfSettings);
            }

            //DataBind();
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


        public ModuleActionCollection ModuleActions
        {
            get
            {

                ActionFormController aformCtrl = new ActionFormController();
                if (!aformCtrl.IsActivated() || aformCtrl.IsTrialExpired()) {
                    return new ModuleActionCollection();
                }

                ModuleActionCollection Actions = new ModuleActionCollection();
                //Actions.Add(GetNextActionID(), "Add Redirect", DotNetNuke.Entities.Modules.Actions.ModuleActionType.ModuleSettings, "", "add.gif", EditUrl("Edit"), false, DotNetNuke.Security.SecurityAccessLevel.Edit, true, false); 
                Actions.Add(GetNextActionID(), "Manage Form", DotNetNuke.Entities.Modules.Actions.ModuleActionType.AddContent, "", "icon_siteSettings_16px.gif", EditUrl("Manage"), false, DotNetNuke.Security.SecurityAccessLevel.Edit, true, false);
                if (AfSettings.IsInitialized.Value) {
                    var standAloneUrl = string.Format("{0}/Manage.aspx?alias={1}&mid={2}&tabid={3}",
                    TemplateSourceDirectory, HttpUtility.UrlEncode(PortalAlias.HTTPAlias), ModuleId, TabId);
                    Actions.Add(GetNextActionID(), "Manage Form (full screen)", DotNetNuke.Entities.Modules.Actions.ModuleActionType.AddContent, "", "icon_siteSettings_16px.gif", standAloneUrl, false, DotNetNuke.Security.SecurityAccessLevel.Edit, true, false);
                }
                Actions.Add(GetNextActionID(), "Reports", DotNetNuke.Entities.Modules.Actions.ModuleActionType.AddContent, "", "icon_lists_16px.gif", EditUrl("Reports"), false, DotNetNuke.Security.SecurityAccessLevel.Edit, true, false);

                if (AfSettings.HasCustomLayout.Value) {
                    Actions.Add(GetNextActionID(), "Layout", DotNetNuke.Entities.Modules.Actions.ModuleActionType.AddContent, "", "icon_lists_16px.gif", EditUrl("Layout"), false, DotNetNuke.Security.SecurityAccessLevel.Edit, true, false);
                }

                return Actions;
            }
        }

    }

}
