using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace avt.ActionForm.Apis.MailChimp.Net
{
    [DataContract]
    public class MailingList
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "date_created")]
        public string DateCreated { get; set; }

        //IList<InterestGroup> _InterestGroups = null;
        //public IList<InterestGroup> InterestGroups
        //{
        //    get {
        //        if (_InterestGroups == null)
        //            _InterestGroups = Lists.GetInterestGroups(Id);
        //        return _InterestGroups;
        //    }
        //    set { _InterestGroups = value; }
        //}
    }
}
