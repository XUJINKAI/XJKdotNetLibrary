using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
    /// TextEditor.xaml 的交互逻辑
    /// </summary>
    public partial class TextEditor : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        private static void StaticNotifyDisplayChanged(object sender, DependencyPropertyChangedEventArgs e) => ((TextEditor)sender).OnNotifyDisplayChanged();
        protected void OnNotifyDisplayChanged()
        {
            OnPropertyChanged(nameof(IsTextEmpty));
            OnPropertyChanged(nameof(PlaceHolderVisibility));
        }

        
        public bool IsTextEmpty => string.IsNullOrEmpty(Text);
        public Visibility PlaceHolderVisibility => IsTextEmpty ? Visibility.Visible : Visibility.Collapsed;


        public new string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public new static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(TextEditor), new FrameworkPropertyMetadata(null, StaticNotifyDisplayChanged)
            { BindsTwoWayByDefault = true, DefaultUpdateSourceTrigger = UpdateSourceTrigger.LostFocus });



        public string PlaceHolderText
        {
            get { return (string)GetValue(PlaceHolderTextProperty); }
            set { SetValue(PlaceHolderTextProperty, value); }
        }
        public static readonly DependencyProperty PlaceHolderTextProperty =
            DependencyProperty.Register("PlaceHolderText", typeof(string), typeof(TextEditor), new FrameworkPropertyMetadata(null));



        public Brush PlaceHolderBrush
        {
            get { return (Brush)GetValue(PlaceHolderBrushProperty); }
            set { SetValue(PlaceHolderBrushProperty, value); }
        }
        public static readonly DependencyProperty PlaceHolderBrushProperty =
            DependencyProperty.Register("PlaceHolderBrush", typeof(Brush), typeof(TextEditor), new FrameworkPropertyMetadata(SystemColors.GrayTextBrush));




        public Brush HoverBorderBrush
        {
            get { return (Brush)GetValue(HoverBorderBrushProperty); }
            set { SetValue(HoverBorderBrushProperty, value); }
        }
        public static readonly DependencyProperty HoverBorderBrushProperty =
            DependencyProperty.Register("HoverBorderBrush", typeof(Brush), typeof(TextEditor), new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromRgb(0, 100, 179))));




        public TextEditor()
        {
            InitializeComponent();
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            if(GetTemplateChild("RealTextBox") is TextBox textBox)
            {
                textBox.Focus();
            }
            base.OnGotFocus(e);
        }
    }
}
