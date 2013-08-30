using avt.ActionForm.Core.Config;
using avt.ActionForm.Core.Fields;
using avt.ActionForm.Core.Input;
using avt.ActionForm.Core.Validation;
using avt.ActionForm.Data;
using avt.ActionForm.Utils;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Globalization;
using DotNetNuke.Entities.Users;
using avt.ActionForm.Core.Utils.Data;
using DotNetNuke.Entities.Modules;

namespace avt.ActionForm.Core.Form
{
    public class ValidationError
    {
        public string Message { get; set; }
        public FormField Field { get; set; }
        public string EventName { get; set; }
    }


    public class FieldData
    {

        public string Value { get; private set; }
        public string Name { get; private set; }
        public FormField Field { get; private set; }

        public List<ValidationError> ValidationErrors { get; private set; }

        public FieldData(string name, string value)
        {
            ValidationErrors = new List<ValidationError>();
            Name = name;
            Value = value;
        }

        public FieldData(FormField field, string value)
        {
            ValidationErrors = new List<ValidationError>();
            Field = field;
            Value = value;

        }

        public bool IsValid(FormData formData)
        {
            if (Field == null)
                return true; // no rules to validate against

            // if value is empty and the field is not required do not evaluate the rest of the validators
            if (string.IsNullOrEmpty(Value)) {
                if (Field.IsRequired)
                    ValidationErrors.Add(
                        new ValidationError() {
                            Message = string.Format("{0} is required.", Field.Title),
                            Field = Field
                        });
                return CheckCtlValidation(formData) && !Field.IsRequired;
            }

            string configFolder = ActionFormController.AppPath.Trim('\\').Trim('/') + "\\Config\\Validators";
            ItemsFromXmlConfig<ValidatorDef> validationDefitions = ItemsFromXmlConfig<ValidatorDef>.GetConfig(configFolder);

            bool isValid = true;
            if (validationDefitions.ItemsHash.ContainsKey(Field.CustomValidator1))
                isValid = CheckValidator(validationDefitions.ItemsHash[Field.CustomValidator1]);

            if (validationDefitions.ItemsHash.ContainsKey(Field.CustomValidator2))
                isValid = CheckValidator(validationDefitions.ItemsHash[Field.CustomValidator2]) && isValid;

            // also call control level validation
            isValid = CheckCtlValidation(formData) && isValid;
            return isValid;
        }

        bool CheckCtlValidation(FormData formData)
        {
            string[] errMessages;
            var ctlIsValid = Field.InputCtrl.IsValid(formData, this, out errMessages);
            if (!ctlIsValid) {
                if (errMessages != null) {
                    foreach (var err in errMessages) {
                        ValidationErrors.Add(new ValidationError() {
                            Message = err,
                            Field = Field
                        });
                    }
                }
            }
            return ctlIsValid;
        }

        bool CheckValidator(ValidatorDef validator)
        {
            var val = Value;
            var err = validator.Validate(ref val);
            Value = val;
            if (err != null)
                ValidationErrors.Add(
                    new ValidationError() {
                        Message = err,
                        Field = Field,
                        EventName = string.Format("{0}-{1}", Field.FormFieldId, validator.Title)
                    });

            return err == null;
        }
    }

    public class FormData
    {
        public ActionFormSettings Settings { get; private set; }
        public Dictionary<string, FieldData> Data { get; private set; }
        public List<ValidationError> ValidationErrors { get; private set; }
        public Dictionary<string, List<ValidationError>> ErrorsByField { get; private set; }

        // the submit will be executed in the context of this user
        // this could be the current user, or the user resulted after executing a login or registration action
        public UserInfo User { get; set; }
        public ReportEntry Report { get; set; }

        public FormData(ActionFormSettings settings)
        {
            Settings = settings;
            Data = new Dictionary<string, FieldData>(StringComparer.OrdinalIgnoreCase);
            ValidationErrors = new List<ValidationError>();
            ErrorsByField = new Dictionary<string, List<ValidationError>>();
            User = UserController.GetCurrentUserInfo();
        }

        public FormData(ActionFormSettings settings, HttpContext context)
        {
            Settings = settings;
            Data = new Dictionary<string, FieldData>(StringComparer.OrdinalIgnoreCase);
            ValidationErrors = new List<ValidationError>();
            ErrorsByField = new Dictionary<string, List<ValidationError>>();
            User = UserController.GetCurrentUserInfo();

            foreach (string key in context.Request.Form.Keys) {
                if (!Settings.FieldsByName.ContainsKey(key))
                    continue;

                Data[key] = new FieldData(Settings.FieldsByName[key], context.Request.Form[key]);

                // call expand tokens
                Data[key].Field.InputCtrl.ExpandTokens(this, Data[key]);
            }

            Validate();
        }

        public void CopyFrom(FormData data)
        {
            // we only need to copy data
            Data = new Dictionary<string, FieldData>(data.Data);
        }

