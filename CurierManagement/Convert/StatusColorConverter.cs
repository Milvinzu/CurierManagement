using CurierManagement.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace CurierManagement.Convert
{
    public class StatusColorConverter : IValueConverter
    {
        public static readonly StatusColorConverter Instance = new StatusColorConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is OrderStatus status)
            {
                return status switch
                {
                    OrderStatus.Pending => new SolidColorBrush(Color.FromRgb(255, 193, 7)),    // Orange
                    OrderStatus.Accepted => new SolidColorBrush(Color.FromRgb(40, 167, 69)),   // Green
                    OrderStatus.InDelivery => new SolidColorBrush(Color.FromRgb(33, 150, 243)), // Blue
                    OrderStatus.Delivered => new SolidColorBrush(Color.FromRgb(76, 175, 80)),  // Success Green
                    OrderStatus.Canceled => new SolidColorBrush(Color.FromRgb(220, 53, 69)),   // Red
                    _ => new SolidColorBrush(Color.FromRgb(108, 117, 125)) // Gray
                };
            }
            return new SolidColorBrush(Color.FromRgb(108, 117, 125));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
