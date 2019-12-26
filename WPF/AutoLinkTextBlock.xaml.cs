using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace XJK.WPF
{
    /// <summary>
    /// AutoLinkTextBox.xaml 的交互逻辑
    /// </summary>
    [ContentProperty("Text")]
    public partial class AutoLinkTextBlock : INotifyPropertyChanged
    {
        public object Content { get; set; }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(AutoLinkTextBlock), new FrameworkPropertyMetadata(null, (sender, e) =>
            {
                ((AutoLinkTextBlock)sender).Render(e.NewValue as string);
            }));

        public event PropertyChangedEventHandler PropertyChanged;

        public AutoLinkTextBlock()
        {
            InitializeComponent();
        }

        private void Render(string text)
        {
            var stackPanel = new StackPanel();
            if (!string.IsNullOrEmpty(text))
            {
                var lines = text.Trim().Split('\n');
                foreach (var line in lines)
                {
                    if (line == "")
                    {
                        stackPanel.Children.Add(new TextPresenter() { Text = Environment.NewLine });
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
                            wrapPanel.Children.Add(new Link() { Content = link, RunCommand = link });
                        }
                        else
                        {
                            wrapPanel.Children.Add(new TextPresenter() { Text = split });
                        }
                    }
                    stackPanel.Children.Add(wrapPanel);
                }
            }
            this.Content = stackPanel;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Content)));
        }
    }
}
