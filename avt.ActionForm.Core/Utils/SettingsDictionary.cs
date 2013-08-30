using avt.ActionForm.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace avt.ActionForm.Core.Utils
{
    public class SettingsDictionary : Dictionary<string, object>, IXmlSerializable
    {
        public SettingsDictionary()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        public SettingsDictionary(Dictionary<string,object> data)
            : base(data, StringComparer.OrdinalIgnoreCase)
        {
        }


        public T GetAs<T>(string name, T defaultValue = default(T))
        {
            if (!ContainsKey(name))
                return defaultValue;

            if (typeof(T) == this[name].GetType())
                return (T)this[name];

            return defaultValue;
        }

        public T GetValue<T>(string name, T defaultValue = default(T))
        {
            if (!ContainsKey(name))
                return defaultValue;

            if (typeof(T) == this[name].GetType())
                return (T) this[name];

            return ConvertUtils.Cast<T>(this[name].ToString(), defaultValue);
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
                if (string.IsNullOrEmpty(key))
                    continue;
                writer.WriteStartElement(key);
                writer.WriteValue(this[key]);
                writer.WriteEndElement();
            }
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }
    }
}
