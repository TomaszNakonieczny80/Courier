using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Courier.BusinessLayer.Models;
using Courier.DataLayer;
using Courier.DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;

namespace Courier.BusinessLayer
{
    public interface IParcelsService
    {
        void Add(Parcel parcel);
        void Add(CarParcel carParcel);
        List<CarParcel> GetAllCarParcel();
        CarParcel GetCarParcel(int? parcelId);
        CarParcel GetCarParcelId(int carParcelId);
        CarParcel GetParcelId(int parcelId);
        List<Parcel> GetParcelsWaitingToBePosted();
        List<Parcel> GetPostedParcels();
        void Remove(CarParcel carParcel);
        List<Parcel> GetParcelsOnTheWay();
        void SetParcelsAsOnTheWay();
        void SetParcelsAsDelivered(List<Parcel> parcelsOnTheWay);
        List<Car> GetAvailableCars();
        void CreateCarParcelsBase();
        List<Shipment> AttachDriverToParcel();
        List<int> GetListCarParcelId(int? carId);
        uint GetAvailableCapcity(int? carId);
        void Update(CarParcel carParcel);
        void Update(Parcel parcel);
        bool AvailableCars();
    }
    public class ParcelsService : IParcelsService
    {
        private readonly Func<IParcelsDbContext> _dbContextFactoryMethod;

        public ParcelsService(Func<IParcelsDbContext> dbContextFactoryMethod)
        {
            _dbContextFactoryMethod = dbContextFactoryMethod;
        }

        public void Add(Parcel parcel)
        {
            using (var context = _dbContextFactoryMethod())
            {
                context.Parcels.Add(parcel);
                context.SaveChanges();
            }
        }

        public void Add(CarParcel carParcel)
        {
            using (var context = _dbContextFactoryMethod())
            {
                context.CarParcels.Add(carParcel);
                context.SaveChanges();
            }
        }

        public void Remove(CarParcel carParcel)
        {
            using (var context = _dbContextFactoryMethod())
            {
                context.CarParcels.Remove(carParcel);
                context.SaveChanges();
            }
        }

        public List<CarParcel> GetAllCarParcel()
        {
            using (var context = _dbContextFactoryMethod())
            {
                var carParcel = context.CarParcels.ToList();
                return carParcel;
            }
        }

        public CarParcel GetCarParcelId(int carParcelId)
        {
            using(var context = _dbContextFactoryMethod())
            {
                return context.CarParcels
                    .FirstOrDefault(carParcel => carParcel.Id == carParcelId);
            }
        }

        public CarParcel GetCarParcel(int? parcelId)
        {
            using (var context = _dbContextFactoryMethod())
            {
                return context.CarParcels
                    .FirstOrDefault(carParcel => carParcel.ParcelId == parcelId);
            }
        }

        public CarParcel GetParcelId(int parcelId)
        {
            using (var context = _dbContextFactoryMethod())
            {
                return context.CarParcels
                    .FirstOrDefault(carParcel => carParcel.ParcelId == parcelId);
            }
        }

        public List<Parcel> GetParcelsWaitingToBePosted()
        {
            using (var context = _dbContextFactoryMethod())
            {
                var parcelsToBePosted = context.Parcels.AsQueryable().Where(parcel => parcel.ParcelStatus == 0).ToList();
                return parcelsToBePosted;
            }
        }

        public List<Parcel> GetPostedParcels()
        {
            using (var context = _dbContextFactoryMethod())
            {
                var parcels = context.Parcels.AsQueryable().Where(parcel => parcel.ParcelStatus == ParcelStatus.Posted).ToList();
                return parcels;
            }
        }

        public List<Parcel> GetParcelsOnTheWay()
        {
            using (var context = _dbContextFactoryMethod())
            {
                var parcels = context.Parcels
                    .AsQueryable()
                    .Include(user => user.Sender)
                    .ThenInclude(address => address.Address)
                    .Include(user => user.Recipient)
                    .ThenInclude(address => address.Address)
                    .Where(parcel => parcel.ParcelStatus == ParcelStatus.OnTheWay).ToList();
                return parcels;
            }
        }

