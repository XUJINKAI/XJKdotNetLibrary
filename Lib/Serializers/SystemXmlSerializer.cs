using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace XJK.Serializers
{
    public class SystemXmlSerializer : IStringSerializer
    {
        public string Serialize<T>(T value)
        {
            if (value == null) throw new NullReferenceException();
            var serializer = new XmlSerializer(typeof(T));
            using var writer = new StringWriter();
            serializer.Serialize(writer, value);
            return writer.ToString();
        }

        public T Deserialize<T>(string xml)
        {
            var serializer = new XmlSerializer(typeof(T));
            using var sr = new StringReader(xml);
            using var xmlreader = XmlReader.Create(sr);
            T obj = (T)serializer.Deserialize(xmlreader);
            return obj;
        }
    }
}
