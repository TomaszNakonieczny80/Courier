using System;
using Courier.DataLayer;
using Courier.DataLayer.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Courier.BusinessLayer
{
    public interface IUsersService
    {
        Task AddAsync(User user);
        bool CheckIfDriver(int userId);
        bool CheckIfPasswordIsCorrect(string userPassword);
        bool CheckIfUserExist(string userEmail);
        Task<User> GetCustomerIdAsync(string userEmail, string password);
        Task<User> GetCourierIdAsync(string userEmail, string password);
        double GetLatitude(int userId);
        double GetLongitude(int userId);
        User GetUser(int userId);
        Task<User> GetUserIdAsync(string userEmail);
    }
    public class UsersService : IUsersService
    {
        private readonly Func<IParcelsDbContext> _dbContextFactoryMethod;

        public UsersService(Func<IParcelsDbContext> dbContextFactoryMethod)
        {
            _dbContextFactoryMethod = dbContextFactoryMethod;
        }

        public async Task AddAsync(User user)
        {
            using (var context = _dbContextFactoryMethod())
            {
                context.Users.Add(user);
                await context.SaveChangesAsync();
            }
        }

        public bool CheckIfUserExist(string userEmail)
        {
            using (var context = _dbContextFactoryMethod())
            {
                var result = context.Users.FirstOrDefault(user => user.Email == userEmail);
                if (result == null)
                {
                    return false;
                }
                return true;
            }
        }

        public bool CheckIfPasswordIsCorrect(string userPassword)
        {
            using (var context = _dbContextFactoryMethod())
            {
                var result = context.Users.FirstOrDefault(user => user.Password == userPassword);
                if (result == null)
                {
                    return false;
                }
                return true;
            }
        }

        public bool CheckIfDriver(int userId)
        {
            using (var context = _dbContextFactoryMethod())
            {
                var result = context.Users.FirstOrDefault(user => user.Id == userId && user.UserType == UserType.Driver);
                if (result == null)
                {
                    return false;
                }
                return true;
            }
        }

        public async Task<User> GetUserIdAsync(string userEmail)
        {
            using (var context = _dbContextFactoryMethod())
            {
                var user = await context.Users.
                    AsQueryable().
                    FirstOrDefaultAsync(user => user.Email == userEmail);
               return user;
            }
        }

        public async Task<User> GetCourierIdAsync(string userEmail, string password)
        {
            using (var context = _dbContextFactoryMethod())
            {
                var userId = await context.Users
                    .AsQueryable()
                    .FirstOrDefaultAsync(user => user.Email == userEmail & user.Password == password & user.UserType == UserType.Driver);
                
                return userId;
            }
        }

        public async Task<User> GetCustomerIdAsync(string userEmail, string password)
        {
            using (var context = _dbContextFactoryMethod())
            {
                var userId = await context.Users
                    .AsQueryable()
                    .FirstOrDefaultAsync(user => user.Email == userEmail & user.Password == password);

                return userId;
            }
        }

        public User GetUser(int userId)
        {
            using (var context = _dbContextFactoryMethod())
            {
                var user = context.Users
                    .Include(address => address.Address)
                    .FirstOrDefault(user => user.Id == userId);
                return user;
            }
        }

        public double GetLatitude(int userId)
        {
            using (var context = _dbContextFactoryMethod())
            {
                var latitude = context.Addresses.FirstOrDefault(address => address.Id == userId).Latitude;
                return latitude;
            }
        }

        public double GetLongitude(int userId)
        {
            using (var context = _dbContextFactoryMethod())
            {
                var longitude = context.Addresses.FirstOrDefault(address => address.Id == userId).Longitude;
                return longitude;
            }
        }
    }
}
