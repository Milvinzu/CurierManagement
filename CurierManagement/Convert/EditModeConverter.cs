using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CurierManagement.Convert
{
    public class EditModeConverter : IValueConverter
    {
        public static readonly EditModeConverter Instance = new EditModeConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isEditMode)
            {
                return isEditMode ? "✏️ Редагування замовлення" : "➕ Нове замовлення";
            }
            return "➕ Нове замовлення";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
