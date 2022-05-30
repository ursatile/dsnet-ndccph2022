using System.Net;
using Autobarn.Data;
using Autobarn.Website.Controllers.api;
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
}
