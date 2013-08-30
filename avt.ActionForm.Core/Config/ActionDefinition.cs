using avt.ActionForm.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace avt.ActionForm.Core.Config
{
    [XmlTypeAttribute(Namespace="")]
    [XmlRoot(Namespace = "", ElementName = "Action", IsNullable = true)]
    public class ActionDefinition
    {
        public ActionDefinition()
        {
            Groups = new List<string>();
            Parameters = new List<ParameterDefinition>();
        }

        public string Id { get; set; }
        public LocalizedContent Title { get; set; }
        public LocalizedContent HelpText { get; set; }
        public string TypeStr { get; set; }
        public bool Final { get; set; }

        [XmlElement(ElementName = "Group")]
        public List<string> Groups { get; set; }

        [XmlElement(ElementName = "Parameter")]
        public List<ParameterDefinition> Parameters { get; set; }

        public StringsDictionary Settings { get; set; }

        public string FooterHtml { get; set; }
        public string JsFunctions { get; set; }

    }
}
