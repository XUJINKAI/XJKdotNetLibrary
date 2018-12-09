using System;
using System.Collections.Generic;
using System.Text;

namespace XJK.DefaultProperty
{
    public interface IDefaultProperty
    {
        object GetDefaultProperty(string PropertyName);
        void SetDefaultProperty(string PropertyName);
        void ResetDefaultProperties();
    }
}
