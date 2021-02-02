using System;
using System.Collections.Generic;
using System.Text;
using Courier.BusinessLayer;
using Courier.DataLayer.Models;

namespace Courier1
{
    class Program
    {
        private Menu _menu = new Menu();
        private IoHelper _ioHelper = new IoHelper();
        private DatabaseManagementService _databaseManagementService = new DatabaseManagementService();
        private UsersService _usersService = new UsersService();
        private bool _exit = false;

        static void Main()
        {
            new Program().Run();
        }
        void Run()
        {
            _databaseManagementService.EnsureDatabaseCreation();

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

        //private void StartMenu()
        //{
        //    Console.WriteLine("\nAvailable options:");
        //    _menu.ClearOption();
        //    _menu.AddOption(new MenuItem { Key = 1, Action = Login, Description = "Login" });
        //    _menu.AddOption(new MenuItem { Key = 2, Action = Registration, Description = "Registration" });
        //    _menu.AddOption(new MenuItem { Key = 3, Action = () => { _exit = true; }, Description = "Exit" });
        //}

        private void Menu()
        {
            Console.WriteLine("\nAvailable options:");
            _menu.ClearOption();
            _menu.AddOption(new MenuItem { Key = 1, Action = AddUser, Description = "Add User" });
            //_menu.AddOption(new MenuItem { Key = 2, Action = AddParcel, Description = "Add Parcel" });
            //_menu.AddOption(new MenuItem { Key = 2, Action = AddCourierCar, Description = "Add courier car" });

            _menu.AddOption(new MenuItem { Key = 3, Action = () => { _exit = true; }, Description = "Exit" });
        }

        void AddUser()
        {
            var firstName = _ioHelper.GetTextFromUser("\nEnter FirstName"); 
            var surname = _ioHelper.GetTextFromUser("\nEnter Surname");
            var email = _ioHelper.GetEmailFromUser("\nEnter email");
            Console.WriteLine("\nAddress:");
            Address address = new Address
            {
                Street = _ioHelper.GetTextFromUser("\nEnter street"),
                HouseNumber = _ioHelper.GetUintFromUser("\nEnter house number"),
                City = _ioHelper.GetTextFromUser("\nEnter city"),
                ZipCode = _ioHelper.GetTextFromUser("\nEnter zip code"),
            };
            var customerType = _ioHelper.GetUserType("\nEnter customer type");



            //var customerType = _ioHelper.GetTextFromUser("\nEnter zip code");


            //if (!_usersService.CheckIfUserExist(email))
            //{
            //    Console.WriteLine("\nUser not recognized");
            //    return;
            //}
            //var password = _ioHelper.GetTextFromUser("\nEnter password");
            //if (!_usersService.CheckIfPasswordIsCorrect(password))
            //{
            //    Console.WriteLine("\nIncorrect password");
            //    return;
            //}

            //Console.WriteLine("\nLogin was successful");

            //_userId = _usersService.GetUserId(email);
            //OpenMenuAfterLogin();
        }

    }
}
