using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Autobarn.Data;
using Autobarn.Data.Entities;
using Autobarn.Messages;
using Autobarn.Website.Controllers.api;
using Autobarn.Website.Models;
using Autobarn.Website.Services;
using EasyNetQ;
using EasyNetQ.Internals;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shouldly;
using Xunit;

namespace Autobarn.Website.Tests.Api.Controllers {
    public class VehiclesControllerTests {
        [Fact]
        public async void GET_Vehicles_Returns_OK() {
            var mockDb = new Mock<IAutobarnDatabase>();
            var c = new VehiclesController(mockDb.Object);
            var result = c.Get() as OkObjectResult;
            result.ShouldNotBeNull();
        }
    }

    public class ModelsControllerTests {
        [Fact]
        public async void POST_Model_Publishes_Notification_To_Bus() {
            var mockDb = new Mock<IAutobarnDatabase>();
            mockDb.Setup(db => db.FindModel(It.IsAny<string>()))
                .Returns(new Model {
                    Manufacturer = new Manufacturer()
                });

            var mockBus = new Mock<IBus>();
            var fakePubSub = new FakePubSub();
            mockBus.SetupGet(mb => mb.PubSub).Returns(fakePubSub);
            var now = new DateTimeOffset(2022, 5, 30, 16, 35, 24, TimeSpan.FromHours(2));
            var clock = new TestingClock(now);
            var c = new ModelsController(mockDb.Object, mockBus.Object, clock);
            c.Post("dmc-delorean", new VehicleDto {
                Registration = "TEST1234",
                Color = "TEST",
                Year = 1985
            });

            fakePubSub.messages.Count.ShouldBeGreaterThan(0);
            var m = fakePubSub.messages[0] as NewVehicleMessage;
            m.Registration.ShouldBe("TEST1234");
            m.ListedAt.ShouldBe(now);
        }
    }

    public class FakePubSub : IPubSub {
        public ArrayList messages { get; set; } = new ArrayList();

        public Task PublishAsync<T>(T message, Action<IPublishConfiguration> configure, CancellationToken cancellationToken = new CancellationToken()) {
            messages.Add(message);
            return Task.CompletedTask;
        }

        public AwaitableDisposable<ISubscriptionResult> SubscribeAsync<T>(string subscriptionId, Func<T, CancellationToken, Task> onMessage, Action<ISubscriptionConfiguration> configure,
            CancellationToken cancellationToken = new CancellationToken()) {
            throw new NotImplementedException();
        }
    }
}
