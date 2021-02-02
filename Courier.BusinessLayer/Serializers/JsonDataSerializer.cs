using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Courier.BusinessLayer.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Courier.BusinessLayer.Serializers
{
    public class JsonDataSerializer
    {
        public void Serialize(List<Shipment> shipmentList, string filePath)
        {
           
            var jsonData = JsonConvert.SerializeObject(shipmentList, Formatting.Indented);
            File.WriteAllText(filePath, jsonData);
            
        }
        public JArray Deserialize(string jsonData)
        {
            return (JArray)JsonConvert.DeserializeObject(jsonData);
        }


      

    }
}
