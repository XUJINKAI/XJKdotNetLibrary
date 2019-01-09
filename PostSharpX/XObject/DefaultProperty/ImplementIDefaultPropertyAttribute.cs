using PostSharp.Aspects;
using PostSharp.Aspects.Advices;
using PostSharp.Aspects.Dependencies;
using PostSharp.Extensibility;
using PostSharp.Patterns.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XJK.XObject.NotifyProperty;

namespace XJK.XObject.DefaultProperty
{
    [AspectTypeDependency(AspectDependencyAction.Order, AspectDependencyPosition.Before, typeof(AggregatableAttribute))]
    [IntroduceInterface(typeof(IDefaultProperty), AncestorOverrideAction = InterfaceOverrideAction.Ignore, OverrideAction = InterfaceOverrideAction.Ignore)]
    [MulticastAttributeUsage(Inheritance = MulticastInheritance.Strict)]
    [Serializable]
    public class ImplementIDefaultPropertyAttribute : InstanceLevelAspect, IDefaultProperty
    {
        new IDefaultProperty Instance
        {
            get => base.Instance as IDefaultProperty;
        }

        [IntroduceMember(OverrideAction = MemberOverrideAction.OverrideOrIgnore)]
        public object GetPropertyDefaultValue(string PropertyName, out ValueDefaultType defaultValueType)
        {
            return Instance.GetPropertyDefaultValueEx(PropertyName, out defaultValueType);
        }

        [IntroduceMember(OverrideAction = MemberOverrideAction.OverrideOrIgnore)]
        public object ResetPropertyDefaultValue(string PropertyName, out ValueDefaultType defaultValueType)
        {
            var value = Instance.GetPropertyDefaultValueEx(PropertyName, out defaultValueType);
            Instance.GetType().GetProperty(PropertyName).SetValue(this, value);
            return value;
        }

        [IntroduceMember(OverrideAction = MemberOverrideAction.OverrideOrIgnore)]
        public void ResetAllPropertiesDefaultValue(ValueDefaultType overrideType = ValueDefaultType.All)
        {
            Instance.ResetInstaceAllPropertiesByDefaultValue(overrideType);
        }
        
    }
}
