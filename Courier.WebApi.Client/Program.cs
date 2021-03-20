using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Courier.WebApi.Client.Model;
using Newtonsoft.Json;


namespace Courier.WebApi.Client
{
    class Program
    {
        private int _userId;
        static void Main(string[] args)
        {
            new Program().Run();
        }

        private void Run()
        {
            Console.WriteLine("Available options:");
            Console.WriteLine("\n1. Login");
            Console.WriteLine("2. Exit");

            while (true)
            {
                var optionNo = GetIntFromUser("\nEnter option number");

                switch (optionNo)
                {
                    case 1:
                        Login();
                        break;
                    case 2:
                        Exit();
                        break;
                    default:
                        Console.WriteLine("Unknown option");
                        break;
                }
            }
        }
        private void Menu()
        {
            Console.WriteLine("\nAvailable options:");
            Console.WriteLine("\n1. Download Shipment list");
            Console.WriteLine("2. Exit");

            while (true)
            {
                var optionNo = GetIntFromUser("\nEnter option number");

                switch (optionNo)
                {
                    case 1:
                        DownloadShipmentList();
                        break;
                    case 2:
                        Exit();
                        break;
                    default:
                        Console.WriteLine("Unknown option");
                        break;
                }
            }
        }

        private void Exit()
        {
            System.Environment.Exit(0);
        }

        public class ResponseMessage
        {
            public string output { get; set; }
            public int Id { get; set; }
        }

        private void Login()
        {
           
            var email = GetTextFromUser("\nProvide email");
            var password = GetTextFromUser("Provide password");

            using (var httpClient = new HttpClient())
            {
                var response = httpClient
                    .GetAsync(@$"http://localhost:10500/api/login/courier?email={email}&password={password}").Result;
                var responseText = response.Content.ReadAsStringAsync().Result;

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var reponseObject = JsonConvert.DeserializeObject<ResponseMessage>(responseText);

                    if (reponseObject.Id == 0)
                    {
                        Console.WriteLine($"\nFailed. Email or password not recognized");
                        return;
                    }
                    else
                    {
                        Console.WriteLine($"\nLogin successful");
                        //_userId = int.Parse(responseText);
                        _userId = reponseObject.Id;
                        Menu();
                    }
                }
                else
                {
                    Console.WriteLine($"Failed. Status code: {response.StatusCode}");
                    return;
                }
            }
        }

        private void DownloadShipmentList()
        {
            using (var httpClient = new HttpClient())
            {
                var response = httpClient.GetAsync(@$"http://localhost:10500/api/shipmentlist/{_userId}").Result;
                var responseText = response.Content.ReadAsStringAsync().Result;

                if (response.IsSuccessStatusCode)
                {
                    if (responseText == null)
                    {
                        Console.WriteLine("\nYou don't have any parcel attached to delivery");
                    }
                    else
                    {
                        var responseObject = JsonConvert.DeserializeObject<List<Shipment>>(responseText);
                        Console.WriteLine($"Success. Response content: ");
                        foreach (var parcel in responseObject)
                        {
                            PrintShipmentList(parcel);
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Failed. Status code: {response.StatusCode}");
                }
            }

        }

        private void PrintShipmentList(Shipment shipment)
        {
            Console.WriteLine($"CarId: {shipment.CarId}, ParcelNumber: {shipment.ParcelNumber}, RegisterDate: {shipment.RegisterDate}");
        }

        private string GetTextFromUser(string message)
        {
            Console.Write($"{message}: ");
            return Console.ReadLine();
        }

        private int GetIntFromUser(string message)
        {
            int result;

            while (!int.TryParse(GetTextFromUser(message), out result))
            {
                Console.WriteLine("Incorrect. Try again...");
            }

            return result;
        }
    }
}
