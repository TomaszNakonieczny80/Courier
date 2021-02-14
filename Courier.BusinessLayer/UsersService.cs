using System;
using System.Collections.Generic;
using System.Text;
using Courier.DataLayer;
using Courier.DataLayer.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Courier.BusinessLayer
{
    public interface IUsersService
    {
        void Add(User user);
        bool CheckIfDriver(int userId);
        bool CheckIfPasswordIsCorrect(string userPassword);
        bool CheckIfUserExist(string userEmail);
        double GetLatitude(int userId);
        double GetLongitude(int userId);
        User GetUser(int userId);
        int GetUserId(string userEmail);
    }
    public class UsersService : IUsersService
    {
        public void Add(User user)
        {
            using (var context = new ParcelsDbContext())
            {
                context.Users.Add(user);
                context.SaveChanges();
            }
        }

        public bool CheckIfUserExist(string userEmail)
        {
            using (var context = new ParcelsDbContext())
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
            using (var context = new ParcelsDbContext())
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
            using (var context = new ParcelsDbContext())
            {
                var result = context.Users.FirstOrDefault(user => user.Id == userId && user.UserType == UserType.Driver);
                if (result == null)
                {
                    return false;
                }
                return true;
            }
        }

        public int GetUserId(string userEmail)
        {
            using (var context = new ParcelsDbContext())
            {
                var userId = context.Users.FirstOrDefault(user => user.Email == userEmail).Id;
                return userId;
            }
        }

        public User GetUser(int userId)
        {
            using (var context = new ParcelsDbContext())
            {
                var user = context.Users
                    .Include(address => address.Address)
                    .FirstOrDefault(user => user.Id == userId);
                return user;
            }
        }

        public double GetLatitude(int userId)
        {
            using (var context = new ParcelsDbContext())
            {
                var latitude = context.Addresses.FirstOrDefault(address => address.Id == userId).Latitude;
                return latitude;
            }
        }

        public double GetLongitude(int userId)
        {
            using (var context = new ParcelsDbContext())
            {
                var longitude = context.Addresses.FirstOrDefault(address => address.Id == userId).Longitude;
                return longitude;
            }
        }
    }
}
