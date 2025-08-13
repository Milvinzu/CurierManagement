using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CurierManagement.Convert
{
    public class CoordinateDisplayConverter : IValueConverter
    {
        public static readonly CoordinateDisplayConverter Instance = new CoordinateDisplayConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double coordinate && coordinate != 0)
            {
                return $"{coordinate:F6}";
            }
            return "Не обрано";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str && double.TryParse(str, out double result))
            {
                return result;
            }
            return 0.0;
        }
    }
}
