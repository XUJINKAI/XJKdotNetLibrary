using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace XJK.WPF.Converters
{
    public class BoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool bln)
            {
                if (targetType == typeof(bool))
                {
                    return bln;
                }
                if (targetType == typeof(Visibility))
                {
                    return bln ? Visibility.Visible : Visibility.Collapsed;
                }
                throw new InvalidCastException("targetType");
            }
            throw new InvalidCastException("value");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
