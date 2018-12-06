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
using XJK.NotifyProperty;
using PostSharp.Patterns.Model;
using System.ComponentModel;
using XJK.XStorage;
using PostSharp.Patterns.Collections;
using System.Diagnostics;
using XJK;
using XJK.Xml;

namespace DB
{
    [Aggregatable]
    public class DbConfig : DatabaseObject
    {
        public int Age { get; set; }
        [Child] public SubInfo SubInfo { get; set; }
        [Child] public DataCollection<FavItem> DataCollection { get; set; }
        [Child] public DataDictionary<string, FavItem> DataDictionary { get; set; }
        [Child] public DataDictionary<int, DataDictionary<string, FavItem>> DictDictionary { get; set; }

        public DbConfig()
        {
            Age = 10;
            SubInfo = new SubInfo();
            DataCollection = new DataCollection<FavItem>()
            {
                new FavItem()
            };
            DataDictionary = new DataDictionary<string, FavItem>()
            {
                { "A", new FavItem() },
                { "B", new FavItem() },
            };
            DictDictionary = new DataDictionary<int, DataDictionary<string, FavItem>>()
            {
                {123, new DataDictionary<string, FavItem>()
                {
                    {"abc", new FavItem(){ Movie = Helper.RandomString(10)} }
                } },
            };
        }
    }

    [Aggregatable]
    public class SubInfo : DatabaseObject
    {
        public double Height { get; set; } = 1.75;
    }

    [Aggregatable]
    public class FavItem : DatabaseObject
    {
        public string Movie { get; set; } = "Matrix";
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
            DbConfig.Age = Helper.RandomInt(30);
        }
        private void Button_Click_Change_Object(object sender, RoutedEventArgs e)
        {
            DbConfig.SubInfo = new SubInfo() { Height = Helper.RandomDouble(1.7, 1.75, 2) };
        }
        private void Button_Click_Change_Object_Property(object sender, RoutedEventArgs e)
        {
            DbConfig.SubInfo.Height = Helper.RandomDouble(0, 200);
        }
        // Collection
        private void Button_Click_Change_Collection(object sender, RoutedEventArgs e)
        {
            DbConfig.DataCollection.Add(new FavItem());
        }
        private void Button_Click_Change_Collection_Item_Property(object sender, RoutedEventArgs e)
        {
            if (DbConfig.DataCollection.Count == 0) return;
            var idx = Helper.RandomInt(DbConfig.DataCollection.Count);
            DbConfig.DataCollection[idx].Movie = Helper.RandomString(20);
        }
        private void Button_Click_Change_Collection_Remove(object sender, RoutedEventArgs e)
        {
            if (DbConfig.DataCollection.Count == 0) return;
            var idx = Helper.RandomInt(DbConfig.DataCollection.Count);
            var obj = DbConfig.DataCollection[idx];
            DbConfig.DataCollection.RemoveAt(idx);
            obj.Movie = Helper.RandomString(1000);
        }
        // Dictionary
        private void Button_Click_Change_Dictionary(object sender, RoutedEventArgs e)
        {
            DbConfig.DataDictionary.Add(Helper.RandomString(5), new FavItem() { Movie = Helper.RandomString(5) });
        }
        private void Button_Click_Change_Dictionary_Property(object sender, RoutedEventArgs e)
        {
            var keys = DbConfig.DataDictionary.Keys;
            if (keys.Count == 0) return;
            var idx = Helper.RandomInt(keys.Count);
            int i = 0;
            var key = keys.Where(o => i++ == idx).First();
            DbConfig.DataDictionary[key].Movie = Helper.RandomString(20);
        }
        private void Button_Click_Change_Dictionary_Remove(object sender, RoutedEventArgs e)
        {
            var keys = DbConfig.DataDictionary.Keys;
            if (keys.Count == 0) return;
            var idx = Helper.RandomInt(keys.Count);
            int i = 0;
            var key = keys.Where(o => i++ == idx).First();
            var obj = DbConfig.DataDictionary[key];
            DbConfig.DataDictionary.Remove(key);
            obj.Movie = Helper.RandomString(1000);
        }
        // Agg
        private void Button_Click_CountChilds(object sender, RoutedEventArgs e)
        {
            Title = $"Children.Count: {PostSharp.Post.Cast<DbConfig, IAggregatable>(DbConfig).GetChildren().Count}";
        }

        private void Button_Click_InitConfig(object sender, RoutedEventArgs e)
        {
            DbConfig = new DbConfig()
            {
                SubInfo = new SubInfo(),
                DataCollection = new DataCollection<FavItem>(),
                DataDictionary = new DataDictionary<string, FavItem>(),
                DictDictionary = new DataDictionary<int, DataDictionary<string, FavItem>>(),
            };
            DbConfig.PropertyChanged += DbConfig_PropertyChanged;
            LogBox.Text = DbConfig.GetXmlData();
        }

        private void Button_Click_Break(object sender, RoutedEventArgs e)
        {
            DbConfig.DictDictionary = new DataDictionary<int, DataDictionary<string, FavItem>>()
            {
                {123, new DataDictionary<string, FavItem>()
                {
                    {"abc", new FavItem(){ Movie = Helper.RandomString(10)} }
                } },
            };
            Debugger.Break();
        }
    }
}
