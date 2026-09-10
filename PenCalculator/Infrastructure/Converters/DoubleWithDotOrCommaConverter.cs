using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PenCalculator.Infrastructure.Converters
{
    public class DoubleWithDotOrCommaConverter : IValueConverter
    {
        private readonly CultureInfo _ru = CultureInfo.GetCultureInfo("ru-RU");

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double d)
                return d.ToString("N2", _ru); // 10 000,50 — с пробелами и 2 знаками
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s && !string.IsNullOrWhiteSpace(s))
            {
                s = s.Replace(" ", "").Replace(".", ",");

                // Если две запятые — оставляем только до второй
                int first = s.IndexOf(',');
                if (first >= 0)
                {
                    int second = s.IndexOf(',', first + 1);
                    if (second >= 0)
                        s = s.Substring(0, second);
                }

                if (double.TryParse(s, NumberStyles.Number, _ru, out var result))
                    return result;
            }
            return 0.0;
        }
    }

}
