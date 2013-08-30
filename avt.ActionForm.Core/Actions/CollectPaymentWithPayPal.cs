using avt.ActionForm.Core.Form;
using avt.ActionForm.Core.Form.Result;
using avt.ActionForm.Core.Utils;
using avt.ActionForm.Data;
using avt.ActionForm.Utils;
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
    public enum ePayPalRecurringInterval
    {
        None,
        Monthly,
        Yearly
    }

    public class CollectPaymentWithPayPal : IAction
    {
        public const string _PayPalUrl = "https://www.paypal.com/cgi-bin/webscr";
        public const string _PayPalUrlSandBox = "https://www.sandbox.paypal.com/cgi-bin/webscr";

        public ActionInfo ActionInfo { get; set; }
        public string SandboxAccount { get; set; }
        public string LiveAccount { get; set; }
        public bool TestMode { get; set; }
        public ePayPalRecurringInterval Recurring { get; set; }
        public string ItemTitle { get; set; }
        public string CurrencyCode { get; set; }
        public string Amount { get; set; }
        public int CancelPageId { get; set; }
        public int PendingPageId { get; set; }
        public bool GenerateOrderId { get; set; }

        public void Init(ActionInfo actionInfo, SettingsDictionary settings)
        {
            ActionInfo = actionInfo;
            SandboxAccount = settings.GetValue("SandboxAccount", "");
            LiveAccount = settings.GetValue("LiveAccount", "");
            TestMode = settings.GetValue("TestMode", false);
            Recurring = settings.GetValue("Recurring", ePayPalRecurringInterval.None);
            ItemTitle = settings.GetValue("ItemTitle", "");
            CurrencyCode = settings.GetValue("CurrencyCode", "USD");
            Amount = settings.GetValue("Amount", "0");
            CancelPageId = settings.GetValue("CancelPageId", -1);
            PendingPageId = settings.GetValue("PendingPageId", -1);
            GenerateOrderId = settings.GetValue("GenerateOrderId", false);
        }

        public IFormEventResult Execute(ActionFormSettings settings, FormData data, eActionContext context)
        {
            // force saving data in database so we pick it up after payment is successfull
            //new SaveReport().Execute(settings, data, eSubmitStatus.PendingPayment);

            // redirect to paypal page
            string moduleDir = HttpRuntime.AppDomainAppVirtualPath.TrimEnd('/') + "/DesktopModules/AvatarSoft/ActionForm";
            string payPalHandleUrl = moduleDir.TrimEnd('/') + "/PayPal.aspx?mid={0}&action={1}&id={2}&paypal-continue=";

            return new RedirectToUrl() { 
                Status  = eSubmitStatus.PendingPayment,
                Url = string.Format(UriUtils.ToAbsoluteUrl(payPalHandleUrl), settings.ModuleId, ActionInfo.Id, data["ReportEntryId"]) };
        }

        public string GetHtmlFormForSubmit(ActionFormSettings settings, FormData data, string baseUrl)
        {
            string url = TestMode ? _PayPalUrlSandBox : _PayPalUrl;
            StringBuilder sbForm = new StringBuilder();

            var id = data["ReportEntryId"];
            var returnUrl = UriUtils.ToAbsoluteUrl(string.Format("{0}?mid={1}&id={2}&action={3}&paypal-return=", baseUrl, settings.ModuleId, id, ActionInfo.Id));
            var notifyUrl = UriUtils.ToAbsoluteUrl(string.Format("{0}?mid={1}&id={2}&action={2}&paypal-ipn=", baseUrl, settings.ModuleId, id, ActionInfo.Id));
            var cancelUrl = UriUtils.ToAbsoluteUrl(DotNetNuke.Common.Globals.NavigateURL(CancelPageId));

            sbForm.Append("<html><body>");
            sbForm.AppendFormat("<form id='payForm' method='post' action='{0}'>", url);
            sbForm.AppendFormat("<input type='hidden' name='cmd' value='{0}'>", Recurring == ePayPalRecurringInterval.None ? "_xclick" : "_xclick-subscriptions");

            sbForm.AppendFormat("<input type='hidden' name='business' value='{0}'>", data.ApplyAllTokens(TestMode ? SandboxAccount : LiveAccount));
            sbForm.AppendFormat("<input type='hidden' name='item_number' value='{0}'>", GenerateOrderId ? Guid.NewGuid().ToString().Replace("-", "") : "");
            sbForm.AppendFormat("<input type='hidden' name='item_name' value='{0}'>", data.ApplyAllTokens(ItemTitle));
            sbForm.AppendFormat("<input type='hidden' name='no_shipping' value='1'>");
            sbForm.AppendFormat("<input type='hidden' name='return' value='{0}'>", returnUrl);
            sbForm.AppendFormat("<input type='hidden' name='rm' value='2'>");
            sbForm.AppendFormat("<input type='hidden' name='notify_url' value='{0}'>", notifyUrl);
            sbForm.AppendFormat("<input type='hidden' name='cancel_return' value='{0}'>", cancelUrl);
            sbForm.AppendFormat("<input type='hidden' name='currency_code' value='{0}'>", data.ApplyAllTokens(CurrencyCode));
            sbForm.AppendFormat("<input type='hidden' name='custom' value='{0}'>", id);

            // write amount absed on recuring
            decimal amount = 0;
            decimal.TryParse(data.ApplyAllTokens(Amount), out amount);

            switch (Recurring) {
                case ePayPalRecurringInterval.None:
                    sbForm.AppendFormat("<input type='hidden' name='amount' value='{0}'>", amount.ToString("0.00"));
                    break;
                default:
                    sbForm.AppendFormat("<input type='hidden' name='src' value='1'>");
                    sbForm.AppendFormat("<input type='hidden' name='a3' value='{0}'>", amount.ToString("0.00"));
                    sbForm.AppendFormat("<input type='hidden' name='p3' value='{0}'>", 1);
                    if (Recurring == ePayPalRecurringInterval.Monthly) {
                        sbForm.AppendFormat("<input type='hidden' name='t3' value='{0}'>", "M");
                    } else {
                        sbForm.AppendFormat("<input type='hidden' name='t3' value='{0}'>", "Y");
                    }
                    break;
            }

            sbForm.Append("</form>");

            sbForm.Append("<script type = 'text/javascript'>");
            sbForm.Append("document.forms['payForm'].submit ();");
            sbForm.Append("</script>");

            sbForm.Append("</body></html>");

            return sbForm.ToString();
        }

        public bool IsValidResponse(HttpRequest responseData)
        {
            if (responseData.Form["payment_status"] != "Completed")
                return false;

            try {
                // Create the request back
                string url = TestMode ? _PayPalUrlSandBox : _PayPalUrl;
                
                HttpWebRequest wr = (HttpWebRequest)HttpWebRequest.Create(url);
                wr.Method = "POST";
                wr.ContentType = "application/x-www-form-urlencoded";
                string strFormValues = Encoding.ASCII.GetString(responseData.BinaryRead(responseData.ContentLength));
                if (strFormValues.IndexOf("cmd=_flow") > 0) {
                    strFormValues = strFormValues.Replace("cmd=_flow", "cmd=_notify-validate");
                } else {
                    strFormValues += "&cmd=_notify-validate";
                }

                wr.ContentLength = strFormValues.Length;
                System.IO.StreamWriter sw = new System.IO.StreamWriter(wr.GetRequestStream(), System.Text.Encoding.ASCII);
                sw.Write(strFormValues);
                sw.Close();


                // send the request, read the response
                string responseText = "";
                using (HttpWebResponse response = (HttpWebResponse)wr.GetResponse()) {
                    System.IO.StreamReader reader = new System.IO.StreamReader(response.GetResponseStream());
                    responseText = reader.ReadToEnd();
                    //HttpContext.Current.Response.Write(responseText);
                }

                return responseText == "VERIFIED";
            } catch (Exception ex) {
                return false;
            }
        }
    }
}
