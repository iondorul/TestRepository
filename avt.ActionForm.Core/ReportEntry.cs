using avt.ActionForm.Core.Fields;
using avt.ActionForm.Core.Form;
using avt.ActionForm.Core.Utils;
using avt.ActionForm.Data;
using DotNetNuke.Entities.Users;
using System;
using System.Xml;

namespace avt.ActionForm
{
    public enum eSubmitStatus
    {
        Submitted,
        PendingPayment,
        Draft
    }

    public class ReportEntry
    {
        public ReportEntry()
        {
            DateSubmitted = DateTime.Now;
            ValidationKey = Guid.NewGuid(); //.ToString().Replace("-", "").Substring(0, 10).ToLower();
        }

        public int ReportEntryId { get; set; }
        public int ModuleId { get; set; }
        public string ReferrerInfo { get; set; }
        public int UserId { get; set; }
        public string RemoteAddress { get; set; }
        public string BrowserInfo { get; set; }
        public DateTime DateSubmitted { get; set; }
        public string FormData { get; set; }

        public eSubmitStatus SubmitStatus { get; set; }
        public string ActionStr { get; set; }
        //public IFormAction Action { get; set; }

        public Guid ValidationKey { get; set; }
        public bool SkipValidation { get; set; }

        XmlDocument _XmlFormData = null;
        public XmlDocument XmlFormData
        {
            get
            {
                if (_XmlFormData == null) {
                    _XmlFormData = new XmlDocument();
                    _XmlFormData.LoadXml(FormData);
                }

                return _XmlFormData;
            }
        }

        public void Save()
        {
            ReportEntryId = DataProvider.Instance().SaveReport(ReportEntryId, ModuleId, ReferrerInfo, UserId, RemoteAddress, BrowserInfo, DateSubmitted,
                FormData, (int)SubmitStatus, ActionStr, ValidationKey, SkipValidation);
        }

        public static ReportEntry Get(int id)
        {
            return CBO.FillObject<ReportEntry>(
                DataProvider.Instance().GetReportEntry(id));
        }

        public static ReportEntry Get(string reportKey)
        {
            if (string.IsNullOrEmpty(reportKey))
                return null;

            return CBO.FillObject<ReportEntry>(
                DataProvider.Instance().GetReportEntry(reportKey));
        }

        public static FormData GetAsFormData(int reportId)
        {
            if (reportId == -1)
                return null;

            var report = Get(reportId);
            if (report == null)
                return null;

            return report.ToFormData();
        }

        public static FormData GetAsFormData(string reportKey)
        {
            if (string.IsNullOrEmpty(reportKey))
                return null;

            var report = Get(reportKey);
            if (report == null)
                return null;

            return report.ToFormData();
        }

        public FormData ToFormData()
        {
            var settings = new ActionFormSettings(ModuleId);
            var data = new FormData(settings);

            foreach (FormField field in settings.Fields) {
                if (field.IsActive) {
                    var value = XmlFormData.FirstChild[field.TitleCompacted] == null ? "" : XmlFormData.FirstChild[field.TitleCompacted].InnerText;
                    data.Data[field.TitleCompacted] = new FieldData(field, value);
                }
            }

            // get the rest of the data that are not actually fields
            foreach (XmlElement dataNode in XmlFormData.FirstChild.ChildNodes) {
                if (data[dataNode.Name] == null)
                    data[dataNode.Name] = dataNode.InnerText;
            }

            data["ReportEntryId"] = ReportEntryId.ToString();
            data["ReportKey"] = ValidationKey.ToString();

            // load user
            data.User = UserController.GetUserById(settings.PortalId, UserId);
            data.Report = this;

            if (!SkipValidation)
                data.Validate();

            return data;
        }
    }
}