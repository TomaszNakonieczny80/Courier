using System;
using System.Collections.Generic;
using System.Text;

namespace Courier.DataLayer.Models
{
    public class CarParcel
    {
        public int Id { get; set; }
        public int ParcelId { get; set; }
        public double ParcelLatitude { get; set; }
        public double ParcelLongitude { get; set; }
        public int CarId { get; set; }
        public double CarLatitude { get; set; }
        public double CarLongitude { get; set; }
        public double Distance { get; set; }
        public uint AvailableCapcity { get; set; }
        public bool Full { get; set; }
        public bool Posted { get; set; }
    }
}
