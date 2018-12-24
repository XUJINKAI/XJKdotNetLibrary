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
using XJK.XObject;
using XJK.XObject.DefaultProperty;
using XJK.XObject.NotifyProperty;
using PostSharp.Patterns.Recording;

namespace DB
{
    [Aggregatable]
    [Recordable]
    public class DbConfig : DatabaseObject
    {
        [DefaultValueByMethod(nameof(ResetCountMethod))]
        public int ResetCount { get; set; }
        
        [DefaultValue("XJK")]
        public string Name { get; set; }
        
        [Child]
        [DefaultValueNewInstance]
        public SubInfo SubInfo { get; set; }
        
        [Child]
        [DefaultValueNewInstance]
        public DataCollection<FavItem> DataCollection { get; set; }
        
        [Child]
        [DefaultValueNewInstance]
        public DataDictionary<string, FavItem> DataDictionary { get; set; }

        public int ResetCountMethod() => ResetCount + 1;
    }

    [Aggregatable]
    [Recordable]
    public class SubInfo : DatabaseObject
    {
        [DefaultValue(1.75)]
        public double Height { get; set; }
    }

    [Aggregatable]
    [Recordable]
    public class FavItem : DatabaseObject
    {
        [DefaultValue("Matrix")]
        public string Movie { get; set; }
    }
    
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        DbConfig DbConfig;

        public MainWindow()
        {
            InitializeComponent();
        }
        
        private int counter = 1;
        private void DbConfig_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            LogBox.Text = DbConfig.GetXmlData();
            Title = $"{counter++}: {e.GetNestedPropertyName()}, on {DateTime.Now}";
            Trace.WriteLine($"NestedPropertyName: {e.GetNestedPropertyName()}");
        }

        // Config
        private void Button_Click_InitConfig(object sender, RoutedEventArgs e)
        {
            DbConfig = new DbConfig();
            DbConfig.PropertyChanged += DbConfig_PropertyChanged;
            LogBox.Text = DbConfig.GetXmlData();
        }
        private void Button_Click_Reset(object sender, RoutedEventArgs e)
        {
            DbConfig.ResetAllPropertiesDefaultValue();
        }
        private void Button_Click_Refresh(object sender, RoutedEventArgs e)
        {
            LogBox.Text = DbConfig.GetXmlData();
        }
        private void Button_Click_Parse(object sender, RoutedEventArgs e)
        {
            DbConfig.SetByXml(LogBox.Text);
            if(DbConfig.ParseError.IsNotNullOrEmpty())
            {
                MessageBox.Show(DbConfig.ParseError);
            }
        }
        // DB
        private void Button_Click_Change_Property(object sender, RoutedEventArgs e)
        {
            DbConfig.Name = RandomGenerator.RandomString(5);
        }
        private void Button_Click_Change_Object_Property(object sender, RoutedEventArgs e)
        {
            DbConfig.SubInfo.Height = RandomGenerator.RandomDouble(0, 200);
        }
        // Collection
        private void Button_Click_Change_Collection(object sender, RoutedEventArgs e)
        {
            DbConfig.DataCollection.Add(new FavItem());
        }
        private void Button_Click_Change_Collection_Item_Property(object sender, RoutedEventArgs e)
        {
            if (DbConfig.DataCollection.Count == 0) return;
            var idx = RandomGenerator.RandomInt(DbConfig.DataCollection.Count);
            DbConfig.DataCollection[idx].Movie = RandomGenerator.RandomString(20);
        }
        private void Button_Click_Change_Collection_Remove(object sender, RoutedEventArgs e)
        {
            if (DbConfig.DataCollection.Count == 0) return;
            var idx = RandomGenerator.RandomInt(DbConfig.DataCollection.Count);
            var obj = DbConfig.DataCollection[idx];
            DbConfig.DataCollection.RemoveAt(idx);
            obj.Movie = RandomGenerator.RandomString(1000);
        }
        // Dictionary
        private void Button_Click_Change_Dictionary(object sender, RoutedEventArgs e)
        {
            DbConfig.DataDictionary.Add(RandomGenerator.RandomString(5), new FavItem() { Movie = RandomGenerator.RandomString(5) });
        }
        private void Button_Click_Change_Dictionary_Property(object sender, RoutedEventArgs e)
        {
            var keys = DbConfig.DataDictionary.Keys;
            if (keys.Count == 0) return;
            var idx = RandomGenerator.RandomInt(keys.Count);
            int i = 0;
            var key = keys.Where(o => i++ == idx).First();
            DbConfig.DataDictionary[key].Movie = RandomGenerator.RandomString(20);
        }
        private void Button_Click_Change_Dictionary_Remove(object sender, RoutedEventArgs e)
        {
            var keys = DbConfig.DataDictionary.Keys;
            if (keys.Count == 0) return;
            var idx = RandomGenerator.RandomInt(keys.Count);
            int i = 0;
            var key = keys.Where(o => i++ == idx).First();
            var obj = DbConfig.DataDictionary[key];
            DbConfig.DataDictionary.Remove(key);
            obj.Movie = RandomGenerator.RandomString(1000);
        }
        // Agg
        private void Button_Click_CountChilds(object sender, RoutedEventArgs e)
        {
            Title = $"Children.Count: {PostSharp.Post.Cast<DbConfig, IAggregatable>(DbConfig).GetChildren().Count}";
        }

        private void Button_Click_Break(object sender, RoutedEventArgs e)
        {
            Debugger.Break();
        }
    }
}
