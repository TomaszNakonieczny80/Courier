﻿using System;


namespace Courier.WebApi.Client.Model
{
    public class Shipment
    {
        public int Id { get; set; }
        public int ParcelId { get; set; }
        public Guid ParcelNumber { get; set; }
        public int? CarId { get; set; }
        public DateTime RegisterDate { get; set; }
        public double DistanceToParcel { get; set; }
        public double TravelTimeToParcel { get; set; }
        public DateTime ScheduledPickUpTime { get; set; }
        public DateTime PickedUpTime { get; set; }
        public double DistanceToRecipient { get; set; }
        public double TravelTimeToRecipient { get; set; }
        public DateTime ScheduledDeliveryTime { get; set; }
        public DateTime DeliveredTime { get; set; }
        public bool New { get; set; }
        public bool CurrentShipment { get; set; }
    }
}
