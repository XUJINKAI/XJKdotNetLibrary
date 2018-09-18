using PostSharp.Aspects;
using PostSharp.Aspects.Advices;
using PostSharp.Aspects.Dependencies;
using PostSharp.Extensibility;
using PostSharp.Patterns.Model;
using PostSharp.Reflection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace XJK.NotifyPropertyChanged
{
    /// <summary>
    /// 订阅Instance属性的PropertyChanged，并向上传递
    /// </summary>
    [Serializable]
    [AspectRoleDependency(AspectDependencyAction.Order, AspectDependencyPosition.After, "Tracing")]
    [AspectRoleDependency(AspectDependencyAction.Order, AspectDependencyPosition.After, "Threading")]
    [AspectRoleDependency(AspectDependencyAction.Order, AspectDependencyPosition.After, "Validation")]
    [AspectRoleDependency(AspectDependencyAction.Order, AspectDependencyPosition.Before, "Caching")]
    [AspectTypeDependency(AspectDependencyAction.Order, AspectDependencyPosition.After, typeof(NotifyPropertyChangedAttribute))]
    [AspectTypeDependency(AspectDependencyAction.Order, AspectDependencyPosition.After, typeof(AggregatableAttribute))]
    [IntroduceInterface(typeof(INotifyPropertyChanged), OverrideAction = InterfaceOverrideAction.Ignore, AncestorOverrideAction = InterfaceOverrideAction.Ignore)]
    [IntroduceInterface(typeof(INotifyPropertyChangedEx), OverrideAction = InterfaceOverrideAction.Ignore, AncestorOverrideAction = InterfaceOverrideAction.Ignore)]
    [MulticastAttributeUsage(MulticastTargets.Class, Inheritance = MulticastInheritance.Strict, PersistMetaData = true, AllowMultiple = false, TargetTypeAttributes = MulticastAttributes.UserGenerated)]
    public class NotifyPropertyChangedEx : InstanceLevelAspect, INotifyPropertyChanged, INotifyPropertyChangedEx
    {
        public bool PropagationNotificationEx { get; set; } = true;

        private readonly Dictionary<object, PropertyChangedEventHandler> Dict = new Dictionary<object, PropertyChangedEventHandler>();
        
        [ImportMember(nameof(OnPropertyChanged), IsRequired = true)]
        public Action<string> OnPropertyChangedMethod;

        [ImportMember(nameof(OnPropertyChangedEx), IsRequired = true)]
        public Action<PropertyChangedEventArgsEx> OnPropertyChangedExMethod;


        [IntroduceMember(OverrideAction = MemberOverrideAction.Ignore)]
        public event PropertyChangedEventHandler PropertyChanged;

        [IntroduceMember(OverrideAction = MemberOverrideAction.Ignore)]
        public event PropertyChangedEventHandlerEx PropertyChangedEx;


        [IntroduceMember(OverrideAction = MemberOverrideAction.Ignore, Visibility = Visibility.Family, LinesOfCodeAvoided = 2)]
        public void OnPropertyChanged(string propertyName)
        {
            OnPropertyChangedMethod?.Invoke(propertyName);
        }

        [IntroduceMember(OverrideAction = MemberOverrideAction.Ignore, Visibility = Visibility.Family, LinesOfCodeAvoided = 2)]
        public void OnPropertyChangedEx(PropertyChangedEventArgsEx e)
        {
            OnPropertyChangedExMethod?.Invoke(e);
        }


        // For this.Object = ...
        // subscribe Object's event
        [OnLocationSetValueAdvice, MethodPointcut(nameof(InstancePropertiesSelector))]
        public void OnInstancePropertySet(LocationInterceptionArgs args)
        {
            if (ReferenceEquals(args.Value, args.GetCurrentValue())) return;

            if (args.GetCurrentValue() is INotifyPropertyChanged current)
            {
                var success = Dict.TryGetValue(current, out PropertyChangedEventHandler rmAction);
                if (success)
                {
                    current.PropertyChanged -= rmAction;
                    Dict.Remove(current);
                }
            }

            args.ProceedSetValue();

            if (args.Value is INotifyPropertyChanged newValue)
            {
                void action(object s, PropertyChangedEventArgs o)
                {
                    var Name = args.LocationName;
                    OnPropertyChangedEx(PropertyChangedEventArgsEx.NewItemPropertyChange(Name, o.PropertyName, newValue));
                    if (PropagationNotificationEx)
                    {
                        OnPropertyChanged(Name);
                    }
                }
                newValue.PropertyChanged += action;
                Dict.Add(newValue, action);
            }
        }

        private IEnumerable<PropertyInfo> InstancePropertiesSelector(Type type)
        {
            const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public;
            return from property in type.GetProperties(bindingFlags)
                   where property.CanWrite// && typeof(INotifyPropertyChanged).IsAssignableFrom(property.PropertyType)
                   select property;
        }
    }
}
