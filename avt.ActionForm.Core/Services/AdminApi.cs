using avt.ActionForm.Core.Actions;
using avt.ActionForm.Utils;
using avt.ActionForm.Utils.Api;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Security.Roles;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Linq;
using System.Web.Script.Serialization;
using avt.ActionForm.Core.Utils;
using System;
using avt.ActionForm.Core.Fields;
using System.Xml.XPath;

namespace avt.ActionForm
{
    //TODO: maybe an attribute to take care of security??

    /// <summary>
    /// This file exists here because .ashx files do not support CodeFile attribute
    /// </summary>
    public class AdminApi : IHttpHandler
    {
        #region IHttpHandler Members

        public bool IsReusable
        {
            // Return false in case your Managed Handler cannot be reused for another request.
            // Usually this would be false in case you have some state information preserved per request.
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            // extract some common variables
            var portal = PortalControllerEx.GetCurrentPortal(context);

            // register the API and some dependency injection
            var api = new ApiContext();
            api.Container.RegisterProperty("portalAlias", () => portal.PortalAlias);
            api.Container.RegisterProperty("portalSettings", () => portal);
            api.Container.RegisterProperty("portalId", () => portal == null ? -1 : portal.PortalId);
            api.Container.RegisterProperty("moduleInfo", () => new ModuleController().GetModule(ConvertUtils.Cast<int>(context.Request.QueryString["mid"], -1)));
            api.Container.RegisterProperty("moduleId", () => ConvertUtils.Cast<int>(context.Request.QueryString["mid"], -1));

            api.Execute(this, context);
        }

        #endregion


        [WebMethod(DefaultResponseType = eResponseType.Json)]
        public bool Refresh()
        {
            //App.Instance.ClearCache();
            return true;
        }

        [WebMethod(DefaultResponseType = eResponseType.Json, RequiredEditPermissions = false)]
        public Dictionary<string, string> GetLocalization(string locale)
        {
            var resourceFolder = Path.Combine(HttpRuntime.AppDomainAppPath, "DesktopModules/AvatarSoft/ActionForm/App_LocalResources");
            var resourceFile = Path.Combine(resourceFolder, "Form." + locale + ".resx");
            if (!File.Exists(resourceFile))
                resourceFile = Path.Combine(resourceFolder, "Form.resx");

            var doc = new XPathDocument(resourceFile);
            var resources = new Dictionary<string, string>();
            foreach (XPathNavigator nav in doc.CreateNavigator().Select("root/data")) {
                if (nav.NodeType != XPathNodeType.Comment) {
                    var selectSingleNode = nav.SelectSingleNode("value");
                    if (selectSingleNode != null) {
                        resources[nav.GetAttribute("name", String.Empty)] = selectSingleNode.Value;
                    }
                }
            }

            return resources;
        }

        [WebMethod(DefaultResponseType = eResponseType.Json, RequiredEditPermissions = true)]
        public IEnumerable<ListItem> GetData(ModuleInfo moduleInfo, PortalSettings portalSettings, string dataSource)
        {
            // first, try to see if it's a type
            var type = Type.GetType(dataSource);
            if (type != null && type.IsEnum) {
                return ListItem.FromEnum(type);
            }

            switch (dataSource.ToLower()) {
                case "roles":
                    return from RoleInfo r in new RoleController().GetPortalRoles(moduleInfo.PortalID)
                           select new ListItem() { Name = r.RoleName, Value = r.RoleID.ToString() };
                case "portalfiles":
                    List<ListItem> items = new List<ListItem>();
                    PopulateFiles(items, portalSettings, "");
                    return items;
                case "portalfolders":
                    List<ListItem> folders = new List<ListItem>();
                    PopulateFolders(folders, portalSettings, "");
                    return folders;
                case "portalpages":
                    List<ListItem> pages = new List<ListItem>();
                    PopulatePages(pages, TabController.GetTabsByParent(-1, portalSettings.PortalId), "");
                    return pages;
                case "recurringpayments":
                    return ListItem.FromEnum<ePayPalRecurringInterval>();
                case "currency":
                    return from c in Currency.LoadFromConfiguration(Path.Combine(ActionFormSettings.ModulePhysicalPath, "Config/Currency.xml"))
                           select new ListItem() { Name = c.Name, Value = c.Code };
            }

            return null;
        }

