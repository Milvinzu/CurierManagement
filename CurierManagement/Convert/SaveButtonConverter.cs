using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CurierManagement.Convert
{
    public class SaveButtonConverter : IValueConverter
    {
        public static readonly SaveButtonConverter Instance = new SaveButtonConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isEditMode)
            {
                return isEditMode ? "💾 Оновити" : "➕ Створити";
            }
            return "➕ Створити";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
