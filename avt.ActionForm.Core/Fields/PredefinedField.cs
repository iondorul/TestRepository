using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using avt.ActionForm.Core.Config;

namespace avt.ActionForm.Core.Fields
{
    public class PredefinedField : FormField
    {

        public static List<FormField> GetFieldsToAddOnInit(string configFolder)
        {
            List<FormField> fields = new List<FormField>();
            
            ItemsFromXmlConfig<FormField> formFieldDefitions = ItemsFromXmlConfig<FormField>.GetConfig(configFolder);
            foreach (FormField field in formFieldDefitions.Items) {
                if (field.AddOnInit) {
                    fields.Add(field);
                }
            }

            return fields;
        }

    }
}
