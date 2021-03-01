using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Courier.BusinessLayer;
using Courier.DataLayer.Models;
using Microsoft.AspNetCore.Mvc;

namespace Courier.WebApi.Controllers
{
    [Route("api/users")]
    public class RegistrationsController : ControllerBase
    {
        private readonly IUsersService _usersService;

        public RegistrationsController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        /// <summary>
        /// Endpoint allowing to register user
        /// </summary>
        [HttpPost]
        public async Task PostUserAddress([FromBody] User user)
        {
            await _usersService.AddAsync(user);
        }

    }
}
