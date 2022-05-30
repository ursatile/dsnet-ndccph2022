using EasyNetQ;
using Messages;

const string AMQP = "amqps://jiskiydu:O964LyptH8ATxnX8-k4GtdGdzNAIbcQe@hummingbird.rmq.cloudamqp.com/jiskiydu";
var bus = RabbitHutch.CreateBus(AMQP);
await bus.PubSub.SubscribeAsync<Greeting>("dylanbeattie", Handle);
Console.WriteLine("Listening for messages...");
Console.ReadLine();
Console.WriteLine("Exiting! Thank you!");

static void Handle(Greeting greeting) {
    if (greeting.Name.Contains("5")) {
        throw new InvalidOperationException("Messages with a 5 in are not allowed!");
    }
    Console.WriteLine($"Message from {greeting.MachineName}: Hello {greeting.Name}");
}