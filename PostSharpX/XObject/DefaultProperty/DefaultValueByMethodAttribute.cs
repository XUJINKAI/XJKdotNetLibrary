using PostSharp.Aspects.Dependencies;
using PostSharp.Patterns.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace XJK.XObject.DefaultProperty
{
    [AspectTypeDependency(AspectDependencyAction.Conflict, typeof(DefaultValueAttribute))]
    [AspectTypeDependency(AspectDependencyAction.Conflict, typeof(DefaultValueNewInstanceAttribute))]
    [AspectTypeDependency(AspectDependencyAction.Conflict, typeof(ReferenceAttribute))]
    [AspectTypeDependency(AspectDependencyAction.Conflict, typeof(ParentAttribute))]
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
