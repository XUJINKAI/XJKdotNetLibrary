using System;
using System.Collections.Generic;
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

namespace INotify
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        MainCls Main;
        public MainWindow()
        {
            InitializeComponent();
            Main = new MainCls()
            {
                BaseAbsClsString1 = "BaseAbsClsString1_init",
                MainString1 = "MainString1_init",
                Element1 = new ElementCls()
                {
                    ElementString1 = "ElementString1_init",
                },
                List = new XJK.NotifyPropertyChanged.ObservableCollectionEx<ElementCls>()
                {
                    new ElementCls(){ ElementString1 = "List.Item0_init" },
                },
            };
            Main.PropertyChanged += Main_PropertyChanged;
            Main.PropertyChangedEx += Main_PropertyChangedEx;
        }

        private void Main_PropertyChangedEx(object sender, XJK.NotifyPropertyChanged.PropertyChangedEventArgsEx e)
        {
            LogBox.Text += $"{e} = {e.TryGetItemPropertyValue()} {Environment.NewLine}";
        }

        private void Main_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            LogBox.Text += $"{e.PropertyName}{Environment.NewLine}";
        }

        //

        private void Test_BaseAbsClsString1(object sender, RoutedEventArgs e)
        {
            Main.BaseAbsClsString1 = "BaseAbsClsString1_m__" + XJK.Helper.RandomString(10);
        }
        
        private void Test_MainString1(object sender, RoutedEventArgs e)
        {
            Main.MainString1 = "MainString1_m__" + XJK.Helper.RandomString(10);
        }
        
        private void Test_Element1__New(object sender, RoutedEventArgs e)
        {
            Main.Element1 = new ElementCls()
            {
                ElementString1 = "ElementString1_m_new"
            };
        }

        private void Test_Element1_ElementString1(object sender, RoutedEventArgs e)
        {
            Main.Element1.ElementString1 = "ElementString1_m__" + XJK.Helper.RandomString(10);
        }

        private void Test_List__New(object sender, RoutedEventArgs e)
        {
            Main.List = new XJK.NotifyPropertyChanged.ObservableCollectionEx<ElementCls>()
            {
                new ElementCls(){ ElementString1 = "List_new__Element" }
            };
        }

        private void Test_List__Add(object sender, RoutedEventArgs e)
        {
            Main.List.Add(new ElementCls() { ElementString1 = "new_list_item1" });
        }

        private void Test_List_0_ElementString1(object sender, RoutedEventArgs e)
        {
            Main.List[0].ElementString1 = "List_0_ElementString1_m_after__" + XJK.Helper.RandomString(10);
        }
        
        //

        private void Test(object sender, RoutedEventArgs e)
        {
        }

    }
}
