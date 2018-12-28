using PostSharp.Aspects.Dependencies;
using PostSharp.Patterns.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace XJK.XObject.DefaultProperty
{
    [AspectTypeDependency(AspectDependencyAction.Conflict, typeof(DefaultValueAttribute))]
    [AspectTypeDependency(AspectDependencyAction.Conflict, typeof(DefaultValueByMethodAttribute))]
    [AspectTypeDependency(AspectDependencyAction.Conflict, typeof(ReferenceAttribute))]
    [AspectTypeDependency(AspectDependencyAction.Conflict, typeof(ParentAttribute))]
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DefaultValueNewInstanceAttribute : Attribute
    {
    }
}
