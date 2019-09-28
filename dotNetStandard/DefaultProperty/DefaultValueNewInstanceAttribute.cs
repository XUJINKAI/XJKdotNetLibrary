using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace XJK.DefaultProperty
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class DefaultValueNewInstanceAttribute : Attribute
    {
    }
}
