using System;
using System.Collections.Generic;
using System.Text;

namespace XJK.XObject.DefaultProperty
{
    public interface IDefaultProperty
    {
        object GetPropertyDefaultValue(string PropertyName, out ValueDefaultType valueDefaultType);
        object ResetPropertyDefaultValue(string PropertyName, out ValueDefaultType valueDefaultType);
        void ResetAllPropertiesDefaultValue(ValueDefaultType filterType = ValueDefaultType.All);
    }
}
