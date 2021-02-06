using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Sockets;
using System.Text;

namespace Courier.DataLayer.Models
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
