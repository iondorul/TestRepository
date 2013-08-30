using avt.ActionForm.Core.Form;
using avt.ActionForm.Core.Form.Result;
using avt.ActionForm.Core.Utils;
using avt.ActionForm.Data;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Membership;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace avt.ActionForm.Core.Actions
{
    public class UserLogin : IAction
    {
        public void Init(ActionInfo actionInfo, SettingsDictionary settings)
        {
        }

        public IFormEventResult Execute(ActionFormSettings settings, FormData data, eActionContext context)
        {
            string username = data.GetValueByFieldType("open-username");
            if (username == null)
                username = data.GetValueByFieldType("open-email");
            if (username == null)
                throw new ArgumentException("Invalid username!");

            // get the password - note that it could come from registration, so there are a few scenarios to handle
            string password = data["RegRandomPass"];
            if (string.IsNullOrEmpty(password))
                password = data.GetValueByFieldType("open-password-confirm");
            if (string.IsNullOrEmpty(password))
                password = data.GetValueByFieldType("open-password");
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Invalid password!");

            UserLoginStatus loginStatus = UserLoginStatus.LOGIN_FAILURE;
            UserInfo cUser = UserController.UserLogin(
                PortalController.GetCurrentPortalSettings().PortalId,
                username,
                password,
                "",
                PortalController.GetCurrentPortalSettings().PortalName,
                ActionFormController.GetUserIp(),
                ref loginStatus,
                true
            );

            // if user exist, let's ignore this
            if ((cUser == null || cUser.UserID <= 0) && loginStatus != UserLoginStatus.LOGIN_SUCCESS && loginStatus != UserLoginStatus.LOGIN_SUPERUSER) {
                throw new Exception("Login failed! Invalid username or password (status " + loginStatus.ToString() + ")!"); // + loginStatus.ToString()); 
            }

            // put current user in form data so the rest of the actions can take it from there
            data.User = cUser;
            return null;
        }
    }
}
