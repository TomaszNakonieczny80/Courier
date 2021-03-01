using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Courier.BusinessLayer;
using Courier.WebApi.Models;
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
        /// <returns>User Id</returns>
        [HttpGet]
        public async Task<ResponseMessage> GetCustomerId ([FromQuery] string email, [FromQuery] string password)
        {

            var courier = await _usersService.GetCustomerIdAsync(email, password);
            if (courier == null)
            {
                var unauthorizedResponse = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                        Content = new StringContent("User not recognized", Encoding.UTF8, "application/json")
                };
                
                return new ResponseMessage() {Output = "User not recognized"}; 
            }

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(courier.Id.ToString(), Encoding.UTF8, "application/json")
            };
            
            return new ResponseMessage()
            {
                Id = courier.Id,
                Output = "succeeded"
            };
        }

        /// <summary>
        /// Endpoint allowing to log in only for couriers 
        /// </summary>
        /// <returns>User Id</returns>
        [HttpGet("courier")]
        public async Task<ResponseMessage> GetCourierId([FromQuery] string email, [FromQuery] string password)
        {
            var courier = await _usersService.GetCourierIdAsync(email, password);
            if (courier == null)
            {
                var unauthorizedResponse = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent("User not recognized", Encoding.UTF8, "application/json")
                };

                return new ResponseMessage() { Output = "User not recognized" };
            }

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(courier.Id.ToString(), Encoding.UTF8, "application/json")
            };

            return new ResponseMessage()
            {
                Id = courier.Id,
                Output = "succeeded"
            };
        }
    }
}
