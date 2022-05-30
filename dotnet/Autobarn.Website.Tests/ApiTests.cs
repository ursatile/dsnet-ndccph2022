﻿using Autobarn.Data.Entities;
using Newtonsoft.Json;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Autobarn.Website.Tests {
	public class ApiTests : IClassFixture<TestWebApplicationFactory<Startup>> {
		private readonly TestWebApplicationFactory<Startup> factory;

		public ApiTests(TestWebApplicationFactory<Startup> factory) {
			this.factory = factory;
		}

		[Fact]
		public async void GET_vehicles_returns_success_status_code() {
			var client = factory.CreateClient();
			var response = await client.GetAsync("/api/vehicles");
			Assert.True(response.IsSuccessStatusCode);
		}

		[Fact]
		public async void GET_vehicles_returns_vehicle_data() {
			var client = factory.CreateClient();
			var response = await client.GetAsync("/api/vehicles");
			var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<dynamic>(json);
            ((string) result._links.self.href).ShouldStartWith("/api/vehicles");
            ((string)result._links.next.href).ShouldStartWith("/api/vehicles");
            ((object) result._links.previous).ShouldBeNull();
        }

        [Fact]
        public async void GET_vehicles_returns_valid_paging() {
            var client = factory.CreateClient();
            var response = await client.GetAsync("/api/vehicles");
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<dynamic>(json);
            var href = (string) result._links.next.href;
			response = await client.GetAsync(href);
            json = await response.Content.ReadAsStringAsync();
            result = JsonConvert.DeserializeObject<dynamic>(json);
			((int)result.items.Count).ShouldBeGreaterThan(0);
            ((string) result.items[0].registration).ShouldNotBeNullOrWhiteSpace();
        }

		[Fact]
		public async void POST_creates_vehicle() {
			var registration = Guid.NewGuid().ToString("N");
			var client = factory.CreateClient();
			var vehicle = new {
				modelCode = "volkswagen-beetle",
				registration,
				color = "Green",
				year = "1985"
			};
			var content = new StringContent(JsonConvert.SerializeObject(vehicle), Encoding.UTF8, "application/json");
			var response = await client.PostAsync($"/api/vehicles", content);
			response.StatusCode.ShouldBe(HttpStatusCode.OK);
			var (_, result) = await client.GetVehicle(registration);
			result.Color.ShouldBe("Green");
			await client.DeleteAsync($"/api/vehicles/{registration}");
		}


		[Fact]
		public async void PUT_creates_vehicle() {
			var registration = Guid.NewGuid().ToString("N");
			var client = factory.CreateClient();
			await client.PutVolkswagen(registration, "Green", 1985);
			var (_, vehicle) = await client.GetVehicle(registration);
			vehicle.Color.ShouldBe("Green");
			await client.DeleteAsync($"/api/vehicles/{registration}");
		}

		[Fact]
		public async void PUT_updates_vehicle() {
			var registration = Guid.NewGuid().ToString("N");
			var client = factory.CreateClient();
			await client.PutVolkswagen(registration, "Green", 1985);
			var (_, vehicle) = await client.GetVehicle(registration);
			vehicle.Color.ShouldBe("Green");
			await client.PutVolkswagen(registration, "Brown", 1987);
			(_, vehicle) = await client.GetVehicle(registration);
			vehicle.Color.ShouldBe("Brown");
			await client.DeleteAsync($"/api/vehicles/{registration}");
		}
	}

	public static class HttpClientExtensions {
		public static async Task<HttpResponseMessage> PutVolkswagen(this HttpClient client, string registration, string color, int year) {
			var vehicle = new { modelCode = "volkswagen-beetle", registration, color, year };
			var content = new StringContent(JsonConvert.SerializeObject(vehicle), Encoding.UTF8, "application/json");
			return await client.PutAsync($"/api/vehicles/{registration}", content);
		}

		public static async Task<(HttpResponseMessage, Vehicle)> GetVehicle(this HttpClient client, string registration) {
			var response = await client.GetAsync($"/api/vehicles/{registration}");
			var json = response.Content.ReadAsStringAsync().Result;
			var vehicle = JsonConvert.DeserializeObject<Vehicle>(json);
			return (response, vehicle);
		}

		public static async Task<HttpResponseMessage> DeleteVehicle(this HttpClient client, string registration) {
			return await client.DeleteAsync($"/api/vehicles/{registration}");
		}
	}
}