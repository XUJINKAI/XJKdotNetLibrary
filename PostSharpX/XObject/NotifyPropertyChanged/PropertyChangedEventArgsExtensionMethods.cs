using System.ComponentModel;

namespace XJK.NotifyPropertyChanged
{
    public static class PropertyChangedEventArgsExtensionMethods
    {
        public static object TryRetriveProperty(this PropertyChangedEventArgs e, object Target)
        {
            return Target?.GetType().GetProperty(e.PropertyName)?.GetValue(Target);
        }
    }

    public static class PropertyChangedEventArgsExExtensionMethods
    {
        public static object TryRetriveProperty(this PropertyChangedEventArgsEx e, object Target)
        {
            return Target?.GetType().GetProperty(e.PropertyName)?.GetValue(Target);
        }
    }
}
