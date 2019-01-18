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
    public partial class TextPresenter : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        private static void StaticNotifyDisplayChanged(object sender, DependencyPropertyChangedEventArgs e) => ((TextPresenter)sender).OnNotifyDisplayChanged();
        protected void OnNotifyDisplayChanged()
        {
            OnPropertyChanged(nameof(DisplayText));
            OnPropertyChanged(nameof(DisplayTextBrush));
            OnPropertyChanged(nameof(IsTextEmpty));
        }

        public string DisplayText => IsTextEmpty ? PlaceHolderText : Text;
        public Brush DisplayTextBrush => IsTextEmpty ? PlaceHolderBrush : Foreground;
        public bool IsTextEmpty => string.IsNullOrEmpty(Text);


        public new string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public new static readonly DependencyProperty TextProperty =
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
        


        public Brush PlaceHolderBrush
        {
            get { return (Brush)GetValue(PlaceHolderBrushProperty); }
            set { SetValue(PlaceHolderBrushProperty, value); }
        }
        public static Brush GetPlaceHolderBrush(DependencyObject obj) => (Brush)obj.GetValue(PlaceHolderBrushProperty);
        public static void SetPlaceHolderBrush(DependencyObject obj, Brush value) => obj.SetValue(PlaceHolderBrushProperty, value);
        public static readonly DependencyProperty PlaceHolderBrushProperty =
            DependencyProperty.Register("PlaceHolderBrush", typeof(Brush), typeof(TextPresenter), new FrameworkPropertyMetadata(SystemColors.GrayTextBrush, StaticNotifyDisplayChanged));


        public TextPresenter()
        {
            InitializeComponent();
        }
    }
}
