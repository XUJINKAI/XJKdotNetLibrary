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

namespace PostSharpExtension
{
    [Serializable]
    [AspectRoleDependency(AspectDependencyAction.Order, AspectDependencyPosition.After, "Tracing")]
    [AspectRoleDependency(AspectDependencyAction.Order, AspectDependencyPosition.After, "Threading")]
    [AspectRoleDependency(AspectDependencyAction.Order, AspectDependencyPosition.After, "Validation")]
    [AspectRoleDependency(AspectDependencyAction.Order, AspectDependencyPosition.Before, "Caching")]
    [AspectTypeDependency(AspectDependencyAction.Order, AspectDependencyPosition.After, typeof(NotifyPropertyChangedAttribute))]
    [AspectTypeDependency(AspectDependencyAction.Order, AspectDependencyPosition.After, typeof(AggregatableAttribute))]
    [IntroduceInterface(typeof(INotifyPropertyChanged), OverrideAction = InterfaceOverrideAction.Ignore, AncestorOverrideAction = InterfaceOverrideAction.Ignore)]
    [IntroduceInterface(typeof(INotifyNestedPropertyChanged), OverrideAction = InterfaceOverrideAction.Ignore, AncestorOverrideAction = InterfaceOverrideAction.Ignore)]
    [MulticastAttributeUsage(MulticastTargets.Class, Inheritance = MulticastInheritance.Strict)]
    public class NotifyNestedPropertyChanged : InstanceLevelAspect, INotifyPropertyChanged, INotifyNestedPropertyChanged
    {
        public bool Propagation { get; set; }

        private readonly Dictionary<object, PropertyChangedEventHandler> Dict = new Dictionary<object, PropertyChangedEventHandler>();

        public NotifyNestedPropertyChanged(bool propagation = false)
        {
            Propagation = propagation;
        }

        [ImportMember("OnPropertyChanged", IsRequired = true)]
        public Action<string> OnPropertyChangedMethod;

        [ImportMember("OnNestedPropertyChanged", IsRequired = true)]
        public Action<string, string> OnNestedPropertyChangedMethod;


        [IntroduceMember(OverrideAction = MemberOverrideAction.Ignore)]
        public event PropertyChangedEventHandler PropertyChanged;

        [IntroduceMember(OverrideAction = MemberOverrideAction.Ignore)]
        public event NestedPropertyChangedEventHandler NestedPropertyChanged;


        [IntroduceMember(OverrideAction = MemberOverrideAction.Ignore, Visibility = Visibility.Family, LinesOfCodeAvoided = 2)]
        public void OnPropertyChanged(string propertyName)
        {
            OnPropertyChangedMethod?.Invoke(propertyName);
        }

        [IntroduceMember(OverrideAction = MemberOverrideAction.Ignore, Visibility = Visibility.Family, LinesOfCodeAvoided = 2)]
        public void OnNestedPropertyChanged(string propertyName, string childPropertyName)
        {
            OnNestedPropertyChangedMethod?.Invoke(propertyName, childPropertyName);
        }


        [OnLocationSetValueAdvice, MethodPointcut(nameof(SelectProperties))]
        public void OnPropertySet(LocationInterceptionArgs args)
        {
            if (args.Value == args.GetCurrentValue()) return;

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
                    OnNestedPropertyChanged(Name, o.PropertyName);
                    if (Propagation)
                    {
                        OnPropertyChanged(Name);
                    }
                }
                newValue.PropertyChanged += action;
                Dict.Add(newValue, action);
            }
        }

        private IEnumerable<PropertyInfo> SelectProperties(Type type)
        {
            const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public;
            return from property in type.GetProperties(bindingFlags)
                   where property.CanWrite && typeof(INotifyPropertyChanged).IsAssignableFrom(property.PropertyType)
                   select property;
        }
    }
}
