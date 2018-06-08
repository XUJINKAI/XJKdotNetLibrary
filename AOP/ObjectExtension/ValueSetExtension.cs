using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Windows.Foundation.Collections;
using XJK.MethodWrapper;

namespace XJK.ObjectExtension
{
    public static class ValueSetExtension
    {
        public const string FuncMethodBase64String = "MethodCall";

        public static ValueSet ToValueSet(this MethodCallInfo methodCall)
        {
            var set = new ValueSet
            {
                { FuncMethodBase64String, methodCall.ToBase64BinaryString() },
            };
            return set;
        }

        public static MethodCallInfo ToMethodCall(this ValueSet set)
        {
            string b64 = set[FuncMethodBase64String] as string;
            var method = BinarySerialization.FromBase64BinaryString<MethodCallInfo>(b64);
            return method;
        }
    }

    internal static class BinarySerialization
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
