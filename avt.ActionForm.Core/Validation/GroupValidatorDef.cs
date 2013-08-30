using System;
using System.Collections.Generic;
using System.Text;
using avt.ActionForm.Core.Config;
using System.Xml;
using System.Text.RegularExpressions;
using DotNetNuke.Entities.Modules;

namespace avt.ActionForm.Core.Validation
{
    public class GroupValidatorDef : IConfigItem
    {
        public GroupValidatorDef()
        {
            ValidatorParams = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public string Title { get; set; }
        public string JsValidatorName { get; set; }
        public string JsValidationCode { get; set; }
        public IGroupValidator Validator { get; set; }
        public bool IsServerSideValidation { get { return Validator.IsServerSideValidation; } }
        public Dictionary<string, string> ValidatorParams { get; set; }
        public string ErrorMessageDefault { get; set; }

        public void LoadFromXml(XmlNode xmlConfig)
        {
            Title = xmlConfig["Title"].InnerText;
            if (xmlConfig["JsValidator"] != null) {
                JsValidatorName = xmlConfig["JsValidator"].InnerText;
            } else {
                JsValidatorName = Regex.Replace(Title, "[^A-Za-z]", "");
            }

            if (xmlConfig["JsValidationCode"] != null)
                JsValidationCode = xmlConfig["JsValidationCode"].InnerText;

            try {
                ErrorMessageDefault = xmlConfig["ErrorMessage"]["default"].InnerText;
            } catch { ErrorMessageDefault = ""; }

            Validator = ActionFormController.CreateInstance<IGroupValidator>(xmlConfig["Type"].InnerText);
            
            if (xmlConfig["Params"] != null) {
                foreach (XmlNode xmlParam in xmlConfig["Params"].ChildNodes){
                    ValidatorParams[xmlParam.Name] = xmlParam.InnerText.Trim();
                }
            }
           
        }

        public string Validate(IDictionary <string, string> input)
        {
            var err = ErrorMessageDefault;
            var validation = Validator.Validate(input, this);

            if (!validation.IsValid)
                return validation.ErrMessage;

            return null;
        }


        public string GetKey()
        {
            return Title;
        }
    }
}
