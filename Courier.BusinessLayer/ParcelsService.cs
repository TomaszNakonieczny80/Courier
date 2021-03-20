using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Courier.DataLayer;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Courier.BusinessLayer.Serializers;
using Courier.DataLayer.Models;
using EventStore.Client;


namespace Courier.BusinessLayer
{
    public interface IParcelsService
    {
        Task AddAsync(Parcel parcel);
        List<Parcel> GetParcelsOnTheWay();
        void SetParcelsAsOnTheWay();
        void SetParcelsAsDelivered(List<Parcel> parcelsOnTheWay);
        void GenerateShipmentList();
        DateTime SetRaportDate();
        Task <List<Shipment>> GenerateShipmentListAsync(int courierId);
        bool ExistShipmentList();
        void ClearShipmentList();
    }
    public class ParcelsService : IParcelsService
    {
        private readonly Func<IParcelsDbContext> _dbContextFactoryMethod;
        private ITimeService _timeService;
        private ICarsService _carsService;
        private IShipmentsService _shipmentsService;
        private IDistanceCalculator _distanceCalculator;
        private DateTime _raportDate;
        private List<Shipment> _shipmentList;

        public ParcelsService(Func<IParcelsDbContext> dbContextFactoryMethod, 
            ITimeService timeService, 
            ICarsService carsService, 
            IShipmentsService shipmentsService,
            IDistanceCalculator distanceCalculator)
        {
            _dbContextFactoryMethod = dbContextFactoryMethod;
            _timeService = timeService;
            _carsService = carsService;
            _shipmentsService = shipmentsService;
            _distanceCalculator = distanceCalculator;
        }

        public DateTime SetRaportDate()
        {
            var raportDate = _timeService.currentTime().Date.AddDays(1);
            _raportDate = raportDate;
            
            return raportDate;
        }

        public bool ExistShipmentList()
        {
            if (_shipmentList.Count == 0)
            {
                return false;
            }

            return true;
        }

        public void ClearShipmentList()
        {
            _shipmentList.Clear();
        }

        public async Task AddAsync(Parcel parcel)
        {
            using (var context = _dbContextFactoryMethod())
            {
                context.Parcels.Add(parcel);
                await context.SaveChangesAsync();
            }
        }

        public async Task AddAsync(CarParcel carParcel)
        {
            using (var context = _dbContextFactoryMethod())
            {
                context.CarParcels.Add(carParcel);
                await context.SaveChangesAsync();
            }
        }

        public async Task RemoveAsync(CarParcel carParcel)
        {
            using (var context = _dbContextFactoryMethod())
            {
                context.CarParcels.Remove(carParcel); 
                await context.SaveChangesAsync();
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
                var parcels = context.Parcels.AsQueryable().Where(parcel => parcel.ParcelStatus == ParcelStatus.Posted && parcel.DeliveredAutomatically == true).ToList();
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
                    .Where(parcel => parcel.ParcelStatus == ParcelStatus.OnTheWay && parcel.DeliveredAutomatically == true).ToList();
                return parcels;
            }
        }

        public void SetParcelsAsOnTheWay()
        {
            foreach (var parcel in GetPostedParcels())
            {
                parcel.ParcelStatus = ParcelStatus.OnTheWay;
                UpdateAsync(parcel).Wait();
            }
        }

