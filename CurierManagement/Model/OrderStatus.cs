using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurierManagement.Model
{
    public enum OrderStatus
    {
        Pending,      // Очікує обробки
        Accepted,     // Прийнято кур’єром
        InDelivery,   // В дорозі
        Delivered,    // Доставлено
        Canceled      // Скасовано
    }
}
