using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml;
using avt.ActionForm.Core.Config;
using avt.ActionForm.Core.Input;
using avt.ActionForm.RegCore;
using avt.ActionForm.RegCore.Storage;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using avt.ActionForm.Utils;
using System.IO;
using System.Text.RegularExpressions;
using avt.ActionForm.Core.Fields;
using avt.ActionForm.Core.Validation;
using avt.ActionForm.Core;


namespace avt.ActionForm
{
    public class ActionFormController : IUpgradeable, IPortable
    {

        #region RegCore

        public static bool IsAdmin { get { return DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo().IsInRole(DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings().AdministratorRoleName); } }
        public static string RegCoreServer { get { return "http://www.dnnsharp.com/DesktopModules/RegCore/"; } }
        public static string ProductName { get { return "Action Form"; } }
        public static string ProductCode { get { return "AFORM"; } }
        public static string ProductKey { get { return "<RSAKeyValue><Modulus>5ti+ykCPLEFUv658aKufKTrcno/ekvJFY2qbG0yIh9gn6/AJ6GdIvXtrTcuRhDqf0lMyX8Erh/8+EgoAJo7+sCQLqkOQ9ebj/+hYql118c4xa455FWaOC2k4PKzbK/tMvGn7ZDnULNc+8uEVBIuR6e2QAG98EpGzsf+jpXdtYds=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>"; } }
        public static string Version { get { return "3.1.0"; } }
        public static string Build
        {
            get
            {
                var version = System.Reflection.Assembly.GetAssembly(typeof(ActionFormController)).GetName().Version;
                return version.ToString();
            }
        }

        static public string DocSrv = RegCoreServer + "/Api.aspx?cmd=doc&product=" + ProductCode + "&version=" + Version;
        static public string BuyLink = RegCoreServer + "/Api.aspx?cmd=buy&product=" + ProductCode + "&version=" + Version;

        public List<ListItem> Hosts
        {
            get
            {
                List<ListItem> hosts = new List<ListItem>();
                PortalAliasController paCtrl = new PortalAliasController();
                foreach (DictionaryEntry de in paCtrl.GetPortalAliases()) {
                    PortalAliasInfo paInfo = (PortalAliasInfo)de.Value;
                    hosts.Add(new ListItem(paInfo.HTTPAlias, paInfo.HTTPAlias));
                }
                return hosts;
            }
        }
        internal IActivationDataStore GetActivationSrc()
        {
            return new DsLicFile();
        }

        public static IRegCoreClient RegCore
        {
            get
            {
                return RegCoreClient.Get(new RegCoreServer(RegCoreServer).ApiScript, ProductCode, new DsLicFile(), false);
            }
        }

        public bool IsActivated()
        {
            return RegCore.IsActivated(ProductCode, Version, HttpContext.Current.Request.Url.Host);
        }

        public bool IsTrial()
        {
            return RegCore.IsTrial(ProductCode, Version, HttpContext.Current.Request.Url.Host);
        }

        public bool IsTrialExpired()
        {
            return RegCore.IsTrialExpired(ProductCode, Version, HttpContext.Current.Request.Url.Host);
        }

        public int TrialDaysLeft()
        {
            return RegCore.GetValidActivation(ProductCode, Version, HttpContext.Current.Request.Url.Host).RegCode.DaysLeft;
        }

        public void Activate(string regCode, string host, string actCode)
        {
            if (string.IsNullOrEmpty(actCode)) {
                RegCore.Activate(regCode, ProductCode, Version, host, ProductKey);
            } else {
                RegCore.Activate(regCode, ProductCode, Version, host, ProductKey, actCode);
            }
        }

        #endregion

            

        public ActionFormController()
        {

        }

        public static string AppPath { get { return Path.Combine(HttpRuntime.AppDomainAppPath, "DesktopModules\\AvatarSoft\\ActionForm"); } }

        public static Dictionary<string, InputTypeDef> InputTypes
        {
            get
            {
                string configFolder = Path.Combine(HttpRuntime.AppDomainAppPath, "DesktopModules\\AvatarSoft\\ActionForm\\Config\\InputTypes");
                var ctlConfig = new ConfigFolder<InputTypeDef>(configFolder, App.GetMasterCacheKey());
                return ctlConfig.GetItems(null, "Name");
            }
        }

        public static Dictionary<string, FormField> PredefinedFields
        {
            get
            {
                string configFolder = AppPath.Trim('\\').Trim('/') + "\\Config\\PredefinedFields";
                ItemsFromXmlConfig<FormField> filedTypesDefitions = ItemsFromXmlConfig<FormField>.GetConfig(configFolder);
                return filedTypesDefitions.ItemsHash;
            }
        }

