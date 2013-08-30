using avt.ActionForm.Apis.MailChimp.Net;
using avt.ActionForm.Core.Form;
using avt.ActionForm.Core.Form.Result;
using avt.ActionForm.Core.Utils;
using avt.ActionForm.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace avt.ActionForm.Core.Actions
{
    public class SubscribeToMailchimp : IAction
    {
        public string ApiKey { get; set; }
        public string ListName { get; set; }

        public ActionInfo ActionInfo { get; set; }

        public void Init(ActionInfo actionInfo, SettingsDictionary settings)
        {
            ApiKey = settings.GetValue("ApiKey", "");
            ListName = settings.GetValue("ListName", "");
            ActionInfo = actionInfo;
        }

        public IFormEventResult Execute(ActionFormSettings settings, FormData data, eActionContext context)
        {
            try {
                var apiKey = new ApiKey(ApiKey);
                var list = ListsApi.GetByName(ListName, apiKey);
                var subscriber = new Subscriber(data.GetValueByFieldType("open-email"));
                if (data["FirstName"] != null)
                    subscriber.FirstName = data["FirstName"];
                if (data["LastName"] != null)
                    subscriber.LastName = data["LastName"];
                ListsApi.Subscribe(list.Id, subscriber, apiKey);
                //DnnSharpApp.Instance.Logger.Info("Done pushing user {0} to MailChimp.", user.Email);
            } catch (Exception ex) {
                //DnnSharpApp.Instance.Logger.Error(ExceptionUtils.FlattenException(ex));
                throw ex;
            }

            return null;
        }
    }


}
