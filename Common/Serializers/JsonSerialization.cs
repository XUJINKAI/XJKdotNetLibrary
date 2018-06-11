using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace XJK.Serializers
{
    public static class JsonSerialization
    {
        public static string ToJsonString(this object obj)
        {
            using (var memoryStream = new MemoryStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
                serializer.WriteObject(memoryStream, obj);
                string result = Encoding.UTF8.GetString(memoryStream.ToArray());
                return result;
            }
        }

        public static T FromJsonString<T>(string json) where T : new()
        {
            using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                object obj = serializer.ReadObject(memoryStream);
                return (T)obj;
            }
        }
    }
}
