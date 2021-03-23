using System.Collections.Generic;
using System.Threading.Tasks;
using Courier.BusinessLayer;
using Courier.DataLayer.Models;
using Microsoft.AspNetCore.Mvc;

namespace Courier.WebApi.Controllers
{
    [Route("api/shipmentlist")]
    public class ShipmentListsController : ControllerBase
    {
        private readonly IParcelsService _parcelsService;
        private readonly IShipmentsService _shipmentsService;

        public ShipmentListsController(IParcelsService parcelsService, IShipmentsService shipmentsService)
        {
            _parcelsService = parcelsService;
            _shipmentsService = shipmentsService;
        }

        /// <summary>
        /// Endpoint allowing to get shipment list for user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>List of parcels to delivery by user</returns>

        [HttpGet("{userId}")]
        public async Task<List<Shipment>> GetShipmentList(int userId)
        {
            return await _parcelsService.GetShipmentListAsync(userId);
        }

        /// <summary>
        /// Endpoint allowing to set parcel picked up time
        /// </summary>
        /// <param name="parcelId"></param>
        
        [HttpPost("pickedup/{parcelId}")]
        public async Task PostParcelPicedUp(int parcelId)
        {
            await _shipmentsService.SetPickedUpTimeAsync(parcelId);
        }


        /// <summary>
        /// Endpoint allowing to set parcel as delivered
        /// </summary>
        /// <param name="parcelId"></param>
        
        [HttpPost("delivered/{parcelId}")]
        public async Task PostParcelDelivered(int parcelId)
        {
            _shipmentsService.SetDeliveredTimeAsync(parcelId).Wait();
            
            _parcelsService.SetParcelAsDeliveredAsync(parcelId).Wait();

            await _shipmentsService.SetDeliveryScoringAsync(parcelId);
        }

        /// <summary>
        /// Endpoint allowing to download shipments
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>List of shipments for user</returns>

        [HttpGet("shipments/{userId}")]
        public async Task<List<Shipment>> GetAll(int userId)
        {
            return await _shipmentsService.GetShipmentsAsync(userId);
        }
    }
}
