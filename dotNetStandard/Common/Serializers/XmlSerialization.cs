using System;
using System.IO;
using System.Xml.Serialization;

namespace XJK
{
    public static class XmlSerialization
    {
        public static string ToXmlText<T>(T o)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, o);
                return writer.ToString();
            }
        }

        public static T FromXmlText<T>(string xml)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (StringReader reader = new StringReader(xml))
            {
                T obj = (T)serializer.Deserialize(reader);
                return obj;
            }
        }
    }
}
