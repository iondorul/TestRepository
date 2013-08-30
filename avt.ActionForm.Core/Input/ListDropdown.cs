using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using avt.ActionForm.Core.Fields;
using System.Web.UI;
using System.Web;
using DotNetNuke.Common.Lists;
using System.Collections.Specialized;
using avt.ActionForm.Utils;

namespace avt.ActionForm.Core.Input
{
    public class ListDropdown : BaseControl
    {

        public override IList<ListItem> GetOptions(InputTypeDef typeDef, FormField field)
        {
            ListController cc = new ListController();
            ListEntryInfoCollection ec;

            var listKey = typeDef.HandlerData;
            var iSubList = listKey.IndexOf('-');
            if (iSubList == -1) {
                ec = cc.GetListEntryInfoCollection(listKey);
            } else {
                ec = cc.GetListEntryInfoCollection(listKey.Substring(iSubList + 1), listKey.Substring(0, iSubList));
            }

            List<ListItem> items = new List<ListItem>();
            if (typeDef.HasFlag("first-empty")) {
                items.Add(new ListItem(""));
            }

            foreach (ListEntryInfo li in ec) {
                if (typeDef.HasFlag("name")) {
                    items.Add(new ListItem(li.Text));
                } else {
                    items.Add(new ListItem(li.Text, li.Value));
                }
            }
            return items;
        }

    }
}
