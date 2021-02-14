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
        public void Add(Car car)
        {
            using (var context = new ParcelsDbContext())
            {
                context.Cars.Add(car);
                context.SaveChanges();
            }
        }

        
    }
}
