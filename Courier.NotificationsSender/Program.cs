using EventStore.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Courier.NotificationsSender.Models;

namespace BookStore.NotficationsSender
{
    class Program
    {
        static void Main(string[] args)
        {
            const string stream = "courier_parcelDelivered";
            const int defaultPort = 2113;

            var settings = EventStoreClientSettings.Create($"esdb://127.0.0.1:{defaultPort}?Tls=false");

            using (var client = new EventStoreClient(settings))
            {
                client.SubscribeToStreamAsync(
                    stream,
                    StreamPosition.End,
                    EventArrived);

                Console.WriteLine("Press any key to close...");
                Console.ReadLine();
            }
        }

        private async static Task EventArrived(
            StreamSubscription subscription,
            ResolvedEvent resolvedEvent,
            CancellationToken cancellationToken)
        {
            var jsonData = Encoding.UTF8.GetString(resolvedEvent.Event.Data.ToArray());

            var parcelsDelivered = JsonConvert.DeserializeObject<List<Parcel>>(jsonData);

            foreach (var parcel in parcelsDelivered)
            {
                Console.WriteLine($"\nParcel number: {parcel.ParcelNumber}");
                Console.WriteLine($"Recipient data:");
                Console.WriteLine($"Name: {parcel.Recipient.FirstName}");
                Console.WriteLine($"Surname: {parcel.Recipient.Surname}");
                Console.WriteLine($"Street: {parcel.Recipient.Address.Street}");
                Console.WriteLine($"House number: {parcel.Recipient.Address.HouseNumber}");
                Console.WriteLine($"Zip code: {parcel.Recipient.Address.ZipCode}");
                Console.WriteLine($"City: {parcel.Recipient.Address.City}");
                Console.WriteLine($"Email address of sender: {parcel.Sender.Email}");
            }
            
        }
    }
}
