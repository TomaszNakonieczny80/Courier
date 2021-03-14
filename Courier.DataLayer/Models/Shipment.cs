using System;
using System.Collections.Generic;
using System.Text;

namespace Courier.DataLayer.Models
{
    public class Shipment
    {
        public int Id { get; set; }
        public Guid ParcelNumber { get; set; }
        public int? CarId { get; set; }
        public DateTime RegisterDate { get; set; }

    }
}
