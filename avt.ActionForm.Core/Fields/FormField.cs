using avt.ActionForm.Core.Actions;
using avt.ActionForm.Core.Config;
using avt.ActionForm.Core.Form;
using avt.ActionForm.Core.Input;
using avt.ActionForm.Core.Utils;
using avt.ActionForm.Core.Validation;
using avt.ActionForm.Data;
using avt.ActionForm.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Xml;
using System.Linq;
using System.Xml.Serialization;
using DnnSharp.Common.ActiveTable;

namespace avt.ActionForm.Core.Fields
{
    [Table("avtActionForm_FormFields")]
    public class FormField :  ActiveTableBase<FormField>, IConfigItem
    {
        public FormField()
        {
            DateCreated = DateTime.Now;
            IsActive = true;

            ColOffset = 0;
            ColSpan = 7;

            Parameters = new SettingsDictionary();
        }


        #region Database Fields Mappings

        [PrimaryKey]
        public int FormFieldId { get; set; }

        [Field]
        public int ModuleId { get; set; }

        [Field]
        public string Title { get; set; }

        [Field]
        public string Name { get; set; }

        [Field]
        public string ShortDesc { get; set; }

        [Field]
        public string HelpText { get; set; }

        [Field]
        public string ShowCondition { get; set; }

        public bool IsVisibile { get { return string.IsNullOrEmpty(ShowCondition) || TokenUtils.TokenizeAsBool(ShowCondition); } }

        [Field]
        public bool DisableAutocomplete { get; set; }

        public string TitleCompacted
        {
            get { return Regex.Replace(TokenUtils.Tokenize(string.IsNullOrEmpty(Name) ? Title : Name), "[^A-Za-z0-9]", ""); }
        }
        public string TitleTokenized { get { return TokenUtils.Tokenize(Title); } }

        public bool AddOnInit { get; set; }

        [Field]
        public string InputTypeStr { get; set; }

        public SettingsDictionary Parameters { get; set; }

        [HasMany("FieldId", OrderBy="OrderIndex")]
        public IList<ActionInfo> Actions { get; set; }

        [ScriptIgnore]
        [XmlIgnore]
        [Field]
        public string InputData
        {
            get
            {
                return new JavaScriptSerializer().Serialize(Parameters);
            }
            set
            {
                var data = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(value);
                if (data != null) {
                    Parameters = new SettingsDictionary(data);
                } else {
                    Parameters = new SettingsDictionary();
                }
            }
        }


        IInputCtrl _InputCtrl;
        public IInputCtrl InputCtrl
        {
            get
            {
                if (_InputCtrl == null) {
                    Dictionary<string, InputTypeDef> typeDefs = ActionFormController.InputTypes;
                    if (typeDefs.ContainsKey(InputTypeStr)) {
                        _InputCtrl = typeDefs[InputTypeStr].Handler;
                    } else {
                        _InputCtrl = typeDefs["open-text"].Handler;
                    }
                }
                return _InputCtrl;
            }
        }

        [Field]
        public int ViewOrder { get; set; }

        [Field]
        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        [Field]
        public bool IsRequired { get; set; }

        [Field]
        public string CssClass { get; set; }

        [Field]
        public string CssStyles { get; set; }

        [Field]
        public string LabelCssClass { get; set; }

        [Field]
        public string LabelCssStyles { get; set; }

        //[Field]
        //public string DefaultValue { get; set; }

        [Field]
        public int ColIndex { get; set; }

        [Field]
        public int RowIndex { get; set; }
        
        [Field]
        public int ColSpan { get; set; }

        [Field]
        public int ColOffset { get; set; }

        [ScriptIgnore]
        [Field]
        public DateTime DateCreated { get; set; }

        [Field]
        public bool IsEnabled { get; set; }

        [Field]
        public string CustomValidator1 { get; set; }

        [Field]
        public string CustomValidator2 { get; set; }

        [Field]
        public string ValidationGroup { get; set; }

        [Field]
        public string GroupValidator { get; set; }

        [Obsolete("Not used")]
        public bool SaveToUserProfile { get; set; }

        #endregion

        public override void Save()
        {
            if (InputTypeStr == "open-email") {
                CustomValidator1 = "Email Address";
            } else if (InputTypeStr == "open-number") {
                CustomValidator1 = "Integer Number";
            }

            base.Save();

            // also save all actions
            int order = 0;
            if (Actions != null) {
                foreach (var action in Actions) {
                    if (action.IsDeleted) {
                        action.Delete();
                    } else {
                        action.FieldId = FormFieldId;
                        action.ModuleId = ModuleId;
                        action.OrderIndex = order++;
                        action.Save();
                    }
                }
                Actions = Actions.Where(x => !x.IsDeleted).OrderBy(x => x.OrderIndex).ToList();
            }

        }

        //public void Save1()
        //{
        //    if (InputTypeStr == "open-email") {
        //        CustomValidator1 = "Email Address";
        //    } else if (InputTypeStr == "open-number") {
        //        CustomValidator1 = "Integer Number";
        //    }

