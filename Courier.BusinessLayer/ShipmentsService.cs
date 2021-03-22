using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Courier.DataLayer;
using Courier.DataLayer.Models;

namespace Courier.BusinessLayer
{
    public interface IShipmentsService
    {
        Task AddAsync(Shipment shipment);
        void CreateDeliverySchedule(List<Shipment> courierShipmentList);
        List<Shipment> GetShipmentList(List<Parcel> parcelsNotServe);
        Task<int> SetDeliveredTimeAsync(int parcelId);
        Task<int> SetPickedUpTimeAsync(int parcelId);
    }

    public class ShipmentsService : IShipmentsService
    {
        private readonly Func<IParcelsDbContext> _dbContextFactoryMethod;
        private readonly ITimeService _timeService;
        private DateTime _raportDate;
        public ShipmentsService(Func<IParcelsDbContext> dbContextFactoryMethod, ITimeService timeService)
        {
            _dbContextFactoryMethod = dbContextFactoryMethod;
            _timeService = timeService;
        }
        public DateTime SetRaportDate()
        {
            var raportDate = _timeService.currentTime().Date.AddDays(1);
            _raportDate = raportDate;

            return raportDate;
        }

        public async Task AddAsync(Shipment shipment)
        {
            using (var context = _dbContextFactoryMethod())
            {
                context.Shipments.Add(shipment);
                await context.SaveChangesAsync();
            }
        }

        public void CreateDeliverySchedule(List<Shipment> courierShipmentList)
        {
            DateTime startDate = SetRaportDate().AddHours(8);
           // DateTime startDate = _timeService.currentTime().AddHours(8);
            
            foreach (var shipment in courierShipmentList)
            {
                double travelTimeToParcelMinutes = shipment.TravelTimeToParcel * 60;
                double travelTimeToRecipienMinutes = shipment.TravelTimeToRecipient * 60;

                var scheduledPickUpDate = startDate.AddMinutes(travelTimeToParcelMinutes);
                var backToBaseWithParcelDate = startDate.AddMinutes(travelTimeToParcelMinutes * 2);
                var scheduledDeliveryDate = backToBaseWithParcelDate.AddMinutes(travelTimeToRecipienMinutes);
                var backToBaseFromRecipientDate = backToBaseWithParcelDate.AddMinutes(travelTimeToParcelMinutes * 2);
                
                startDate = backToBaseFromRecipientDate;

                shipment.ScheduledPickUpTime = scheduledPickUpDate;
                shipment.ScheduledDeliveryTime = scheduledDeliveryDate;
                shipment.New = false;
                
                UpdateAsync(shipment).Wait();
            }
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
