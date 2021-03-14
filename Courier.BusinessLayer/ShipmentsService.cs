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
    }

    public class ShipmentsService : IShipmentsService
    {
        private readonly Func<IParcelsDbContext> _dbContextFactoryMethod;
        public ShipmentsService(Func<IParcelsDbContext> dbContextFactoryMethod)
        {
            _dbContextFactoryMethod = dbContextFactoryMethod;
        }

        public async Task AddAsync(Shipment shipment)
        {
           
            using (var context = _dbContextFactoryMethod())
            {
                context.Shipments.Add(shipment);
                await context.SaveChangesAsync();
            }

        }
    }
}
