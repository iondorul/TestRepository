using avt.ActionForm.Core.Form;
using avt.ActionForm.Core.Form.Result;
using avt.ActionForm.Core.Utils;
using avt.ActionForm.Data;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Membership;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Log.EventLog;
using DotNetNuke.Services.Mail;
using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;

namespace avt.ActionForm.Core.Actions
{
    public class SendMail : IAction
    {
        public string From { get; set; }
        public string To { get; set; }
        public bool DetermineEmail { get; set; }
        public string ReplyTo { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string File1 { get; set; }
        public string File2 { get; set; }
        public string File3 { get; set; }
        public string File4 { get; set; }
        public string File5 { get; set; }

        public void Init(ActionInfo actionInfo, SettingsDictionary settings)
        {
            From = settings.GetValue("From", "");
            To = settings.GetValue("To", "");
            DetermineEmail = settings.GetValue("DetermineEmail", false);
            ReplyTo = settings.GetValue("ReplyTo", "");
            Subject = settings.GetValue("Subject", "");
            Body = settings.GetValue("Body", "");
            File1 = settings.GetValue("File1", "");
            File2 = settings.GetValue("File2", "");
            File3 = settings.GetValue("File3", "");
            File4 = settings.GetValue("File4", "");
            File5 = settings.GetValue("File5", "");
        }

        public IFormEventResult Execute(ActionFormSettings settings, FormData data, eActionContext context)
        {
            var toEmail = GetRecipientEmail(data);
            var subject = data.ApplyAllTokens(Subject);
            var emailFrom = GetFromEmail(data);
            var replyTo = data.ApplyAllTokens(ReplyTo);
            var attachments = GetAttachments(data);
            var body = data.ApplyAllTokens(Body);

            // ?
            //data["ResourceUrl"] = TargetAbsoluteUrl();

            Mail.SendMail(emailFrom, toEmail, "", "", replyTo, DotNetNuke.Services.Mail.MailPriority.Normal,
                subject, MailFormat.Html, Encoding.UTF8, body, attachments, "", "", "", "", 
                DotNetNuke.Entities.Host.Host.GetHostSettingsDictionary()["SMTPEnableSSL"] == "Y");

            return null;

//            // return a result that would work if nothing else.
//            return new MessageHtml() {
//                Html = @"Email successfully sent to " + data["Email"] + @"!
//                    <br /><br />
//                    <a href = '" + HttpContext.Current.Request.RawUrl + "' style='display: block; margin: auto; padding: 6px 10px; width: 100px; margin-top: 10px; font-size: 12px;' class = 'ui-state-default'>Continue...</a>"
//            };
        }

        string GetFromEmail(FormData data)
        {
            string emailFrom = data.ApplyAllTokens(From);
            if (!string.IsNullOrEmpty(From))
                return emailFrom;

            emailFrom = data.ApplyAllTokens(DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings().Email);
            return emailFrom;
        }

        string GetRecipientEmail(FormData data)
        {
            string toEmail = data.ApplyAllTokens(To);
            if (!string.IsNullOrEmpty(toEmail))
                return toEmail;

            if (DetermineEmail) {
                // default to first email field in the form
                toEmail = data.GetValueByFieldType("open-email");
                if (toEmail != null)
                    return toEmail;

                // get user email
                toEmail = data.User == null ? null : data.User.Email;
                if (toEmail != null)
                    return toEmail;
            }

            throw new ArgumentException("Can't send email because the email address to send to could not be determined.");
        }

        string[] GetAttachments(FormData data)
        {
            List<string> mappedFiles = new List<string>();
            if (!string.IsNullOrEmpty(File1))
                mappedFiles.Add(HttpContext.Current.Server.MapPath(PortalController.GetCurrentPortalSettings().HomeDirectory + "/" + File1));
            if (!string.IsNullOrEmpty(File2))
                mappedFiles.Add(HttpContext.Current.Server.MapPath(PortalController.GetCurrentPortalSettings().HomeDirectory + "/" + File2));
            if (!string.IsNullOrEmpty(File3))
                mappedFiles.Add(HttpContext.Current.Server.MapPath(PortalController.GetCurrentPortalSettings().HomeDirectory + "/" + File3));
            if (!string.IsNullOrEmpty(File4))
                mappedFiles.Add(HttpContext.Current.Server.MapPath(PortalController.GetCurrentPortalSettings().HomeDirectory + "/" + File4));
            if (!string.IsNullOrEmpty(File5))
                mappedFiles.Add(HttpContext.Current.Server.MapPath(PortalController.GetCurrentPortalSettings().HomeDirectory + "/" + File5));

            return mappedFiles.ToArray();
        }

    }
}
