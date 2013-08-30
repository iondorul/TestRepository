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
using System.Web;
using System.Web.Script.Serialization;

namespace avt.ActionForm.Core.Actions
{
    public class UserRegistration : IAction
    {
        public bool EmailUsername { get; set; }
        public bool RandomPass { get; set; }
        public bool SendDnnMail { get; set; }
        public bool LoginIfExists { get; set; }

        public void Init(ActionInfo actionInfo, SettingsDictionary settings)
        {
            EmailUsername = settings.GetValue("EmailUsername", false);
            RandomPass = settings.GetValue("RandomPass", false);
            SendDnnMail = settings.GetValue("SendDnnMail", false);
            LoginIfExists = settings.GetValue("LoginIfExists", false);
        }

        public IFormEventResult Execute(ActionFormSettings settings, FormData data, eActionContext context)
        {
            // if IngoreUserexists is set to true, then first check a user doesn't already exists
            if (LoginIfExists && CheckUserExists(data)) {
                new UserLogin().Execute(settings, data, context);
                return null;
            }

            var portal = PortalController.GetCurrentPortalSettings();

            UserInfo newUser = new UserInfo();
            newUser.PortalID = portal.PortalId;
            newUser.Username = EmailUsername ? data.GetValueByFieldType("open-email") : data.GetValueByFieldType("open-username");
            newUser.FirstName = data["FirstName"] ?? " ";
            newUser.LastName = data["LastName"] ?? " "; ;
            newUser.DisplayName = newUser.FirstName + " " + newUser.LastName;
            newUser.Email = data.GetValueByFieldType("open-email");
            newUser.Profile.SetProfileProperty("Email", newUser.Email);

            //if (HttpContext.Current != null && !string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["language"]))
            //    newUser.Profile.PreferredLocale = HttpContext.Current.Request.QueryString["language"];

            newUser.Membership.Approved = portal.UserRegistration != 3;
            newUser.Membership.Password = GetPassword(data);

            UserCreateStatus createStatus = DotNetNuke.Entities.Users.UserController.CreateUser(ref newUser);
            if (createStatus != UserCreateStatus.Success)
                throw new Exception(UserController.GetUserCreateStatus(createStatus));

            if (SendDnnMail) {
                Mail.SendMail(newUser, 
                    portal.UserRegistration != 3 ? MessageType.UserRegistrationPublic : MessageType.UserRegistrationVerified,
                    portal);
            }

            // put current user in form data so the rest of the actions can take it from there
            data.User = newUser;
            data["NewUserId"] = newUser.UserID.ToString();

            return null;
        }

        private string GetPassword(FormData data)
        {
            string password;
            if (RandomPass) {
                password = UserController.GeneratePassword();
                data["RegRandomPass"] = password;
            } else {
                password = data.GetValueByFieldType("open-password-confirm");
                if (password == null)
                    password = data.GetValueByFieldType("open-password");
                if (password == null)
                    throw new ArgumentException("Could not determine password field needed to create new user account!");
            }
            return password;
        }

        bool CheckUserExists(FormData data)
        {
            string username = data.GetValueByFieldType("open-username");
            if (username == null)
                username = data.GetValueByFieldType("open-email");
            if (username == null)
                throw new ArgumentException("Invalid username!");

            UserInfo user = UserController.GetUserByName(PortalController.GetCurrentPortalSettings().PortalId, username);
            return user != null && user.UserID > 0;
        }

    }
}
