using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;


namespace Courier.BusinessLayer
{
    public interface ICoordinatesService
    {
        string GetCoordinatesForAddress(string country, string city, string street, string building);
    }
    public class CoordinatesService : ICoordinatesService
    {
        public string GetCoordinatesForAddress(string country, string city, string street, string building)
        {
            var client = new RestClient($"https://nominatim.openstreetmap.org/?q={street}+{building}+{city}+{country}&format=json&limit=1");
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);

            var jsonData = response.Content;
            if (jsonData == "[]")
            {

                return null;
            }
           
            return jsonData;
        }
    }
}
