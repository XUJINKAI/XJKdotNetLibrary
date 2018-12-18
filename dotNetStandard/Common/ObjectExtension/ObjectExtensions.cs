using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XJK
{
    public static class ObjectExtensions
    {
        public static T CastTo<T>(this object obj) where T : class
        {
            return (T)obj;
        }

        public static T TryCastTo<T>(this object obj) where T : class
        {
            if (obj is T t) return t;
            else return null;
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (action == null) throw new ArgumentNullException("action");

            foreach (T item in source)
            {
                action(item);
            }
        }

        public static IEnumerable<List<T>> Split<T>(this IEnumerable<T> List, int nSize)
        {
            var bigList = List.ToList();
            for (int i = 0; i < bigList.Count; i += nSize)
            {
                yield return bigList.GetRange(i, Math.Min(nSize, bigList.Count - i));
            }
        }


        public static IEnumerable<(TKey, TValue)> Tuples<TKey, TValue>(this IDictionary<TKey, TValue> dict)
        {
            foreach (KeyValuePair<TKey, TValue> kvp in dict)
                yield return (kvp.Key, kvp.Value);
        }

        public static string ToFormatTableString(this Dictionary<string, string> dict)
        {
            string str;
            str = dict.Aggregate("", (sum, next) => $"{sum}{Environment.NewLine}{next.Key,-20} = {next.Value}");
            if (str.Length > 1) str = str.Substring(1);
            return str;
        }
    }
}
