using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurierManagement.Model
{
    public class Courier
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal HourlyRcate { get; set; }
        public decimal RatePerKm { get; set; }

        public ICollection<DeliveryPackage> Packages { get; set; } = new List<DeliveryPackage>();
    }
}
