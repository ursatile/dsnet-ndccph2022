using System;
using System.IO;
using System.Threading.Tasks;
using Autobarn.Messages;
using Autobarn.PricingEngine;
using EasyNetQ;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;

namespace Autobarn.PricingClient {
    class Program {
        private static readonly IConfigurationRoot config = ReadConfiguration();

        static void Main(string[] args) {
            using var channel = GrpcChannel.ForAddress(config["AutobarnPricingServerUrl"]);
            var client = new Pricer.PricerClient(channel);
            var amqp = config.GetConnectionString("AutobarnRabbitMqConnectionString");
            using var bus = RabbitHutch.CreateBus(amqp);
            var handler = MakeHandler(client);
            bus.PubSub.Subscribe("Autobarn.PricingClient", handler);
            Console.WriteLine("Autobarn.PricingClient ready! Listening for NewVehicleMessages...");
            Console.ReadLine();
        }

        private static Func<NewVehicleMessage, Task> MakeHandler(Pricer.PricerClient client) {
            return async message => {
                Console.WriteLine(
                    $"Getting price for {message.ManufacturerName} {message.ModelName} ({message.Color}, {message.Year}");
                var priceRequest = new PriceRequest {
                    Color = message.Color,
                    Year = message.Year,
                    Make = message.ManufacturerName,
                    Model = message.ModelName
                };
                var priceReply = await client.GetPriceAsync(priceRequest);
                Console.WriteLine($"Price: {priceReply.Price} {priceReply.CurrencyCode}");
            };
        }

        private static IConfigurationRoot ReadConfiguration() {
            var basePath = Directory.GetParent(AppContext.BaseDirectory).FullName;
            return new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();
        }
    }
}
