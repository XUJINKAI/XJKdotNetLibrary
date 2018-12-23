using System;
using System.Collections.Generic;
using System.Text;

namespace XJK.XObject.DefaultProperty
{
    [Flags]
    public enum ValueDefaultType : int
    {
        None = 0,
        NoAttribute = 1,
        ValueAttribute = 2,
        MethodAttribute = 4,
        All = ~0,
    }
}
