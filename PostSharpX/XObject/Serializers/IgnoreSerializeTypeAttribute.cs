﻿using System;
using System.Collections.Generic;
using System.Text;

namespace XJK.XObject.Serializers
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class IgnoreSerializeTypeAttribute : Attribute
    {
        public Type Type { get; private set; }

        public IgnoreSerializeTypeAttribute(Type ignoreType)
        {
            Type = ignoreType;
        }
    }
}