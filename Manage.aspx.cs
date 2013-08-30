using System.Reflection;
using avt.ActionForm.Core.Config;
using avt.ActionForm.RegCore;
using avt.ActionForm.Utils;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Permissions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace avt.ActionForm
{
    public partial class ManageNew : System.Web.UI.Page
    {
        protected PortalAliasInfo PortalAlias { get; set; }
        protected PortalSettings Portal { get; set; }
        protected TabInfo Tab { get; set; }
        protected ModuleInfo Module { get; set; }      
        protected UserInfo DnnUser { get; set; }
        protected string StrAlias { get; set; }

        protected string UnlockTrialUrl { get; set; }
        protected string ActivateUrl { get; set; }
        protected string ViewUrl { get; set; }

        protected string EditHtmlLayoutUrl { get; set; }

        protected void Page_Init(object sender, EventArgs e)
        {           
            //var actions = new List<ActionDefinition>();
            //actions.Add(new ActionDefinition() {
            //    Parameters = new List<ActionParameterDefinition>() {
            //        new ActionParameterDefinition() { Name = "ca" },
            //        new ActionParameterDefinition() { },
            //        new ActionParameterDefinition() { }
            //    }
            //});
            //actions.Add(new ActionDefinition());

            //System.Xml.Serialization.XmlSerializer xmlSerializer = new System.Xml.Serialization.XmlSerializer(actions.GetType());
            //StringBuilder xmlSb = new StringBuilder();
            //System.Xml.XmlWriter xmlWriter = System.Xml.XmlWriter.Create(xmlSb);
            //xmlSerializer.Serialize(xmlWriter, actions);
            //Response.Write(xmlSb);

            //var actions2 = new ConfigFolder<ActionDefinition>(Server.MapPath(TemplateSourceDirectory) + "\\Config\\Actions");
            //var items = actions2.GetItems("Title");
        }

        protected void Page_Load(object sender, EventArgs e)
        {         

            if (string.IsNullOrEmpty(Request.QueryString["alias"])) {
                throw new Exception("Invalid portal alias.");
            }

            Portal = PortalControllerEx.GetCurrentPortal(Request.QueryString["alias"]);
            if (Portal == null)
                throw new Exception("Invalid portal " + PortalAlias.PortalID);

            PortalAlias = Portal.PortalAlias;
            if (PortalAlias == null)
                throw new Exception("Invalid portal alias " + Request.QueryString["alias"]);


            if (string.IsNullOrEmpty(Request.QueryString["tabId"]))
                throw new Exception("Invalid tab ID");

            int tabId;
            if (!int.TryParse(Request.QueryString["tabId"], out tabId))
                throw new Exception("Invalid tab ID " + Request.QueryString["tabId"]);

            Tab = new TabController().GetTab(tabId, Portal.PortalId, false);
            if (Tab == null)
                throw new Exception("Invalid tab " + tabId);


            if (string.IsNullOrEmpty(Request.QueryString["mid"]))
                throw new Exception("Invalid module ID");

            int modId;
            if (!int.TryParse(Request.QueryString["mid"], out modId))
                throw new Exception("Invalid module ID " + Request.QueryString["mid"]);

            Module = new ModuleController().GetModule(modId, tabId);
            if (Module == null)
                throw new Exception("Invalid module " + modId);

            StrAlias = PortalControllerEx.SanitizePortalAlias(Request.Url, PortalAlias);

            ViewUrl = DotNetNuke.Common.Globals.NavigateURL(Tab.TabID);

            DnnUser = UserController.GetCurrentUserInfo();

            if (!ModulePermissionController.CanEditModuleContent(Module)) {
                // redirect to access denied page/login
                //Page.ClientScript.RegisterClientScriptBlock(GetType(), "access-denied", "top.location = '" + DotNetNuke.Common.Globals.AccessDeniedURL() +"';", true);
                Response.Write("Access Denied!");
                Response.End();
            }

            EditHtmlLayoutUrl = DotNetNuke.Common.Globals.NavigateURL(Module.TabID, "Layout", "mid=" + Module.ModuleID);

            pnlNoActivations.Visible = ActionFormController.RegCore.AllActivations.Count == 0;
            pnlActivations.Visible = ActionFormController.RegCore.AllActivations.Count > 0;

            rpActivations.DataSource = ActionFormController.RegCore.AllActivations.Values;
            rpActivations.DataBind();

            btnUnlockTrial.Visible = ActionFormController.RegCore.AllActivations.Values.FirstOrDefault(x => x.RegCode.IsTrial) == null;

            var manageUrl = TemplateSourceDirectory + "/RegCore";

            // append refresh to return URL so we know to refresh the page
            var returnUrl = Request.RawUrl;
            returnUrl += "#refresh";

            UnlockTrialUrl = manageUrl + "/UnlockTrial.aspx?t=" + HttpUtility.UrlEncode(typeof(ActionFormController).AssemblyQualifiedName) + "&rurl=" + HttpUtility.UrlEncode(returnUrl);
            ActivateUrl = manageUrl + "/Activation.aspx?t=" + HttpUtility.UrlEncode(typeof(ActionFormController).AssemblyQualifiedName) + "&rurl=" + HttpUtility.UrlEncode(returnUrl);

            Control ctrlAct = LoadControl(TemplateSourceDirectory.TrimEnd('/') + "/RegCore/QuickStatusAndLink.ascx");
            (ctrlAct as IRegCoreComponent).InitRegCore(true, ActionFormController.RegCoreServer, ActionFormController.ProductName, ActionFormController.ProductCode, ActionFormController.ProductKey, ActionFormController.Version, TemplateSourceDirectory.TrimEnd('/') + "/RegCore/", typeof(ActionFormController));
            pnlLicense.Controls.Add(ctrlAct);

            // serialize constant data right from the start to save requests
            RegisterJson("g_fieldDefs", ActionFormController.InputTypes);
            RegisterJson("g_predefFields", ActionFormController.PredefinedFields);
            RegisterJson("g_validatorDefs", ActionFormController.ValidatorDefs);
            RegisterJson("g_groupValidatorDefs", ActionFormController.GroupValidatorDefs);
            RegisterJson("g_actionDefs", ActionFormController.ActionDefs);
            RegisterJson("g_templates", GetTemplates());
            RegisterJson("g_jQueryThemes", GetjQueryThemes());
        }

        void RegisterJson(string varName, object obj)
        {
            string json = new JavaScriptSerializer().Serialize(obj);
            Page.ClientScript.RegisterClientScriptBlock(GetType(), "init" + varName, "var " + varName + " = " + json + ";", true);
        }

        IList<string> GetTemplates()
        {
            List<string> templates = new List<string>();

            foreach (string dir in System.IO.Directory.GetDirectories(Server.MapPath(TemplateSourceDirectory + "/templates/Form"))) {
                if (System.IO.Path.GetFileName(dir)[0] == '.')
                    continue;
                
                templates.Add(Path.GetFileName(dir));
            }

            return templates;
        }

        IList<string> GetjQueryThemes()
        {
            List<string> themes = new List<string>();

            foreach (string dir in System.IO.Directory.GetDirectories(Server.MapPath(TemplateSourceDirectory + "/templates/jQuery"))) {
                if (System.IO.Path.GetFileName(dir)[0] == '.') {
                    continue;
                }

                themes.Add(System.IO.Path.GetFileName(dir));
            }

            return themes;
        }

    }
}
