using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Xml.Serialization;

namespace avt.ActionForm.Utils
{
    public class StringsDictionary : Dictionary<string, string>, IXmlSerializable
    {
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            if (reader.IsEmptyElement)
                return;

            reader.ReadStartElement();

            while (reader.NodeType != System.Xml.XmlNodeType.EndElement) {

                string key = reader.Name;
                string val = HttpUtility.HtmlDecode(reader.ReadInnerXml());

                this.Add(key.Trim(), val.Trim());

                //reader.ReadEndElement();
            }

            reader.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            foreach (var key in this.Keys) {
                writer.WriteStartElement("item");

                writer.WriteAttributeString("key", key);
                writer.WriteValue(this[key]);
                writer.WriteEndElement();
            }
        }
    }
}