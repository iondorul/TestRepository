﻿using avt.ActionForm.Core.Form;
using avt.ActionForm.Core.Form.Result;
using avt.ActionForm.Core.Utils;
using avt.ActionForm.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace avt.ActionForm.Core.Actions
{
    public class SaveState : IAction
    {
        public ActionInfo ActionInfo { get; set; }
        public string Key { get; set; }

        public void Init(ActionInfo actionInfo, SettingsDictionary settings)
        {
            ActionInfo = actionInfo;
            Key = settings.GetValue("Key", "");
        }

        public IFormEventResult Execute(ActionFormSettings settings, FormData data, eActionContext context)
        {
            if (HttpContext.Current == null)
                return null;

            // save form state
            HttpContext.Current.Session[Key] = data;

            return null;

        }
    }
}
