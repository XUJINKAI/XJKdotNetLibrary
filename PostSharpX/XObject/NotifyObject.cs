using PostSharp.Aspects;
using PostSharp.Patterns.Model;
using System;
using System.ComponentModel;
using System.Diagnostics;
using XJK.XObject.DefaultProperty;
using XJK.XObject.NotifyProperty;

namespace XJK.XObject
{
    /// <summary>
    /// INotifyPropertyChanged, IDefaultProperty
    /// </summary>
    [NotifyPropertyChanged]
    [NestedNotifyPropertyChanged]
    public abstract class NotifyObject : INotifyPropertyChanged
    {
        public PropertyChangedEventHandler _propertyChanged;
        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                _propertyChanged += value;
                //Debug.WriteLine($"{this.GetType()}: {_propertyChanged.GetInvocationList().Length}");
            }
            remove
            {
                _propertyChanged -= value;
            }
        }

        protected void OnPropertyChanged(string PropertyName)
        {
            _propertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            _propertyChanged?.Invoke(this, e);
        }
    }
}
