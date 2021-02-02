using System;
using System.Collections.Generic;
using System.Text;

namespace Courier.DataLayer.Models
{
    public enum ParcelSize
    {
        Small = 15,
        Medium = 50,
        Large = 150
    }

    public enum ParcelStatus
    {
        WaitingToBePosted,
        Posted,
    }


    public class Parcel
    {
        public int Id { get; set; }
        public Guid ParcelNumber { get; set; }
        public User Recipient { get; set; }
        public int SenderId { get; set; }
        public ParcelSize ParcelSize { get; set; }
        public DateTime RegisterDate { get; set; }
        public ParcelStatus ParcelStatus { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
