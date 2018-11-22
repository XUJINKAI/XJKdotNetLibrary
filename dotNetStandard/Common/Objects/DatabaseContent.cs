using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace XJK.Objects
{
    public class DatabaseContent : Dictionary<string, object>, IXmlSerializable
    {
        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();

            if (wasEmpty)
                return;

            while (reader.NodeType != XmlNodeType.EndElement)
            {
                reader.MoveToContent();
                string key = reader.GetAttribute("Key");
                string typeName = reader.GetAttribute("ValueType");
                reader.ReadStartElement("Entry");
                string valueXml = reader.ReadOuterXml();
                reader.ReadEndElement();

                Type type = Type.GetType(typeName);
                var valueSerializer = new XmlSerializer(type);
                var value = valueSerializer.Deserialize(new StringReader(valueXml));
                this.Add(key, value);
            }
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            foreach (var key in this.Keys)
            {
                var value = this[key];
                if (value == null) continue;
                var valueType = value.GetType();
                var valueSerializer = new XmlSerializer(valueType);
                var assemblyQualify = valueType.Assembly.GetName().Name != "mscorlib";

                writer.WriteStartElement("Entry");
                writer.WriteAttributeString("Key", key);
                writer.WriteAttributeString("ValueType",
                    assemblyQualify ? valueType.AssemblyQualifiedName : valueType.FullName);
                valueSerializer.Serialize(writer, value);
                writer.WriteEndElement();
            }
        }
    }

}
