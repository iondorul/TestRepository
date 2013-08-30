using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using avt.ActionForm.Core.Fields;
using System.Web.UI;
using System.Web;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using Microsoft.ApplicationBlocks.Data;
using avt.ActionForm.Utils;
using avt.ActionForm.Core.Form;

namespace avt.ActionForm.Core.Input
{
    public class MultipleChoice : BaseControl
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
            List<ListItem> items = new List<ListItem>();
            // read data from field
            if (field.Parameters.ContainsKey("Values")) {
                var values = field.Parameters["Values"].ToString();
                // is it SQL?
                if (Regex.IsMatch(values, "select.+from", RegexOptions.IgnoreCase | RegexOptions.Singleline)) {
                    using (var dr = SqlHelper.ExecuteReader(DotNetNuke.Common.Utilities.Config.GetConnectionString(), System.Data.CommandType.Text, values)) {
                        while (dr.Read()) {
                            if (dr.FieldCount > 1) {
                                items.Add(new ListItem(dr[0].ToString(), dr[1].ToString()));
                            } else if (dr.FieldCount == 1) {
                                items.Add(new ListItem(dr[0].ToString()));
                            }
                        }
                    }
                } else {
                    foreach (string val in TokenUtils.Tokenize(values).Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)) {
                        items.Add(ParseListItem(val));
                    }
                }
            } else if (typeDef.HandlerData != null) {
                foreach (string val in typeDef.HandlerData.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries)) {
                    items.Add(new ListItem(TokenUtils.Tokenize(val)));
                }
            }

            return items;
        }

    }
}
