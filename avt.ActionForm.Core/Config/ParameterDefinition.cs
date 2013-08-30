using avt.ActionForm.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace avt.ActionForm.Core.Config
{
    public class ParameterDefinition
    {
        public string Id { get; set; }
        public LocalizedContent Title { get; set; }
        public LocalizedContent HelpText { get; set; }
        public string Type { get; set; }
        public LocalizedContent DefaultValue { get; set; }
        public StringsDictionary Settings { get; set; }
    }
}
