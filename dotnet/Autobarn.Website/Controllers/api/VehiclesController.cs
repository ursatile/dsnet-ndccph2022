using System;
using Autobarn.Data;
using Autobarn.Data.Entities;
using Autobarn.Website.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Autobarn.Website.Controllers.api {
    [Route("api/[controller]")]
    [ApiController]
    public class VehiclesController : ControllerBase {
        private readonly IAutobarnDatabase db;

        public VehiclesController(IAutobarnDatabase db) {
            this.db = db;
        }

        //// GET: api/vehicles
        //[Produces("application/hal+json")]
        //[HttpGet]
        //public IActionResult Get(int index = 0, int count = 10) {
        //    var items = db.ListVehicles().Skip(index).Take(count);
        //    var total = db.CountVehicles();
        //    // ReSharper disable once InconsistentNaming
        //    var _links = Hal.Paginate("/api/vehicles", index, count, total);
        //    return Ok(new {
        //        _links,
        //        items,
        //    });
        //}

        [HttpGet]
        public IActionResult Get(char regStart = 'a') {
            var items = db.ListVehicles().Where(
                x => x.Registration.StartsWith(regStart.ToString(), StringComparison.InvariantCultureIgnoreCase)
                //x => x.Registration.ToLower().StartsWith(regStart.ToString().ToLower())
            );
            var total = db.CountVehicles();
            // ReSharper disable once InconsistentNaming
            var _links = Hal.PaginateByLetter("/api/vehicles", regStart);
            return Ok(new {
                _links,
                items,
            });
        }
        // GET api/vehicles/ABC123
        [HttpGet("{id}")]
        public IActionResult Get(string id) {
            var vehicle = db.FindVehicle(id);
            if (vehicle == default) return NotFound();
            var resource = vehicle.ToResource();
            resource._actions = new {
                delete = new {
                    name = "Delete this vehicle",
                    href = $"/api/vehicles/{id}",
                    method = "DELETE"
                }
            };
            return Ok(resource);
        }



        // PUT api/vehicles/ABC123
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] VehicleDto dto) {
            var vehicleModel = db.FindModel(dto.ModelCode);
            var vehicle = new Vehicle {
                Registration = dto.Registration,
                Color = dto.Color,
                Year = dto.Year,
                ModelCode = vehicleModel.Code
            };
            db.UpdateVehicle(vehicle);
            return Ok(dto);
        }

        // DELETE api/vehicles/ABC123
        [HttpDelete("{id}")]
        public IActionResult Delete(string id) {
            var vehicle = db.FindVehicle(id);
            if (vehicle == default) return NotFound();
            db.DeleteVehicle(vehicle);
            return NoContent();
        }
    }
}
