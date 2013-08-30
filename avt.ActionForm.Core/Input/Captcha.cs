using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using avt.ActionForm.Core.Fields;
using System.Web.UI;
using System.Web;
using avt.ActionForm.Core.Validation;
using avt.ActionForm.Core.Config;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using DotNetNuke.Entities.Portals;
using System.Web.Security;
using DotNetNuke.Services.Exceptions;
using avt.ActionForm.Utils;

namespace avt.ActionForm.Core.Input
{
    public class Captcha : BaseControl
    {

        public override NameValueCollection GetData(InputTypeDef typeDef, FormField field)
        {
            var captcha = Encrypt(EncodeTicket(GetNextCaptcha()), DateTime.Now.AddSeconds(12000));
            var data = new NameValueCollection();
            data["ImageUrl"] = GetUrl(captcha);
            data["CaptchaEncrypted"] = captcha;
            return data;
        }

        private string GetUrl(string captcha)
        {

            //var url = string.Format("{0}/ImageChallenge.captcha.aspx?captcha={1}&alias={2}",
            //    HttpRuntime.AppDomainAppVirtualPath.TrimEnd('/'),
            //    PortalController.GetCurrentPortalSettings().PortalAlias.HTTPAlias);

            var url = string.Format("{0}?captcha={1}&alias={2}",
                (HttpContext.Current.CurrentHandler as Page).ResolveUrl("ImageChallenge.captcha.aspx"),
                captcha,
                PortalController.GetCurrentPortalSettings().PortalAlias.HTTPAlias);

            return url;
        }

        private static string _Separator = ":-:";
        private string EncodeTicket(string captcha)
        {
            var sb = new StringBuilder();

            sb.Append("274");
            sb.Append(_Separator + "60");
            sb.Append(_Separator + captcha);
            sb.Append(_Separator + "");

            return sb.ToString();
        }

        private static string Encrypt(string content, DateTime expiration)
        {
            var ticket = new FormsAuthenticationTicket(1, HttpContext.Current.Request.UserHostAddress, DateTime.Now, expiration, false, content);
            return FormsAuthentication.Encrypt(ticket);
        }

        private static string Decrypt(string encryptedContent)
        {
            string decryptedText = string.Empty;
            try {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(encryptedContent);
                if ((!ticket.Expired)) {
                    decryptedText = ticket.UserData;
                }
            } catch (ArgumentException exc) {
                Exceptions.LogException(exc);
            }
            return decryptedText;
        }

        private int _CaptchaLength = 6;
        private string _CaptchaChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ23456789";

        protected virtual string GetNextCaptcha()
        {
            var sb = new StringBuilder();
            var rand = new Random();
            int n;
            var intMaxLength = _CaptchaChars.Length;

            for (n = 0; n <= _CaptchaLength - 1; n++) {
                sb.Append(_CaptchaChars.Substring(rand.Next(intMaxLength), 1));
            }
            return sb.ToString();
        }



        public override bool IsValid(Form.FormData formData, Form.FieldData field, out string[] errMessages)
        {
            // find encrypted captcha
            var captcha = "";
            foreach (string key in HttpContext.Current.Request.Params.Keys) {
                if (key == field.Field.TitleCompacted + "captchaenc") {
                    captcha = Decrypt(HttpContext.Current.Request.Params[key]);
                    string[] parts = Regex.Split(captcha, _Separator);
                    captcha = parts[2];
                    break;
                }
            }

            var isValid = field.Value.Equals(captcha, StringComparison.OrdinalIgnoreCase);
            errMessages = null;
            if (!isValid)
                errMessages = new string[] { "Invalid security code" };
            
            return isValid;
        }
    }
}
