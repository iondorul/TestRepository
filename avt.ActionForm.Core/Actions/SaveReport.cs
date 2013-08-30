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
using System.Xml;

namespace avt.ActionForm.Core.Actions
{
    public class SaveReport : IAction
    {
        public void Init(ActionInfo actionInfo, SettingsDictionary settings)
        {
            
        }

        public IFormEventResult Execute(ActionFormSettings settings, FormData data, eActionContext context)
        {
            Execute(settings, data, null, eSubmitStatus.Submitted);
            return null;
        }

        public ReportEntry Execute(ActionFormSettings settings, FormData data, IFormEventResult result, eSubmitStatus submitStatus)
        {
            // merge data in xml
            StringBuilder strXML = new StringBuilder();
            XmlWriterSettings xmlSettings = new XmlWriterSettings();
            xmlSettings.Indent = false;
            xmlSettings.OmitXmlDeclaration = true;
            XmlWriter Writer = XmlWriter.Create(strXML, xmlSettings);

            Writer.WriteStartElement("data");
            foreach (string k in data.Data.Keys) {
                Writer.WriteElementString(k, data[k]);
            }
            Writer.WriteEndElement(); // data
            Writer.Close();

            int reportId = -1;
            if (data.Data.ContainsKey("ReportEntryId")) {
                int.TryParse(data.Data["ReportEntryId"].Value, out reportId);
            }

            var report = reportId == -1 ? null : ReportEntry.Get(reportId);
            if (report == null) {
                report = new ReportEntry();
                report.ModuleId = settings.ModuleId;
            }

            // try to keep IP and browser info of the original
            if (string.IsNullOrEmpty(report.RemoteAddress)) {
                try { report.RemoteAddress = HttpContext.Current.Request.UserHostAddress; } catch { }
                report.RemoteAddress = report.RemoteAddress ?? "";
            }

            if (string.IsNullOrEmpty(report.BrowserInfo)) {
                try { report.BrowserInfo = HttpContext.Current.Request.UserAgent; } catch { }
                report.BrowserInfo = report.BrowserInfo ?? "";
            }

            if (string.IsNullOrEmpty(report.ReferrerInfo)) {
                try { report.ReferrerInfo = ReferrerUri != null ? ReferrerUri.ToString() : ""; } catch { }
                report.ReferrerInfo = report.ReferrerInfo ?? "";
            }

            if (data.User != null)
                report.UserId = data.User.UserID;

            //if (report.UserId <= 0) {
            //    UserInfo cUser = UserController.GetCurrentUserInfo();
            //    report.UserId = cUser != null ? cUser.UserID : -1;
            //}

            if (result != null) {
                report.ActionStr = string.Format("<?xml version=\"1.0\"?><action><type>{0}</type><data>{1}</data></action>",
                    result.GetType(), FormResultFactory.Serialize(result));
            }

            report.FormData = strXML.ToString();
            report.SubmitStatus = submitStatus;
            report.SkipValidation = data["SkipValidation"] == "true";
            report.Save();

            // save ID in form data
            data["ReportEntryId"] = report.ReportEntryId.ToString();
            data["ReportKey"] = report.ValidationKey.ToString();

            return report;
        }


        Uri _ReferrerUri = null;
        private Uri ReferrerUri
        {
            get
            {
                if (_ReferrerUri == null) {
                    if (HttpContext.Current.Request.UrlReferrer != null && !string.IsNullOrEmpty(HttpContext.Current.Request.UrlReferrer.ToString())) {
                        _ReferrerUri = HttpContext.Current.Request.UrlReferrer;
                    }
                }

                return _ReferrerUri;
            }
        }

    }
}
