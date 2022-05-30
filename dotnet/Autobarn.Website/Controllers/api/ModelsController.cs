using Autobarn.Data;
using Autobarn.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Autobarn.Website.Models;

namespace Autobarn.Website.Controllers.api {
    [Route("api/[controller]")]
    [ApiController]
    public class ModelsController : ControllerBase {
        private readonly IAutobarnDatabase db;

        public ModelsController(IAutobarnDatabase db) {
            this.db = db;
        }

        [HttpGet]
        [Produces("application/hal+json")]
        public IActionResult Get() {
            var models = db.ListModels();
            var result = models.Select(model => model.ToResource());
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Produces("application/hal+json")]
        public IActionResult Get(string id) {
            var vehicleModel = db.FindModel(id);
            if (vehicleModel == default) return NotFound();
            var resource = vehicleModel.ToResource();
            resource._actions = new {
                create = new {
                    href = $"/api/models/{id}",
                    method = "POST",
                    name = $"Create a new {vehicleModel.Manufacturer.Name} {vehicleModel.Name}",
                    type = "application/json"
                }
            };
            return Ok(resource);
        }

        // POST api/vehicles
        [HttpPost("{id}")]
        public IActionResult Post(string id, [FromBody] VehicleDto dto) {
            var existing = db.FindVehicle(dto.Registration);
            if (existing != default)
                return Conflict(
                    $"Sorry, the vehicle with registration {dto.Registration} is already in our database and you can't list the same vehicle twice.");
            var vehicleModel = db.FindModel(id);
            var vehicle = new Vehicle {
                Registration = dto.Registration,
                Color = dto.Color,
                Year = dto.Year,
                VehicleModel = vehicleModel
            };
            db.CreateVehicle(vehicle);
            return Created(
                $"/api/vehicles/{vehicle.Registration}",
                vehicle.ToResource());
        }
    }
}