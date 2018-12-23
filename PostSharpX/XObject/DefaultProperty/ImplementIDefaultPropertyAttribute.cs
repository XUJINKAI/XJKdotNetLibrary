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
    [MulticastAttributeUsage(Inheritance = MulticastInheritance.Multicast)]
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
            var properties = XConfig.Select_ResetAllDefaultProperties(Instance.GetType());
            foreach (var property in properties)
            {
                if (property.CanWrite)
                {
                    var value = Instance.GetPropertyDefaultValueEx(property, out var type);
                    if (overrideType.HasFlag(type))
                    {
                        property.SetValue(Instance, value);
                    }
                }
                else if (property.CanRead && property.GetValue(Instance) is IDefaultProperty propertyInstance)
                {
                    propertyInstance.ResetAllPropertiesDefaultValue(overrideType);
                }
            }
        }

        [OnInstanceConstructedAdvice]
        public void InitializeProperties()
        {
            Instance.ResetAllPropertiesDefaultValue();
        }

    }
}
