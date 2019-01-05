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
using System.Xml.Serialization;
using PostSharp.Patterns.Model;
using System.ComponentModel;
using System.Diagnostics;
using XJK;
using XJK.ReflectionUtils;
using XJK.XObject;
using XJK.XObject.DefaultProperty;
using XJK.XObject.NotifyProperty;
using XJK.XObject_Test;
using PostSharp.Patterns.Recording;
using static XJK.RandomGenerator;
using PostSharp.Patterns.Collections;
using System.Collections;
using System.Collections.ObjectModel;

namespace DB
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        Database database;

        public MainWindow()
        {
            InitializeComponent();
        }

        private int counter = 1;
        private void DbConfig_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            LogBox.Text = database.GetXmlData();
            Title = $"{counter++}: {e.GetNestedPropertyName()}, on {DateTime.Now}";
            Trace.WriteLine($"NestedPropertyName: {e.GetNestedPropertyName()}");
        }

        // Config
        private void Button_Click_InitConfig(object sender, RoutedEventArgs e)
        {
            database = new Database();
            database.PropertyChanged += DbConfig_PropertyChanged;
            LogBox.Text = database.GetXmlData();
        }
        private void Button_Click_Reset(object sender, RoutedEventArgs e)
        {
            database.ResetAllPropertiesDefaultValue();
        }
        private void Button_Click_Refresh(object sender, RoutedEventArgs e)
        {
            LogBox.Text = database.GetXmlData();
        }
        private void Button_Click_Parse(object sender, RoutedEventArgs e)
        {
            database.SetByXml(LogBox.Text);
            if (database.ParseError.IsNotNullOrEmpty())
            {
                MessageBox.Show(database.ParseError);
            }
        }
        // DB
        private void Button_Click_Change_Property(object sender, RoutedEventArgs e)
        {
            database.DefaultValue_String = RandomString(5);
        }
        private void Button_Click_Change_Object_Property(object sender, RoutedEventArgs e)
        {
            database.DefaultValueNewInstance_Instance.Field = RandomGuid();
        }
        // Collection
        private void Button_Click_Change_Collection(object sender, RoutedEventArgs e)
        {
            var item = new SubInstance() { Field = RandomGuid() };
            database.SubDatabase.Collection.Add(item);
        }
        private void Button_Click_Change_Collection_Item_Property(object sender, RoutedEventArgs e)
        {
            if (database.SubDatabase.Collection.Count == 0) return;
            var idx = RandomInt(database.SubDatabase.Collection.Count);
            database.SubDatabase.Collection[idx].Field = RandomString(20);
        }
        private void Button_Click_Change_Collection_Remove(object sender, RoutedEventArgs e)
        {
            if (database.SubDatabase.Collection.Count == 0) return;
            var idx = RandomInt(database.SubDatabase.Collection.Count);
            var obj = database.SubDatabase.Collection[idx];
            database.SubDatabase.Collection.RemoveAt(idx);
            obj.Field = RandomString(1000);
        }
        private void Button_Click_Change_Collection_Clear(object sender, RoutedEventArgs e)
        {
            database.SubDatabase.Collection.Clear();
        }
        // Dictionary
        private void Button_Click_Change_Dictionary(object sender, RoutedEventArgs e)
        {
            database.SubDatabase.Dictionary.Add(RandomString(5), new SubInstance() { Field = RandomString(5) });
        }
        private void Button_Click_Change_Dictionary_Property(object sender, RoutedEventArgs e)
        {
            var keys = database.SubDatabase.Dictionary.Keys;
            if (keys.Count == 0) return;
            var idx = RandomInt(keys.Count);
            int i = 0;
            var key = keys.Where(o => i++ == idx).First();
            database.SubDatabase.Dictionary[key].Field = RandomString(20);
        }
        private void Button_Click_Change_Dictionary_Remove(object sender, RoutedEventArgs e)
        {
            var keys = database.SubDatabase.Dictionary.Keys;
            if (keys.Count == 0) return;
            var idx = RandomInt(keys.Count);
            int i = 0;
            var key = keys.Where(o => i++ == idx).First();
            var obj = database.SubDatabase.Dictionary[key];
            database.SubDatabase.Dictionary.Remove(key);
            obj.Field = RandomString(1000);
        }
        private void Button_Click_Change_Dictionary_Clear(object sender, RoutedEventArgs e)
        {
            database.SubDatabase.Dictionary.Clear();
        }
        // Agg, rec
        private void Button_Click_CountChilds(object sender, RoutedEventArgs e)
        {
            Title = $"Children.Count: {database.AsIAggregatable().GetChildren().Count}";
        }

        private void Record_Clear(object sender, RoutedEventArgs e)
        {
            RecordingServices.DefaultRecorder.Clear();
        }

        private void Button_Click_Break(object sender, RoutedEventArgs e)
        {
            Debugger.Break();
        }
    }
}
