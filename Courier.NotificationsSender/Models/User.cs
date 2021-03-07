using System;
using System.Collections.Generic;
using System.Text;

namespace Courier.NotificationsSender.Models
{
    public enum UserType
    {
        Customer,
        Driver
    }

    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public Address Address { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public UserType UserType { get; set; }

    }
}
