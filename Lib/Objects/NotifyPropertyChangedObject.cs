using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace XJK
{
    /// <summary>
    /// <para>Implement INotifyPropertyChanged, include OnPropertyChanged method.</para>
    /// <para>This class DO NOT trigger the event automatic, use XJK.XObject.NotifyObject for that feature.</para>
    /// </summary>
    public abstract class ImplementOnPropertyChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
