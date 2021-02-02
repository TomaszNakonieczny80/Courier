using System;
using System.Collections.Generic;
using System.Text;
using Courier.DataLayer;
using Courier.DataLayer.Models;

namespace Courier.BusinessLayer
{
    public class CarsService
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
