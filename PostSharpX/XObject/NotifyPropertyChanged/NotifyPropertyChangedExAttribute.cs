using PostSharp.Aspects;
using PostSharp.Aspects.Advices;
using PostSharp.Aspects.Dependencies;
using PostSharp.Constraints.Internals;
using PostSharp.Extensibility;
using PostSharp.Patterns.Model;
using PostSharp.Reflection;
using PostSharp.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace XJK.NotifyPropertyChanged
{
    /// <summary>
    /// 订阅 Property 的 PropertyChanged, CollectionChanged, 并触发 OnPropertyChangedEx
    /// </summary>
    [AspectRoleDependency(AspectDependencyAction.Order, AspectDependencyPosition.After, "Tracing")]
    [AspectRoleDependency(AspectDependencyAction.Order, AspectDependencyPosition.After, "Threading")]
    [AspectRoleDependency(AspectDependencyAction.Order, AspectDependencyPosition.After, "Validation")]
    [AspectRoleDependency(AspectDependencyAction.Order, AspectDependencyPosition.Before, "Caching")]
    [AspectTypeDependency(AspectDependencyAction.Order, AspectDependencyPosition.After, typeof(NotifyPropertyChangedAttribute))]
    [AspectTypeDependency(AspectDependencyAction.Order, AspectDependencyPosition.After, typeof(AggregatableAttribute))]
    [HasConstraint]
    [IntroduceInterface(typeof(INotifyPropertyChanged), OverrideAction = InterfaceOverrideAction.Ignore, AncestorOverrideAction = InterfaceOverrideAction.Ignore)]
    [IntroduceInterface(typeof(INotifyPropertyChangedEx), OverrideAction = InterfaceOverrideAction.Ignore, AncestorOverrideAction = InterfaceOverrideAction.Ignore)]
    [MulticastAttributeUsage(MulticastTargets.Class, Inheritance = MulticastInheritance.Strict, PersistMetaData = true, AllowMultiple = false, TargetTypeAttributes = MulticastAttributes.UserGenerated)]
    [Serializer(typeof(Serializer))]
    public class NotifyPropertyChangedExAttribute : InstanceLevelAspect, INotifyPropertyChanged, INotifyPropertyChangedEx
    {
        public bool PropagationEvent { get; set; } = false;
        /// <summary>
        /// Also Notify when property = property;
        /// </summary>
        public bool NotifySameReference { get; set; } = false;

        private readonly Dictionary<INotifyPropertyChanged, PropertyChangedEventHandler> PropertyChangedDict = new Dictionary<INotifyPropertyChanged, PropertyChangedEventHandler>();
        private readonly Dictionary<INotifyCollectionChanged, NotifyCollectionChangedEventHandler> CollectionChangedDict = new Dictionary<INotifyCollectionChanged, NotifyCollectionChangedEventHandler>();

        [ImportMember(nameof(OnPropertyChanged), IsRequired = true)]
        public Action<string> OnPropertyChangedMethod;

        [ImportMember(nameof(OnPropertyChangedEx), IsRequired = true)]
        public Action<PropertyChangedEventArgsEx> OnPropertyChangedExMethod;


#pragma warning disable CS0067
        [IntroduceMember(OverrideAction = MemberOverrideAction.Ignore)]
        public event PropertyChangedEventHandler PropertyChanged;

        [IntroduceMember(OverrideAction = MemberOverrideAction.Ignore)]
        public event PropertyChangedEventHandlerEx PropertyChangedEx;
#pragma warning restore

        [IntroduceMember(OverrideAction = MemberOverrideAction.Ignore, Visibility = Visibility.Family, LinesOfCodeAvoided = 2)]
        public void OnPropertyChanged(string propertyName)
        {
            OnPropertyChangedMethod?.Invoke(propertyName);
        }
        
        [IntroduceMember(OverrideAction = MemberOverrideAction.Ignore, Visibility = Visibility.Family, LinesOfCodeAvoided = 2)]
        public void OnPropertyChangedEx(PropertyChangedEventArgsEx e)
        {
            if (PropagationEvent)
            {
                OnPropertyChanged(e.PropertyName);
            }
            OnPropertyChangedExMethod?.Invoke(e);
        }
        

        // For this.Object = ...
        // subscribe Object's event
        [OnLocationSetValueAdvice, MethodPointcut(nameof(SelectINotifyPropertyChanged))]
        public void OnSetProperty_INotifyPropertyChanged(LocationInterceptionArgs args)
        {
            if (!NotifySameReference && ReferenceEquals(args.Value, args.GetCurrentValue())) return;

            var oldValue = args.GetCurrentValue();
            if (oldValue != null)
            {
                if(oldValue is INotifyPropertyChanged oldNotifyProperty)
                {
                    UninstallPropertyNotification(oldNotifyProperty);
                }
                if (oldValue is INotifyCollectionChanged oldNotifyCollection)
                {
                    UninstallCollectionNotification(oldNotifyCollection);
                }
                if (oldValue is IList oldNotifyList && oldNotifyList != null)
                {
                    UninstallCollectionItemNotification(oldNotifyList);
                }
            }

            args.ProceedSetValue();

            if (args.Value is INotifyCollectionChanged notifyCollection)
            {
                InstallCollectionNotification(args.LocationName, notifyCollection);
            }
            else if (args.Value is INotifyPropertyChanged notifyProperty)
            {
                InstallPropertyNotification(args.LocationName, notifyProperty);
            }
            if (args.Value is IList notifyList && notifyList != null)
            {
                InstallCollectionItemNotification(args.LocationName, notifyList);
            }
        }

        // Property
        private void InstallPropertyNotification(string PropertyName, INotifyPropertyChanged inotify)
        {
            void Inotify_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                OnPropertyChangedEx(PropertyChangedEventArgsEx.NewItemPropertyChange(PropertyName, sender, e.PropertyName));
            }
            inotify.PropertyChanged += Inotify_PropertyChanged;
            PropertyChangedDict.Add(inotify, Inotify_PropertyChanged);
        }

        private void UninstallPropertyNotification(INotifyPropertyChanged inotify)
        {
            var success = PropertyChangedDict.TryGetValue(inotify, out PropertyChangedEventHandler handler);
            if (success)
            {
                inotify.PropertyChanged -= handler;
                PropertyChangedDict.Remove(inotify);
            }
        }

        // Collection
        private void InstallCollectionNotification(string PropertyName, INotifyCollectionChanged collection)
        {
            void Collection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                OnPropertyChangedEx(PropertyChangedEventArgsEx.NewCollectionChange(PropertyName, e.NewItems, e.OldItems));
                InstallCollectionItemNotification(PropertyName, e.NewItems);
                UninstallCollectionItemNotification(e.OldItems);
            }
            collection.CollectionChanged += Collection_CollectionChanged;
            CollectionChangedDict.Add(collection, Collection_CollectionChanged);
        }

        private void UninstallCollectionNotification(INotifyCollectionChanged collection)
        {
            var success = CollectionChangedDict.TryGetValue(collection, out NotifyCollectionChangedEventHandler handler);
            if (success)
            {
                collection.CollectionChanged -= handler;
                CollectionChangedDict.Remove(collection);
            }
        }

        // IList's Items
        private void InstallCollectionItemNotification(string PropertyName, IList items)
        {
            if (items == null) return;
            void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                OnPropertyChangedEx(PropertyChangedEventArgsEx.NewCollectionItemPropertyChange(PropertyName, sender, e.PropertyName));
            }
            foreach (var item in items)
            {
                if (item is INotifyPropertyChanged itemProperty)
                {
                    itemProperty.PropertyChanged += item_PropertyChanged;
                    PropertyChangedDict.Add(itemProperty, item_PropertyChanged);
                }
            }
        }

        private void UninstallCollectionItemNotification(IList items)
        {
            if (items == null) return;
            bool success;
            PropertyChangedEventHandler handler;
            foreach (var item in items)
            {
                if(item is INotifyPropertyChanged notify)
                {
                    success = PropertyChangedDict.TryGetValue(notify, out handler);
                    if (success)
                    {
                        notify.PropertyChanged -= handler;
                        PropertyChangedDict.Remove(notify);
                    }
                }
            }
        }

        // Selector
        private IEnumerable<PropertyInfo> SelectINotifyPropertyChanged(Type type)
        {
            const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public;
            return from property in type.GetProperties(bindingFlags)
                   where property.CanWrite
                        && !IsDefined(property, typeof(IgnoreAutoChangeNotificationAttribute))
                        //&& typeof(INotifyPropertyChanged).IsAssignableFrom(property.PropertyType)
                   select property;
        }

        //TODO What's this?
        //没有不行，又不会写，但又好像不影响
        [CompilerGenerated]
        public class Serializer : ReferenceTypeSerializer
        {
            public override object CreateInstance(Type type, IArgumentsReader constructorArguments)
            {
                return Activator.CreateInstance(type);
            }
            public override void DeserializeFields(object obj, IArgumentsReader initializationArguments) { }
            public override void SerializeObject(object obj, IArgumentsWriter constructorArguments, IArgumentsWriter initializationArguments) { }
        }
    }
}