        //    FormFieldId = DataProvider.Instance().UpdateFormField(
        //        FormFieldId, ModuleId, Title, Name, ShortDesc, HelpText,
        //        InputTypeStr, InputData,
        //        ViewOrder, IsRequired, IsActive,
        //        CssClass, CssStyles,
        //        DefaultValue, ColIndex, DateCreated,
        //        LabelCssClass, LabelCssStyles, IsEnabled,
        //        CustomValidator1, CustomValidator2, ValidationGroup, GroupValidator, DisableAutocomplete, ColOffset, ColSpan, RowIndex//,SaveToUserProfile
        //    );
        //}

        //public void Delete()
        //{
        //    if (FormFieldId <= 0)
        //        return;
        //    DataProvider.Instance().DeleteField(FormFieldId);
        //}

        public void LoadFromXml(XmlNode xmlField)
        {
            //FormField field = new FormField();
            //try { FormFieldId = Convert.ToInt32(xmlField["Id"].InnerText); } catch { FormFieldId = -1; }
            Title = xmlField["Title"].InnerText;
            if (xmlField["OverrideName"] != null) {
                Name = xmlField["OverrideName"].InnerText;
            } else if (xmlField["Name"] != null) {
                Name = xmlField["Name"].InnerText;
            }

            try { AddOnInit = bool.Parse(xmlField["AddOnInit"].InnerText); } catch { }
            try { IsActive = bool.Parse(xmlField["IsActive"].InnerText); } catch { IsActive = true; }
            try { IsEnabled = bool.Parse(xmlField["IsEnabled"].InnerText); } catch { IsEnabled = true; }
            try { ShortDesc = xmlField["ShortDesc"].InnerText; } catch { }
            try { HelpText = xmlField["HelpText"].InnerText; } catch { }
            try { ShowCondition = xmlField["ShowCondition"].InnerText; } catch { }
            try { InputTypeStr = xmlField["InputType"].InnerText; } catch { }
            try { InputData = xmlField["InputData"].InnerText; } catch { }
            try { CssClass = xmlField["CssClass"].InnerText; } catch { }
            try { CssStyles = xmlField["CssStyles"].InnerText; } catch { }
            //try { DefaultValue = xmlField["DefaultValue"].InnerText; } catch { }
            try { LabelCssClass = xmlField["LabelCssClass"].InnerText; } catch { }
            try { LabelCssStyles = xmlField["LabelCssStyles"].InnerText; } catch { }
            try { CustomValidator1 = xmlField["CustomValidator1"].InnerText; } catch { }
            try { CustomValidator2 = xmlField["CustomValidator2"].InnerText; } catch { }
            try { SaveToUserProfile = bool.Parse(xmlField["SaveToUserProfile"].InnerText); } catch { SaveToUserProfile = false; }
            try { DisableAutocomplete = bool.Parse(xmlField["DisableAutocomplete"].InnerText); } catch { DisableAutocomplete = false; }

            try { IsRequired = xmlField["IsRequired"].InnerText.ToLower() == "true"; } catch { }
            try { ValidationGroup = xmlField["ValidationGroup"].InnerText; } catch { }
            try { GroupValidator = xmlField["GroupValidator"].InnerText; } catch { }

            try { ViewOrder = int.Parse(xmlField["ViewOrder"].InnerText); } catch { }
            try { RowIndex = int.Parse(xmlField["RowIndex"].InnerText); } catch { }
            try { ColSpan = int.Parse(xmlField["ColSpan"].InnerText); } catch { }
            try { ColOffset = int.Parse(xmlField["ColOffset"].InnerText); } catch { }

            // return field;
        }

        public string GetKey()
        {
            return Title + FormFieldId.ToString();
        }

        public override int GetHashCode()
        {
            if (FormFieldId > 0)
                return FormFieldId;

            try {
                return Title.GetHashCode();
            } catch {
                return -1;
            }
        }

        public override bool Equals(object obj)
        {
            try {
                return (obj as FormField).FormFieldId == FormFieldId;
            } catch {
                return false;
            }
        }