        public static IDictionary<string, ValidatorDef> ValidatorDefs
        {
            get
            {
                string configFolder = Path.Combine(HttpRuntime.AppDomainAppPath, "DesktopModules\\AvatarSoft\\ActionForm\\Config\\Validators");
                ItemsFromXmlConfig<ValidatorDef> validatorDefs = ItemsFromXmlConfig<ValidatorDef>.GetConfig(configFolder);
                return validatorDefs.ItemsHash;
            }
        }

        public static IDictionary<string, GroupValidatorDef> GroupValidatorDefs
        {
            get
            {
                string configFolder = Path.Combine(HttpRuntime.AppDomainAppPath, "DesktopModules\\AvatarSoft\\ActionForm\\Config\\GroupValidators");
                ItemsFromXmlConfig<GroupValidatorDef> validatorDefs = ItemsFromXmlConfig<GroupValidatorDef>.GetConfig(configFolder);
                return validatorDefs.ItemsHash;
            }
        }

        public static IDictionary<string, ActionDefinition> ActionDefs
        {
            get
            {
                string configFolder = Path.Combine(HttpRuntime.AppDomainAppPath, "DesktopModules\\AvatarSoft\\ActionForm\\Config\\Actions");
                var actionConfig = new ConfigFolder<ActionDefinition>(configFolder, App.GetMasterCacheKey());
                return actionConfig.GetItems();
            }
        }


        public static T CreateInstance<T>(string strDataType)
            where T : class
        {
            Type dataType = Type.GetType(strDataType);
            if (dataType == null) {
                dataType = Type.GetType(strDataType.Substring(0, strDataType.IndexOf(",") + 1) + typeof(T).Assembly.ToString());
            }

            return Activator.CreateInstance(dataType) as T;
        }

        public static string ApplyAllTokens(IDictionary<string, string> data, string input)
        {
            foreach (string k in data.Keys)
                input = Regex.Replace(input, "\\[" + k + "\\]", data[k], RegexOptions.IgnoreCase);
            return TokenUtils.Tokenize(input);
        }


        public static string GetUserIp()
        {
            string ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip)) {
                ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            return ip;
        }




        public static string JsonEncode(string s, bool appendDelimiters = false)
        {
            return JsonUtil.JsonEncode(s, appendDelimiters);
        }

        public static string GetLocaleFile()
        {
            var resourceFolder = Path.Combine(HttpRuntime.AppDomainAppPath, "DesktopModules\\AvatarSoft\\ActionForm\\App_LocalResources");
            var resourceFile = Path.Combine(resourceFolder, "Form." + System.Globalization.CultureInfo.CurrentUICulture.Name + ".resx");
            if (!File.Exists(resourceFile))
                resourceFile = Path.Combine(resourceFolder, "Form.resx");
            return resourceFile;
        }

        public static string Localize(string key, string defaultText = null)
        {
            var text = DotNetNuke.Services.Localization.Localization.GetString(key, GetLocaleFile());
            if (defaultText != null && string.IsNullOrEmpty(text))
                text = defaultText;
            return text;
        }

        public string UpgradeModule(string Version)
        {
            RegCore.Upgrade(ProductCode, Version, ProductKey, false);

            return "Done";
        }

        public string ExportModule(int moduleId)
        {
            ActionFormSettings afConfig = new ActionFormSettings();
            afConfig.Load(moduleId);

            StringBuilder strXML = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;
            XmlWriter Writer = XmlWriter.Create(strXML, settings);

            Writer.WriteStartElement("ActionForm");
            afConfig.Serialize(Writer, null, false);
            Writer.WriteEndElement(); // ("ActionForm");
            Writer.Close();

            return strXML.ToString();
        }

        public void ImportModule(int ModuleID, string Content, string Version, int UserID)
        {
            ModuleController modCtrl = new ModuleController();
            ModuleInfo module = modCtrl.GetModule(ModuleID, -1);

            PortalController portalCtrl = new PortalController();
            PortalInfo portal = portalCtrl.GetPortal(module.PortalID);

            XmlNode xmlRoot = DotNetNuke.Common.Globals.GetContent(Content, "ActionForm");

            ActionFormSettings afConfig = new ActionFormSettings();
            afConfig.Load(ModuleID, xmlRoot);
            afConfig.Save();
        }
    }

}