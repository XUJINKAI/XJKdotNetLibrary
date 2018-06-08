using System;
using System.Collections.Generic;
using System.Linq;

namespace XJK
{
    public static class ListExtension
    {
        public static IEnumerable<List<T>> Split<T>(this IEnumerable<T> List, int nSize)
        {
            var bigList = List.ToList();
            for (int i = 0; i < bigList.Count; i += nSize)
            {
                yield return bigList.GetRange(i, Math.Min(nSize, bigList.Count - i));
            }
        }
    }
}
