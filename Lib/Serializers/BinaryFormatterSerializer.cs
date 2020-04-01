using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace XJK.Serializers
{
    public class BinaryFormatterSerializer : IBytesSerializer
    {
        public byte[] SerializeToBinary<T>(T value)
        {
            if (value == null) throw new NullReferenceException();
            var bf = new BinaryFormatter();
            using var ms = new MemoryStream();
            bf.Serialize(ms, value);
            return ms.ToArray();
        }

        public T DeserializeFromBinary<T>(byte[] bytes)
        {
            if (bytes == null) throw new NullReferenceException();
            using var ms = new MemoryStream();
            var bf = new BinaryFormatter();
            ms.Write(bytes, 0, bytes.Length);
            ms.Seek(0, SeekOrigin.Begin);
            return (T)bf.Deserialize(ms);
        }
    }
}
