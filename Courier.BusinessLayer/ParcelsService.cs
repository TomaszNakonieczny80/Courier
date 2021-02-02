using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Courier.BusinessLayer.Models;
using Courier.DataLayer;
using Courier.DataLayer.Models;

namespace Courier.BusinessLayer
{
    public class ParcelsService
    {
       
        public void Add(Parcel parcel)
        {
            using (var context = new ParcelsDbContext())
            {
                context.Parcels.Add(parcel);
                context.SaveChanges();
            }
        }

        public void Add(CarParcel carParcel)
        {
            using (var context = new ParcelsDbContext())
            {
                context.CarParcels.Add(carParcel);
                context.SaveChanges();
            }
        }

        public void Remove(CarParcel carParcel)
        {
            using (var context = new ParcelsDbContext())
            {
                context.CarParcels.Remove(carParcel);
                context.SaveChanges();
            }
        }

        public List<CarParcel> GetAllCarParcel()
        {
            using (var context = new ParcelsDbContext())
            {
                var carParcel = context.CarParcels.ToList();
                return carParcel;
            }
        }

        public CarParcel Get(int carId)
        {
            using (var context = new ParcelsDbContext())
            {
                return context.CarParcels
                    .FirstOrDefault(carParcel => carParcel.Id == carId);
            }
        }

        public CarParcel GetCarParcelId(int carParcelId)
        {
            using (var context = new ParcelsDbContext())
            {
                return context.CarParcels
                    .FirstOrDefault(carParcel => carParcel.Id == carParcelId);
            }
        }

        public CarParcel GetParcelId(int parcelId)
        {
            using (var context = new ParcelsDbContext())
            {
                return context.CarParcels
                    .FirstOrDefault(carParcel => carParcel.ParcelId == parcelId);
            }
        }

        public List<Parcel> GetParcelsWaitingToBePosted()
        {
            using (var context = new ParcelsDbContext())
            {
                var parcelsToBePosted = context.Parcels.Where(parcel => parcel.ParcelStatus == 0).ToList();
                return parcelsToBePosted;
            }
        }

        public List<Car> GetAvailableCars()
        {
            using (var context = new ParcelsDbContext())
            {
                var carsAvailable = context.Cars.Where(car => car.Available == true).ToList();
                return carsAvailable;
            }
        }

        public void CreatCarParcelsBase()
        {
            foreach (var parcel in GetParcelsWaitingToBePosted())
            {
                using (var context = new ParcelsDbContext())
                {
                    foreach (var carsAvailable in GetAvailableCars())
                    {
                        var carAvailable = context.Cars.Where(car => car.Id == carsAvailable.Id).FirstOrDefault();
                        
                        CarParcel carParcel = new CarParcel
                        {
                            ParcelId = parcel.Id,
                            ParcelLatitude = parcel.Latitude,
                            ParcelLongitude = parcel.Longitude,
                            CarId = carAvailable.Id,
                            CarLatitude = carAvailable.Latitude,
                            CarLongitude = carAvailable.Longitude,
                            Distance = Math.Round((Math.Sqrt((Math.Pow((parcel.Latitude - carAvailable.Latitude), 2)) + ((Math.Pow((parcel.Longitude - carAvailable.Longitude), 2)))) * 73)),
                            AvailableCapcity = carAvailable.Capacity,
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
            
            int carId;
            foreach (var selectedParcel in GetParcelsWaitingToBePosted())
            {
                //Sprawdzam czy sa samochody z wolną przestrzenia ładunkowa, jezeli wszystkie sa juz załadowane na full zamykam liste przewozowa,
                //paczki bez przypisanego kuriera beda mogły by obsłuzone kolejnego dnia
                if (AvailableCars())
                {
                    //dla kazdej paczki oczekujacej na wysłanie dobieram najblizszego kuriera do nadawcy
                    //jezeli kurier jest juz zapakowany na full biore nastepnego najblizszego
                    
                    using (var context = new ParcelsDbContext())
                    {
                        carId = context.CarParcels.Where(parcel => parcel.Full == false)
                            .OrderBy(parcel => parcel.Distance)
                            .FirstOrDefault(parcel => parcel.ParcelId == selectedParcel.Id)
                            .CarId;
                    }

                    int parcelWeight = (int) selectedParcel.ParcelSize;
                    //aktualizuje dostepna ładownosc samochodu po przypisaniu paczki, 
                    foreach (var carParcelId in GetListCarParcelId(carId))
                    {
                        var carParcel = GetCarParcelId(carParcelId);
                        carParcel.AvailableCapcity -= (uint) parcelWeight;

                        Update(carParcel);
                    }
                    //oznaczam paczke jako wysłana/przypisana do samochodu
                    
                    var parcel = GetParcelId(selectedParcel.Id);
                    parcel.Posted = true;

                    Update(parcel);

                    selectedParcel.ParcelStatus = ParcelStatus.Posted;
                    Update(selectedParcel);

                    //jezeli w aucie pozostało mniej miejsca niz na 1 duza poczke to oznaczam go jako zapełniony na maksa
                    if (GetAvailableCapcity(carId) < 150)
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
                else
                {
                    return shipementList;
                }
            }
            return shipementList;
        }

        public List<int> GetListCarParcelId(int carId)
        {
            using (var context = new ParcelsDbContext())
            {
                var carParcelsId = context.CarParcels.Where(carParcel => carParcel.CarId == carId).Select(carParcel => carParcel.Id).ToList();
                return carParcelsId;
            }
        }

        public uint GetAvailableCapcity(int carId)
        {
            using (var context = new ParcelsDbContext())
            {
                var availableCapacity = context.CarParcels.FirstOrDefault(carParcel => carParcel.CarId == carId).AvailableCapcity;
                return availableCapacity;
            }
        }

        public void Update(CarParcel carParcel)
        {
            using (var context = new ParcelsDbContext())
            {
                context.CarParcels.Update(carParcel);
                context.SaveChanges();
            }
        }

        public void Update(Parcel parcel)
        {
            using (var context = new ParcelsDbContext())
            {
                context.Parcels.Update(parcel);
                context.SaveChanges();
            }
        }

        public bool AvailableCars()
        {
            using (var context = new ParcelsDbContext())
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
