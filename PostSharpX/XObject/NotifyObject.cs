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
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string PropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
    }
}
