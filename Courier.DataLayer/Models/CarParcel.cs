using System;
using System.Collections.Generic;
using System.Text;

namespace Courier.DataLayer.Models
{
    public class CarParcel
    {
        public int Id { get; set; }
        public int ParcelId { get; set; }
        public int ParcelSize { get; set;}
        //public double ParcelLatitude { get; set; }
        //public double ParcelLongitude { get; set; }
        public int CarId { get; set; }
        public uint AverageSpeed { get; set; }
        //public double CarLatitude { get; set; }
        //public double CarLongitude { get; set; }
        public double DistanceToParcel { get; set; }
        public double TravelTimeToParcel { get; set; }
        
        //public double RecipientLatitude { get; set; }
        //public double RecipientLongitude { get; set; }
        public double DistanceToRecipient { get; set; }
        public double TravelTimeToRecipient { get; set; }
        public double TotalTravelTime { get; set; }
        public double AvailableTime { get; set; }
       

        public uint AvailableCapcity { get; set; }
        public bool Full { get; set; }
        public bool Posted { get; set; }
    }
}
