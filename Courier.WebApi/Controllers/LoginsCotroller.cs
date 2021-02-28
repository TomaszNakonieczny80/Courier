using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Courier.BusinessLayer;
using Courier.DataLayer.Models;
using Microsoft.AspNetCore.Mvc;

namespace Courier.WebApi.Controllers
{
    [Route("api/login")]
    public class LoginsController : ControllerBase
    {
        private readonly IUsersService _usersService;

        public LoginsController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        /// <summary>
        /// Endpoint allowing to log in for all users
        /// </summary>
        ///
        /// <returns>user Id</returns>
        [HttpGet]
        public async Task<int?> GetCustomerId([FromQuery] string email, [FromQuery] string password)
        {
            var courierId = await _usersService.GetCustomerIdAsync(email, password);
            if (courierId == null)
            {
                throw new ArgumentException("User not recognized");
            }
            return courierId?.Id;
        }

        /// <summary>
        /// Endpoint allowing to log only for couriers 
        /// </summary>

        /// <returns>User Id</returns>
        [HttpGet("courier")]
        public async Task<int?> GetCourierId([FromQuery] string email, [FromQuery] string password)
        {
            var courierId = await _usersService.GetCourierIdAsync(email, password);
            if (courierId == null)
            {
                throw new ArgumentException("User not recognized");
            }
            return courierId?.Id;
        }
    }
}
