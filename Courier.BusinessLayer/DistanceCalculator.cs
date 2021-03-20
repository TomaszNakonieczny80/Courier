using System;
using System.Collections.Generic;
using System.Text;
using Courier.DataLayer.Models;

namespace Courier.BusinessLayer
{
    public interface IDistanceCalculator
    {
        double GetDistanceToParcel(Parcel parcel, Car carAvailable);
        double GetDistanceToRecipient(Parcel parcel, Car carAvailable);
    }

    public class DistanceCalculator : IDistanceCalculator
    {
        public double GetDistanceToParcel(Parcel parcel, Car carAvailable)
        {
            return Math.Round((Math.Sqrt((Math.Pow((parcel.ParcelLatitude - carAvailable.Latitude), 2)) +
                                         ((Math.Pow((parcel.ParcelLongitude - carAvailable.Longitude), 2)))) * 73));
        }

        public double GetDistanceToRecipient(Parcel parcel, Car carAvailable)
        {
           return Math.Round((Math.Sqrt((Math.Pow((parcel.RecipientLatitude - carAvailable.Latitude), 2)) + 
                                        ((Math.Pow((parcel.RecipientLongitude - carAvailable.Longitude), 2)))) * 73));
        }

    }
}
