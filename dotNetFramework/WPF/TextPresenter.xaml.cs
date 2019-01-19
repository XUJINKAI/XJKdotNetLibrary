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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace XJK.WPF
{
    /// <summary>
    /// Text.xaml 的交互逻辑
    /// </summary>
    [ContentProperty(nameof(Text))]
    public partial class TextPresenter : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        private static void StaticNotifyDisplayChanged(object sender, DependencyPropertyChangedEventArgs e) => ((TextPresenter)sender).OnNotifyDisplayChanged();
        protected void OnNotifyDisplayChanged()
        {
            OnPropertyChanged(nameof(PlaceHolderVisibility));
        }

        public Visibility PlaceHolderVisibility => string.IsNullOrEmpty(Text) ? Visibility.Visible : Visibility.Hidden;


        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(TextPresenter), new FrameworkPropertyMetadata(null, StaticNotifyDisplayChanged));



        public string PlaceHolderText
        {
            get { return (string)GetValue(PlaceHolderTextProperty); }
            set { SetValue(PlaceHolderTextProperty, value); }
        }
        public static string GetPlaceHolderText(DependencyObject obj) => (string)obj.GetValue(PlaceHolderTextProperty);
        public static void SetPlaceHolderText(DependencyObject obj, string value) => obj.SetValue(PlaceHolderTextProperty, value);
        public static readonly DependencyProperty PlaceHolderTextProperty = 
            DependencyProperty.Register("PlaceHolderText", typeof(string), typeof(TextPresenter), new FrameworkPropertyMetadata(null, StaticNotifyDisplayChanged));
        
        

        public Style TextBlockStyle
        {
            get { return (Style)GetValue(TextBlockStyleProperty); }
            set { SetValue(TextBlockStyleProperty, value); }
        }
        public static Style GetTextBlockStyle(DependencyObject obj) => (Style)obj.GetValue(TextBlockStyleProperty);
        public static void SetTextBlockStyle(DependencyObject obj, Style value) => obj.SetValue(TextBlockStyleProperty, value);
        public static readonly DependencyProperty TextBlockStyleProperty =
            DependencyProperty.RegisterAttached("TextBlockStyle", typeof(Style), typeof(TextPresenter), new FrameworkPropertyMetadata(null, StaticNotifyDisplayChanged), value =>
            {
                return value == null || value is Style style && style.TargetType.Equals(typeof(TextBlock));
            });


        public Style PlaceHolderTextBlockStyle
        {
            get { return (Style)GetValue(PlaceHolderTextBlockStyleProperty); }
            set { SetValue(PlaceHolderTextBlockStyleProperty, value); }
        }
        public static Style GetPlaceHolderTextBlockStyle(DependencyObject obj) => (Style)obj.GetValue(PlaceHolderTextBlockStyleProperty);
        public static void SetPlaceHolderTextBlockStyle(DependencyObject obj, Style value) => obj.SetValue(PlaceHolderTextBlockStyleProperty, value);
        public static readonly DependencyProperty PlaceHolderTextBlockStyleProperty =
            DependencyProperty.RegisterAttached("PlaceHolderTextBlockStyle", typeof(Style), typeof(TextPresenter), new FrameworkPropertyMetadata(null, StaticNotifyDisplayChanged), value =>
            {
                return value == null || value is Style style && style.TargetType.Equals(typeof(TextBlock));
            });



        public TextPresenter()
        {
            InitializeComponent();
        }
    }
}
