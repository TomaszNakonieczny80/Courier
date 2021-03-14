using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;
using Unity;
using Courier.BusinessLayer;
using Courier.BusinessLayer.Serializers;
using Courier.DataLayer.Models;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;

namespace Courier
{
    class Program
    {
        private IMenu _menu;
        private IIoHelper _ioHelper;
        private IDatabaseManagementService _databaseManagementService;
        private IUsersService _usersService;
        private IParcelsService _parcelsService;
        private ICarsService _carsService;
        private ICoordinatesService _coordinatesService;
        private ITimeService _timeService;
        private INotificationsService _notificationService;
        private int _userId;



        private bool _exit = false;

        static void Main()
        {
            var container = new UnityDiContainerProvider().GetContainer();
            container.Resolve<Program>().Run();
        }

        public Program(
            IMenu menu,
            IIoHelper ioHelper,
            IDatabaseManagementService databaseManagementService,
            IUsersService usersService,
            IParcelsService parcelsService,
            ICarsService carsService,
            ICoordinatesService coordinatesService,
            ITimeService timeService,
            INotificationsService notificationService)
        {
            _menu = menu;
            _ioHelper = ioHelper;
            _databaseManagementService = databaseManagementService;
            _usersService = usersService;
            _parcelsService = parcelsService;
            _carsService = carsService;
            _coordinatesService = coordinatesService;
            _timeService = timeService;
            _notificationService = notificationService;
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
            var deltaMilisec = delta.TotalMilliseconds;
            var eightHoursMilisec = 28800000;
            var eighteenHoursMilisec = 64800000;

            Timer aTimerForRaport = new Timer();
            aTimerForRaport.Elapsed += new ElapsedEventHandler(ShipementRaport);

            aTimerForRaport.Interval = deltaMilisec / 60;
            aTimerForRaport.Enabled = true;

            Timer aTimerForStartDelivery = new Timer();
            aTimerForStartDelivery.Elapsed += new ElapsedEventHandler(SetStatusAsOnTheWay);

            aTimerForStartDelivery.Interval = (deltaMilisec + eightHoursMilisec) / 60;
            aTimerForStartDelivery.Enabled = true;

            Timer aTimerForStopDelivery = new Timer();
            aTimerForStopDelivery.Elapsed += new ElapsedEventHandler(SetStatusOnDelivered);

            aTimerForStopDelivery.Interval = (deltaMilisec + eighteenHoursMilisec) / 60;
            aTimerForStopDelivery.Enabled = true;


        }

        private void ShipementRaport(object source, ElapsedEventArgs e)
        {
            GenerateShipmentList();
        }

        private void SetStatusAsOnTheWay(object source, ElapsedEventArgs e)
        {
            _parcelsService.SetParcelsAsOnTheWay();
        }

        private void SetStatusOnDelivered(object source, ElapsedEventArgs e)
        {
            var parcelsOnTheWay = _parcelsService.GetParcelsOnTheWay();
            if (parcelsOnTheWay != null)
            {
                _notificationService.NotifyParcelsDelivered(parcelsOnTheWay);
            }
            _parcelsService.SetParcelsAsDelivered(parcelsOnTheWay);
        }
     
        void GenerateShipmentList()
        {
          _parcelsService.GenerateShipmentList();
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
            var sender = _usersService.GetUser(_userId);
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
                Sender = new User
                {
                    FirstName = sender.FirstName,
                    Surname = sender.Surname,
                    Email = sender.Email,
                    Address = new Address
                    {
                        Street = sender.Address.Street,
                        HouseNumber = sender.Address.HouseNumber,
                        City = sender.Address.City,
                        ZipCode = sender.Address.ZipCode,
                        Latitude = sender.Address.Latitude,
                        Longitude = sender.Address.Longitude,
                    },
                },
                ParcelSize = parcelSize,
                RegisterDate = _timeService.currentTime(),
                ParcelStatus = ParcelStatus.WaitingToBePosted,
                ParcelLatitude = _usersService.GetLatitude(_userId),
                ParcelLongitude = _usersService.GetLongitude(_userId),
                RecipientLatitude = latitude,
                RecipientLongitude = longitude,

            };

            _parcelsService.AddAsync(newParcel).Wait();
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

                var averageSpeed = _ioHelper.GetUintFromUser("\nAverage speed km/h");
                if (averageSpeed < 0)
                {
                    Console.WriteLine("\n min speed cant be equal or less than 0 , try again...");
                    return;
                }

                Car newCar = new Car
                {
                    Brand = _ioHelper.GetTextFromUser("\nBrand"),
                    Model = _ioHelper.GetTextFromUser("\nModel"),
                    RegistrationNumber = _ioHelper.GetTextFromUser("\nRegistration number"),
                    AverageSpeed = averageSpeed,
                    Capacity = capacity,
                    UserId = _userId,
                    Available = true ,
                    Latitude = _usersService.GetLatitude(_userId),
                    Longitude = _usersService.GetLongitude(_userId),
                };
                _carsService.AddAsync(newCar).Wait();
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
            var user = _usersService.GetUserIdAsync(email).Result;
            _userId = user.Id;
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
            _usersService.AddAsync(newUser).Wait();

            Console.WriteLine("\nNew user added successfully");
        }
    }
}
