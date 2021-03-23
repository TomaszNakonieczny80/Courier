using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Courier.DataLayer;
using Courier.DataLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace Courier.BusinessLayer
{
    public interface IShipmentsService
    {
        Task AddAsync(Shipment shipment);
        void CreateDeliverySchedule(Shipment shipment, DateTime raportDate);
        List<Shipment> GetShipmentList(List<Parcel> parcelsNotServe);
        Task<List<Shipment>> GetShipmentsAsync(int userId);
        Task<int> SetDeliveredTimeAsync(int parcelId);
        Task SetDeliveryScoringAsync(int parcelId);
        Task<int> SetPickedUpTimeAsync(int parcelId);
    }

    public class ShipmentsService : IShipmentsService
    {
        private readonly Func<IParcelsDbContext> _dbContextFactoryMethod;
        private readonly ITimeService _timeService;
        private readonly ICarsService _carsService;
        
        public ShipmentsService(Func<IParcelsDbContext> dbContextFactoryMethod, ITimeService timeService, ICarsService carsService)
        {
            _dbContextFactoryMethod = dbContextFactoryMethod;
            _timeService = timeService;
            _carsService = carsService;
        }
       
        public async Task AddAsync(Shipment shipment)
        {
            using (var context = _dbContextFactoryMethod())
            {
                context.Shipments.Add(shipment);
                await context.SaveChangesAsync();
            }
        }

        public void CreateDeliverySchedule(Shipment shipment, DateTime raportDate)
        {
            DateTime startDate = raportDate.AddHours(8);
            
            double travelTimeToParcelMinutes = shipment.TravelTimeToParcel * 60;
            double travelTimeToRecipienMinutes = shipment.TravelTimeToRecipient * 60;

            var scheduledPickUpDate = startDate.AddMinutes(travelTimeToParcelMinutes);
            var backToBaseWithParcelDate = startDate.AddMinutes(travelTimeToParcelMinutes * 2);
            var scheduledDeliveryDate = backToBaseWithParcelDate.AddMinutes(travelTimeToRecipienMinutes);
            var backToBaseFromRecipientDate = backToBaseWithParcelDate.AddMinutes(travelTimeToParcelMinutes * 2);
            
            startDate = backToBaseFromRecipientDate;

            shipment.ScheduledPickUpTime = scheduledPickUpDate;
            shipment.ScheduledDeliveryTime = scheduledDeliveryDate;
            shipment.Scoring = 0;

            UpdateAsync(shipment).Wait();
           
        }

        public async Task<List<Shipment>> GetShipmentsAsync(int userId)
        {
            var carId = _carsService.GetCarId(userId);

            using (var context = _dbContextFactoryMethod())
            {
                var shipments = await context.Shipments.AsQueryable().Where(shipment => shipment.CarId == carId).ToListAsync();
                return shipments;
            }
        }

        public Shipment GetShipment(int parcelId)
        {
            using (var context = _dbContextFactoryMethod())
            {
                var shipment = context.Shipments.AsQueryable().FirstOrDefault(shipment => shipment.ParcelId == parcelId);
                return shipment;
            }
        }
        
        public async Task SetDeliveryScoringAsync(int parcelId)
        {
            var scoring = CountScoring(parcelId);
            var shipment = GetShipment(parcelId);
            shipment.Scoring = scoring;

            await UpdateAsync(shipment);
        }

        public int CountScoring(int parcelId)
        {
            var shipment = GetShipment(parcelId);

            var scheduledDeliveryTime = shipment.ScheduledDeliveryTime;
            var deliveredTime = shipment.DeliveredTime;
            var delta = deliveredTime - scheduledDeliveryTime;
            var deltaMinutes = delta.TotalMinutes;
            var deltaMinutesAbsoluteValue = Math.Abs(deltaMinutes);

            if (deltaMinutesAbsoluteValue <= 10)
            {
                return 5;
            }

            if (deltaMinutesAbsoluteValue <= 20)
            {
                return 4;
            }

            if (deltaMinutesAbsoluteValue <= 30)
            {
                return 3;
            }

            if (deltaMinutesAbsoluteValue <= 40)
            {
                return 2;
            }

            if (deltaMinutesAbsoluteValue <= 50)
            {
                return 1;
            }

            return 0;
        }

        public List<Shipment> GetShipmentList(List<Parcel> parcelsNotServe)
        {
            List<Shipment> shipmentList = new List<Shipment>();
            foreach (var parcel in parcelsNotServe)
            {
                using (var context = _dbContextFactoryMethod())
                {
                    var shipment = context.Shipments.AsQueryable().FirstOrDefault(shipment => shipment.ParcelId == parcel.Id);
                    shipmentList.Add(shipment);
                }
            }

            return shipmentList;
        }

        public async Task<int> SetPickedUpTimeAsync(int parcelId)
        {
            using (var context = _dbContextFactoryMethod())
            {
                var shipment = context.Shipments.AsQueryable().FirstOrDefault(shipment => shipment.ParcelId == parcelId);
                
                if (shipment == null)
                {
                    return 0;
                }

                shipment.PickedUpTime = _timeService.currentTime();
                await UpdateAsync(shipment);

                return shipment.ParcelId;
            }
        }

        public async Task<int> SetDeliveredTimeAsync(int parcelId)
        {
            using (var context = _dbContextFactoryMethod())
            {
                var shipment = context.Shipments.AsQueryable().FirstOrDefault(shipment => shipment.ParcelId == parcelId);

                if (shipment == null)
                {
                    return 0;
                }

                shipment.DeliveredTime = _timeService.currentTime();
                await UpdateAsync(shipment);

                return shipment.ParcelId;
            }
        }

        public async Task UpdateAsync(Shipment shipment)
        {
            using (var context = _dbContextFactoryMethod())
            {
                context.Shipments.Update(shipment);
                await context.SaveChangesAsync();
            }
        }
    }
}
