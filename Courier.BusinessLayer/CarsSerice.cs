using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Courier.DataLayer;
using Courier.DataLayer.Models;
using System.Threading.Tasks;

namespace Courier.BusinessLayer
{
    public interface ICarsService
    {
        Task AddAsync(Car car);
        int GetCarId(int userId);
    }
    public class CarsService : ICarsService
    {
        private readonly Func<IParcelsDbContext> _dbContextFactoryMethod;

        public CarsService(Func<IParcelsDbContext> dbContextFactoryMethod)
        {
            _dbContextFactoryMethod = dbContextFactoryMethod;
        }

        public async Task AddAsync(Car car)
        {
            using (var context = _dbContextFactoryMethod())
            {
                context.Cars.Add(car);
                await context.SaveChangesAsync();
            }
        }
        public int GetCarId(int userId)
        {
            using (var context = _dbContextFactoryMethod())
            {
                return context.Cars.FirstOrDefault(user => user.UserId == userId).Id;
            }
        }
    }
}
