using System;
using System.Collections.Generic;
using System.Text;

namespace XJK.XSerializers
{
    [AttributeUsage(AttributeTargets.Class)]
    public class IgnoreSerializeTypeAttribute : Attribute
    {
        public Type Type { get; private set; }

        public IgnoreSerializeTypeAttribute(Type ignoreType)
        {
            Type = ignoreType;
        }
    }
}
