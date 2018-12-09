using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// AutoLinkTextBox.xaml 的交互逻辑
    /// </summary>
    public partial class AutoLinkTextBox : UserControl
    {
        public AutoLinkTextBox()
        {
            InitializeComponent();
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            string text = newContent as string;
            if (!string.IsNullOrEmpty(text))
            {
                var stackPanel = new StackPanel();
                var lines = text.Split('\n');
                foreach(var line in lines)
                {
                    if (line == "")
                    {
                        stackPanel.Children.Add(new TextBlock());
                        continue;
                    }
                    var wrapPanel = new WrapPanel();
                    var Splits = Regex.Split(line, @"(\[.+?\](?:\(.+?\))?)");
                    foreach (var split in Splits)
                    {
                        if (string.IsNullOrEmpty(split)) continue;
                        var match = Regex.Match(split, @"^\[(.+?)\]\((.+?)\)$");
                        if (match.Success)
                        {
                            wrapPanel.Children.Add(new Link() { Content = match.Groups[1].Value, RunCommand = match.Groups[2].Value });
                        }
                        else if (split.StartsWith("[") && split.EndsWith("]"))
                        {
                            var link = split.Substring(1, split.Length - 2);
                            wrapPanel.Children.Add(new Link() { Content = link });
                        }
                        else
                        {
                            wrapPanel.Children.Add(new TextBox() { Text = split });
                        }
                    }
                    stackPanel.Children.Add(wrapPanel);
                }
                this.Content = stackPanel;
            }
        }
    }
}
