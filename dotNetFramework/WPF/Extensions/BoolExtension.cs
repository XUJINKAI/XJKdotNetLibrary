using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace XJK.WPF.Extensions
{
    public static class BoolExtension
    {
        public static Visibility ToVisibility(this bool bln)
        {
            return bln ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
