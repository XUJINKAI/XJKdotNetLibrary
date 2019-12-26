using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XJK
{
    public static class StringExtension
    {
        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        public static bool IsNotNullOrEmpty(this string s)
        {
            return !string.IsNullOrEmpty(s);
        }


        public static string JoinToString<T>(this T[] array, Func<T, string> transform_func = null, string sep = ",")
        {
            if (array == null) return "";
            if (transform_func == null) transform_func = x => x.ToString();
            return string.Join(sep, array.Select(transform_func));
        }

        public static string JoinToString<T>(this IEnumerable<T> list, Func<T, string> transform_func = null, string sep = ",")
        {
            if (list == null) return "";
            if (transform_func == null) transform_func = x => x.ToString();
            return string.Join(sep, list.Select(transform_func));
        }

        public static string JoinToString(this string[] strings, string sep = ", ")
        {
            if (strings == null) return "";
            return string.Join(sep, strings);
        }

        public static string JoinToString(this IEnumerable<string> strings, string sep = ", ")
        {
            if (strings == null) return "";
            return string.Join(sep, strings);
        }


        public static string Dup(this char ch, int times)
        {
            return new string(ch, times);
        }

        public static string Dup(this string s, int times)
        {
            var result = new StringBuilder();
            while (times > 0)
            {
                result.Append(s);
                times--;
            }
            return result.ToString();
        }

        public static byte[] ConvertToBytesArray(this string s)
        {
            return Encoding.ASCII.GetBytes(s);
        }

        public static string ConvertToString(this byte[] bytes)
        {
            return Encoding.ASCII.GetString(bytes);
        }
    }
}
