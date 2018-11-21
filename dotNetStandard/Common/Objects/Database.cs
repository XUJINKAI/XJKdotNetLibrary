using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using XJK.Serializers;

namespace XJK.Objects
{
    public class Database
    {
        public Database() { }

        public Database(string xmlText)
        {
            XmlText = xmlText;
        }

        public readonly DatabaseContent Content = new DatabaseContent();

        public string XmlText
        {
            get => Content.ToXmlText();
            set { ClearContent(); LoadInXmlText(value); }
        }

        public void SaveToFile(string filePath)
        {
            File.WriteAllText(filePath, XmlText);
        }

        public void LoadInXmlText(string xml)
        {
            var db = XmlSerialization.FromXmlText<DatabaseContent>(xml);
            foreach ((string key, object value) in db.Tuples())
            {
                this[key] = value;
            }
        }

        public void ClearContent()
        {
            Content.Clear();
        }

        public object this[string key]
        {
            get => Content[key];
            set=> Content[key] = value;
        }

        public T Get<T>(string key, T defValue)
        {
            if (Content.TryGetValue(key, out object value))
            {
                if (value is T transValue)
                {
                    return transValue;
                }
            }
            return defValue;
        }

        public void Set(string key, object value)
        {
            this[key] = value;
        }


        public static Database FromText(string xmlText)
        {
            return new Database(xmlText);
        }

        public static T FromText<T>(string xmlText) where T : Database, new()
        {
            return new T { XmlText = xmlText };
        }

        public static Database FromFile(string filePath)
        {
            return new Database(File.ReadAllText(filePath));
        }

        public static T FromFile<T>(string filePath) where T : Database, new()
        {
            return new T() { XmlText = File.ReadAllText(filePath) };
        }


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
}
