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
using System.Xml;

namespace avt.ActionForm
{
   
    public partial class Initialize : System.Web.UI.Page
    {
        protected PortalAliasInfo PortalAlias { get; set; }
        protected PortalSettings Portal { get; set; }
        protected TabInfo Tab { get; set; }
        protected ModuleInfo Module { get; set; }
        protected UserInfo DnnUser { get; set; }
        protected string StrAlias { get; set; }

        protected string ViewUrl { get; set; }

        protected string EditHtmlLayoutUrl { get; set; }

        public IList<string> FormTemplates { get; set; }

        protected class FormTemplate
        {
            public FormTemplate()
            { 
            }

            public FormTemplate(string file)
            {
                FilePath = file;
                Name = Path.GetFileNameWithoutExtension(file);

                var xml = new XmlDocument();
                xml.Load(FilePath);
                try {
                    Description = xml.DocumentElement["ActionForm"]["Description"].InnerText;
                } catch { }
            }

            public string FilePath { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }

            public void Load(int moduleId)
            {
                var xml = new XmlDocument();
                xml.Load(FilePath);

                ActionFormSettings settings = new ActionFormSettings();
                settings.Load(moduleId, xml.DocumentElement.ChildNodes[0]);
                settings.Save();
            }
        }


        protected void Page_Init(object sender, EventArgs e)
        {
            
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

            if (!Page.IsPostBack) {
                rpTemplates.DataSource = GetFormTemplates();
                rpTemplates.DataBind();
            }
            rpTemplates.ItemCommand += rpTemplates_ItemCommand;
        }

        void rpTemplates_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            switch (e.CommandName) {
                case "fromTemplate":
                    var template = new FormTemplate(Server.MapPath(TemplateSourceDirectory + "/Config/Templates/" + e.CommandArgument + ".config"));
                    template.Load(Module.ModuleID);
                    Response.Redirect("Manage.aspx?" + Request.QueryString.ToString());
                    break;
            }
        }

        IList<FormTemplate> GetFormTemplates()
        {
            List<FormTemplate> templates = new List<FormTemplate>();
            templates.Add(new FormTemplate(Server.MapPath(TemplateSourceDirectory + "/Config/Templates/Blank Form.config")));

            foreach (string file in Directory.GetFiles(Server.MapPath(TemplateSourceDirectory + "/Config/Templates"))) {
                if ((!Path.GetExtension(file).Equals(".config", StringComparison.OrdinalIgnoreCase) && !Path.GetExtension(file).Equals(".xml", StringComparison.OrdinalIgnoreCase)) 
                    || Path.GetFileNameWithoutExtension(file).Equals("Blank Form"))
                    continue;

                templates.Add(new FormTemplate(file));
            }

            return templates;
        }

    }
}
