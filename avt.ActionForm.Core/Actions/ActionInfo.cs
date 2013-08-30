using avt.ActionForm.Core.Fields;
using avt.ActionForm.Core.Form;
using avt.ActionForm.Core.Form.Result;
using avt.ActionForm.Core.Utils;
using avt.ActionForm.Data;
using avt.ActionForm.Utils;
using DnnSharp.Common.ActiveTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml.Serialization;

namespace avt.ActionForm.Core.Actions
{
    public enum eActionContext
    {
        Init,
        Submit,
        AjaxValidation,
        Validation
    }

    [Table("avtActionForm_FormActions")]
    public class ActionInfo : ActiveTableBase<ActionInfo>
    {
        public ActionInfo()
        {
            Parameters = new SettingsDictionary();
        }

        [PrimaryKey]
        [XmlIgnore]
        public int Id { get; set; }

        [Field]
        public int ModuleId { get; set; }

        [Field]
        public string Description { get; set; }

        [Field]
        [XmlIgnore]
        public int? FieldId { get; set; }

        // this is only used for import/export - FieldId can't be migrated since it's an autoincrement primary key
        public string FieldName { get;set; }

        [Field]
        public string EventName { get; set; }

        [Field]
        public string ActionType { get; set; }

        [Field]
        public int OrderIndex { get; set; }

        /// <summary>
        /// This is a MyToken that determines either the action will be executed
        /// </summary>
        [Field]
        public string Condition { get; set; }

        /// <summary>
        /// Not a database fileds, just used to trigger the deletion
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// ScriptIgnore otherwise it will throw error when populated from JS
        /// </summary>
        [ScriptIgnore]
        [Field]
        public DateTime LastModified { get; set; }

        [ScriptIgnore]
        [Field]
        public int LastModifiedBy { get; set; }

        public SettingsDictionary Parameters { get; set; }

        [ScriptIgnore]
        [Field]
        public string ActionData
        {
            get {
                return new JavaScriptSerializer().Serialize(Parameters);
            }
            set {
                var data =  new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(value);
                if (data != null) {
                    Parameters = new SettingsDictionary(data);
                } else {
                    Parameters = new SettingsDictionary();
                }
            }
        }

        public override void Save()
        {
            LastModified = DateTime.Now;
            LastModifiedBy = DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo().UserID;

            base.Save();
            // TODO : caching
        }

        public IFormEventResult Execute(ActionFormSettings settings, FormData data, eActionContext context)
        {
            // evlauate condition
            if (!string.IsNullOrEmpty(Condition) && !TokenUtils.TokenizeAsBool(Condition))
                return null;

            var action = ActionFormController.CreateInstance<IAction>(ActionFormController.ActionDefs[ActionType].TypeStr);
            action.Init(this, Parameters);
            return action.Execute(settings, data, context);
        }


        public IAction GetExecutableAction(ActionFormSettings settings, FormData data)
        {
            var action = ActionFormController.CreateInstance<IAction>(ActionFormController.ActionDefs[ActionType].TypeStr);
            action.Init(this, Parameters);
            return action;
        }


    }
}
