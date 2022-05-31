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
            Field<ListGraphType<VehicleGraphType>>(
                "VehiclesByYear",
                "Return all vehicles of a specific year",
                new QueryArguments(MakeNonNullIntArgument("year", "What cars do you want from a specific year?")),
                resolve: GetVehiclesByYear);
        }

        private QueryArgument MakeNonNullIntArgument(string name, string description) {
            return new QueryArgument<NonNullGraphType<IntGraphType>> {
                Name = name, Description = description
            };
        }

        private object GetVehiclesByYear(IResolveFieldContext<object> context) {
            //var year = context.GetArgument<int>("year");
            //return db.ListVehicles().Where(v => v.Year == year);

            // private object GetVehiclesByYear(IResolveFieldContext<object> context) {
            var year = context.GetArgument<int>("year");
            var op = context.GetArgument<string>("op");
            op = op?.ToLowerInvariant();
            return op switch {
                "newer" => db.ListVehicles().Where(v => v.Year > year),
                "older" => db.ListVehicles().Where(v => v.Year < year),
                _ => db.ListVehicles().Where(v => v.Year == year)
            };
        }


        private object GetVehiclesByColor(IResolveFieldContext<object> context) {
            var color = context.GetArgument<string>("color");
            if (color == "Brown") throw new InvalidOperationException("Brown cars are not allowed in GraphQL!");
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
        private object GetVehiclesByYearV2(IResolveFieldContext<object> context) {
            var year = context.GetArgument<int>("year");
            var compare = context.GetArgument<CompareGraphType>("CompareType");
            var resultCount = context.GetArgument<int?>("ResultCount");

            return compare switch {
                CompareGraphType.After => !resultCount.HasValue
                    ? db.ListVehicles().Where(v => v.Year > year)
                    : db.ListVehicles().Where(v => v.Year > year).Take(resultCount.Value),
                CompareGraphType.Before => !resultCount.HasValue
                    ? db.ListVehicles().Where(v => v.Year < year)
                    : db.ListVehicles().Where(v => v.Year < year).Take(resultCount.Value),
                _ => !resultCount.HasValue
                    ? db.ListVehicles().Where(v => v.Year == year)
                    : db.ListVehicles().Where(v => v.Year == year).Take(resultCount.Value)
            };
        }

        private QueryArgument MakeEnumArgument(string name, string description) {
            return new QueryArgument<NonNullGraphType<EnumerationGraphType<CompareGraphType>>> {
                Name = name, Description = description
            };
        }

        public enum CompareGraphType {
            Exact,
            Before,
            After
        }
    }
}
