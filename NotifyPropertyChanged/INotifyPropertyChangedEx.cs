using PostSharp.Patterns.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace XJK.NotifyPropertyChanged
{
    public delegate void PropertyChangedEventHandlerEx(object sender, PropertyChangedEventArgsEx e);

    public interface INotifyPropertyChangedEx
    {
        event PropertyChangedEventHandlerEx PropertyChangedEx;
    }
}
