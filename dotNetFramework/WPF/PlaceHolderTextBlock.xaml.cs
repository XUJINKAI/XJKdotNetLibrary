using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace XJK.WPF
{
    /// <summary>
    /// PlaceHolderTextBlock.xaml 的交互逻辑
    /// </summary>
    public partial class PlaceHolderTextBlock : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Visibility PlaceHolderVisibility => string.IsNullOrWhiteSpace(Text) ? Visibility.Visible : Visibility.Hidden;

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(PlaceHolderTextBlock)
            , new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (sender, e) =>
            {
                var @this = (PlaceHolderTextBlock)sender;
                @this.OnPropertyChanged(nameof(PlaceHolderVisibility));
            }));


        public string PlaceHolderText
        {
            get { return (string)GetValue(PlaceHolderTextProperty); }
            set { SetValue(PlaceHolderTextProperty, value); }
        }
        public static readonly DependencyProperty PlaceHolderTextProperty = DependencyProperty.Register(nameof(PlaceHolderText), typeof(string), 
            typeof(PlaceHolderTextBlock), new PropertyMetadata("nothing here...  "));



        public PlaceHolderTextBlock()
        {
            InitializeComponent();
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
