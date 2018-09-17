using System;

namespace PostSharpExtension
{
    public interface INotifyNestedPropertyChanged
    {
        event NestedPropertyChangedEventHandler NestedPropertyChanged;
    }


    public delegate void NestedPropertyChangedEventHandler(object sender, NestedPropertyChangedEventArgs e);

    public class NestedPropertyChangedEventArgs : EventArgs
    {
        public string PropertyName { get; set; }
        public string NestedPropertyName { get; set; }

        public NestedPropertyChangedEventArgs(string name, string childProperty)
        {
            PropertyName = name;
            NestedPropertyName = childProperty;
        }
    }
}
