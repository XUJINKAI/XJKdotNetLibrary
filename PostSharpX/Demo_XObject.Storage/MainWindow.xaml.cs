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
using XJK.NotifyPropertyChanged;
using PostSharp.Patterns.Model;
using System.ComponentModel;
using XJK.Storage;

namespace DB
{
    public class DbConfig : DatabaseObject
    {
        public string Name { get; set; } = "XJK";
        public int Age { get; set; } = 10;
        public DataCollection<object> FavObjects { get; set; } = new DataCollection<object>() { 1, "abc", 2.3 };
        public DataCollection<FavItem> FavItems { get; set; } = new DataCollection<FavItem>() { new FavItem() };
        public SubInfo Info { get; set; } = new SubInfo();
    }

    public class  SubInfo: DatabaseObject
    {
        public double Height { get; set; } = 1.75;
    }

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
            DbConfig = new DbConfig()
            {
                //Name = "XJK<>",
                //Age = 111,
            };
            DbConfig.PropertyChanged += DbConfig_PropertyChanged;
            LogBox.Text = DbConfig.GetXml();
        }

        private void DbConfig_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Log($"PropertyChanged: {e.PropertyName}");
            LogBox.Text = DbConfig.GetXml();
            Title = DateTime.Now.ToString() + " Err:" + DbConfig.ParseErrors.Count.ToString();
        }

        public void Log(object obj)
        {
            LogBox.Text += obj.ToString() + Environment.NewLine;
        }

        private void Button_Click_Parse(object sender, RoutedEventArgs e)
        {
            DbConfig.OverrideProperties(LogBox.Text);
        }

        private void Button_Click_Change(object sender, RoutedEventArgs e)
        {
            DbConfig.Age = new Random().Next(100);
        }
    }
}