        public void SetParcelsAsOnTheWay()
        {
            foreach (var parcel in GetPostedParcels())
            {
                parcel.ParcelStatus = ParcelStatus.OnTheWay;
                Update(parcel);
            }
        }

        public void SetParcelsAsDelivered(List<Parcel> parcelsOnTheWay)
        {
            foreach (var parcel in parcelsOnTheWay)
            {
                parcel.ParcelStatus = ParcelStatus.Delivered;
                Update(parcel);
            }
        }
        
        public List<Car> GetAvailableCars()
        {
            using (var context = _dbContextFactoryMethod())
            {
                var carsAvailable = context.Cars.AsQueryable().Where(car => car.Available == true).ToList();
                return carsAvailable;
            }
        }

        public void CreateCarParcelsBase()
        {
            foreach (var parcel in GetParcelsWaitingToBePosted())
            {
                using (var context = _dbContextFactoryMethod())
                {
                    foreach (var carsAvailable in GetAvailableCars())
                    {
                        var carAvailable = context.Cars.AsQueryable().Where(car => car.Id == carsAvailable.Id).FirstOrDefault();
                        var distanceToParcel = Math.Round((Math.Sqrt((Math.Pow((parcel.ParcelLatitude - carAvailable.Latitude), 2)) + ((Math.Pow((parcel.ParcelLongitude - carAvailable.Longitude), 2)))) * 73));
                        var distanceToRecipient = Math.Round((Math.Sqrt((Math.Pow((parcel.RecipientLatitude - carAvailable.Latitude), 2)) + ((Math.Pow((parcel.RecipientLongitude - carAvailable.Longitude), 2)))) * 73));
                        var averageSpeed = carAvailable.AverageSpeed;
                        var travelTimeToParcel = distanceToParcel / averageSpeed;
                        var travelTimeToRecipient = distanceToRecipient / averageSpeed;
                        var totalTravelTime = ((distanceToParcel / averageSpeed) + (distanceToRecipient / averageSpeed))*2;
                        double workingHours = 10;
                        
                        CarParcel carParcel = new CarParcel
                        {
                            ParcelId = parcel.Id,
                            CarId = carAvailable.Id,
                            DistanceToParcel = distanceToParcel,
                            DistanceToRecipient = distanceToRecipient,
                            AvailableCapcity = carAvailable.Capacity,
                            AverageSpeed = averageSpeed,
                            TravelTimeToParcel = travelTimeToParcel,
                            TravelTimeToRecipient = travelTimeToRecipient,
                            TotalTravelTime = totalTravelTime,
                       
                            AvailableTime = workingHours,
                            Full = false,
                            Posted = false
                        };
                        Add(carParcel);
                    }
                }
            }
        }