        public void SetParcelsAsDelivered(List<Parcel> parcelsOnTheWay)
        {
            foreach (var parcel in parcelsOnTheWay)
            {
                parcel.ParcelStatus = ParcelStatus.Delivered;
                UpdateAsync(parcel).Wait();
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

        public async Task CreateCarParcelsBaseAsync()
        {
            foreach (var parcel in GetParcelsWaitingToBePosted())
            {
                using (var context = _dbContextFactoryMethod())
                {

                    foreach (var carsAvailable in GetAvailableCars())
                    {
                        
                        var carAvailable = context.Cars.AsQueryable().Where(car => car.Id == carsAvailable.Id)
                            .FirstOrDefault();

                        var distanceToParcel = _distanceCalculator.GetDistanceToParcel(parcel, carAvailable);

                        var distanceToRecipient = _distanceCalculator.GetDistanceToRecipient(parcel, carAvailable);
                        
                        var averageSpeed = carAvailable.AverageSpeed;
                        var travelTimeToParcel = distanceToParcel / averageSpeed;
                        var travelTimeToRecipient = distanceToRecipient / averageSpeed;
                        var totalTravelTime =
                            ((distanceToParcel / averageSpeed) + (distanceToRecipient / averageSpeed)) * 2;

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
                        

                        await AddAsync(carParcel);
                    }
                }
            }
        }

        public List<Shipment> AttachDriverToParcels()
        {
            List<Shipment> shipementList = new List<Shipment>();

           // DateTime startDate = _raportDate.AddHours(8);
            foreach (var selectedParcel in GetParcelsWaitingToBePosted())
            {
                //Sprawdzam czy sa samochody z wolną przestrzenia ładunkowa, jezeli wszystkie sa juz załadowane na full zamykam liste przewozowa,
                //paczki bez przypisanego kuriera beda mogły by obsłuzone kolejnego dnia

                
                if (AvailableCars())
                {
                    //dla kazdej paczki oczekujacej na wysłanie dobieram najblizszego kuriera do nadawcy
                    //jezeli kurier jest juz zapakowany na full lub total czas odbioru i przesyłki przekaracza godziny pracy kuriera biore nastepnego najblizszego kuriera

                    int? carId;
                    

                    using (var context = _dbContextFactoryMethod())
                    {
                        carId = context.CarParcels
                            .AsQueryable()
                            .Where(parcel => parcel.Full == false && parcel.AvailableTime - parcel.TotalTravelTime >= 0)
                            .OrderBy(parcel => parcel.DistanceToParcel)
                            .FirstOrDefault(parcel => parcel.ParcelId == selectedParcel.Id)
                            ?.CarId;
                    }
                    

                    if (carId == null)
                    {
                        return shipementList;
                    }

                    int parcelWeight = (int) selectedParcel.ParcelSize;
                    //licze ile czasu dostepnego zostanie kurierowi po przypisaniu paczki
                    var selectedCarParcel = GetCarParcel(selectedParcel.Id);
                    double deltaTime = selectedCarParcel.AvailableTime -
                                       selectedCarParcel.TotalTravelTime;

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
                    UpdateAsync(selectedParcel).Wait();

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

                    
                    //double travelTimeToParcelMinutes = selectedCarParcel.TravelTimeToParcel * 60;
                    //double travelTimeToRecipienMinutes = selectedCarParcel.TravelTimeToRecipient * 60;

                    //var schedulePickUpDate = startDate.AddMinutes(travelTimeToParcelMinutes);
                    //var backToBaseWithParcelDate = startDate.AddMinutes(travelTimeToParcelMinutes * 2);
                    //var scheduleDeliveryDate = backToBaseWithParcelDate.AddMinutes(travelTimeToRecipienMinutes);
                    //var backToBaseFromRecipientDate = backToBaseWithParcelDate.AddMinutes(travelTimeToParcelMinutes * 2);

                    //startDate = backToBaseFromRecipientDate;

                    Shipment shipment = new Shipment()
                    {
                        ParcelNumber = selectedParcel.ParcelNumber,
                        CarId = carId,
                        RegisterDate = selectedParcel.RegisterDate,
                        TravelTimeToParcel = selectedCarParcel.TravelTimeToParcel,
                        TravelTimeToRecipient = selectedCarParcel.TravelTimeToRecipient,
                        //ScheduledPickUpTime = schedulePickUpDate,
                        //ScheduledDeliveryTime = scheduleDeliveryDate,
                    };

                    
                    shipementList.Add(shipment);
                   
                    _shipmentsService.AddAsync(shipment).Wait();

                    selectedParcel.CarId = (int) carId;
                    selectedParcel.DeliveredAutomatically = true;
                    UpdateAsync(selectedParcel).Wait();
                }
            }

            return shipementList;
        }

        public async Task ClearCarParcelBaseAsync()
        {
            foreach (var carParcel in GetAllCarParcel())
            {
                await RemoveAsync(carParcel);
            }
        }
        public void GenerateShipmentList()
        {
            CreateCarParcelsBaseAsync().Wait();
            var shipmentList = AttachDriverToParcels();
            _shipmentList = shipmentList;

            GenerateShipementListPerCourier(shipmentList);
            //foreach (var courier in shipmentList)
            //{
            //    List<Shipment> courierShipmentList = shipmentList.Where(shipment => shipment.CarId == courier.CarId).ToList();


            //    string systemDrivePath = Path.GetPathRoot(Environment.SystemDirectory);
            //    string targetPath = $"{systemDrivePath}Shiping_list";
            //    Directory.CreateDirectory(targetPath);

            //    var date = _timeService.currentTime().ToString(("MM-dd-yyyy"));

            //    string fileName = $"DriverId{courier.CarId}_{date}.json";
            //    string filePath = Path.Combine(targetPath, fileName);

            //    var jsonDataSerializer = new JsonDataSerializer();
            //    jsonDataSerializer.Serialize(courierShipmentList, filePath);
            //}

            //po wygenerowaniu raportu czyszcze tablice carParcel,
            //paczki nie przypisane beda uwzgleniane w kolejnym raporcie na pierwszym m-cu
            ClearCarParcelBaseAsync().Wait();
        }

        public void GenerateShipementListPerCourier(List<Shipment> shipmentList)
        {
            foreach (var courier in shipmentList)
            {
                List<Shipment> courierShipmentList = shipmentList.Where(shipment => shipment.CarId == courier.CarId).ToList();


                string systemDrivePath = Path.GetPathRoot(Environment.SystemDirectory);
                string targetPath = $"{systemDrivePath}Shiping_list";
                Directory.CreateDirectory(targetPath);

                var date = _timeService.currentTime().ToString(("MM-dd-yyyy"));

                string fileName = $"DriverId{courier.CarId}_{date}.json";
                string filePath = Path.Combine(targetPath, fileName);

                var jsonDataSerializer = new JsonDataSerializer();
                jsonDataSerializer.Serialize(courierShipmentList, filePath);
            }
        }

        //public async Task<List<Shipment>> GenerateShipmentListAsync(int courierId)
        //{
        //    await CreateCarParcelsBaseAsync();

        //    var shipmentList = AttachDriverToParcels();
        //    var carId = _carsService.GetCarId(courierId);

        //    List<Shipment> courierShipmentList = shipmentList.Where(shipment => shipment.CarId == carId).ToList();

        //    _shipmentsService.CreateDeliverySchedule(courierShipmentList);

        //    await ClearCarParcelBaseAsync();
        //    return courierShipmentList;
        //}

        public async Task<List<Shipment>> GenerateShipmentListAsync(int courierId)
        {
            var carId = _carsService.GetCarId(courierId);
            List<Shipment> courierShipementList = null;

            var parcelsNotServed = GetCourierParcelsNotServed(carId);
            if (parcelsNotServed.Count != 0)
            {
                List<Shipment> courierShipmentList = _shipmentList.Where(shipment => shipment.CarId == carId).ToList();

                _shipmentsService.CreateDeliverySchedule(courierShipmentList);

                foreach (var parcel in parcelsNotServed)
                {
                    parcel.DeliveredAutomatically = false;
                    await UpdateAsync(parcel);
                }

                var dupa = new Shipment
                {
                    CarId = 1,
                };

                courierShipementList.Add(dupa);
                return courierShipmentList;
            }

            return courierShipementList;
        }

        public List<Parcel> GetCourierParcelsNotServed(int carId)
        {
            using (var context = _dbContextFactoryMethod())
            {
                var parcels = context.Parcels.AsQueryable().Where(parcel => parcel.CarId == carId && parcel.ParcelStatus == ParcelStatus.Posted && parcel.DeliveredAutomatically == true).ToList();
                return parcels;
            }
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

        public async Task UpdateAsync(Parcel parcel)
        {
            using (var context = _dbContextFactoryMethod())
            {
                context.Parcels.Update(parcel);
                await context.SaveChangesAsync();
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
