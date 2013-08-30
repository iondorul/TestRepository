using avt.ActionForm.Utils;
using DotNetNuke.Entities.Modules;
using System;
using System.Web;

namespace avt.ActionForm
{
    public partial class Manage : PortalModuleBase
    {
        protected string StrAlias { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            ModuleConfiguration.ModuleControl.ControlTitle =
                new ModuleController().GetModule(ModuleId, TabId, false).ModuleTitle;

            StrAlias = PortalControllerEx.SanitizePortalAlias(Request.Url, PortalSettings.PortalAlias);

            var settings = new ActionFormSettings(ModuleId);
            if (settings.IsInitialized.Value) {
                frmAdmin.Attributes["src"] = string.Format("{0}/Manage.aspx?alias={1}&mid={2}&tabid={3}",
                    TemplateSourceDirectory, HttpUtility.UrlEncode(StrAlias), ModuleId, TabId);
            } else {
                frmAdmin.Attributes["src"] = string.Format("{0}/Initialize.aspx?alias={1}&mid={2}&tabid={3}",
                    TemplateSourceDirectory, HttpUtility.UrlEncode(StrAlias), ModuleId, TabId);
            }

            if (!Page.ClientScript.IsClientScriptIncludeRegistered("afJquery")) {
                if (DotNetNuke.Common.Globals.DataBaseVersion.Major < 7) {
                    Page.ClientScript.RegisterClientScriptInclude("afJquery", TemplateSourceDirectory + "/js/jquery-1.8.3.js?v=" + avt.ActionForm.ActionFormController.Build);
                } else {
                    Page.ClientScript.RegisterClientScriptBlock(GetType(), "afJquery", "if (window.jQuery) { afjQuery = jQuery; } else { document.write('<script src=\"" + TemplateSourceDirectory + "/js/jquery-1.8.3.js?v=" + avt.ActionForm.ActionFormController.Build + "\"><\\/script>'); }", true);
                }
            }
        }

    }

}
