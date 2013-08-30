using avt.ActionForm.Core.Form;
using avt.ActionForm.Core.Form.Result;
using avt.ActionForm.Core.Utils;
using avt.ActionForm.Data;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Membership;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Mail;
using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace avt.ActionForm.Core.Actions
{
    public class ExecuteSql : IAction
    {
        public string ConnectionString { get; set; }
        public string SqlQuery { get; set; }
        public string OutputTokenName { get; set; }
        public bool ShowErrors { get; set; }

        public void Init(ActionInfo actionInfo, SettingsDictionary settings)
        {
            ConnectionString = settings.GetValue("ConnectionString", "");
            SqlQuery = settings.GetValue("SqlQuery", "");
            OutputTokenName = settings.GetValue("OutputTokenName", "");
            ShowErrors = settings.GetValue("ShowErrors", false);
        }

        public IFormEventResult Execute(ActionFormSettings settings, FormData data, eActionContext context)
        {
            string sql = data.ApplyAllTokensEncodeSql(SqlQuery);
            if (string.IsNullOrEmpty(sql))
                return null; // nothing to execute

            string token = OutputTokenName;
            SqlConnection connection = null;
            try {
                var connStr = ConnectionString;
                if (string.IsNullOrEmpty(connStr))
                    connStr = DotNetNuke.Common.Utilities.Config.GetConnectionString();

                connection = new SqlConnection(connStr);
                var cmd = new SqlCommand(sql, connection);
                cmd.CommandTimeout = 60 * 10; // 10 minutes
                connection.Open();
                if (string.IsNullOrEmpty(token)) {
                    cmd.ExecuteNonQuery();
                } else {
                    data[token] = cmd.ExecuteScalar().ToString();
                }

                //if (string.IsNullOrEmpty(token)) {
                //    SqlHelper.ExecuteNonQuery(DotNetNuke.Common.Utilities.Config.GetConnectionString(), System.Data.CommandType.Text, sql);
                //} else {
                //    data[token] = (SqlHelper.ExecuteScalar(DotNetNuke.Common.Utilities.Config.GetConnectionString(), System.Data.CommandType.Text, sql) ?? "").ToString();
                //}
            } catch (Exception ex) {
                //if (UserController.GetCurrentUserInfo().IsInRole(PortalController.GetCurrentPortalSettings().AdministratorRoleName))
                //    throw new Exception("Error executing query " + sql, ex);
                Exceptions.LogException(new Exception("Error executing query " + sql, ex));
                if (ShowErrors) {
                    throw; // original error from SQL
                } else {
                    throw new Exception("There was an error with the form. Please contact the administrator or check logs for more info.");
                }
            } finally {
                if (connection != null)
                    connection.Close();
            }

            return null;
        }

    }
}
