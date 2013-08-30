using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace avt.ActionForm.Core.Form.Result
{
    public class FormResultFactory
    {
        public static IFormEventResult GetAction(string actionStr)
        {
            var xml = new XmlDocument();
            xml.LoadXml(actionStr);

            Type dataType = Type.GetType(xml.DocumentElement["type"].InnerText);
            using (TextReader reader = new StringReader(xml.DocumentElement["data"].InnerXml)) {
                var serializer = new XmlSerializer(dataType);
                return (IFormEventResult) serializer.Deserialize(reader);
            }
        }

        public static string Serialize(IFormEventResult action)
        {
            var emptyNamepsaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            var serializer = new XmlSerializer(action.GetType());
            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;

            using (var stream = new StringWriter())
            using (var writer = XmlWriter.Create(stream, settings)) {
                serializer.Serialize(writer, action, emptyNamepsaces);
                return stream.ToString();
            }
        }
    }
}