        public void Serialize(XmlWriter writer, FormData InitData)
        {
            writer.WriteStartElement("Field");
            writer.WriteElementWithCData("Id", FormFieldId.ToString());
            writer.WriteElementWithCData("Title", Title);
            writer.WriteElementWithCData("OverrideName", Name);
            writer.WriteElementWithCData("Name", TitleCompacted);
            writer.WriteElementWithCData("ShortDesc", ShortDesc);
            writer.WriteElementWithCData("HelpText", HelpText);
            writer.WriteElementWithCData("ShowCondition", ShowCondition ?? "");
            writer.WriteElementString("InputType", InputTypeStr);
            writer.WriteElementString("InputData", InputData);

            foreach (var p in Parameters) {
                if (p.Value == null || p.Key == "Value") // we'll handle the value later
                    continue;

                if (p.Value.GetType() == typeof(Dictionary<string, object>)) {
                    var dic = (Dictionary<string, object>)p.Value;
                    writer.WriteStartElement(p.Key);
                    foreach (var subKey in dic.Keys)
                        writer.WriteElementString(subKey, dic[subKey].ToString());
                    writer.WriteEndElement(); // ("p.Key");
                } else {
                    writer.WriteElementString(p.Key, TokenUtils.Tokenize(p.Value.ToString()));
                }

            }

            //foreach (var option in InputData.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries))
            //    writer.WriteElementString("Option", option);
            Dictionary<string, InputTypeDef> typeDefs = ActionFormController.InputTypes;
            if (typeDefs.ContainsKey(InputTypeStr)) {
                //if (!string.IsNullOrEmpty(typeDefs[InputTypeStr].HandlerData)) {
                //    foreach (var option in typeDefs[InputTypeStr].HandlerData.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                //        writer.WriteElementString("Option", option);
                //}
                var options = typeDefs[InputTypeStr].Handler.GetOptions(typeDefs[InputTypeStr], this);
                if (options != null) {
                    foreach (var option in options) {
                        writer.WriteStartElement("Option");
                        writer.WriteAttributeString("value", option.Value);
                        writer.WriteValue(option.Text);
                        writer.WriteEndElement(); // ("Option");
                    }
                }

                foreach (var flag in typeDefs[InputTypeStr].Flags)
                    writer.WriteElementString("Flag", flag);

                if (InitData != null && InitData[TitleCompacted] != null) {
                    writer.WriteElementWithCData("Value", InitData[TitleCompacted]);
                } else {
                    // var value = DefaultValue; // typeDefs[InputTypeStr].Handler.GetValue(typeDefs[InputTypeStr], this);
                    if (Parameters.ContainsKey("Value")) {
                        writer.WriteElementWithCData("Value", TokenUtils.Tokenize(Parameters["Value"].ToString()));
                    }
                }

                var data = typeDefs[InputTypeStr].Handler.GetData(typeDefs[InputTypeStr], this);
                if (data != null) {
                    foreach (string key in data.Keys) {
                        writer.WriteStartElement("Data");
                        writer.WriteElementString(key, data[key].ToString());
                        writer.WriteEndElement(); // ("Data");
                    }
                }
            }

            writer.WriteElementString("IsRequired", IsRequired.ToString());
            
            writer.WriteElementString("IsActive", IsActive.ToString());
            writer.WriteElementString("IsEnabled", IsEnabled.ToString());
            writer.WriteElementWithCData("CssClass", CssClass);
            writer.WriteElementWithCData("CssStyles", CssStyles);
            //writer.WriteElementWithCData("DefaultValue", DefaultValue);
            writer.WriteElementWithCData("LabelCssClass", LabelCssClass);
            writer.WriteElementWithCData("LabelCssStyles", LabelCssStyles);
            
            string configFolder = ActionFormController.AppPath.Trim('\\').Trim('/') + "\\Config\\Validators";
            var validationDefitions = ItemsFromXmlConfig<ValidatorDef>.GetConfig(configFolder);

            writer.WriteElementString("CustomValidator1", CustomValidator1);
            if (validationDefitions.ItemsHash.ContainsKey(CustomValidator1))
                writer.WriteElementString("CustomValidator1JsName", validationDefitions.ItemsHash[CustomValidator1].JsValidatorName + ModuleId);

            writer.WriteElementString("CustomValidator2", CustomValidator2);
            if (validationDefitions.ItemsHash.ContainsKey(CustomValidator2))
                writer.WriteElementString("CustomValidator2JsName", validationDefitions.ItemsHash[CustomValidator2].JsValidatorName + ModuleId);

            // group validators
            string gconfigFolder = ActionFormController.AppPath.Trim('\\').Trim('/') + "\\Config\\GroupValidators";
            var gvalidationDefitions = ItemsFromXmlConfig<GroupValidatorDef>.GetConfig(gconfigFolder);

            writer.WriteElementString("ValidationGroup", ValidationGroup);
            writer.WriteElementString("GroupValidator", GroupValidator);
            if (gvalidationDefitions.ItemsHash.ContainsKey(GroupValidator))
                writer.WriteElementString("GroupValidatorJsName", gvalidationDefitions.ItemsHash[GroupValidator].JsValidatorName + ModuleId);

            writer.WriteElementString("SaveToUserProfile", SaveToUserProfile.ToString());
            writer.WriteElementString("DisableAutocomplete", DisableAutocomplete.ToString());

            writer.WriteElementString("ViewOrder", ViewOrder.ToString());
            writer.WriteElementString("RowIndex", RowIndex.ToString());
            writer.WriteElementString("ColSpan", ColSpan.ToString());
            writer.WriteElementString("ColOffset", ColOffset.ToString());

            writer.WriteEndElement(); // ("Field");
        }
    }

}
