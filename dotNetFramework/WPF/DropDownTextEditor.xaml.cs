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
    public partial class DropDownTextEditor : UserControl, INotifyPropertyChanged
    {
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(DropDownTextEditor)
            , new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (sender, e) =>
            {
                if(sender is DropDownTextEditor editor)
                {
                    editor.OnPropertyChanged(nameof(PlaceHolderVisibility));
                }
            }));
        

        public string PlaceHolderText
        {
            get { return (string)GetValue(PlaceHolderTextProperty); }
            set { SetValue(PlaceHolderTextProperty, value); }
        }
        public static readonly DependencyProperty PlaceHolderTextProperty = DependencyProperty.Register("PlaceHolderText", typeof(string), typeof(DropDownTextEditor), new PropertyMetadata("Click to Edit..."));


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


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string PropertyName = null)
        {
            if (PropertyName == null) throw new ArgumentNullException(nameof(PropertyName));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        public Visibility PlaceHolderVisibility
        {
            get
            {
                return string.IsNullOrWhiteSpace(Text) ? Visibility.Visible : Visibility.Hidden;
            }
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


        public DropDownTextEditor()
        {
            InitializeComponent();
        }
        
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.CaretIndex = textBox.Text.Length;
            }
        }
    }
}
