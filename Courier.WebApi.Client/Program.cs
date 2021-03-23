using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Courier.WebApi.Client.Model;
using Newtonsoft.Json;


namespace Courier.WebApi.Client
{
    class Program
    {
        private int _userId;
        private List<Shipment> _shipmentList;
        private List<Shipment> _shipments;
        static void Main(string[] args)
        {
            new Program().Run();
        }

        private void Run()
        {
          //  SetTimer();

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
            Console.WriteLine("\n1. Download shipment list");
            Console.WriteLine("2. Download shipment schedule");
            Console.WriteLine("3. Set picked up time");
            Console.WriteLine("4. Set parcel as delivered");
            Console.WriteLine("5. Download scoring");
            Console.WriteLine("6. Exit");

            while (true)
            {
                var optionNo = GetIntFromUser("\nEnter option number");

                switch (optionNo)
                {
                    case 1:
                        DownloadShipmentList();
                        break;
                    case 2:
                        DownloadShimpentSchedule();
                        break;
                    case 3:
                        SetParcelPickedUpTime();
                        break;
                    case 4:
                        SetParcelAsDelivered();
                        break;
                    case 5:
                        DownloadShipmentScoring();
                        break;
                    case 6:
                        Exit();
                        break;
                    default:
                        Console.WriteLine("Unknown option");
                        break;
                }
            }
        }

        public void DownloadShipmentScoring()
        {
            if (_shipmentList == null)
            {
                Console.WriteLine("\nNo shipment list available to score");
            }
            else
            {
                List<int> scorings = new List<int>();

                DownloadShipments();

                foreach (var parcel in _shipmentList)
                {
                    var scoring = _shipments.FirstOrDefault(shipment => shipment.CarId == parcel.CarId).Scoring;
                    scorings.Add(scoring);
                }

                var averageScoring = scorings.Average();

                Console.WriteLine($"\nYour delivery average scoring is: {averageScoring} points");
            }
        }

        public void DownloadShipments()
        {
            using (var httpClient = new HttpClient())
            {
                var response = httpClient.GetAsync($"http://localhost:10500/api/shipmentlist/shipments/{_userId}").Result;
                var responseText = response.Content.ReadAsStringAsync().Result;

                if (response.IsSuccessStatusCode)
                {
                    var responseObject = JsonConvert.DeserializeObject<List<Shipment>>(responseText);
                    _shipments = responseObject;
                }
                else
                {
                    Console.WriteLine($"Http query failure. Status code: {response.StatusCode}");
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

        public void SetParcelPickedUpTime()
        {
            var parcelId = GetIntFromUser("\nEnter parcel Id");
            if (_shipmentList == null || _shipmentList.FirstOrDefault(shipment => shipment.ParcelId == parcelId) == null)
            {
                Console.WriteLine("\nWrong Parcel number");
                return;
            }

            using (var httpClient = new HttpClient())
            {
                var response = httpClient.PostAsync($"http://localhost:10500/api/shipmentlist/pickedup/{parcelId}", null).Result;
                var responseText = response.Content.ReadAsStringAsync().Result;

                if (response.IsSuccessStatusCode)
                {
                    if (responseText == "0")
                    {
                        Console.WriteLine("\nWrong number or parcel doesn't exist");
                    }
                    else
                    {
                        Console.WriteLine("\nPicked up time was set.");
                    }
                }
                else
                {
                    Console.WriteLine($"Http query failure. Status code: {response.StatusCode}");
                }
            }
        }

        public void SetParcelAsDelivered()
        {
            var parcelId = GetIntFromUser("\nEnter parcel Id");
            if (_shipmentList == null || _shipmentList.FirstOrDefault(shipment => shipment.ParcelId == parcelId) == null)
            {
                Console.WriteLine("\nWrong Parcel number");
                return;
            }

            using (var httpClient = new HttpClient())
            {
                var response = httpClient.PostAsync($"http://localhost:10500/api/shipmentlist/delivered/{parcelId}", null).Result;
                var responseText = response.Content.ReadAsStringAsync().Result;

                if (response.IsSuccessStatusCode)
                {
                    if (responseText == "0")
                    {
                        Console.WriteLine("\nWrong number or parcel doesn't exist");
                    }
                    else
                    {
                        Console.WriteLine("\nParcel was set as delivered.");
                    }
                }
                else
                {
                    Console.WriteLine($"Http query failure. Status code: {response.StatusCode}");
                }
            }
        }


        private void DownloadShipmentList()
        {
            if (_shipmentList == null)
            {
                using (var httpClient = new HttpClient())
                {
                    var response = httpClient.GetAsync(@$"http://localhost:10500/api/shipmentlist/{_userId}").Result;
                    var responseText = response.Content.ReadAsStringAsync().Result;

                    if (response.IsSuccessStatusCode)
                    {
                        if (responseText == "[]")
                        {
                            Console.WriteLine("\nYou don't have any parcel attached to delivery");
                            _shipmentList = null;
                        }
                        else
                        {
                            var responseObject = JsonConvert.DeserializeObject<List<Shipment>>(responseText);
                            Console.WriteLine($"Success. Response content: ");
                            foreach (var parcel in responseObject)
                            {
                                PrintShipmentList(parcel);
                            }

                            _shipmentList = responseObject;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Failed. Status code: {response.StatusCode}");
                    }
                }
                
            }

            else
            {
                Console.WriteLine($"\nSuccess. Response content: ");
                foreach (var parcel in _shipmentList)
                {
                    PrintShipmentList(parcel);
                }
            }
        }

        private void DownloadShimpentSchedule()
        {
            if (_shipmentList == null)
            {
                using (var httpClient = new HttpClient())
                {
                    var response = httpClient.GetAsync(@$"http://localhost:10500/api/shipmentlist/{_userId}").Result;
                    var responseText = response.Content.ReadAsStringAsync().Result;

                    if (response.IsSuccessStatusCode)
                    {
                        if (responseText == "[]")
                        {
                            Console.WriteLine("\nYou don't have any parcel attached to delivery");
                            _shipmentList = null;
                        }
                        else
                        {
                            var responseObject = JsonConvert.DeserializeObject<List<Shipment>>(responseText);
                            Console.WriteLine($"Success. Response content: ");
                            foreach (var parcel in responseObject)
                            {
                                PrintShipmentSchedule(parcel);
                            }

                            _shipmentList = responseObject;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Failed. Status code: {response.StatusCode}");
                    }
                }
            }

            else
            {
                Console.WriteLine($"\nSuccess. Response content: ");
                foreach (var parcel in _shipmentList)
                {
                    PrintShipmentSchedule(parcel);
                }
            }
        }

        private void PrintShipmentList(Shipment shipment)
        {
            Console.WriteLine($"Parcel Id: {shipment.ParcelId}, Parcel number: {shipment.ParcelNumber}, Register date: {shipment.RegisterDate}");
        }

        private void PrintShipmentSchedule(Shipment shipment)
        {
            Console.WriteLine($"Parcel Id: {shipment.ParcelId}, Distance to parcel: {shipment.DistanceToParcel} km, Travel time to parcel: {shipment.TravelTimeToParcel} hours, Distance to recipient: {shipment.DistanceToRecipient} km, Travel time to recipient: {shipment.TravelTimeToRecipient} hours");
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
