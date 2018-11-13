using PostSharp.Aspects;
using PostSharp.Patterns.Model;
using System;
using System.ComponentModel;

namespace XJK.NotifyPropertyChanged
{
    [NotifyPropertyChanged]
    [NotifyPropertyChangedEx(PropagationEvent = false)]
    public abstract class NotifyObject : INotifyPropertyChanged, INotifyPropertyChangedEx
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangedEventHandlerEx PropertyChangedEx;
        protected bool CrossEvent = true;
        private int CanCrossFireEvent = 0;

        protected virtual void OnPropertyChanged(string Name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Name));
            if (CrossEvent && CanCrossFireEvent == 0)
            {
                CanCrossFireEvent++;
                OnPropertyChangedEx(PropertyChangedEventArgsEx.NewPropertyChange(Name));
                CanCrossFireEvent--;
            }
        }

        protected virtual void OnPropertyChangedEx(PropertyChangedEventArgsEx e)
        {
            if (CrossEvent && CanCrossFireEvent == 0)
            {
                CanCrossFireEvent++;
                OnPropertyChanged(e.PropertyName);
                CanCrossFireEvent--;
            }
            PropertyChangedEx?.Invoke(this, e);
        }
    }
}
