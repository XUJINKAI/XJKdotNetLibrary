using PostSharp.Aspects;
using PostSharp.Patterns.Model;
using System;
using System.ComponentModel;

namespace XJK.NotifyProperty
{
    [NotifyPropertyChanged]
    [NestedNotifyPropertyChanged]
    public abstract class NotifyObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string PropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
    }
}