        public void Validate()
        {
            // validate data on the server too
            //Dictionary<string, bool> validGroups = new Dictionary<string, bool>();
            foreach (var de in Settings.FieldsByName.Where(x => x.Value.IsVisibile)) {

                var fieldName = de.Key;
                var field = Settings.FieldsByName[fieldName];

                //// intialize group validators to true (which means that gruop is valid)
                //if (!string.IsNullOrEmpty(field.ValidationGroup) && !validGroups.ContainsKey(field.ValidationGroup))
                //    validGroups[field.ValidationGroup] = true;

                // first check if is required
                if (!RequiredFieldChecksOut(field)) {
                    ErrorsByField[fieldName] = new List<ValidationError>();
                    ErrorsByField[fieldName].Add(new ValidationError() {
                        Message = string.Format("Missing required field {0}.", field.Title),
                        Field = field
                    });
                } else if (Data.ContainsKey(fieldName) && !Data[fieldName].IsValid(this)) {
                    // now the field validators
                    ErrorsByField[fieldName] = new List<ValidationError>();
                    ErrorsByField[fieldName].AddRange(Data[fieldName].ValidationErrors);
                }
            }

            // validate groups
            string groupValidationFolder = ActionFormController.AppPath.Trim('\\').Trim('/') + "\\Config\\GroupValidators";
            var groupValidationDefs = ItemsFromXmlConfig<GroupValidatorDef>.GetConfig(groupValidationFolder);
            foreach (var group in Settings.Fields.Where(x => x.IsVisibile && !string.IsNullOrEmpty(x.ValidationGroup)).Select(x => x.ValidationGroup).Distinct()) {
                var groupValidatorName = Settings.Fields.First(x => x.IsVisibile && x.ValidationGroup == group && !string.IsNullOrEmpty(x.ValidationGroup)).GroupValidator;

                if (!groupValidationDefs.ItemsHash.ContainsKey(groupValidatorName))
                    continue; // ?? could this happen?

                var groupValidator = groupValidationDefs.ItemsHash[groupValidatorName];
                var fieldsInGroup = Settings.Fields.Where(x => x.IsVisibile && x.ValidationGroup == group && x.GroupValidator == groupValidatorName && Data.ContainsKey(x.TitleCompacted))
                    .ToDictionary(x => x.TitleCompacted, x => Data[x.TitleCompacted].Value);

                var err = groupValidator.Validate(fieldsInGroup);
                if (!string.IsNullOrEmpty(err)) {
                    ErrorsByField[groupValidatorName] = new List<ValidationError>();
                    ErrorsByField[groupValidatorName].Add(new ValidationError() {
                        Message = err.ToString(),
                        EventName = string.Format("{0}-{1}", group, groupValidatorName)
                    });
                }
            }

            // merge all errors
            foreach (var err in ErrorsByField.Values)
                ValidationErrors.AddRange(err);

            // and store in a token
            Add("ValidationErrors", string.Join("<br/>", ValidationErrors.Select(x => x.Message).ToArray()));
        }

        private bool RequiredFieldChecksOut(FormField field)
        {
            if (!field.IsRequired)
                return true;

            // check that the field was actually submitted
            if (!Data.ContainsKey(field.TitleCompacted) || string.IsNullOrEmpty(Data[field.TitleCompacted].Value))
                return false;

            // also look at the checkbxes - they should be true
            if (field.InputTypeStr == "closed-truefalse" && !Data[field.TitleCompacted].Value.Equals("true", StringComparison.OrdinalIgnoreCase))
                return false;

            //return field.IsRequired && (!Data.ContainsKey(fieldName) || string.IsNullOrEmpty(Data[fieldName].Value) || (field.))
            return true;
        }

        public bool IsValid
        {
            get { return ValidationErrors.Count == 0; }
        }

        public void Add(FormField field, string value)
        {
            Data[field.TitleCompacted] = new FieldData(field, value);

            // call expand tokens
            Data[field.TitleCompacted].Field.InputCtrl.ExpandTokens(this, Data[field.TitleCompacted]);
        }

        public void Add(string name, string value)
        {
            Data[name] = new FieldData(name, value);
        }

        public string GetValueByFieldType(string fieldType)
        {
            foreach (string key in Data.Keys) {

                if (Data[key].Field == null)
                    continue;

                if (Data[key].Field.InputTypeStr.Equals(fieldType, StringComparison.OrdinalIgnoreCase))
                    return Data[key].Value;
            }
            return null;
        }

        public string GetValue(string fieldName)
        {
            if (!Data.ContainsKey(fieldName))
                return null;
            return Data[fieldName].Value;
        }

        public string this[string key]
        {
            get
            {
                return GetValue(key);
            }
            set
            {
                Add(key, value);
            }
        }

        public string ApplyAllTokens(string input, ModuleInfo module = null)
        {
            foreach (string k in Data.Keys)
                input = Regex.Replace(input, "\\[" + k + "\\]", Data[k].Value, RegexOptions.IgnoreCase);
            return TokenUtils.Tokenize(input, module, User, false, true);
        }

        public string ApplyAllTokensEncodeSql(string input)
        {
            foreach (string k in Data.Keys) {
                var val = Data[k].Value;
                if (Data[k].Field != null && Data[k].Field.InputTypeStr.IndexOf("datetime") == 0) {
                    // convert date to database invariant format
                    DateTime date;
                    if (DateTime.TryParse(val, CultureInfo.CurrentUICulture.DateTimeFormat, DateTimeStyles.None, out date)) {
                        val = SqlTable.EncodeSql(DateTime.Parse(val, CultureInfo.CurrentUICulture.DateTimeFormat), false);
                    }
                }
                input = Regex.Replace(input, "\\[" + k + "\\]", SqlTable.EncodeSql(val, false), RegexOptions.IgnoreCase);
            }
            return TokenUtils.Tokenize(input, null, User, false, true);
        }
    }

}
