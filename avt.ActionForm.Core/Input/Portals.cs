using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using avt.ActionForm.Core.Fields;
using System.Web.UI;
using System.Web;
using DotNetNuke.Entities.Portals;
using System.Collections.Specialized;
using avt.ActionForm.Utils;

namespace avt.ActionForm.Core.Input
{
    public class Portals : BaseControl
    {
        ListItem ParseListItem(string val)
        {
            ListItem li = new ListItem(val.Trim(' ', '\r'));
            if (li.Text.IndexOf('|') != -1) {
                li.Value = li.Text.Substring(li.Text.IndexOf('|') + 1).Trim();
                li.Text = li.Text.Substring(0, li.Text.IndexOf('|')).Trim();
            }
            return li;
        }

        public override IList<ListItem> GetOptions(InputTypeDef typeDef, FormField field)
        {
            var items = new List<ListItem>();
            PortalController pc = new PortalController();
            foreach (PortalInfo portal in pc.GetPortals().Cast<PortalInfo>().OrderBy(x => x.PortalID)) {
                if (typeDef.HasFlag("except0") && portal.PortalID == 0)
                    continue;
                items.Add(new ListItem(portal.PortalName, portal.PortalID.ToString()));
            }
            return items;
        }
    }
}

