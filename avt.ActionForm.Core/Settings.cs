using avt.ActionForm.Core;
using avt.ActionForm.Core.Actions;
using avt.ActionForm.Core.Fields;
using avt.ActionForm.Core.Form;
using avt.ActionForm.Core.Form.Result;
using avt.ActionForm.Core.Utils;
using avt.ActionForm.Core.Utils.Config;
using avt.ActionForm.Data;
using avt.ActionForm.Utils;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Log.EventLog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace avt.ActionForm
{
    public enum FormOpenMode
    {
        Popup,
        Inline,
        Page,
        Always
    }

    public enum eLabelAlign
    {
        Default,
        Left,
        Center,
        Right,

        Top,
        Inside
    }

    public enum eFieldSpacing
    {
        Loose,
        Normal,
        Compact
    }


    public enum eRenderContext
    {
        Form,
        CustomLayout
    }

    public class ActionFormSettings : DbConfig
    {
        public static readonly string ModulePhysicalPath = Path.Combine(HttpRuntime.AppDomainAppPath, "DesktopModules\\AvatarSoft\\ActionForm");

        public ActionFormSettings()
            : base("avtActionForm_FormSettings")
        {
        }

        public ActionFormSettings(int moduleId)
            : base("avtActionForm_FormSettings")
        {
            Load(moduleId);
        }

        [Section]
        public int ModuleId { get; set; }

        [ScriptIgnore]
        public ModuleInfo Module { get { return new ModuleController().GetModule(ModuleId); } }

        public int PortalId { get { return new ModuleController().GetModule(ModuleId).PortalID; } }


        #region Initial State

        [Property(Default = "false")]
        public Setting<bool> IsInitialized { get; set; }

        /// <summary>
        /// Only used to determine if migration
        /// </summary>
        [Property(Default = "")]
        public Setting<string> TargetType { get; set; }

        [Property(Default = "")]
        public Setting<string> InitJs { get; set; }

        [Property(Default = FormOpenMode.Always)]
        public Setting<FormOpenMode> OpenFormMode { get; set; }

        public string FormUrl(int tabId)
        {
            switch (OpenFormMode.Value) {
                case ActionForm.FormOpenMode.Inline:
                    return "javascript: showFormInline" + ModuleId + "();";
                case ActionForm.FormOpenMode.Always:
                    return "javascript: ;";
                case ActionForm.FormOpenMode.Popup:
                    return "javascript: showFormPopup" + ModuleId + "();";
            }

            return DotNetNuke.Common.Globals.NavigateURL(tabId, "Form", "mid", ModuleId.ToString());
        }

        [Property(Default = "<p>This is a sample text, edit this to match the purpose and describe the resource you're targeting (page, file).<br/><br/><a href = \"[FormUrl]\">ACCESS this resource</a> now.</p>")]
        public Setting<string> FrontEndTemplate { get; set; }

        public string FrontEndTemplateTokenized(int tabId)
        {
            return TokenUtils.Tokenize(
                Regex.Replace(FrontEndTemplate.Value, "\\[FormUrl\\]", FormUrl(tabId), RegexOptions.IgnoreCase)
            );
        }

        [Property(Default = 600)]
        public Setting<int?> PopupWidth { get; set; }

        [Property(Default = null)]
        public Setting<int?> PopupHeight { get; set; }

        #endregion


        #region Form Setup

        public const string DefaultFormTemplate = "bootstrap";
        public const string DefaultjQueryTheme = "sunny";

        public eRenderContext RenderContext { get; set; }

        [Property(Default = DefaultFormTemplate)]
        public Setting<string> FormTemplate { get; set; }

        [Property(Default = DefaultjQueryTheme)]
        public Setting<string> jQueryTheme { get; set; }

        //[Property(Default = "Left")]
        //public Setting<eFormAlign> FormAlign { get; set; }

        [Property(Default = "Default")]
        public Setting<eLabelAlign> LabelAlign { get; set; }

        [Property(Default = null)]
        public Setting<int?> LabelWidth { get; set; }

        [Property]
        public Setting<eFieldSpacing> FieldSpacing { get; set; }

        [Property(Default = false)]
        public Setting<bool> HasCustomLayout { get; set; }

        [Property(Default = "")]
        public Setting<string> LayoutHtml { get; set; }

        [Property]
        public Setting<bool> IsDebug { get; set; }

        [Property(Default = true)]
        public Setting<bool> ClientSideValidation { get; set; }

        #endregion


        public void Debug(string message, params object[] args)
        {
            if (!IsDebug.Value)
                return;

            var objEv = new EventLogController();
            //objEventLogInfo.LogTypeKey = EventLogController.EventLogType..ToString();
            var objEventLogInfo = new LogInfo();
            objEventLogInfo.LogTypeKey = EventLogController.EventLogType.ADMIN_ALERT.ToString();
            //objEventLogInfo.LogTypeKey = "avt.ActionForm";
            objEventLogInfo.AddProperty("ActionForm", string.Format(message, args));
            objEv.AddLog(objEventLogInfo);
        }


        IList<FormField> _Fields = null;

        [ScriptIgnore]
        public IList<FormField> Fields
        {
            get
            {
                if (_Fields == null)
                    _Fields = FormField.GetAllByProperty("ViewOrder", new KeyValuePair<string, object>("ModuleId", ModuleId) );

                return _Fields;
            }
            set { _Fields = value; }
        }

        Dictionary<string, FormField> _FieldsByName;
        [ScriptIgnore]
        public Dictionary<string, FormField> FieldsByName
        {
            get
            {
                if (_FieldsByName == null) {
                    _FieldsByName = new Dictionary<string, FormField>(StringComparer.OrdinalIgnoreCase);
                    foreach (FormField field in Fields) {
                        _FieldsByName[field.TitleCompacted] = field;
                    }
                }
                return _FieldsByName;
            }
        }


        public string GetAjaxUrl(FormData data = null)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("{0}/DesktopModules/AvatarSoft/ActionForm/Submit.ashx?", HttpRuntime.AppDomainAppVirtualPath.TrimEnd('/'));

            // pass query string along and fill some more params
            NameValueCollection query = new NameValueCollection(HttpContext.Current.Request.QueryString);
            query["portalid"] = PortalId.ToString();
            query["mid"] = ModuleId.ToString();

            // also load report id 
            if (data != null) {
                if (data["ReportKey"] != null)
                    query["submission"] = data["ReportKey"];
                //if (data["CurrentAction"] != null)
                //    ajaxUrl += string.Format("&action={0}", data["CurrentAction"]);
            }

            // append it to url
            foreach (var key in query.AllKeys) {
                if (key == "b" || key == "event")
                    continue; // these will be appended later by each button
                sb.AppendFormat("{0}={1}&", key, HttpUtility.UrlEncode(query[key]));
            }
            sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
        }

        static object _lockMigrate = new object();

        public void Load(int moduleId)
        {
            ModuleId = moduleId;
            base.Load();

            // check if we need to migrate actions to new avtActionForm_FormActions table
            if (!string.IsNullOrEmpty(TargetType.Value)) { // modSettings.ContainsKey("TargetType")) {
                lock (_lockMigrate) {
                    // migrate actions
                    var sql = File.ReadAllText(Path.Combine(ModulePhysicalPath, "Config/Migrate.SqlDataProvider"))
                        .Replace("{ModuleId}", ModuleId.ToString())
                        .Replace("{databaseOwner}", DotNetNuke.Common.Utilities.Config.GetDataBaseOwner())
                        .Replace("{objectQualifier}", DotNetNuke.Common.Utilities.Config.GetObjectQualifer());
                    DotNetNuke.Data.DataProvider.Instance().ExecuteScript(sql);
                    DotNetNuke.Common.Utilities.DataCache.ClearModuleCache(new ModuleController().GetModule(ModuleId).TabID);
                }
                HttpRuntime.Cache.Remove(App.GetMasterCacheKey());
                HttpContext.Current.Response.Redirect(HttpContext.Current.Request.RawUrl);
            }
        }


        public void Load(int moduleId, XmlNode xmlConfig)
        {
            XmlNode xmlSettings = xmlConfig["Settings"];

            ModuleId = moduleId;

            FrontEndTemplate.Value = ReadValueStr(xmlSettings["FrontEndTemplate"], "");
            HasCustomLayout.Value = ReadValueStr(xmlSettings["Layout"], "") == "custom";
            LayoutHtml.Value = ReadValueStr(xmlSettings["LayoutHtml"], "");

            //FormTitle = ReadValueStr(xmlSettings["FormTitle"], "");
            //FormAlign.Value = ReadValueEnum<eFormAlign>(xmlSettings["OpenFormMode"], eFormAlign.Center);
            LabelAlign.Value = ReadValueEnum<eLabelAlign>(xmlSettings["LabelAlign"], eLabelAlign.Default);
            LabelWidth.Value = ReadValueInt(xmlSettings["LabelWidth"], -1);
            FieldSpacing.Value = ReadValueEnum<eFieldSpacing>(xmlSettings["FieldSpacing"], eFieldSpacing.Loose);
            OpenFormMode.Value = ReadValueEnum<FormOpenMode>(xmlSettings["OpenFormMode"], FormOpenMode.Always);
            FormTemplate.Value = ReadValueStr(xmlSettings["FormTemplate"], "bootstrap");
            if (string.IsNullOrEmpty(FormTemplate.Value))
                FormTemplate.Value = DefaultFormTemplate;

            //IsDebug = ReadValueBool(xmlSettings["IsDebug"], false);
            InitJs.Value = ReadValueStr(xmlSettings["InitJs"], "");
            PopupWidth.Value = ReadValueInt(xmlSettings["PopupWidth"], 400);
            PopupHeight.Value = ReadValueInt(xmlSettings["PopupHeight"], 0);
            jQueryTheme.Value = ReadValueStr(xmlSettings["jQueryTheme"], "sunny");

            // load fields
            FormField.DeleteAllByProperty("ModuleId", moduleId);
            int viewOrder = 0;
            foreach (XmlNode xmlField in xmlConfig["Fields"].ChildNodes) {
                FormField field = new FormField();
                field.LoadFromXml(xmlField);
                field.ModuleId = ModuleId;
                field.ViewOrder = viewOrder++;

                var dic = field.Parameters.GetValue<Dictionary<string, object>>("ShowIn", null);
                if (dic != null && dic.ContainsKey("ButtonsPane") && (bool)dic["ButtonsPane"])
                    field.ViewOrder = 9999;

                field.Save();
            }

            // load actions
            ActionInfo.DeleteAllByProperty("ModuleId", moduleId);
            XmlSerializer serializer = new XmlSerializer(typeof(ActionInfo));
            foreach (XmlElement actionXml in xmlConfig.SelectNodes("//Actions/ActionInfo")) {
                var stringReader = new StringReader(actionXml.OuterXml);
                ActionInfo action = (ActionInfo)serializer.Deserialize(stringReader);
                action.Id = -1;
                action.ModuleId = moduleId;
                if (!string.IsNullOrEmpty(action.FieldName)) {
                    var field = FieldsByName[action.FieldName];
                    if (field != null)
                        action.FieldId = field.FormFieldId;
                }
                action.Save();
            }

            //// serialize actions
            //writer.WriteStartElement("Actions");
            //XmlSerializer serializer = new XmlSerializer(typeof(ActionInfo));
            //foreach (ActionInfo action in ActionInfo.GetActions(ModuleId)) {
            //    serializer.Serialize(writer, action);
            //}
            //writer.WriteEndElement(); // ("Actions");
        }

        public override void Save()
        {
            IsInitialized.Value = true;

            // defauult the template
            if (!Regex.IsMatch(FrontEndTemplate.Value, "\\[FormUrl\\]"))
                FrontEndTemplate.Value += "<a href = '[FormUrl]'>GET IT NOW!</a>";

            base.Save();


            // save fields
            foreach (FormField field in Fields) {
                field.ModuleId = ModuleId;
                field.Save();
            }
        }


        #region client scripts

        public List<string> ClientScripts
        {
            get
            {
                string key = "AFORM-ClientScripts-" + ModuleId;
                if (HttpContext.Current.Items[key] == null) {
                    return new List<string>();
                }
                return HttpContext.Current.Items[key] as List<string>;
            }
        }

        public void RegisterClientScriptBlock(string script)
        {
            string key = "AFORM-ClientScripts-" + ModuleId;
            if (HttpContext.Current.Items[key] == null) {
                HttpContext.Current.Items[key] = new List<string>();
            }
            (HttpContext.Current.Items[key] as List<string>).Add(script);
        }

        #endregion


        public FormData Init()
        {
            var formData = new FormData(this);
            var actions = ActionInfo.GetAllByProperty("OrderIndex",
                        new KeyValuePair<string, object>("ModuleId", ModuleId),
                        new KeyValuePair<string, object>("EventName", "init"));
            ExecuteActions(actions, formData, eActionContext.Init);
            return formData;
        }

        public IFormEventResult Execute(FormData data, int reportId, FormField button, string eventType)
        {
            //IFormEventResult fallbackAction = null;
            string moduleDir = HttpRuntime.AppDomainAppVirtualPath.TrimEnd('/') + "/DesktopModules/AvatarSoft/ActionForm";

            //if (data == null)
            //    data = new FormData(this);
            // save draft in database, first of all, or get it if already exists
            if (reportId == -1 && data.Data.ContainsKey("ReportEntryId")) {
                int.TryParse(data.Data["ReportEntryId"].Value, out reportId);
            }

            if (reportId != -1)
                data.Report = ReportEntry.Get(reportId);

            // if still null, start a new submission
            if (data.Report == null)
                data.Report = new SaveReport().Execute(this, data, null, eSubmitStatus.Draft);

            // also validate data 
            if ((!button.Parameters.ContainsKey("CausesValidation") || button.Parameters["CausesValidation"].ToBool(true)) && !data.IsValid) {
                Debug("Submitted data is invalid");

                var vldResult = ExecuteValidation(data);

                // if result is Skip, it actually tells to submit anyway
                if (!(vldResult is Skip))
                    return vldResult;
            }

            // Execute list of actions for submit
            var actions = ActionInfo.GetAllByProperty("OrderIndex",
                        new KeyValuePair<string, object>("ModuleId", ModuleId),
                        new KeyValuePair<string, object>("FieldId", button.FormFieldId),
                        new KeyValuePair<string, object>("EventName", eventType));

            var result = ExecuteActions(actions, data, eActionContext.Submit);
            if (result != null)
                return result;

            // TODO: if no result action, let's just reload page
            return new RedirectToUrl() {
                Url = DotNetNuke.Common.Globals.NavigateURL(new ModuleController().GetModule(ModuleId).TabID)
            };

        }

        public IFormEventResult ExecuteValidation(FormData data)
        {
            // check if we have a validator that has custom actions
            foreach (var err in data.ValidationErrors) {
                if (string.IsNullOrEmpty(err.EventName))
                    continue;

                var actions = ActionInfo.GetAllByProperty("OrderIndex",
                        new KeyValuePair<string, object>("ModuleId", ModuleId),
                        new KeyValuePair<string, object>("EventName", err.EventName));

                var resultvld = ExecuteActions(actions, data, eActionContext.Validation);
                if (resultvld != null)
                    return resultvld;
            }

            // execute list of actions for validation-failed event (any validation error)
            var actionsVldFailed = ActionInfo.GetAllByProperty("OrderIndex",
                        new KeyValuePair<string, object>("ModuleId", ModuleId),
                        new KeyValuePair<string, object>("EventName", "validation-failed"));

            var resultVldAny = ExecuteActions(actionsVldFailed, data, eActionContext.Validation);
            if (resultVldAny != null)
                return resultVldAny;

            // no validation results, just return a error message with all errors concatenated
            return new ErrorMessage() {
                Message = string.Join("<br/>", data.ValidationErrors.Select(x => x.Message).ToArray())
            };
        }

        public IFormEventResult ExecuteActions(IList<ActionInfo> actions, FormData data, eActionContext context)
        {
            IFormEventResult result = null;

            // get index of first action to execute, since this could be a multistep submit
            var iFirstAction = GetFirstActionToExecute(actions, data);

            foreach (var act in actions.Skip(iFirstAction + 1)) {

                result = act.Execute(this, data, context) ?? result;

                // if there is a result, then save current action and end execution for now
                // could be that the submission is complete or user action is required (such as confirmation, payment, etc), 
                if (result != null) {

                    // save form state in database and return control to the browser
                    data["CurrentAction"] = act.Id.ToString();

                    // update report
                    // never save report for init actions - since this basically executes for each page view
                    if (context != eActionContext.Init)
                        new SaveReport().Execute(this, data, result, result.Status);
                    return result;
                }
            }

            // never save report for init actions - since this basically executes for each page view
            if (context != eActionContext.Init)
                new SaveReport().Execute(this, data, null, eSubmitStatus.Submitted);

            return null;
        }

        int GetFirstActionToExecute(IList<ActionInfo> actions, FormData data)
        {
            if (string.IsNullOrEmpty(data["CurrentAction"]))
                return -1;

            int firstActionId = -1;
            if (!int.TryParse(data["CurrentAction"], out firstActionId))
                return -1;

            // find it in list
            for (var i = 0; i < actions.Count; i++) {
                if (actions[i].Id == firstActionId)
                    return i;
            }

            return -1;
        }



        public string GenerateCsvReport(DateTime startDate, DateTime endDate)
        {
            List<ReportEntry> entries = CBO.FillCollection<ReportEntry>(
                    DataProvider.Instance().GetReport(
                        ModuleId,
                        startDate <= DateTime.MinValue || startDate >= DateTime.MaxValue ? System.Data.SqlTypes.SqlDateTime.MinValue : startDate,
                        endDate <= DateTime.MinValue || endDate >= DateTime.MaxValue ? System.Data.SqlTypes.SqlDateTime.MaxValue : endDate
                    )
            );

            CsvWriter csvw = new CsvWriter();

            // write header
            csvw.Write("EntryId").Write("ModuleId").Write("FormTitle").Write("ReferrerInfo").Write("UserId")
                .Write("RemoteAddress").Write("BrowserInfo").Write("DateSubmitted");

            foreach (FormField field in Fields) {
                if (field.IsActive)
                    csvw.Write(field.TitleTokenized);
            }
            csvw.EndRow();

            foreach (ReportEntry re in entries) {

                if (re.SubmitStatus != eSubmitStatus.Submitted)
                    continue;

                csvw.Write(re.ReportEntryId).Write(re.ModuleId).Write(Module.ModuleTitle).Write(re.ReferrerInfo).Write(re.UserId)
                    .Write(re.RemoteAddress).Write(re.BrowserInfo).Write(re.DateSubmitted);

                foreach (FormField field in Fields) {
                    if (field.IsActive) {
                        try {
                            csvw.Write(re.XmlFormData.FirstChild[field.TitleCompacted].InnerText);
                        } catch {
                            csvw.Write("");
                        }
                    }
                }

                csvw.EndRow();
            }

            return csvw.ToString();
        }


        #region Serialization

        public string SerializeToXmlStr(FormData InitData, bool evaluateShowCondition)
        {
            StringBuilder strXML = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            XmlWriter Writer = XmlWriter.Create(new StringWriterWithEncoding(strXML, Encoding.UTF8), settings);

            Writer.WriteStartElement("Form");
            Serialize(Writer, InitData, evaluateShowCondition);
            Writer.WriteEndElement(); // ("Form");
            Writer.Close();

            return strXML.ToString();
        }

        public void Serialize(XmlWriter writer, FormData InitData, bool evaluateShowCondition)
        {
            var portal = PortalController.GetCurrentPortalSettings();

            writer.WriteStartElement("Settings");
            writer.WriteElementWithCData("Version", ActionFormController.Version);
            writer.WriteElementWithCData("Build", ActionFormController.Build);
            writer.WriteElementWithCData("RenderContext", RenderContext.ToString());
            writer.WriteElementWithCData("FrontEndTemplate", FrontEndTemplate.Value);
            writer.WriteElementWithCData("HasCustomLayout", HasCustomLayout.Value.ToString());
            writer.WriteElementWithCData("LayoutHtml", LayoutHtml.Value);

            // serialize open mode
            //writer.WriteElementWithCData("FormTitle", FormTitle);
            //writer.WriteElementString("FormAlign", FormAlign.ToString());
            writer.WriteElementString("LabelAlign", LabelAlign.ToString().ToLower());
            writer.WriteElementString("LabelWidth", LabelWidth.ToString());
            writer.WriteElementString("FieldSpacing", FieldSpacing.ToString().ToLower());
            writer.WriteElementWithCData("OpenFormMode", OpenFormMode.ToString());
            writer.WriteElementString("FormTemplate", FormTemplate.Value);
            writer.WriteElementString("FormTemplateFolder", string.Format("{0}/templates/Form/{1}", App.ModuleRelativePath, FormTemplate));
            writer.WriteElementString("UploadUrl", string.Format("{0}/UploadFile.ashx", App.ModuleRelativePath));

            writer.WriteElementString("ModuleId", ModuleId.ToString());
            writer.WriteElementString("IsDebug", IsDebug.ToString());
            writer.WriteElementString("ClientSideValidation", ClientSideValidation.ToString());

            writer.WriteElementWithCData("InitJs", InitJs.Value);
            writer.WriteElementString("PopupWidth", PopupWidth.ToString());
            writer.WriteElementString("PopupHeight", PopupHeight.ToString());
            writer.WriteElementString("jQueryTheme", jQueryTheme.Value);

            writer.WriteElementString("DateFormat", CultureInfo.CurrentUICulture.DateTimeFormat.ShortDatePattern.Replace("M", "m").Replace("yyyy", "yy"));
            writer.WriteElementString("BaseId", "dnn" + ModuleId);

            writer.WriteElementString("AjaxSubmitUrl", GetAjaxUrl());
            writer.WriteElementString("PageUrl", DotNetNuke.Common.Globals.NavigateURL(new ModuleController().GetModule(ModuleId).TabID));
            writer.WriteElementString("LocalizedStringsUrl", string.Format("{0}/DesktopModules/AvatarSoft/ActionForm/AdminApi.ashx?method=GetLocalization&locale=" + CultureInfo.CurrentUICulture.Name, HttpRuntime.AppDomainAppVirtualPath.TrimEnd('/')));
            if (portal != null)
                writer.WriteElementString("PortalHomeUrl", portal.HomeDirectory);

            writer.WriteEndElement(); // ("Settings");

            // serialize fields
            writer.WriteStartElement("Fields");
            foreach (FormField field in Fields.OrderBy(x => x.RowIndex).ThenBy(x => x.ColIndex)) {
                if (evaluateShowCondition && !field.IsVisibile)
                    continue;
                field.Serialize(writer, InitData);
            }
            writer.WriteEndElement(); // ("Fields");

            // serialize actions
            //writer.WriteStartElement("Actions");
            XmlSerializer serializer = new XmlSerializer(typeof(List<ActionInfo>), new XmlRootAttribute("Actions"));
            try {
                var actions = ActionInfo.GetAllByProperty("OrderIndex", new KeyValuePair<string, object>("ModuleId", ModuleId));
                // migrate FieldId to FieldName
                foreach (var a in actions) {
                    if (a.FieldId == null)
                        continue;
                    var field = Fields.SingleOrDefault(x => x.FormFieldId == a.FieldId.Value);
                    if (field != null)
                        a.FieldName = field.TitleCompacted;
                }
                serializer.Serialize(writer, actions);
            } catch (Exception ex) {
                throw ex;
            }
            //writer.WriteEndElement(); // ("Actions");
        }

        #endregion


        #region Helpers to parse settings

        string ReadValueStr(XmlNode xmlNode, string defValue)
        {
            if (xmlNode == null) {
                return defValue;
            }

            return HttpUtility.HtmlDecode(xmlNode.InnerText);
        }

        string ReadValueStr(Hashtable modSettings, string key, string defValue)
        {
            if (modSettings.ContainsKey(key)) { // && !string.IsNullOrEmpty(modSettings[key].ToString())) {
                return modSettings[key].ConvertToString();
            }

            return defValue;
        }

        bool ReadValueBool(Hashtable modSettings, string key, bool defValue)
        {
            if (modSettings.ContainsKey(key)) {
                try {
                    return Convert.ToBoolean(modSettings[key].ToString());
                } catch { return defValue; }
            }

            return defValue;
        }

        bool ReadValueBool(XmlNode xmlNode, bool defValue)
        {
            if (xmlNode != null) {
                try {
                    return Convert.ToBoolean(xmlNode.InnerText);
                } catch { return defValue; }
            }

            return defValue;
        }

        int ReadValueInt(Hashtable modSettings, string key, int defValue)
        {
            if (modSettings.ContainsKey(key)) {
                try {
                    return Convert.ToInt32(modSettings[key].ToString());
                } catch { return defValue; }
            }

            return defValue;
        }

        int ReadValueInt(XmlNode xmlNode, int defValue)
        {
            if (xmlNode != null) {
                try {
                    return Convert.ToInt32(xmlNode.InnerText);
                } catch { return defValue; }
            }

            return defValue;
        }

        T ReadValueEnum<T>(Hashtable modSettings, string key, T defValue)
        {
            if (modSettings.ContainsKey(key)) {
                try {
                    return (T)Enum.Parse(typeof(T), modSettings[key].ToString(), true);
                } catch { return defValue; }
            }

            return defValue;
        }

        T ReadValueEnum<T>(XmlNode xmlNode, T defValue)
        {
            if (xmlNode != null) {
                try {
                    return (T)Enum.Parse(typeof(T), xmlNode.InnerText, true);
                } catch { return defValue; }
            }

            return defValue;
        }

        public static string ReadValueFromXml(string xmlStr, string xpath, string defaultValue)
        {
            XmlDocument xmlDoc = new XmlDocument();
            try {
                xmlDoc.LoadXml(xmlStr);
            } catch { return defaultValue; }

            XmlNode xmlNode = xmlDoc.SelectSingleNode(xpath);
            if (xmlNode == null)
                return defaultValue;

            return xmlNode.InnerText;
        }

        #endregion
    }
}