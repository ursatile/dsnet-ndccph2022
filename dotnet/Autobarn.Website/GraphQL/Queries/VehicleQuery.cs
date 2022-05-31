using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Autobarn.Data;
using Autobarn.Data.Entities;
using Autobarn.Website.GraphQL.GraphTypes;
using GraphQL;
using GraphQL.Types;

namespace Autobarn.Website.GraphQL.Queries {
    public sealed class VehicleQuery : ObjectGraphType {
        private readonly IAutobarnDatabase db;

        public VehicleQuery(IAutobarnDatabase db) {
            this.db = db;
            Field<ListGraphType<VehicleGraphType>>(
                "Vehicles",
                "Query to retrieve all vehicles in the system",
                resolve: GetAllVehicles);

            Field<VehicleGraphType>(
                "Vehicle",
                "Query to retrieve a single vehicle",
                new QueryArguments(MakeNonNullStringArgument("registration",
                    "The registration plate of the vehicle you want")),
                resolve: GetVehicle
            );

            Field<ListGraphType<VehicleGraphType>>(
                "VehiclesByColor",
                "Return all vehicles of a specific color",
                new QueryArguments(MakeNonNullStringArgument("color",
                    "What color cars do you want?")),
                    resolve: GetVehiclesByColor);
        }

        private object GetVehiclesByColor(IResolveFieldContext<object> context) {
            var color = context.GetArgument<string>("color");
            return db.ListVehicles().Where(v => v.Color.Contains(color));
        }

        private object GetVehicle(IResolveFieldContext<object> context) {
            var registration = context.GetArgument<string>("registration");
            return db.FindVehicle(registration);
        }

        private QueryArgument MakeNonNullStringArgument(string name, string description) {
            return new QueryArgument<NonNullGraphType<StringGraphType>> {
                Name = name, Description = description
            };
        }

        private IEnumerable<Vehicle> GetAllVehicles(IResolveFieldContext<object> context) {
            return db.ListVehicles();
        }
    }
}
