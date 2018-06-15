using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Windows.Foundation.Collections;
using XJK.Serializers;

namespace XJK.AOP
{
    public static class ValueSetExtension
    {
        public const string FuncMethodBase64Key = "MethodCall";

        public static ValueSet ToValueSet(this MethodCallInfo methodCall)
        {
            var set = new ValueSet
            {
                { FuncMethodBase64Key, methodCall.ToBase64BinaryString() },
            };
            return set;
        }

        public static MethodCallInfo ToMethodCall(this ValueSet set)
        {
            if (!set.ContainsKey(FuncMethodBase64Key))
            {
                Debugger.Break();
            }
            string b64 = set[FuncMethodBase64Key] as string;
            var method = BinarySerialization.FromBase64BinaryString<MethodCallInfo>(b64);
            return method;
        }
    }
}
