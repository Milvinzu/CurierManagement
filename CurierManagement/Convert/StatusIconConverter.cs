using CurierManagement.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CurierManagement.Convert
{
    public class StatusIconConverter : IValueConverter
    {
        public static readonly StatusIconConverter Instance = new StatusIconConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is OrderStatus status)
            {
                return status switch
                {
                    OrderStatus.Pending => "⏳",
                    OrderStatus.Accepted => "✅",
                    OrderStatus.InDelivery => "🚚",
                    OrderStatus.Delivered => "📦",
                    OrderStatus.Canceled => "❌",
                    _ => "📦"
                };
            }
            return "📦";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
