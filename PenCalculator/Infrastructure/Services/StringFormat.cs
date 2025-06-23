using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PenCalculator.Infrastructure.Services
{
    internal static class StringFormat
    {
        public static string FormatCulture(double val)
        {
            // Изменение культуры (локали) для замены разделителя на пробел
            var cultureWithSpaceSeparator = new CultureInfo("ru-RU");
            cultureWithSpaceSeparator.NumberFormat.NumberGroupSeparator = " ";
            return Math.Round(val, 2).ToString("N", cultureWithSpaceSeparator);
        }
    }
}
