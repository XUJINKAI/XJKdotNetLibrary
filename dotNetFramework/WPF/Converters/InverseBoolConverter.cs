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
    public class InverseBoolConverter : IValueConverter
    {
        private static readonly BoolConverter BoolConverter = new BoolConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is bool bln)
            {
                return BoolConverter.Convert(!bln, targetType, parameter, culture);
            }
            throw new InvalidCastException("value");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
