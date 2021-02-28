using System;
using System.Collections.Generic;
using System.Text;
using Courier.DataLayer.Models;

namespace Courier
{
    public interface IIoHelper
    {
        string GetEmailFromUser(string message);
        int GetIntFromUser(string message);
        ParcelSize GetParcelSize(string message);
        string GetTextFromUser(string message);
        uint GetUintFromUser(string message);
        UserType GetUserType(string message);
    }
    public class IoHelper : IIoHelper
    {
        
        public int GetIntFromUser(string message)
        {
            int number;
            while (!int.TryParse(GetTextFromUser(message), out number))
            {
                Console.WriteLine("\nNot na integer - try again...");
            }

            return number;
        }

        public string GetTextFromUser(string message)
        {
            Console.Write($"{message}: ");
            return Console.ReadLine();
        }

        public string GetEmailFromUser(string message)
        {
            Console.Write($"{message}: ");
            string email = Console.ReadLine();
            while (email.IndexOf("@") <= 0)
            {
                Console.WriteLine("\nWrong format of email, try again");
                email = GetTextFromUser("\nEnter email");
            }

            return email;
        }

        public uint GetUintFromUser(string message)
        {
            uint result;

            while (!uint.TryParse(GetTextFromUser(message), out result))
            {
                Console.WriteLine("Not an unsinged integer. Try again...");
            }

            return result;
        }

        public UserType GetUserType(string message)
        {
            var correctValues = "";

            foreach (var userType in (UserType[])Enum.GetValues(typeof(UserType)))
            {
                correctValues += $"{userType},";
            }

            object result;
            while (!Enum.TryParse(typeof(UserType), GetTextFromUser($"{message} ({correctValues})"), out result))
            {
                Console.WriteLine("Not a correct value - use one from the brackets. Try again...");
            }

            return (UserType)result;
        }

        public ParcelSize GetParcelSize(string message)
        {
            var correctValues = "";

            foreach (var parcelSize in (ParcelSize[])Enum.GetValues(typeof(ParcelSize)))
            {
                correctValues += $"{parcelSize},";
            }

            object result;
            while (!Enum.TryParse(typeof(ParcelSize), GetTextFromUser($"{message} ({correctValues})"), out result))
            {
                Console.WriteLine("Not a correct value - use one from the brackets. Try again...");
            }

            return (ParcelSize)result;
        }
    }
}
