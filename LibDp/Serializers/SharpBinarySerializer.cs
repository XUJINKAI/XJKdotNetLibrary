using System.IO;
using Polenter.Serialization;

namespace XJK.Serializers
{
    public class SharpBinarySerializer : IBytesSerializer
    {
        private readonly SharpSerializer serializer;

        public SharpBinarySerializer()
        {
            serializer = new SharpSerializer(true);
        }

        public byte[] SerializeToBinary<T>(T value)
        {
            using var ms = new MemoryStream();
            serializer.Serialize(value, ms);
            return ms.ToArray();
        }

        public T DeserializeFromBinary<T>(byte[] bytes)
        {
            using var ms = new MemoryStream(bytes);
            var o = serializer.Deserialize(ms);
            return (T)o;
        }
    }
}
