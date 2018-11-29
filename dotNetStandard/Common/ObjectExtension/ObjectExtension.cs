using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Text;

namespace XJK
{
    public static class ObjectExtension
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
    }
}