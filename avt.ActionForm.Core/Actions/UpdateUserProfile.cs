using avt.ActionForm.Core.Form;
using avt.ActionForm.Core.Form.Result;
using avt.ActionForm.Core.Utils;
using avt.ActionForm.Data;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Membership;
using DotNetNuke.Services.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace avt.ActionForm.Core.Actions
{
    public class UpdateUserProfile : IAction
    {
        public bool UpdatePassword { get; set; }
        public string UpdateDisplayName { get; set; }

        public void Init(ActionInfo actionInfo, SettingsDictionary settings)
        {
            UpdatePassword = settings.GetValue("UpdatePassword", false);
            UpdateDisplayName = settings.GetValue("UpdateDisplayName", "");
        }

        public IFormEventResult Execute(ActionFormSettings settings, FormData data, eActionContext context)
        {
            PortalSettings portal = DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings();
            DotNetNuke.Entities.Users.UserController usrCtrl = new UserController();

            if (data.User == null || data.User.UserID <= 0)
                return null;

            // try to map form fields profile properties
            foreach (string key in data.Data.Keys) {
                if (DotNetNuke.Entities.Profile.ProfileController.GetPropertyDefinitionByName(data.User.PortalID, key) != null) {
                    data.User.Profile.SetProfileProperty(key, data[key]);
                    continue;
                }

                if (!settings.FieldsByName.ContainsKey(key))
                    continue; // probably a token

                if (DotNetNuke.Entities.Profile.ProfileController.GetPropertyDefinitionByName(data.User.PortalID, settings.FieldsByName[key].TitleTokenized) != null) {
                    data.User.Profile.SetProfileProperty(settings.FieldsByName[key].TitleTokenized, data[key]);
                    continue;
                }

                if (DotNetNuke.Entities.Profile.ProfileController.GetPropertyDefinitionByName(data.User.PortalID, settings.FieldsByName[key].TitleTokenized.Replace(" ", "")) != null) {
                    data.User.Profile.SetProfileProperty(settings.FieldsByName[key].TitleTokenized.Replace(" ", ""), data[key]);
                    continue;
                }

                if (DotNetNuke.Entities.Profile.ProfileController.GetPropertyDefinitionByName(data.User.PortalID, settings.FieldsByName[key].TitleCompacted) != null) {
                    data.User.Profile.SetProfileProperty(settings.FieldsByName[key].TitleCompacted, data[key]);
                    continue;
                }
            }

            if (!string.IsNullOrEmpty(UpdateDisplayName))
                data.User.DisplayName = data[UpdateDisplayName];

            UserController.UpdateUser(data.User.IsSuperUser ? -1 : data.User.PortalID, data.User);

            // also update password if there's a password field in the form
            if (UpdatePassword) {
                var passUpdate = data.GetValueByFieldType("open-password-confirm") ?? data.GetValueByFieldType("open-password");
                if (!string.IsNullOrEmpty(passUpdate))
                    UserController.ChangePassword(data.User, null, passUpdate);
            }

            return null;
        }

    }
}
