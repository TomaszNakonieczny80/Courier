using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Courier.BusinessLayer;
using Courier.BusinessLayer.Models;
using Courier.BusinessLayer.Serializers;
using Courier.DataLayer.Models;

namespace Courier
{
    class Program
    {
        private Menu _menu = new Menu();
        private IoHelper _ioHelper = new IoHelper();
        private DatabaseManagementService _databaseManagementService = new DatabaseManagementService();
        private UsersService _usersService = new UsersService();
        private ParcelsService _parcelsService = new ParcelsService();
        private CarsService _carsService = new CarsService();
        private CoordinatesService _coordinatesService = new CoordinatesService();
        private TimeService _timeService = new TimeService();
        private int _userId;



        private bool _exit = false;

        static void Main()
        {
            new Program().Run();
        }
        void Run()
        {
            SetTimer();
            _databaseManagementService.EnsureDatabaseCreation();
          
            LoginMenu();
            do
            {
                Console.WriteLine("");
                _menu.PrintAvailableOptions();

                int userChoice = _ioHelper.GetIntFromUser("\nSelect option");

                _menu.ExecuteOption(userChoice);
            }
            while (!_exit);
        }
        void OpenMainMenu()
        {
            Menu();
            do
            {
                Console.WriteLine("");
                _menu.PrintAvailableOptions();

                int userChoice = _ioHelper.GetIntFromUser("\nSelect option");

                _menu.ExecuteOption(userChoice);
            }
            while (!_exit);
        }

        private void LoginMenu()
        {
            Console.WriteLine("\nAvailable options:");
            _menu.ClearOption();
            _menu.AddOption(new MenuItem { Key = 1, Action = Login, Description = "Login" });
            _menu.AddOption(new MenuItem { Key = 2, Action = Registration, Description = "Registration" });
            _menu.AddOption(new MenuItem { Key = 3, Action = () => { _exit = true; }, Description = "Exit" });
        }
        private void Menu()
        {
            Console.WriteLine("\nAvailable options:");
            _menu.ClearOption();
            _menu.AddOption(new MenuItem { Key = 1, Action = AddParcel, Description = "Add parcel" });
            _menu.AddOption(new MenuItem { Key = 2, Action = AddCourierCar, Description = "Add courier car" });
            _menu.AddOption(new MenuItem { Key = 3, Action = () => { _exit = true; }, Description = "Exit" });
        }

        private void SetTimer()
        {
            var timeNow = _timeService.currentTime();
            var delta = timeNow.Date.AddDays(1) - timeNow;
            var deltaMillsek = delta.TotalMilliseconds;
            
            Timer aTimer = new Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
         
            aTimer.Interval = deltaMillsek/60;
            aTimer.Enabled = true;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            GenerateShipmentList();
        }
        void GenerateShipmentList()
        {
           CreateCarParcelBase();
           var shipmentList = _parcelsService.AttachDriverToParcel();
           foreach (var courier in shipmentList)
           {
               List<Shipment> courierShipmentList = shipmentList.Where(shipment => shipment.DriverId == courier.DriverId).ToList();

               
               var date = _timeService.currentTime().ToString(("MM-dd-yyyy"));
               var filePath = $@"C:\GitRepository\Courier\Courier\Shipment_List/DriverId{courier.DriverId}_{date}.json";

                var jsonDataSerializer = new JsonDataSerializer();
                jsonDataSerializer.Serialize(courierShipmentList, filePath);
           }

            //po wygenerowaniu raportu czyszcze tablice carParcel,
            //paczki nie przypisane beda uwzgleniane w kolejnym raporcie na pierwszym m-cu
            foreach (var carParcel in _parcelsService.GetAllCarParcel())
            {
                _parcelsService.Remove(carParcel);
            }          
        }


        void CreateCarParcelBase()
        {
            _parcelsService.CreatCarParcelsBase();
        }

