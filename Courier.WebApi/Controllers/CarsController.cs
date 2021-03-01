
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Courier.BusinessLayer;
using Courier.DataLayer.Models;



namespace Courier.WebApi.Controllers
{
    [Route("api/cars")]
    public class CarsController : ControllerBase
    {
        private readonly ICarsService _carsService;

        public CarsController(ICarsService carsService)
        {
            _carsService = carsService;
        }

        /// <summary>
        /// Endpoint allowing to add car
        /// </summary>
        [HttpPost]
        public async Task PostCar([FromBody] Car car)
        {
            await _carsService.AddAsync(car);
        }
    }
}
