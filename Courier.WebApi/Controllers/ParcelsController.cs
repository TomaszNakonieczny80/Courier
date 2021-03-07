using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Courier.BusinessLayer;
using Courier.DataLayer.Models;
using Microsoft.AspNetCore.Mvc;

namespace Courier.WebApi.Controllers
{
    [Route("api/parcels")]
    public class ParcelsController : ControllerBase
    {
        private readonly IParcelsService _parcelsService;

        public ParcelsController(IParcelsService parcelsService)
        {
            _parcelsService = parcelsService;
        }

        /// <summary>
        /// Endpoint allowing to add parcel
        /// </summary>
        [HttpPost]
        public async Task PostParcel([FromBody] Parcel parcel)
        {
            await _parcelsService.AddAsync(parcel);
        }
    }
}
