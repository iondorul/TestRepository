using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace avt.ActionForm.Utils
{
    public static class XmlDocumentEx
    {
        public static string ToXmlString(this XmlDocument doc)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            XmlTextWriter xtw = null;
            try {
                xtw = new XmlTextWriter(sw);
                xtw.Formatting = Formatting.Indented;
                doc.WriteTo(xtw);
            } finally {
                if (xtw != null)
                    xtw.Close();
            }
            return sb.ToString();
        }
    }
}
