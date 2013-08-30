using avt.ActionForm.Core.Form;
using avt.ActionForm.Core.Form.Result;
using avt.ActionForm.Core.Utils;
using avt.ActionForm.Data;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Membership;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Mail;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace avt.ActionForm.Core.Actions
{
    public class GrantRole2 : IAction
    {
        public int RoleId { get; set; }
        public string RoleNames { get; set; }
        public string RoleExpiration { get; set; }

        public void Init(ActionInfo actionInfo, SettingsDictionary settings)
        {
            RoleId = settings.GetValue("RoleId", -1);
            RoleNames = settings.GetValue("RoleNames", "");
            RoleExpiration = settings.GetValue("RoleExpiration", "");
        }

        public IFormEventResult Execute(ActionFormSettings settings, FormData data, eActionContext context)
        {
            if (data.User == null || data.User.UserID <= 0)
                return null; // no user to grant roles for

            // assign roles
            RoleController roleCtrl = new RoleController();
            PortalSettings portal = PortalController.GetCurrentPortalSettings();

            List<string> roleNames = GetRoleNames(roleCtrl, portal, data);
            if (roleNames.Count == 0)
                return null;

            foreach (var roleName in roleNames) {

                var role = roleCtrl.GetRoleByName(portal.PortalId, roleName);
                if (role == null || role.RoleID < 0)
                    continue; // role no longer exists

                if (data.User.IsInRole(roleName)) // remove the role since we may have to reset the expiration date
                    roleCtrl.DeleteUserRole(portal.PortalId, data.User.UserID, role.RoleID);

                var expires = SqlDateTime.MaxValue.Value;
                int days = -1;
                if (int.TryParse(data.ApplyAllTokens(RoleExpiration).Trim(), out days)) {
                    expires = DateTime.Now.AddDays(days);
                }

                roleCtrl.AddUserRole(portal.PortalId, data.User.UserID, role.RoleID, expires);
            }

            DataCache.ClearUserCache(portal.PortalId, data.User.Username);
            return null;
        }

        List<string> GetRoleNames(RoleController roleCtrl, PortalSettings portal, FormData data)
        {
            List<string> roleNames = new List<string>();

            if (RoleId > 0) {
                var role = roleCtrl.GetRole(RoleId, portal.PortalId);
                if (role != null)
                    roleNames.Add(role.RoleName);
                return roleNames;
            }

            // it's not an ID, tokenize it and get it as a list of role names
            var rolesNames = data.ApplyAllTokens(RoleNames);
            foreach (string roleName in rolesNames.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)) {
                var role = roleCtrl.GetRoleByName(portal.PortalId, roleName);
                if (role == null || role.RoleID < 0)
                    continue; // role no longer exists

                roleNames.Add(role.RoleName);
            }

            return roleNames;
        }

    }
}
