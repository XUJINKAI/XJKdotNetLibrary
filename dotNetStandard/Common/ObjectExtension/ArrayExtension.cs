using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XJK
{
    public static class ArrayExtension
    {
        public static string Join<T>(this T[] array, Func<T, string> select_func, string sep = ",")
        {
            return string.Join(sep, array.Select(select_func));
        }
    }
}
