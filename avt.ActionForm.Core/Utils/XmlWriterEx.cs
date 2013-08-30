using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace avt.ActionForm.Utils
{
    public static class XmlWriterEx
    {
        public static void WriteElementWithCData(this XmlWriter writer, string elementName, string cdata)
        {
            writer.WriteStartElement(elementName);
            writer.WriteCData(cdata);
            writer.WriteEndElement(); 
        }
    }
}
