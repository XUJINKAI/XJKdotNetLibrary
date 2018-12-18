using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace XJK.WPF
{
    public static class ListViewEx
    {
        public static void SetSortBy(this GridViewColumn element, string value)
        {
            element.SetValue(SortByProperty, value);
        }

        public static string GetSortBy(this GridViewColumn element)
        {
            return (string)element.GetValue(SortByProperty);
        }

        public static readonly DependencyProperty SortByProperty = DependencyProperty.Register("SortBy", typeof(string), typeof(GridViewColumn), new PropertyMetadata(null));
    }
}
