using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurierManagement.Model
{
    public class RoutePoint
    {
        public int Id { get; set; }
        public int Sequence { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public int DeliveryPackageId { get; set; }
        public DeliveryPackage DeliveryPackage { get; set; } = null!;
    }
}
