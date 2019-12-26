using System;
using System.Collections.Generic;
using System.Linq;

namespace XJK
{
    public static class ObjectExtensions
    {
        public static T CastTo<T>(this object obj) where T : class
        {
            return obj as T;
        }

        public static T RandomSelect<T>(this IEnumerable<T> list)
        {
            var count = list.Count();
            if (count == 0) return default;
            if (count == 1) return list.First();
            var idx = RandomGenerator.RandomInt(count);
            var enumerator = list.GetEnumerator();
            enumerator.MoveNext();
            for (int i = 0; i < idx - 1; i++)
            {
                enumerator.MoveNext();
            }
            return enumerator.Current;
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
