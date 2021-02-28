using System;
using System.Collections.Generic;
using System.Text;

namespace Courier.WebApi.Client.Model
{
    public class Shipment
    {
        public Guid ParcelNumber { get; set; }
        public int? CarId { get; set; }
        public DateTime RegisterDate { get; set; }
    }
}
