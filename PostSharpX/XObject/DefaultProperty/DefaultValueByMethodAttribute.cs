using PostSharp.Aspects.Dependencies;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace XJK.XObject.DefaultProperty
{
    [AspectTypeDependency(AspectDependencyAction.Conflict, typeof(DefaultValueAttribute))]
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DefaultValueByMethodAttribute : Attribute
    {
        public string MethodName { get; private set; }

        public DefaultValueByMethodAttribute(string method)
        {
            MethodName = method;
        }
    }
}
