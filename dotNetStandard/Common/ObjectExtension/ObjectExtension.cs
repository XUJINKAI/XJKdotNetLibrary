using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Text;

namespace XJK
{
    public static class ObjectExtension
    {
        public static T CastTo<T>(this object obj)
        {
            return (T)obj;
        }
    }
}