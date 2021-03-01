using System.Collections.Generic;
using System.Threading.Tasks;
using Courier.BusinessLayer;
using Courier.BusinessLayer.Models;
using Microsoft.AspNetCore.Mvc;

namespace Courier.WebApi.Controllers
{
    [Route("api/shipmentlist")]
    public class ShipmentListsController : ControllerBase
    {
        private readonly IParcelsService _parcelsService;

        public ShipmentListsController(IParcelsService parcelsService)
        {
            _parcelsService = parcelsService;
        }

        /// <summary>
        /// Endpoint allowing to get shipment list for user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>List of parcels to delivery by user</returns>

        [HttpGet("{userId}")]
        public async Task<List<Shipment>> GetShipmentList(int userId)
        {
            return await _parcelsService.GenerateShipmentListAsync(userId);
        }
    }
}
