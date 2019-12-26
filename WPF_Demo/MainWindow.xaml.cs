using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Demo_WPF
{
    public class Data : INotifyPropertyChanged
    {
        private string _name;
        private bool _isStudent;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        protected void OnSetValue<T>(ref T obj, T value, [CallerMemberName] string propertyName = null)
        {
            obj = value;
            OnPropertyChanged(propertyName);
            OnPropertyChanged(nameof(Display));
        }

        public string Display => $"DATA: {Name}, {IsStudent}";
        public string Name { get => _name; set => OnSetValue(ref _name, value); }
        public bool IsStudent { get => _isStudent; set => OnSetValue(ref _isStudent, value); }
    }

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        public Data Data { get; set; } = new Data()
        {
            Name = "XJK",
            IsStudent = false,
        };

        public MainWindow()
        {
            InitializeComponent();
        }
    }
}
