using System;
using System.Collections.Generic;
using System.Text;
using Courier.DataLayer.Models;

namespace Courier
{
    class IoHelper
    {
        //public void PrintAccount(Account account, int index)
        //{
        //    Console.WriteLine($"{index}. {BuildAccountString(account)}");
        //}

        //public void PrintAccount(Account account)
        //{
        //    Console.WriteLine($"{BuildAccountString(account)}");
        //}

        //public void PrintTransfer(Transfer transfer, int index)
        //{
        //    Console.WriteLine($"{index}. {BuildTransferString(transfer)}");
        //}

        //public void PrintTransfer(Transfer transfer)
        //{
        //    Console.WriteLine($"{BuildTransferString(transfer)}");
        //}

        //public void PrintTransfer(AccountAmount transfer)
        //{
        //    Console.WriteLine($"{BuildTransferString(transfer)}");
        //}

        //private string BuildAccountString(Account account)
        //{
        //    return
        //        $"Account name: {account.AccountName}; Account number: {account.AccountNumber}; Balance:{account.Balance} USD";
        //}

        //private string BuildTransferString(Transfer transfer)
        //{
        //    return
        //        $"Transaction date: {transfer.TransactionDate}; Charged account number: {transfer.GuidChargedAccount}; Recipient account number: {transfer.GuidRecipientAccount}; Title: {transfer.TransferTitle}; Amount: {transfer.Amount} USD; Domestic transfer? {transfer.DomesticTransfer}";
        //}

        //private string BuildTransferString(AccountAmount transfer)
        //{
        //    return
        //        $"Account number: {transfer.AccountNumber}; Amount: {transfer.Amount} USD ";
        //}

        public Guid GetGuidFromUser(string message)
        {
            Guid result;

            while (!Guid.TryParse(GetTextFromUser(message), out result))
            {
                Console.WriteLine("\nWrong format of account number. Try again...");
            }
            return result;
        }

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

        public float GetFloatFromUser(string message)
        {
            float result;

            while (!float.TryParse(GetTextFromUser(message), out result))
            {
                Console.WriteLine("\nNot an float. Try again...");
            }

            return result;
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
