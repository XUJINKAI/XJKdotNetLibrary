using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
    /// Link.xaml 的交互逻辑
    /// </summary>
    public partial class Link : UserControl
    {
        public Link()
        {
            InitializeComponent();
        }
        

        public string RunCommand
        {
            get { return (string)GetValue(RunCommandProperty); }
            set { SetValue(RunCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RunCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RunCommandProperty =
            DependencyProperty.Register("RunCommand", typeof(string), typeof(Link), new PropertyMetadata(""));



        public string RunArgs
        {
            get { return (string)GetValue(RunArgsProperty); }
            set { SetValue(RunArgsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RunArgs.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RunArgsProperty =
            DependencyProperty.Register("RunArgs", typeof(string), typeof(Link), new PropertyMetadata(""));



        public bool Underline
        {
            get { return (bool)GetValue(UnderlineProperty); }
            set { SetValue(UnderlineProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LinkUnderline.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UnderlineProperty =
            DependencyProperty.Register("Underline", typeof(bool), typeof(Link), new PropertyMetadata(false));


        
        public TextWrapping TextWrapping
        {
            get { return (TextWrapping)GetValue(TextWrappingProperty); }
            set { SetValue(TextWrappingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextWrapping.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextWrappingProperty =
            DependencyProperty.Register("TextWrapping", typeof(TextWrapping), typeof(Link), new PropertyMetadata(TextWrapping.WrapWithOverflow));




        // Register the routed event
        public static readonly RoutedEvent ClickEvent =
            EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(Link));

        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }


        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            bool setCommand = !string.IsNullOrEmpty(RunCommand);
            if (setCommand)
            {
                Process.Start(RunCommand, RunArgs);
            }
            else
            {
                var text = this.Content as string;
                Process.Start(text, "");
            }
            RaiseEvent(new RoutedEventArgs(ClickEvent));
        }
    }
}
