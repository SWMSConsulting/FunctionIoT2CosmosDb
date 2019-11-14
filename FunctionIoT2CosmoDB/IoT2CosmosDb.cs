using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventHubs;
using System.Text;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System.Security.Authentication;
using System;
using MongoDB.Bson;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace FunctionIoT2CosmoDB
{
    public static class IoT2CosmosDb
    {
        private static HttpClient client = new HttpClient();

        [FunctionName("Function1")]
        public static void Run([IoTHubTrigger("messages/events", Connection = "IoTConnectionString")]EventData message, ILogger log)
        {
            var mes = Encoding.UTF8.GetString(message.Body.Array);

            log.LogInformation($"C# IoT Hub trigger function processed a message: {mes}");

            var connectionString = System.Environment.GetEnvironmentVariable("MongoDbConnectionString", EnvironmentVariableTarget.Process);
            var databaseString = System.Environment.GetEnvironmentVariable("MongoDbDatabaseString", EnvironmentVariableTarget.Process);
            
            MongoClientSettings settings = MongoClientSettings.FromUrl(
                new MongoUrl(connectionString));

            settings.SslSettings =
              new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
            settings.RetryWrites = false;
            var mongoClient = new MongoClient(settings);

            var client = new MongoClient(settings);
            var database = client.GetDatabase(databaseString);

            var collection = database.GetCollection<BsonDocument>((string)message.SystemProperties["iothub-connection-device-id"]);

            mes = mes.Replace("true", "1");
            mes = mes.Replace("false", "0");

            Dictionary<string, double> values = JsonConvert.DeserializeObject<Dictionary<string, double>>(mes);

            DateTime created = (DateTime)message.SystemProperties["iothub-enqueuedtime"];

            var document = new DataPoint() { CreatedAtUtc = created, Values = values };

            collection.InsertOne(document.ToBsonDocument());

        }
    }
}