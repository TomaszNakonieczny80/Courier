using System;
using System.Collections.Generic;
using System.Text;
using Courier.DataLayer;
using Courier.DataLayer.Models;

namespace Courier.BusinessLayer
{
    public interface ICarsService
    {
        void Add(Car car);
    }
    public class CarsService : ICarsService
    {
        private readonly Func<IParcelsDbContext> _dbContextFactoryMethod;

        public CarsService(Func<IParcelsDbContext> dbContextFactoryMethod)
        {
            _dbContextFactoryMethod = dbContextFactoryMethod;
        }

        public void Add(Car car)
        {
            using (var context = _dbContextFactoryMethod())
            {
                context.Cars.Add(car);
                context.SaveChanges();
            }
        }
    }
}
