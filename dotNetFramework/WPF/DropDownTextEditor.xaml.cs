using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    [ContentProperty(nameof(Text))]
    public partial class DropDownTextEditor : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        private static void StaticNotifyDisplayChanged(object sender, DependencyPropertyChangedEventArgs e) => ((DropDownTextEditor)sender).OnNotifyDisplayChanged();
        protected void OnNotifyDisplayChanged()
        {
            OnPropertyChanged(nameof(DisplayText));
        }


        public string DisplayText => string.IsNullOrEmpty(Text) ? PlaceHolderText : Text;


        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(DropDownTextEditor)
            , new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, StaticNotifyDisplayChanged)
            { DefaultUpdateSourceTrigger = UpdateSourceTrigger.LostFocus});
        


        public string PlaceHolderText
        {
            get { return (string)GetValue(PlaceHolderTextProperty); }
            set { SetValue(PlaceHolderTextProperty, value); }
        }
        public static readonly DependencyProperty PlaceHolderTextProperty = DependencyProperty.Register("PlaceHolderText", typeof(string), typeof(DropDownTextEditor), new PropertyMetadata(null));



        public double DropDownHeight
        {
            get { return (double)GetValue(DropDownHeightProperty); }
            set { SetValue(DropDownHeightProperty, value); }
        }
        public static readonly DependencyProperty DropDownHeightProperty = DependencyProperty.Register("DropDownHeight", typeof(double), typeof(DropDownTextEditor), new UIPropertyMetadata(120.0));



        public double DropDownWidth
        {
            get { return (double)GetValue(DropDownWidthProperty); }
            set { SetValue(DropDownWidthProperty, value); }
        }
        public static readonly DependencyProperty DropDownWidthProperty = DependencyProperty.Register("DropDownWidth", typeof(double), typeof(DropDownTextEditor), new UIPropertyMetadata(280.0));

        


        public DropDownTextEditor()
        {
            InitializeComponent();
        }

        private void DropDownThumb_OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            double newHeight = DropDownHeight + e.VerticalChange;
            double newWidth = DropDownWidth + e.HorizontalChange;
            if ((newHeight >= 0) && (newWidth >= 0))
            {
                DropDownHeight = newHeight;
                DropDownWidth = newWidth;
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.CaretIndex = textBox.Text.Length;
            }
        }
        
        private void ClearText(object sender, RoutedEventArgs e)
        {
            Text = "";
        }
    }
}