        void PopulateFiles(List<ListItem> items, PortalSettings portalSettings, string portalFolder)
        {
            var fileTypes = (HttpContext.Current.Request.QueryString["FileTypes"] ?? "").Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

            var physicalFolder = Path.Combine(portalSettings.HomeDirectoryMapPath, portalFolder);
            foreach (string subFolderPath in Directory.GetDirectories(physicalFolder)) {

                var folderName = Path.GetFileName(subFolderPath);
                if (folderName.Equals("Cache", System.StringComparison.OrdinalIgnoreCase))
                    continue;

                var subPortalFolder = Path.Combine(portalFolder, folderName);
                PopulateFiles(items, portalSettings, subPortalFolder);
            }

            foreach (string filePath in Directory.GetFiles(physicalFolder)) {
                if (fileTypes.Length > 0 && !fileTypes.Contains(Path.GetExtension(filePath)))
                    continue;

                var fileName = Path.GetFileName(filePath);
                var relPath = Path.Combine(portalFolder, fileName);
                items.Add(new ListItem() { Name = relPath, Value = relPath });
            }
        }

        void PopulateFolders(List<ListItem> items, PortalSettings portalSettings, string portalFolder)
        {
            if (string.IsNullOrEmpty(portalFolder)) {
                portalFolder = "/";
            } else {
                portalFolder = portalFolder.Replace('\\', '/');
            }

            items.Add(new ListItem() { Name = portalFolder, Value = portalFolder });

            var physicalFolder = Path.Combine(portalSettings.HomeDirectoryMapPath, portalFolder.TrimStart('/'));
            foreach (string subFolderPath in Directory.GetDirectories(physicalFolder)) {

                var folderName = Path.GetFileName(subFolderPath);
                if (folderName.Equals("Cache", System.StringComparison.OrdinalIgnoreCase))
                    continue;

                var subPortalFolder = Path.Combine(portalFolder, folderName);
                if (subPortalFolder.Equals("/Users", StringComparison.OrdinalIgnoreCase) ||
                    subPortalFolder.Equals("/Logs", StringComparison.OrdinalIgnoreCase) ||
                    subPortalFolder.Equals("/Cache", StringComparison.OrdinalIgnoreCase))
                    continue;

                PopulateFolders(items, portalSettings, subPortalFolder);
            }

        }

        void PopulatePages(List<ListItem> items, List<TabInfo> tabs, string ident)
        {
            foreach (TabInfo tab in tabs) {
                items.Add(new ListItem() { Name = ident + tab.TabName, Value = tab.TabID });
                PopulatePages(items, TabController.GetTabsByParent(tab.TabID, tab.PortalID), ident + "....");
            }
        }


        [WebMethod(DefaultResponseType = eResponseType.Json, RequiredEditPermissions = true)]
        public ActionFormSettings GetSettings(ModuleInfo moduleInfo)
        {
            return new ActionFormSettings(moduleInfo.ModuleID);
        }

        [WebMethod(DefaultResponseType = eResponseType.Json, RequiredEditPermissions = true)]
        public ActionFormSettings SaveSettings(ModuleInfo moduleInfo)
        {
            var json = new StreamReader(HttpContext.Current.Request.InputStream).ReadToEnd();
            var settings = new JavaScriptSerializer().Deserialize<ActionFormSettings>(json);

            // ensure nobody can fool us to update a different module
            settings.ModuleId = moduleInfo.ModuleID;
            settings.Save();

            return GetSettings(moduleInfo);
        }


