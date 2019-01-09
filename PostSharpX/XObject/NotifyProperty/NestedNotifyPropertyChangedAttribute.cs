﻿using PostSharp.Aspects;
using PostSharp.Aspects.Advices;
using PostSharp.Aspects.Dependencies;
using PostSharp.Constraints.Internals;
using PostSharp.Extensibility;
using PostSharp.Patterns.Model;
using PostSharp.Patterns.Recording;
using PostSharp.Reflection;
using PostSharp.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using XJK.XObject.DefaultProperty;

namespace XJK.XObject.NotifyProperty
{
    /// <summary>
    /// 订阅 Property 的 PropertyChanged
    /// </summary>
    [AspectRoleDependency(AspectDependencyAction.Order, AspectDependencyPosition.After, "Tracing")]
    [AspectRoleDependency(AspectDependencyAction.Order, AspectDependencyPosition.After, "Threading")]
    [AspectRoleDependency(AspectDependencyAction.Order, AspectDependencyPosition.After, "Validation")]
    [AspectRoleDependency(AspectDependencyAction.Order, AspectDependencyPosition.Before, "Caching")]
    [AspectTypeDependency(AspectDependencyAction.Order, AspectDependencyPosition.After, typeof(RecordableAttribute))]
    [AspectTypeDependency(AspectDependencyAction.Order, AspectDependencyPosition.After, typeof(AggregatableAttribute))]
    [AspectTypeDependency(AspectDependencyAction.Order, AspectDependencyPosition.After, typeof(NotifyPropertyChangedAttribute))]
    [IntroduceInterface(typeof(INotifyPropertyChanged), OverrideAction = InterfaceOverrideAction.Ignore, AncestorOverrideAction = InterfaceOverrideAction.Ignore)]
    [MulticastAttributeUsage(MulticastTargets.Class, Inheritance = MulticastInheritance.Strict, PersistMetaData = true, AllowMultiple = false, TargetTypeAttributes = MulticastAttributes.UserGenerated)]
    [HasConstraint]
    [PSerializable]
    public class NestedNotifyPropertyChangedAttribute : InstanceLevelAspect, INotifyPropertyChanged
    {
        private Type appliedTo;

        public override void CompileTimeInitialize(Type type, AspectInfo aspectInfo)
        {
            this.appliedTo = type;
        }

#pragma warning disable CS0067
        [IntroduceMember(OverrideAction = MemberOverrideAction.Ignore, LinesOfCodeAvoided = 1)]
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore

        [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
        private Dictionary<object, PropertyChangedEventHandler> HandlerDict = new Dictionary<object, PropertyChangedEventHandler>();


        [ImportMember(nameof(OnPropertyChanged), IsRequired = true, Order = ImportMemberOrder.Default)]
        public Action<PropertyChangedEventArgs> OnPropertyChangedMethod;

        [IntroduceMember(OverrideAction = MemberOverrideAction.Ignore, Visibility = Visibility.Family, LinesOfCodeAvoided = 2)]
        public void OnPropertyChanged(PropertyChangedEventArgs arg) => OnPropertyChangedMethod(arg);


        [OnInstanceConstructedAdvice]
        public void ReadOnlyProperty_NotifyPropertyChanged()
        {
            if (this.appliedTo == this.Instance.GetType())
            {
                var properties = XConfig.Select_NotifyProperties(Instance.GetType(), false);
                foreach (var property in properties)
                {
                    var value = property.GetValue(Instance);
                    if (value is INotifyPropertyChanged notifyPropertyChanged)
                    {
                        if (!HandlerDict.ContainsKey(value))
                        {
                            var handler = MakeNestedPropertyChangedHandler(property.Name);
                            notifyPropertyChanged.PropertyChanged += handler;
                            HandlerDict.Add(value, handler);
                        }
                    }
                }
            }
        }

        private IEnumerable<PropertyInfo> SelectINotifyPropertyChanged(Type type)
        {
            var result = XConfig.Select_NotifyProperties(type, true);
            return result;
        }

        [OnLocationSetValueAdvice, MethodPointcut(nameof(SelectINotifyPropertyChanged))]
        public void OnSetProperty_INotifyPropertyChanged(LocationInterceptionArgs args)
        {
            if (ReferenceEquals(args.Value, args.GetCurrentValue())) return;

            var oldValue = args.GetCurrentValue();
            if (oldValue is INotifyPropertyChanged oldNotifyProperty)
            {
                if(HandlerDict.TryGetValue(oldValue, out var handler))
                {
                    oldNotifyProperty.PropertyChanged -= handler;
                    HandlerDict.Remove(oldValue);
                }
            }

            args.ProceedSetValue();

            if (args.Value is INotifyPropertyChanged notifyProperty)
            {
                var handler = MakeNestedPropertyChangedHandler(args.LocationName);
                notifyProperty.PropertyChanged += handler;
                HandlerDict.Add(args.Value, handler);
            }
        }

        private PropertyChangedEventHandler MakeNestedPropertyChangedHandler(string PropertyName)
        {
            //TODO 继承几次就执行几次
            return (sender, e) =>
            {
                OnPropertyChanged(new NestedPropertyChangedEventArgs(PropertyName, e));
            };
        }
    }
}
