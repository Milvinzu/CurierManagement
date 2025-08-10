using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurierManagement.Model
{
    public class DeliveryPackage
    {
        public int Id { get; set; }
        public string PackageName { get; set; } = string.Empty;
        public List<Order> Orders { get; set; } = new();
        public string CreatedBy { get; set; }
        public int CourierId { get; set; }
        public Courier Courier { get; set; } = null!;
        public List<RoutePoint> Route { get; set; } = new();
    }
}
