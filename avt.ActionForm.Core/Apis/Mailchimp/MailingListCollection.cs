using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace avt.ActionForm.Apis.MailChimp.Net
{
    [DataContract]
    public class MailingListCollection
    {
        public MailingListCollection()
        {
            MailingLists = new List<MailingList>();
        }

        [DataMember(Name = "data")]
        public IList<MailingList> MailingLists { get; set; }

    }
}
