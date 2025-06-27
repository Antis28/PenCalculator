using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace PenCalculator.Infrastructure.Converters
{
    internal class MoneyToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || (double)value == 0)
                return "";
            return ((double)value).ToString("N");

            return string.Format("{0:# ###.00}", ((double)value));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double.TryParse((string)value, out var val);
            return val;
            return DependencyProperty.UnsetValue;
        }
    }
}
