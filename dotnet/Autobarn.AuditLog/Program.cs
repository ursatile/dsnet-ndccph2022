using System;
using System.IO;
using Autobarn.Messages;
using EasyNetQ;
using Microsoft.Extensions.Configuration;

namespace Autobarn.AuditLog {
    internal class Program {
        private static readonly IConfigurationRoot config = ReadConfiguration();

        static void Main(string[] args) {
            var amqp = config.GetConnectionString("AutobarnRabbitMqConnectionString");
            using var bus = RabbitHutch.CreateBus(amqp);
            bus.PubSub.Subscribe<NewVehicleMessage>("Autobarn.AuditLog", HandleNewVehicleMessage);
            Console.WriteLine("Listening for NewVehicleMessages...");
            Console.ReadLine();
        }

        private static void HandleNewVehicleMessage(NewVehicleMessage nvm) {
            var csv =
                $"{nvm.Registration},{nvm.ModelName},{nvm.ManufacturerName},{nvm.Color},{nvm.Year},{nvm.ListedAt}";
            Console.WriteLine(csv);
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
