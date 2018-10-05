using System;
using System.Collections.Generic;
using System.Text;

namespace XJK
{
    public static class StringExtension
    {
        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        public static string Join(this string[] strings, string sep = ", ")
        {
            return string.Join(sep, strings);
        }

        public static string Join(this IEnumerable<string> strings, string sep = ", ")
        {
            return string.Join(sep, strings);
        }

        public static string Dup(this char ch, int times)
        {
            return new string(ch, times);
        }

        public static string Dup(this string s, int times)
        {
            while (times > 1)
            {
                s += s;
                times--;
            }
            return s;
        }

        public static byte[] ConvertBytes(this string s)
        {
            return Encoding.ASCII.GetBytes(s);
        }

        public static string ConvertString(this byte[] bytes)
        {
            return Encoding.ASCII.GetString(bytes);
        }
    }
}
