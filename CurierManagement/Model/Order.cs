using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurierManagement.Model
{
    public class Order
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string DeliveryAddress { get; set; } = string.Empty;
        public int Entryway { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Tips { get; set; }
        public string CreatedBy { get; set; } = string.Empty;

        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public string WebAddressInMap { get; set; }
        public int? CourierId { get; set; }
        public Courier? Courier { get; set; }

        public int? DeliveryPackageId { get; set; }
        public DeliveryPackage? DeliveryPackage { get; set; }
    }
}
