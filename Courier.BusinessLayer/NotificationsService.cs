
using System.Collections.Generic;
using EventStore.Client;
using System.Text.Json;
using Courier.DataLayer.Models;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Newtonsoft.Json;
using EventData = EventStore.Client.EventData;

namespace Courier.BusinessLayer
{
    public interface INotificationsService
    {
        void NotifyParcelsDelivered(List<Parcel> newParcels);
        EventData GetEventDataFor(List<Parcel> data);
    }

    public class NotificationsService : INotificationsService
    {
        public void NotifyParcelsDelivered(List<Parcel> newParcels)
        {
            const string stream = "courier_parcelDelivered";
            const int defaultPort = 2113;

            var settings = EventStoreClientSettings.Create($"esdb://127.0.0.1:{defaultPort}?Tls=false");

            using (var client = new EventStoreClient(settings))
            {
                client.AppendToStreamAsync(
                    stream,
                    StreamState.Any,
                    new[] { GetEventDataFor(newParcels) }).Wait();
            }
        }

        public  EventData GetEventDataFor(List<Parcel> data)
        {
            return new EventData(
                Uuid.NewUuid(),
                "parcel_delivered",
                System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(data));
        }
    }
}