        void AddParcel()
        {
            Console.WriteLine("\nEnter data of reciver:");
            var firstName = _ioHelper.GetTextFromUser("\nFirst name");
            var surname = _ioHelper.GetTextFromUser("\nSurname");
            var email = _ioHelper.GetEmailFromUser("\nEmail");
            var city = _ioHelper.GetTextFromUser("\nCity");
            var street = _ioHelper.GetTextFromUser("\nStreet");
            var houseNumber = _ioHelper.GetTextFromUser("\nHouse number");
            
            var zipCode = _ioHelper.GetTextFromUser("\nZip code (00000)");
            while (zipCode.Length != 5)
            {
                Console.WriteLine("\nWrong format of Zip code, try again...");
                zipCode = _ioHelper.GetTextFromUser("\nZip code (00000)");
            }

            var parcelSize = _ioHelper.GetParcelSize("\nParcel size");

            var jsonData = _coordinatesService.GetCoordinatesForAddress("Poland", city, street, houseNumber);
            if (jsonData == null)
            {
                Console.WriteLine("\nReceiver address not recognized");
                return;
            }
            var jsonSerializer = new JsonDataSerializer();
            var deserializedData = jsonSerializer.Deserialize(jsonData);
            var latitude = (double)deserializedData[0]["lat"];
            var longitude = (double)deserializedData[0]["lon"];

            Parcel newParcel = new Parcel
            {
                ParcelNumber = Guid.NewGuid(),
                Recipient = new User
                {
                    FirstName = firstName,
                    Surname = surname,
                    Email = email,
                    Address = new Address
                    {
                        Street = street,
                        HouseNumber = houseNumber,
                        City = city,
                        ZipCode = zipCode,
                        Latitude = latitude,
                        Longitude = longitude
                    },

                },
                SenderId = _userId,
                ParcelSize = parcelSize,
                RegisterDate = _timeService.currentTime(),
                ParcelStatus = ParcelStatus.WaitingToBePosted,
                Latitude = _usersService.GetLatitude(_userId),
                Longitude = _usersService.GetLongitude(_userId),
            };

            _parcelsService.Add(newParcel);
            Console.WriteLine("\nNew parcel added successfully");
        }

        void AddCourierCar()
        {
            if (!_usersService.CheckIfDriver(_userId))
            {
                Console.WriteLine("\nUser isn't a driver, select the other option");
                OpenMainMenu();
            }
            else
            {
                Console.WriteLine("\nEnter car data:");

                var capacity = _ioHelper.GetUintFromUser("\nCapacity (kg)");
                if (capacity < 200)
                {
                    Console.WriteLine("\n Min capacity required 500 kg");
                    return;
                }
                
                Car newCar = new Car
                {
                    Brand = _ioHelper.GetTextFromUser("\nBrand"),
                    Model = _ioHelper.GetTextFromUser("\nModel"),
                    RegistrationNumber = _ioHelper.GetTextFromUser("\nRegistration number"),
                    Capacity = capacity,
                    UserId = _userId,
                    Available = true ,
                    Latitude = _usersService.GetLatitude(_userId),
                    Longitude = _usersService.GetLongitude(_userId),
                };
                _carsService.Add(newCar);
                Console.WriteLine("\nNew car added successfully");
            }
        }
        void Login()
        {
            var email = _ioHelper.GetEmailFromUser("\nEnter email");
            if (!_usersService.CheckIfUserExist(email))
            {
                Console.WriteLine("\nUser not recognized");
                return;
            }
            var password = _ioHelper.GetTextFromUser("\nEnter password");
            if (!_usersService.CheckIfPasswordIsCorrect(password))
            {
                Console.WriteLine("\nIncorrect password");
                return;
            }

            Console.WriteLine("\nLogin was successful");

            _userId = _usersService.GetUserId(email);
            OpenMainMenu();
        }

        void Registration()
        {
            Console.WriteLine("\nEnter your personal data:");
            
            var email = _ioHelper.GetEmailFromUser("\nEmail");
            while (_usersService.CheckIfUserExist(email) == true)
            {
                Console.WriteLine("\nEmail already exist, try again...");
                email = _ioHelper.GetEmailFromUser("\nEnter email");
            }
            
            var password = _ioHelper.GetTextFromUser("\nPassword");
            var firstName = _ioHelper.GetTextFromUser("\nFirst name");
            var surname = _ioHelper.GetTextFromUser("\nSurname");
            var city = _ioHelper.GetTextFromUser("\nCity");
            var street = _ioHelper.GetTextFromUser("\nStreet");
            var houseNumber = _ioHelper.GetTextFromUser("\nHouse number");
            
            var zipCode = _ioHelper.GetTextFromUser("\nZip code (00000)");
            while (zipCode.Length != 5)
            {
                Console.WriteLine("\nWrong format of Zip code, try again...");
                zipCode = _ioHelper.GetTextFromUser("\nZip code (00000)");
            }
            
            
            var userType = _ioHelper.GetUserType("\nEnter customer type");

            var jsonData = _coordinatesService.GetCoordinatesForAddress("Poland", city, street, houseNumber);
            if (jsonData == null)
            {
                Console.WriteLine("\nAddress not recognized");
                return;
            }
            var jsonSerializer = new JsonDataSerializer();
            var deserializedData = jsonSerializer.Deserialize(jsonData);
            var latitude = (double)deserializedData[0]["lat"];
            var longitude = (double)deserializedData[0]["lon"];

            User newUser = new User
            {
                Password = password,
                FirstName = firstName,
                Surname = surname,
                Email = email,
                Address = new Address
                {
                    Street = street,
                    HouseNumber = houseNumber,
                    City = city,
                    ZipCode = zipCode,
                    Latitude = latitude,
                    Longitude = longitude,
                },
                UserType = userType,
            };
            _usersService.Add(newUser);

            Console.WriteLine("\nNew user added successfully");
        }
    }
}
