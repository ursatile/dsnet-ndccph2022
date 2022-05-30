using EasyNetQ;
using Messages;

Console.WriteLine("Hello, World!");
const string AMQP = "amqps://jiskiydu:O964LyptH8ATxnX8-k4GtdGdzNAIbcQe@hummingbird.rmq.cloudamqp.com/jiskiydu";
var bus = RabbitHutch.CreateBus(AMQP);
Console.WriteLine("Press any key to send a message...");
var count =1;
while (true)
{
    Console.ReadKey(true);
    await bus.PubSub.PublishAsync(new Greeting
    {
        Name = $"NDC Copenhagen (message {count++})"
    });
    Console.WriteLine("Published a message!");
}

