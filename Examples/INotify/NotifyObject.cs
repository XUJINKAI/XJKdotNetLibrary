using PostSharp.Patterns.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using XJK.NotifyPropertyChanged;

namespace INotify
{
    public interface INotifyObject : INotifyPropertyChanged, INotifyPropertyChangedEx
    {

    }
    
    [Serializable]
    [NotifyPropertyChanged]
    [NotifyPropertyChangedEx]
    public abstract class NotifyObject : INotifyObject
    {
        public bool PropagationNotificationEx { get; set; } = true;

        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangedEventHandlerEx PropertyChangedEx;

        protected virtual void OnPropertyChanged(string Name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Name));
        }

        protected virtual void OnPropertyChangedEx(PropertyChangedEventArgsEx e)
        {
            PropertyChangedEx?.Invoke(this, e);
        }
    }
}
