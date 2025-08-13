using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CurierManagement.Convert
{
    public class CurrencyConverter : IValueConverter
    {
        public static readonly CurrencyConverter Instance = new CurrencyConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal amount)
            {
                return $"{amount:F2} ₴";
            }
            return "0.00 ₴";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                var cleanStr = str.Replace("₴", "").Trim();
                if (decimal.TryParse(cleanStr, out decimal result))
                {
                    return result;
                }
            }
            return 0m;
        }
    }
}
