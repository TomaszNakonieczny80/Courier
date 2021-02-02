using System;
using System.Collections.Generic;
using System.Text;
using Courier.DataLayer;
using Courier.DataLayer.Models;
using System.Linq;

namespace Courier.BusinessLayer
{
    public class UsersService
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
        
        //public int GetUserId(Guid accountNumerOfRecipient)
        //{
        //    using (var context = new ParcelsDbContext())
        //    {
        //        var userId = context.Accounts.FirstOrDefault(user => user.AccountNumber == accountNumerOfRecipient).UserId;
        //        if (userId == null)
        //        {
        //            return 0;
        //        }

        //        return userId;
        //    }
        //}
    }
}
