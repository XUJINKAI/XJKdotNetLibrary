using System;
using System.Globalization;
using System.Windows.Data;

namespace XJK.WPF.Converters
{
    public class InverseBoolConverter : IValueConverter
    {
        private static readonly BoolConverter BoolConverter = new BoolConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool bln)
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
