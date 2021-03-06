﻿using System;
using System.ComponentModel;

namespace XJK.DefaultProperty
{
    /// <summary>
    /// same like <see cref="DefaultValueAttribute"/>, define property's default value by method name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class DefaultValueByMethodAttribute : Attribute
    {
        public string MethodName { get; }

        public DefaultValueByMethodAttribute(string methodName)
        {
            MethodName = methodName;
        }
    }
}
