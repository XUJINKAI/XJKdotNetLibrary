using System;
using System.Collections;
using System.ComponentModel;

namespace XJK.NotifyProperty
{
    public class NestedPropertyChangedEventArgs : PropertyChangedEventArgs
    {
        public string PropertyNameChain { get; private set; }
        
        public NestedPropertyChangedEventArgs(string propertyName, PropertyChangedEventArgs itemPropertyArgs) : base(propertyName)
        {
            if(itemPropertyArgs is NestedPropertyChangedEventArgs nestedArgs)
            {
                PropertyNameChain = $"{propertyName}.{nestedArgs.PropertyNameChain}";
            }
            else
            {
                PropertyNameChain = $"{propertyName}.{itemPropertyArgs.PropertyName}";
            }
        }
        
        public override string ToString()
        {
            return $"{PropertyNameChain}";
        }
    }

    public static class PropertyChangedEventArgsExtension
    {
        public static string GetNestedPropertyName(this PropertyChangedEventArgs e)
        {
            return (e is NestedPropertyChangedEventArgs ex) ? ex.PropertyNameChain : e.PropertyName;
        }
    }
}
