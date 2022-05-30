using EasyNetQ;
using Messages;

const string AMQP = "amqps://jiskiydu:O964LyptH8ATxnX8-k4GtdGdzNAIbcQe@hummingbird.rmq.cloudamqp.com/jiskiydu";
var bus = RabbitHutch.CreateBus(AMQP);
await bus.PubSub.SubscribeAsync<Greeting>("SUBSCRIPTION_ID", greeting => {
    Console.WriteLine($"Hello {greeting.Name}");
});
Console.WriteLine("Listening for messages...");
Console.ReadLine();
Console.WriteLine("Exiting! Thank you!");
