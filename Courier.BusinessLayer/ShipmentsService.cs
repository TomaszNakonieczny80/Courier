using System;
using System.Collections.Generic;
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
            DateTime startDate = _raportDate.AddHours(8);
            
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

                Update(shipment);
            }

        }

        public void Update(Shipment shipment)
        {
            using (var context = _dbContextFactoryMethod())
            {
                context.Shipments.Update(shipment);
                context.SaveChanges();
            }
        }
    }
}
