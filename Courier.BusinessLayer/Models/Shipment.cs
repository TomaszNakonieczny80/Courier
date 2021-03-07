using System;
using System.Collections.Generic;
using System.Text;

namespace Courier.BusinessLayer.Models
{
    public class Shipment
    {
        public Guid ParcelNumber { get; set; }
        public int? CarId { get; set; }
        public DateTime RegisterDate { get; set; }

    }
}