        [WebMethod(DefaultResponseType = eResponseType.Json, RequiredEditPermissions = true)]
        public IEnumerable<FormField> GetFields(ModuleInfo moduleInfo)
        {
            var settings = new ActionFormSettings(moduleInfo.ModuleID);
            if (settings.IsInitialized.Value)
                return settings.Fields;

            return new FormField[0];

            //// not initialized, return some predefined fields so the form is not initially empy
            //var fields = PredefinedField.GetFieldsToAddOnInit(Path.Combine(ActionFormSettings.ModulePhysicalPath, "Config\\PredefinedFields"));
            //var row = 0;
            //foreach (var field in fields) {
            //    field.RowIndex = row++;
            //}

            //return fields;
        }

        [WebMethod(DefaultResponseType = eResponseType.Json, RequiredEditPermissions = true)]
        public IList<FormField> SaveFields(ModuleInfo moduleInfo)
        {
            var settings = new ActionFormSettings(moduleInfo.ModuleID);
            if (!settings.IsInitialized.Value)
                settings.Save(); // force initialize on settings

            var json = new StreamReader(HttpContext.Current.Request.InputStream).ReadToEnd();
            var fields = new JavaScriptSerializer().Deserialize<List<FormField>>(json);
            if (fields != null) {

                //var orderIndex = 0;
                foreach (var field in fields) {

                    // check if we need to delete this action
                    if (field.IsDeleted) {
                        field.Delete();
                        continue;
                    }

                    var dic = field.Parameters.GetValue<Dictionary<string, object>>("ShowIn", null);
                    if (dic != null && dic.ContainsKey("ButtonsPane") && (bool)dic["ButtonsPane"])
                        field.ViewOrder = 9999;

                    //field.ViewOrder = orderIndex++;
                    field.ModuleId = moduleInfo.ModuleID;
                    field.Save();
                }
                // TODO: maybe not clear all cache always?
                //App.Instance.ClearCache();
            }

            // refresh from DB
            //settings.Load(moduleInfo.ModuleID);

            settings.Fields = null;
            return settings.Fields;
        }



        [WebMethod(DefaultResponseType = eResponseType.Json, RequiredEditPermissions = true)]
        public IList<ActionInfo> GetEventActions(ModuleInfo moduleInfo, string eventName)
        {
            return ActionInfo.GetAllByProperty("OrderIndex",
                new KeyValuePair<string, object>("ModuleId", moduleInfo.ModuleID),
                new KeyValuePair<string, object>("EventName", eventName));
            //return PortalUrlSettings.Get(moduleInfo.PortalID);
            //return PortalUrlSettings.Get(-1);
        }

        [WebMethod(DefaultResponseType = eResponseType.Json, RequiredEditPermissions = true)]
        public IList<ActionInfo> SaveEventActions(ModuleInfo moduleInfo, string eventName)
        {
            var settings = new ActionFormSettings(moduleInfo.ModuleID);
            if (!settings.IsInitialized.Value)
                settings.Save(); // force initialize on settings

            var json = new StreamReader(HttpContext.Current.Request.InputStream).ReadToEnd();
            var actions = new JavaScriptSerializer().Deserialize<List<ActionInfo>>(json);
            if (actions != null) {

                var orderIndex = 0;
                foreach (var action in actions) {

                    // check if we need to delete this action
                    if (action.IsDeleted) {
                        action.Delete();
                        continue;
                    }

                    action.OrderIndex = orderIndex++;
                    action.ModuleId = moduleInfo.ModuleID;
                    action.Save();
                }
                // TODO: maybe not clear all cache always?
                //App.Instance.ClearCache();
            }

            return ActionInfo.GetAllByProperty("OrderIndex",
               new KeyValuePair<string, object>("ModuleId", moduleInfo.ModuleID),
               new KeyValuePair<string, object>("EventName", eventName));
        }
    }
}
