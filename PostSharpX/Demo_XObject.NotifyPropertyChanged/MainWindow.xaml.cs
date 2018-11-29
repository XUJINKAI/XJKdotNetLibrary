using System;
using System.ComponentModel;
using System.Windows;
using System.Reflection;
using XJK.ReflectionUtils;
using System.Collections.ObjectModel;
using XJK;
using XJK.NotifyPropertyChanged;

namespace NotifyPropertyChangedExample
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        MainCls Main;
        Status PauseLog = new Status(false);
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
            };
            NewCollection();
            LogBoxEx.Text += Main.Dump();
            Main.PropertyChanged += Main_PropertyChanged;
            Main.PropertyChangedEx += Main_PropertyChangedEx;
        }

        private void Main_PropertyChangedEx(object sender, PropertyChangedEventArgsEx e)
        {
            if (PauseLog) return;
            LogBox.Text += $"<{e.Type}> {e} = {e.TryGetItemPropertyValue() ?? e.TryRetriveProperty(sender)} {Environment.NewLine}";
        }

        private void Main_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (PauseLog) return;
            LogBoxEx.Text += $"{e.PropertyName} = {e.TryRetriveProperty(sender)}{Environment.NewLine}";
        }

        //

        private void Test_DumpMain(object sender, RoutedEventArgs e)
        {
            LogBox.Text += Main.Dump();
        }

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

        private void Test_SetCombination(object sender, RoutedEventArgs e)
        {
            Main.Combination = "Combine_m__" + XJK.Helper.RandomString(10);
        }

        // List

        private void Test_List__New(object sender, RoutedEventArgs e)
        {
            NewCollection();
        }

        private void Test_List__Add(object sender, RoutedEventArgs e)
        {
            Main.List.Add(new ElementCls() { ElementString1 = "new_list_item1" });
        }

        private void Test_List_0_ElementString1(object sender, RoutedEventArgs e)
        {
            Main.List[0].ElementString1 = "List_0_ElementString1_m_after__" + XJK.Helper.RandomString(10);
        }

        private void Test_ExpectNothing(object sender, RoutedEventArgs e)
        {
            var Cls = Main.List;
            PauseLog.InChanging(() =>
            {
                NewCollection();
            });
            Cls.Add(new ElementCls());
            Cls[0].ElementString1 = Helper.RandomString(10);

            var Ele = Main.List[0];
            PauseLog.InChanging(() =>
            {
                Main.List.Remove(Ele);
            });
            Ele.ElementString1 = Helper.RandomString(10);
        }

        //

        private void NewCollection()
        {
            Main.List = new ObservableCollection<ElementCls>()
            {
                new ElementCls(){ ElementString1 = XJK.Helper.RandomString(10) },
                new ElementCls(){ ElementString1 = XJK.Helper.RandomString(10) },
            };
        }

        private void Test(object sender, RoutedEventArgs e)
        {
        }

        private void Test_ClearLogBox(object sender, RoutedEventArgs e)
        {
            LogBox.Clear();
            LogBoxEx.Clear();
        }

    }
}
