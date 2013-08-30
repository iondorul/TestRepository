using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;

namespace avt.ActionForm.Apis.MailChimp.Net
{
    public static class ListsApi
    {
        public static IList<MailingList> GetAll()
        {
            throw new NotImplementedException();
        }

        public static MailingList GetByName(string name, ApiKey apiKey = null)
        {
            return GetByProperty("list_name", name, apiKey);
        }

        public static MailingList GetById(string id, ApiKey apiKey = null)
        {
            return GetByProperty("list_id", id, apiKey);
        }

        public static MailingList GetByProperty(string propertyName, object propertyValue, ApiKey apiKey = null)
        {
            var data = new Dictionary<string, object>();
            data["output"] = "json";
            data["method"] = "lists";
            data["apikey"] = apiKey.ToString();
            data["filters["+propertyName+"]"] = propertyValue;

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(MailingListCollection));

            using (MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(HttpUtils.Send(apiKey.Url, data)))) {
                var lists = serializer.ReadObject(ms) as MailingListCollection;
                //var lists = JsonConvert.DeserializeObject<MailingListCollection>(HttpUtils.Send(apiKey.Url, data));
                if (lists != null && lists.MailingLists.Count > 0)
                    return lists.MailingLists[0];
            }

            return null;
        }

        public static IList<InterestGroup> GetInterestGroups(string listId, ApiKey apiKey = null)
        {
            var data = new Dictionary<string, object>();
            data["output"] = "json";
            data["method"] = "listInterestGroupings";
            data["apikey"] = apiKey.ToString();
            data["id"] = listId;

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(IList<InterestGroup>));

            using (MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(HttpUtils.Send(apiKey.Url, data)))) {
                return serializer.ReadObject(ms) as IList<InterestGroup>;
            }

            //return JsonConvert.DeserializeObject<IList<InterestGroup>>(HttpUtils.Send(apiKey.Url, data));
        }

        public static void Subscribe(string listId, Subscriber subscriber, ApiKey apiKey = null)
        {
            var data = new Dictionary<string, object>();
            data["output"] = "json";
            data["method"] = "listSubscribe";
            data["apikey"] = apiKey.ToString();
            data["id"] = listId;
            data["email_address"] = subscriber.Email;
            data["double_optin"] = "false";

            if (subscriber.UpdateIfExists)
                data["update_existing"] = "true";

            if (subscriber.AppendInterests)
                data["replace_interests"] = "false";

            if (!string.IsNullOrEmpty(subscriber.FirstName))
                data["merge_vars[FNAME]"] = subscriber.FirstName;

            if (!string.IsNullOrEmpty(subscriber.LastName))
                data["merge_vars[LNAME]"] = subscriber.LastName;

            // merge other fields
            foreach (var key in subscriber.Data.Keys)
                data["merge_vars[" + key + "]"] = subscriber.Data[key];

            if (subscriber.InterestGroups.Count > 0) {
                for (var igroup = 0; igroup < subscriber.InterestGroups.Count; igroup++) {
                    data["merge_vars[GROUPINGS][" + igroup + "][name]"] = subscriber.InterestGroups[igroup].GroupName;
                    data["merge_vars[GROUPINGS][" + igroup + "][groups]"] = string.Join(",", subscriber.InterestGroups[igroup].Interests.ToArray());
                }
                
            }

            HttpUtils.Send(apiKey.Url, data);
        }

    }
}
