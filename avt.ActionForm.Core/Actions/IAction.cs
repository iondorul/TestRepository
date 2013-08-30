using avt.ActionForm.Core.Form;
using avt.ActionForm.Core.Form.Result;
using avt.ActionForm.Core.Utils;
using avt.ActionForm.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace avt.ActionForm.Core.Actions
{
    public interface IAction
    {
        /// <summary>
        /// Initializes the interface using the configuration specified under admin
        /// </summary>
        /// <param name="settings"></param>
        void Init(ActionInfo actionInfo, SettingsDictionary settings);

        /// <summary>
        /// This is the call to execute the action.
        /// </summary>
        /// <param name="settings">The configuration for the current form</param>
        /// <param name="data">Contains the submitted data.</param>
        /// <returns>Return NULL unless some additional setps are required such like redirecting to a different resource.</returns>
        IFormEventResult Execute(ActionFormSettings settings, FormData data, eActionContext context);
    }
}
