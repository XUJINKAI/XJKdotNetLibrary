using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
    /// DropDown.xaml 的交互逻辑
    /// </summary>
    public partial class DropDown : UserControl
    {
        #region Properties
        
        public object Panel
        {
            get { return (object)GetValue(PanelProperty); }
            set { SetValue(PanelProperty, value); }
        }
        public static readonly DependencyProperty PanelProperty = DependencyProperty.Register("Panel", typeof(object), typeof(DropDown), new PropertyMetadata(null));


        
        public bool ClickOpen
        {
            get { return (bool)GetValue(ClickOpenProperty); }
            set { SetValue(ClickOpenProperty, value); }
        }
        public static readonly DependencyProperty ClickOpenProperty =
            DependencyProperty.Register("ClickOpen", typeof(bool), typeof(DropDown), new PropertyMetadata(true));

        

        public bool StayOpen
        {
            get { return (bool)GetValue(StayOpenProperty); }
            set { SetValue(StayOpenProperty, value); }
        }
        public static readonly DependencyProperty StayOpenProperty =
            DependencyProperty.Register("StayOpen", typeof(bool), typeof(DropDown), new PropertyMetadata(false));




        public bool CloseButton
        {
            get { return (bool)GetValue(CloseButtonProperty); }
            set { SetValue(CloseButtonProperty, value); }
        }
        public static readonly DependencyProperty CloseButtonProperty =
            DependencyProperty.Register("CloseButton", typeof(bool), typeof(DropDown), new PropertyMetadata(true));




        public bool CanResize
        {
            get { return (bool)GetValue(CanResizeProperty); }
            set { SetValue(CanResizeProperty, value); }
        }
        public static readonly DependencyProperty CanResizeProperty =
            DependencyProperty.Register("CanResize", typeof(bool), typeof(DropDown), new PropertyMetadata(true));





        public double DropDownHeight
        {
            get { return (double)GetValue(DropDownHeightProperty); }
            set { SetValue(DropDownHeightProperty, value); }
        }
        public static readonly DependencyProperty DropDownHeightProperty = DependencyProperty.Register("DropDownHeight", typeof(double), typeof(DropDown), new UIPropertyMetadata(120.0));


        public double DropDownWidth
        {
            get { return (double)GetValue(DropDownWidthProperty); }
            set { SetValue(DropDownWidthProperty, value); }
        }
        public static readonly DependencyProperty DropDownWidthProperty = DependencyProperty.Register("DropDownWidth", typeof(double), typeof(DropDown), new UIPropertyMetadata(280.0));


        public bool IsOpen
        {
            get
            { return (bool)GetValue(IsOpenProperty); }
            set
            {
                SetValue(IsOpenProperty, value);
            }
        }
        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), typeof(DropDown), new UIPropertyMetadata(false));
        
        #endregion //Properties

        #region event

        private void RaiseRoutedEvent(RoutedEvent routedEvent)
        {
            RoutedEventArgs args = new RoutedEventArgs(routedEvent, this);
            RaiseEvent(args);
        }

        public static readonly RoutedEvent OpenedEvent = EventManager.RegisterRoutedEvent("Opened", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DropDown));
        public event RoutedEventHandler Opened
        {
            add
            {
                AddHandler(OpenedEvent, value);
            }
            remove
            {
                RemoveHandler(OpenedEvent, value);
            }
        }

        public static readonly RoutedEvent ClosedEvent = EventManager.RegisterRoutedEvent("Closed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DropDown));
        public event RoutedEventHandler Closed
        {
            add
            {
                AddHandler(ClosedEvent, value);
            }
            remove
            {
                RemoveHandler(ClosedEvent, value);
            }
        }

        #endregion // event

        private void DropDownThumb_OnDragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            double newHeight = DropDownHeight + e.VerticalChange;
            double newWidth = DropDownWidth + e.HorizontalChange;
            if ((newHeight >= 0) && (newWidth >= 0))
            {
                DropDownHeight = newHeight;
                DropDownWidth = newWidth;
            }
        }

        public static readonly RoutedCommand OpenPanelCommand = new RoutedCommand();
        public static readonly RoutedCommand ClosePanelCommand = new RoutedCommand();

        public DropDown()
        {
            InitializeComponent();
        }
        
        private void Popup_Opened(object sender, EventArgs e)
        {
            RaiseRoutedEvent(OpenedEvent);
            if(sender is Popup popup)
            {
                if(popup.FindName("Panel") is ContentPresenter presenter)
                {
                    presenter.Focus();
                }
            }
        }

        private void Popup_Closed(object sender, EventArgs e)
        {
            RaiseRoutedEvent(ClosedEvent);
        }

        public void OpenPanel()
        {
            if (!IsOpen) IsOpen = true;
        }

        public void ClosePanel()
        {
            if (IsOpen) IsOpen = false;
        }
        
        private void OpenPanelCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenPanel();
        }

        private void ClosePanelCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ClosePanel();
        }

        private void Thumb_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ClosePanel();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (ClickOpen)
            {
                OpenPanel();
            }
        }
    }
}
