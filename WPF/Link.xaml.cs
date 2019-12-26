using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace XJK.WPF
{
    /// <summary>
    /// Link.xaml 的交互逻辑
    /// </summary>
    public partial class Link
    {
        //propdp

        public string RunCommand
        {
            get { return (string)GetValue(RunCommandProperty); }
            set { SetValue(RunCommandProperty, value); }
        }
        public static readonly DependencyProperty RunCommandProperty =
            DependencyProperty.Register("RunCommand", typeof(string), typeof(Link), new PropertyMetadata(null));



        public string RunArgs
        {
            get { return (string)GetValue(RunArgsProperty); }
            set { SetValue(RunArgsProperty, value); }
        }
        public static readonly DependencyProperty RunArgsProperty =
            DependencyProperty.Register("RunArgs", typeof(string), typeof(Link), new PropertyMetadata(null));



        public bool Underline
        {
            get { return (bool)GetValue(UnderlineProperty); }
            set { SetValue(UnderlineProperty, value); }
        }
        public static readonly DependencyProperty UnderlineProperty =
            DependencyProperty.Register("Underline", typeof(bool), typeof(Link), new PropertyMetadata(false));





        public Style HyperLinkStyle
        {
            get { return (Style)GetValue(HyperLinkStyleProperty); }
            set { SetValue(HyperLinkStyleProperty, value); }
        }
        public static Style GetHyperLinkStyle(DependencyObject obj) => (Style)obj.GetValue(HyperLinkStyleProperty);
        public static void SetHyperLinkStyle(DependencyObject obj, Style value) => obj.SetValue(HyperLinkStyleProperty, value);
        public static readonly DependencyProperty HyperLinkStyleProperty =
            DependencyProperty.Register("HyperLinkStyle", typeof(Style), typeof(Link), new PropertyMetadata(null), value =>
            {
                return value == null || value is Style style && style.TargetType.Equals(typeof(Hyperlink));
            });




        // event

        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }
        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Link));


        // func

        public Link()
        {
            InitializeComponent();
        }

        protected virtual void OnClicked()
        {
            if (!string.IsNullOrEmpty(RunCommand))
            {
                Process.Start(RunCommand, RunArgs);
            }
            RaiseEvent(new RoutedEventArgs(ClickEvent));
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            OnClicked();
        }

        private void ContentControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                OnClicked();
                e.Handled = true;
            }
        }
    }
}
