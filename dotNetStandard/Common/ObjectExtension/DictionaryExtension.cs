using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XJK
{
    public static class DictionaryStringStringExtension
    {
        public static IEnumerable<(TKey, TValue)> Tuples<TKey, TValue>(this IDictionary<TKey, TValue> dict)
        {
            foreach (KeyValuePair<TKey, TValue> kvp in dict)
                yield return (kvp.Key, kvp.Value);
        }

        public static string ToFormatTableString(this Dictionary<string, string> dict)
        {
            string str;
            str = dict.Aggregate("", (sum, next) => $"{sum}{C.LF}{next.Key,-20} = {next.Value}");
            if (str.Length > 1) str = str.Substring(1);
            return str;
        }
    }
}
