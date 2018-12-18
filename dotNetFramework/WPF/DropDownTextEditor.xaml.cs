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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace XJK.WPF
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class DropDownTextEditor : UserControl, INotifyPropertyChanged
    {
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty =DependencyProperty.Register("Text", typeof(string), typeof(DropDownTextEditor)
            , new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));



        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }
        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), typeof(DropDownTextEditor), new UIPropertyMetadata(false, (sender, e) =>
        {

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

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            OnPropertyChanged(nameof(PlaceHolderVisibility));
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

        public void OpenPanel()
        {
            IsOpen = true;
        }

        public void ClosePanel()
        {
            if (IsOpen) IsOpen = false;
        }

        private void Popup_Opened(object sender, EventArgs e)
        {
            if (sender is Popup popup)
                if (popup.FindName("EditorTextBox") is TextBox textBox)
                {
                    textBox.CaretIndex = textBox.Text.Length;
                    textBox.Focus();
                }
        }

        private void Popup_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                ClosePanel();
            }
        }

        private void Thumb_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ClosePanel();
        }
    }
}
