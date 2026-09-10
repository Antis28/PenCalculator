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
                return d.ToString("N2", _ru); // красиво: 10 000,50
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s && !string.IsNullOrWhiteSpace(s))
            {
                // 1. Убираем все пробелы и любые пробельные символы
                s = new string(s.Where(c => !char.IsWhiteSpace(c)).ToArray());

                // 2. Если есть запятая — считаем её десятичным разделителем.
                //    Точки (разделители тысяч) убираем полностью.
                if (s.Contains(","))
                {
                    s = s.Replace(".", ""); // удаляем все точки
                }
                else
                {
                    // Если запятой нет, но есть точка — возможно, пользователь ввёл «1000.5» (англ. формат)
                    // Тогда заменим точку на запятую, чтобы парсить как десятичную
                    if (s.Contains("."))
                    {
                        s = s.Replace(".", ",");
                    }
                }

                if (double.TryParse(s, NumberStyles.Number, _ru, out var result))
                    return result;
            }

            return 0.0;
        }
    }


}
