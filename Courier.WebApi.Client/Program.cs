using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Timers;
using Courier.WebApi.Client.Model;
using Newtonsoft.Json;


namespace Courier.WebApi.Client
{
    class Program
    {
        private int _userId;
        private List<Shipment> _shipments;
        static void Main(string[] args)
        {
            new Program().Run();
        }

        private void Run()
        {
            SetTimer();

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
            Console.WriteLine("2. Download shimpent schedule");
            Console.WriteLine("3. Set parcel as picked up");
            Console.WriteLine("4. Set parcel as delivered");
         //   Console.WriteLine("\n5. Download Shipment list");
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
                        SetParcelAsPickedUp();
                        break;
                    //case 4:
                    //    SetParcelAsDelivered;
                    //    break;
                    //case 1:
                    //    DownloadShipmentList();
                    //    break;
                    case 6:
                        Exit();
                        break;
                    default:
                        Console.WriteLine("Unknown option");
                        break;
                }
            }
        }

        private void SetTimer()
        {
            var timeService = new TimeService();
            var timeNow = timeService.currentTime();
            var delta = timeNow.Date.AddDays(1) - timeNow;
            var deltaMilisec = delta.TotalMilliseconds;
            var eightHoursMilisec = 28800000;
            var eighteenHoursMilisec = 64800000;
            var timeMultiplier = 600;

            Timer aTimerForRaport = new Timer();
            aTimerForRaport.Elapsed += new ElapsedEventHandler(GenerateShipmentReport);

            aTimerForRaport.Interval = deltaMilisec / timeMultiplier;
            aTimerForRaport.Enabled = true;
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

        public void GenerateShipmentReport(object source, ElapsedEventArgs e)
        {
            using (var httpClient = new HttpClient())
            {
                var response = httpClient.PostAsync($"http://localhost:10500/api/shipmentlist", null).Result;
                var responseText = response.Content.ReadAsStringAsync().Result;

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("\nNew shipment report was created.");
                }
                else
                {
                    Console.WriteLine($"Http query failure. Status code: {response.StatusCode}");
                }
            }
        }

        public void SetParcelAsPickedUp()
        {
            var parcelId = GetIntFromUser("\nEnter parcel Id");

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
                        Console.WriteLine("\nNew shipment report was created.");
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
            if (_shipments == null)
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
                            _shipments = null;
                        }
                        else
                        {
                            var responseObject = JsonConvert.DeserializeObject<List<Shipment>>(responseText);
                            Console.WriteLine($"Success. Response content: ");
                            foreach (var parcel in responseObject)
                            {
                                PrintShipmentList(parcel);
                            }

                            _shipments = responseObject;
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
                Console.WriteLine($"Success. Response content: ");
                foreach (var parcel in _shipments)
                {
                    PrintShipmentList(parcel);
                }
            }
        }

        private void DownloadShimpentSchedule()
        {
            if (_shipments == null)
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
                            _shipments = null;
                        }
                        else
                        {
                            var responseObject = JsonConvert.DeserializeObject<List<Shipment>>(responseText);
                            Console.WriteLine($"Success. Response content: ");
                            foreach (var parcel in responseObject)
                            {
                                PrintShipmentSchedule(parcel);
                            }

                            _shipments = responseObject;
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
                Console.WriteLine($"Success. Response content: ");
                foreach (var parcel in _shipments)
                {
                    PrintShipmentSchedule(parcel);
                }
            }
        }

        private void PrintShipmentList(Shipment shipment)
        {
            Console.WriteLine($"CarId: {shipment.CarId}, ParcelId: {shipment.ParcelId}, ParcelNumber: {shipment.ParcelNumber}, RegisterDate: {shipment.RegisterDate}");
        }

        private void PrintShipmentSchedule(Shipment shipment)
        {
            Console.WriteLine($"ParcelId: {shipment.ParcelId}, DistanceToParcel: {shipment.DistanceToParcel}km, TravelTimeToParcel: {shipment.TravelTimeToParcel}hours, DistanceToRecipient: {shipment.DistanceToRecipient}km, TravelTimeToRecipient: {shipment.TravelTimeToRecipient}hours");
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
