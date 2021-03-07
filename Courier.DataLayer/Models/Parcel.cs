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
        OnTheWay,
        Delivered,
    }


    public class Parcel
    {
        public int Id { get; set; }
        public Guid ParcelNumber { get; set; }
        public User Recipient { get; set; }
        public int SenderId { get; set; }
        public User Sender { get; set; }
        public ParcelSize ParcelSize { get; set; }
        public DateTime RegisterDate { get; set; }
        public ParcelStatus ParcelStatus { get; set; }
        public double ParcelLatitude { get; set; }
        public double ParcelLongitude { get; set; }
        public double RecipientLatitude { get; set; }
        public double RecipientLongitude { get; set; }

    }
}
