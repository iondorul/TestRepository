using avt.ActionForm.Core.Actions;
using avt.ActionForm.Core.Form.Result;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Exceptions;
using System;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace avt.ActionForm
{
    public partial class PayPal : System.Web.UI.Page
    {
        protected void Page_Load(Object Sender, EventArgs args)
        {
            LoadUiCulture(null);

            // don't cache anything
            Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            Response.Cache.SetValidUntilExpires(false);
            Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();

            try {

                ActionFormSettings settings = new ActionFormSettings();
                settings.Load(Convert.ToInt32(Request.QueryString["mid"]));

                var reportEntry = GetFormData();
                var data = reportEntry.ToFormData();
                var actionInfo = GetActionInfo();
                var action = actionInfo.GetExecutableAction(settings, data) as CollectPaymentWithPayPal;
                var btn = settings.Fields.SingleOrDefault(x => x.FormFieldId == actionInfo.FieldId);

                // proceed to paypal?
                if (Request.QueryString["paypal-continue"] != null) {
                    Response.Write(action.GetHtmlFormForSubmit(settings, data, AppRelativeVirtualPath));
                    return;
                }

                // just got back from PayPal
                // either the payment was already completed via IPN, or we can complete it now
                if (Request.QueryString["paypal-return"] != null) {
                    if (reportEntry.SubmitStatus == eSubmitStatus.Submitted) {
                        // already submitted via IPN; just do the final result
                        FormResultFactory.GetAction(reportEntry.ActionStr).Execute(HttpContext.Current, new Literal());
                    } else {
                        if (action.IsValidResponse(Request)) {
                            var result = settings.Execute(data, reportEntry.ReportEntryId, btn, "click");
                            result.Execute(HttpContext.Current, new Literal());
                        } else {
                            if (action.PendingPageId > 0)
                                Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(action.PendingPageId));
                        }
                    }
                    return;
                }
                
                // this is the respone from IPN, it runs outside of use context
                if (Request.QueryString["paypal-ipn"] != null) {
                    if (action.IsValidResponse(Request)) {
                        var result = settings.Execute(data, reportEntry.ReportEntryId, btn, "click");
                        // nothing to execute since this is the IPN // action.Execute(HttpContext.Current, new Literal());
                    }
                }

            } catch (Exception ex) {
                Exceptions.LogException(ex);
                Response.Write("Error occured: " + ex.Message + "\n<br/>Please contact the website administrators.");
            }
        }

        ReportEntry GetFormData()
        {
            var strId = Request.QueryString["id"] ?? Request.Params["custom"];

            if (string.IsNullOrEmpty(strId))
                throw new ArgumentException("Id for this payment is missing");

            int id;
            if (!int.TryParse(strId, out id))
                throw new ArgumentException("Invalid payment Id");

            var entry = ReportEntry.Get(id);
            if (entry == null)
                throw new ArgumentException("Could not find payment initial data. Please contact the administrators.");

            return entry;
        }

        // TODO: security validation for &action so one can't bypass actions from client side
        ActionInfo GetActionInfo()
        {
            int actionId;
            if (!int.TryParse(Request.QueryString["action"], out actionId))
                throw new ArgumentException("Invalid PayPal action id");

            var actionInfo = ActionInfo.GetOneByProperty("Id", actionId);
            if (actionInfo == null)
                throw new ArgumentException("Invalid PayPal action");

            return actionInfo;
        }

        void LoadUiCulture(string currentCulture)
        {
            CultureInfo culture = null;
            try {
                culture = CultureInfo.GetCultureInfo(currentCulture);
            } catch { }

            if (culture == null) {
                try {
                    culture = new CultureInfo(PortalController.GetCurrentPortalSettings().DefaultLanguage);
                } catch { }
            }

            if (culture == null)
                culture = new CultureInfo("en-US");

            System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
        }

        //private bool ValidateResponse(ActionFormSettings settings)
        //{
        //    PayPalSettings payPalCtrl = new PayPalSettings();
        //    return payPalCtrl.IsValidResponse(ActionFormSettings.ReadValueFromXml(settings.PayPalSettingsXml, "//testMode", "true") == "true", Request);
        //}

    }
}