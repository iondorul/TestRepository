using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace avt.ActionForm.Apis.MailChimp.Net
{
    public class Subscriber
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool UpdateIfExists { get; set; }
        public bool AppendInterests { get; set; }

        public IDictionary<string, object> Data { get; set; }
        public IList<SubcriberInterestGroup> InterestGroups { get; set; }

        public Subscriber(string email)
        {
            InterestGroups = new List<SubcriberInterestGroup>();
            Email = email;
            UpdateIfExists = true;
            AppendInterests = true;
            Data = new Dictionary<string, object>();
        }

        public Subscriber AddInterest(string groupName, string interestName)
        {
            var group = InterestGroups.FirstOrDefault(x => x.GroupName.Equals(groupName, StringComparison.OrdinalIgnoreCase));
            if (group == null) {
                group = new SubcriberInterestGroup() { GroupName = groupName };
                InterestGroups.Add(group);
            } else {
            }
            group.Interests.Add(interestName);
            return this;
        }

        public Subscriber SetField(string name, object value)
        {
            Data[name] = value;
            return this;
        }
    }

    public class SubcriberInterestGroup
    {
        public SubcriberInterestGroup()
        {
            Interests = new List<string>();
        }

        public string GroupName { get; set; }
        public IList<string> Interests { get; set; }
    }
}