        public List<Shipment> AttachDriverToParcel()
        {
            List<Shipment> shipementList = new List<Shipment>();
            
            int? carId;
            foreach (var selectedParcel in GetParcelsWaitingToBePosted())
            {
                //Sprawdzam czy sa samochody z wolną przestrzenia ładunkowa, jezeli wszystkie sa juz załadowane na full zamykam liste przewozowa,
                //paczki bez przypisanego kuriera beda mogły by obsłuzone kolejnego dnia
                if (AvailableCars())
                {
                    //dla kazdej paczki oczekujacej na wysłanie dobieram najblizszego kuriera do nadawcy
                    //jezeli kurier jest juz zapakowany na full lub total czas odbioru i przesyłki przekaracza godziny pracy kuriera biore nastepnego najblizszego kuriera

                    using (var context = _dbContextFactoryMethod())
                    {
                        
                        if (context.CarParcels.AsQueryable().Where(parcel => parcel.Full == false && parcel.AvailableTime - parcel.TotalTravelTime >= 0)
                            .OrderBy(parcel => parcel.DistanceToParcel)
                            .FirstOrDefault(parcel => parcel.ParcelId == selectedParcel.Id) == null)
                        {
                            carId = null;
                        }
                        else
                        {
                            carId = context.CarParcels.AsQueryable().Where(parcel => parcel.Full == false && parcel.AvailableTime - parcel.TotalTravelTime >= 0)
                                .OrderBy(parcel => parcel.DistanceToParcel)
                                .FirstOrDefault(parcel => parcel.ParcelId == selectedParcel.Id)
                                .CarId;
                        }
                        
                    }

                    if (carId != null)
                    {

                        int parcelWeight = (int) selectedParcel.ParcelSize;
                        //licze ile czasu dostepnego zostanie kurierowi po przypisaniu paczki
                        double deltaTime = GetCarParcel(selectedParcel.Id).AvailableTime -
                                           GetCarParcel(selectedParcel.Id).TotalTravelTime;

                        //aktualizuje dostepna ładownosc samochodu i dostepny czas po przypisaniu paczki, 
                        foreach (var carParcelId in GetListCarParcelId(carId))
                        {
                            var carParcel = GetCarParcelId(carParcelId);
                            

                            carParcel.AvailableCapcity -= (uint) parcelWeight;
                            carParcel.AvailableTime = deltaTime;

                            Update(carParcel);
                        }
                        //oznaczam paczke jako wysłana/przypisana do samochodu

                        var parcel = GetParcelId(selectedParcel.Id);
                        parcel.Posted = true;

                        Update(parcel);

                        selectedParcel.ParcelStatus = ParcelStatus.Posted;
                        Update(selectedParcel);

                        var minAvailableCapacity = 150;
                        //jezeli w aucie pozostało mniej miejsca niz na 1 duza poczke 150 kg to oznaczam go jako zapełniony na maksa
                        if (GetAvailableCapcity(carId) < minAvailableCapacity)
                        {
                            foreach (var carParcelId in GetListCarParcelId(carId))
                            {
                                var carParcel = GetCarParcelId(carParcelId);
                                carParcel.Full = true;

                                Update(carParcel);
                            }
                        }

                        shipementList.Add(new Shipment()
                        {
                            ParcelNumber = selectedParcel.ParcelNumber,
                            DriverId = carId,
                            RegisterDate = selectedParcel.RegisterDate,
                        });
                    }
                }
                else
                {
                    return shipementList;
                }
            }
            return shipementList;
        }

        public List<int> GetListCarParcelId(int? carId)
        {
            using (var context = _dbContextFactoryMethod())
            {
                var carParcelsId = context.CarParcels.AsQueryable().Where(carParcel => carParcel.CarId == carId).Select(carParcel => carParcel.Id).ToList();
                return carParcelsId;
            }
        }

        public uint GetAvailableCapcity(int? carId)
        {
            using (var context = _dbContextFactoryMethod())
            {
                var availableCapacity = context.CarParcels.FirstOrDefault(carParcel => carParcel.CarId == carId).AvailableCapcity;
                return availableCapacity;
            }
        }

        public void Update(CarParcel carParcel)
        {
            using (var context = _dbContextFactoryMethod())
            {
                context.CarParcels.Update(carParcel);
                context.SaveChanges();
            }
        }

        public void Update(Parcel parcel)
        {
            using (var context = _dbContextFactoryMethod())
            {
                context.Parcels.Update(parcel);
                context.SaveChanges();
            }
        }

        public bool AvailableCars()
        {
            using (var context = _dbContextFactoryMethod())
            {
              var result =  context.CarParcels.FirstOrDefault(carParcel => carParcel.Full == false);
              if (result == null)
              {
                  return false;
              }
              return true;
            }
        }
    }
}
