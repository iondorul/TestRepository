using System;
using System.Linq;
using System.Globalization;
using System.Text;
using System.Web.UI;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Framework;
using System.Data.SqlTypes;
using LumenWorks.Framework.IO.Csv;
using System.IO;
using avt.ActionForm.Core.Fields;


namespace avt.ActionForm
{
    public partial class Reports : PortalModuleBase
    {
        ActionFormSettings _Settings = null;
        protected ActionFormSettings AfSettings {
            get {
                if (_Settings == null) {
                    _Settings = new ActionFormSettings();
                    _Settings.Load(ModuleId);
                }
                return _Settings;
            }
        }

        /////////////////////////////////////////////////////////////////////////////////
        // EVENT HANDLERS

        protected void Page_Init(object sender, EventArgs e)
        {
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack) {
                BindData();
            }
        }

        private void BindData()
        {
            lbFormName.Text = ModuleConfiguration.ModuleTitle;

            tbStartDate.Text = DateTime.Now.AddMonths(-1).ToShortDateString();
            tbEndDate.Text = DateTime.Now.ToShortDateString();
        }

        protected void OnDownloadReport(object Sender, EventArgs args)
        {
            DateTime startDate;
            DateTime endDate;

            try {
                startDate = DateTime.Parse(tbStartDate.Text);
            } catch { startDate = SqlDateTime.MinValue.Value; }

            try {
                endDate = DateTime.Parse(tbEndDate.Text).AddDays(1);
            } catch { endDate = SqlDateTime.MaxValue.Value; }

            byte[] data = Encoding.UTF8.GetBytes(AfSettings.GenerateCsvReport(startDate, endDate)); // System.IO.File.ReadAllBytes(Server.MapPath(dlInfo.FilePath));

            Response.AddHeader("Content-Type", "application/octet-stream");
            Response.AddHeader("Content-Length", data.Length.ToString());
            Response.AddHeader("Content-Disposition", "attachment; filename=\"" + string.Format("{0}.{1}-{2}.csv", ModuleConfiguration.ModuleTitle, tbStartDate.Text, tbEndDate.Text) + "\"");

            Response.ContentEncoding = Encoding.UTF8;
            Response.Charset = "utf-8";
            Response.BinaryWrite(System.Text.Encoding.UTF8.GetPreamble());
            Response.BinaryWrite(data);
            Response.End();
        }

        protected void OnUploadReport(object Sender, EventArgs args)
        {
            if (!uplCsv.HasFile)
                return;


            var count = 0;
            using (var sr = new StreamReader(uplCsv.FileContent)) {
                 using (CsvReader csv = new CsvReader(sr, true)) {

                    var headers = csv.GetFieldHeaders();

                    while (csv.ReadNextRecord()) {

                        // only import data for current module
                        int moduleId;
                        if (!int.TryParse(csv["ModuleId"], out moduleId) || moduleId != ModuleId)
                            continue;

                        // read entry id
                        int entryId;
                        if (!int.TryParse(csv["EntryId"], out entryId))
                            continue;

                        var entry = ReportEntry.Get(entryId);
                        if (entry == null)
                            continue;

                        var xmlData = entry.XmlFormData;
                        foreach (FormField field in AfSettings.Fields) {
                            //if (!headers.Contains(field.TitleTokenized))
                            //    continue;
                            if (xmlData.DocumentElement[field.TitleCompacted] != null)
                                xmlData.DocumentElement[field.TitleCompacted].InnerText = csv[field.TitleTokenized];
                        }
                        entry.FormData = xmlData.OuterXml;
                        entry.Save();

                        //// read all columns starting with index 8 (the rest are EntryId,ModuleId,FormTitle,ReferrerInfo,UserId,RemoteAddress,BrowserInfo,DateSubmitted)
                        //for (var i = 8; i < headers.Length; i++) {
                        //    // translate field title back to name
                        //    var fieldName =     
                        //}

                        count++;
                    }
                }
            }

            pnlImportStatus.InnerHtml = string.Format("Successfully imported {0} entries", count);
        }

        protected override void OnPreRender(EventArgs e)
        {
            // doing this at another stage will break things work on IE
            if (!Page.ClientScript.IsClientScriptIncludeRegistered("afJquery")) {
                Page.ClientScript.RegisterClientScriptInclude("afJquery", TemplateSourceDirectory + "/js/jquery-1.8.3.js?v=" + ActionFormController.Build);
            }

            if (!Page.ClientScript.IsClientScriptIncludeRegistered("afJqueryUi")) {
                Page.ClientScript.RegisterClientScriptInclude("afJqueryUi", TemplateSourceDirectory + "/js/jquery-ui-1.9.2.js?v=" + ActionFormController.Build);
            }

            CDefault defaultPage = (CDefault)Page;
            defaultPage.AddStyleSheet("AFormJQueryTheme", TemplateSourceDirectory + "/templates/jQuery/sunny/jquery-ui-noscope.css");
            defaultPage.AddStyleSheet("ActionFormAdminUi", TemplateSourceDirectory + "/admin.css");

            base.OnPreRender(e);
        }

    }

}
