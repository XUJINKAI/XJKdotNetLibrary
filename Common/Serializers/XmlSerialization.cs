using System;
using System.IO;
using System.Xml.Serialization;
using XJK.SysX;

namespace XJK.Serializers
{
    public static class XmlSerialization
    {
        public static string ToXmlText<T>(this T o) where T : new()
        {
            var serializer = new XmlSerializer(typeof(T));
            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, o);
                return writer.ToString();
            }
        }

        public static T FromXmlText<T>(string xml, bool FailCreateNew = false) where T : new()
        {
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                using (StringReader reader = new StringReader(xml))
                {
                    T obj = (T)serializer.Deserialize(reader);
                    return obj;
                }
            }
            catch (Exception ex)
            {
                if (FailCreateNew)
                {
                    Log.Info("Parse XML Error, return new()");
                    return new T();
                }
                Log.Error("Parse XML Error", ex);
                throw ex;
            }
        }

        public static void WriteXmlFile<T>(T o, string path) where T : new()
        {
            FS.WriteAllText(path, o.ToXmlText());
        }

        public static T ReadXmlFile<T>(string path, bool FailCreateNew = false) where T : new()
        {
            try
            {
                return FromXmlText<T>(FS.ReadAllText(path), FailCreateNew);
            }
            catch (Exception ex)
            {
                if (FailCreateNew)
                {
                    return new T();
                }
                throw ex;
            }
        }
    }
}
