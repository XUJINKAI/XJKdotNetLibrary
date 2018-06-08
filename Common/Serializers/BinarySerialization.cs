using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace XJKdotNetLibrary.Serializers
{
    public static class BinarySerialization
    {
        public static string ToBase64BinaryString(this object obj)
        {
            var formatter = new BinaryFormatter();
            var stream = new MemoryStream();
            formatter.Serialize(stream, obj);
            stream.Position = 0;
            var bytes = stream.GetBuffer();
            return Convert.ToBase64String(bytes);
        }

        public static T FromBase64BinaryString<T>(string base64string)
        {
            var bytes = Convert.FromBase64String(base64string);
            var formatter = new BinaryFormatter();
            var stream = new MemoryStream(bytes);
            return (T)formatter.Deserialize(stream);
        }
    }
}
