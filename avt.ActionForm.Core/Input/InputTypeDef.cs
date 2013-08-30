using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using avt.ActionForm.Core.Config;
using System.Web.UI.WebControls;
using System.Xml.Serialization;
using System.Web.Script.Serialization;
using avt.ActionForm.Utils;
using avt.ActionForm.Core.Utils;

namespace avt.ActionForm.Core.Input
{
    [XmlTypeAttribute(Namespace = "")]
    [XmlRoot(Namespace = "", ElementName = "Type", IsNullable = true)]
    public class InputTypeDef // : IConfigItem
    {
        public string Title { get; set; }
        public string Name { get; set; }
        [XmlElement(ElementName = "Category")]
        public List<string> Categories { get; set; }
        public LocalizedContent HelpText { get; set; }
        public string TypeStr { get; set; }
        
        [XmlIgnore]
        public IInputCtrl Handler { get { return ActionFormController.CreateInstance<IInputCtrl>(TypeStr); } }
        
        [XmlElement(ElementName = "Flag")]
        public List<string> Flags { get; set; }
        public string HandlerData { get; set; }
        public StringsDictionary Attributes { get; set; }

        [XmlElement(ElementName = "Parameter")]
        public List<ParameterDefinition> Parameters { get; set; }

        public InputTypeDef()
        {
            Flags = new List<string>();
            Attributes = new StringsDictionary();
        }

        public bool HasFlag(string flag)
        {
            if (string.IsNullOrEmpty(flag))
                return false;

            return Flags.FindIndex(x=>x.Equals(flag, StringComparison.OrdinalIgnoreCase)) != -1;
        }

        //public void LoadFromXml(XmlNode xmlTypeDef)
        //{
        //    Title = xmlTypeDef["Title"].InnerText;
        //    Name = xmlTypeDef["Name"].InnerText;
        //    Category = xmlTypeDef["Category"].InnerText;
        //    HelpText = xmlTypeDef["HelpText"].InnerText;
            
        //    Handler = ActionFormController.CreateInstance<IInputCtrl>(xmlTypeDef["Handler"].InnerText);
        //    if (xmlTypeDef["Handler-Flags"] != null) {
        //        foreach (string flag in xmlTypeDef["Handler-Flags"].InnerText.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries)) {
        //            HandlerFlags.Add(flag.ToLower());
        //        }
        //    }
        //    if (xmlTypeDef["Handler-Data"] != null) {
        //        HandlerData = xmlTypeDef["Handler-Data"].InnerText;
        //    }

        //    HandlerParams = new Dictionary<string, Dictionary<string, string>>();
        //    if (xmlTypeDef["Handler-Params"] != null) {
        //        foreach (XmlNode n in xmlTypeDef["Handler-Params"].ChildNodes) {
        //            if (n.GetType() == typeof(XmlElement)) {
        //                HandlerParams[n.Name] = new Dictionary<string, string>();
        //                foreach (XmlNode nsub in n.ChildNodes) {
        //                    if (nsub.GetType() == typeof(XmlElement)) {
        //                        HandlerParams[n.Name][nsub.Name] = nsub.InnerXml;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        public string GetKey()
        {
            return Name;
        }
    }
}
