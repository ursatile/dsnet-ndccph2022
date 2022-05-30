using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace Autobarn.Website.Controllers.api
{
    [Route("api")]
    [ApiController]
    public class DefaultController : ControllerBase {
        public IActionResult Get() {
            return Ok(new {
                message = "Welcome to the Autobarn API",
                version = Assembly.GetExecutingAssembly().FullName,
                _links = new {
                    vehicles = new {
                        href = "/api/vehicles"
                    },
                    models = new {
                        href = "/api/models"
                    }
                }
            });
        }
    }
}