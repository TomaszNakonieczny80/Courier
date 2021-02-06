using System;
using System.Collections.Generic;
using System.Text;

namespace Courier.DataLayer.Models
{
    public class Car
    {
        public int Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string RegistrationNumber { get; set; }
        public uint Capacity { get; set; }
        public int UserId { get; set; }
        public bool Available { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
