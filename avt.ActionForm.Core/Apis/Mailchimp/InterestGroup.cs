using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace avt.ActionForm.Apis.MailChimp.Net
{
    [DataContract]
    public class InterestGroup
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "groups")]
        public IList<Interest> Interests { get; set; }
    }

    public class Interest
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "subscribers")]
        public int SubscriberCount { get; set; }
    }

}
