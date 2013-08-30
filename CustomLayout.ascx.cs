using System;
using System.Linq;
using System.Text;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Framework;
using DotNetNuke.UI.UserControls;

namespace avt.ActionForm
{
    public partial class CustomLayout : PortalModuleBase
    {
        ActionFormSettings _Settings = null;
        protected ActionFormSettings AfSettings
        {
            get
            {
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
            (textEditor as TextEditor).Text = AfSettings.LayoutHtml.Value;

            rpFields.DataSource = AfSettings.Fields;
            rpFields.DataBind();
            //lbFormName.Text = AfSettings.FormTitle;

            //tbStartDate.Text = DateTime.Now.AddMonths(-1).ToShortDateString();
            //tbEndDate.Text = DateTime.Now.ToShortDateString();
        }

        protected void OnGenerate2ColLayout(object Sender, EventArgs args)
        {
            var sb = new StringBuilder();
            for (var iField = 0; iField < AfSettings.Fields.Count; iField++) {
                if (AfSettings.Fields[iField].InputTypeStr != "button") {
                    sb.AppendFormat("<div style=\"width: 40%; float: left; margin: 0 20px 0 0; \">[Fields:{0}]</div>", AfSettings.Fields[iField].TitleCompacted);
                    if (iField % 2 == 1) {
                        sb.Append("<div style=\"clear: both; \"></div>");
                    }
                }
            }
            sb.Append("<div style=\"clear: both; \"></div>");
            sb.AppendFormat("<div style=\"text-align: center; margin: 0 20px 0 0; \">");
            foreach (var btn in AfSettings.Fields.Where(x => x.InputTypeStr == "button"))
                sb.AppendFormat("[Fields:{0}]", btn.TitleCompacted);
            sb.AppendFormat("</div>");

            (textEditor as TextEditor).Text = sb.ToString();
        }

        protected void OnSave(object Sender, EventArgs args)
        {
            AfSettings.HasCustomLayout.Value = true;
            AfSettings.LayoutHtml.Value = (textEditor as TextEditor).Text;
            AfSettings.Save();

            Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(TabId));
        }

        protected override void OnPreRender(EventArgs e)
        {
            // doing this at another stage will break things work on IE
            if (!Page.ClientScript.IsClientScriptIncludeRegistered("afJquery"))
                Page.ClientScript.RegisterClientScriptInclude("afJquery", TemplateSourceDirectory + "/js/jquery-1.8.3.js?v=" + ActionFormController.Build);

            if (!Page.ClientScript.IsClientScriptIncludeRegistered("afJqueryUi"))
                Page.ClientScript.RegisterClientScriptInclude("afJqueryUi", TemplateSourceDirectory + "/js/jquery-ui-1.9.2.js?v=" + ActionFormController.Build);

            CDefault defaultPage = (CDefault)Page;
            defaultPage.AddStyleSheet("AFormJQueryTheme", TemplateSourceDirectory + "/templates/jQuery/sunny/jquery-ui-noscope.css");
            defaultPage.AddStyleSheet("ActionFormAdminUi", TemplateSourceDirectory + "/admin.css");

            base.OnPreRender(e);
        }

    }

}
