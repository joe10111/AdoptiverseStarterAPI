using AdoptiverseAPI.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using AdoptiverseAPI.DataAccess;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Text;

namespace AdoptiverseEndpointTests
{
    public class ShelterCrudEndpointTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public ShelterCrudEndpointTests(WebApplicationFactory<Program> factory) 
        {
            _factory = factory;
        }

        [Fact]
        public async void GetShelter_ReturnsListOfShelters()
        {
            // Anrange
            Shelter shelter1 = new Shelter {CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, FosterProgram = false, Rank = 1, City = "Boulder", Name = "PetShelter1" };
            Shelter shelter2 = new Shelter {CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, FosterProgram = true, Rank = 2, City = "LongMount", Name = "PetShelter2" };

            List<Shelter> shelters = new() { shelter1, shelter2 };
            AdoptiverseApiContext context = GetDbContext();

            
            context.Shelters.AddRange(shelters);
            context.SaveChanges();

            HttpClient client = _factory.CreateClient();

            // Act
            HttpResponseMessage response = await client.GetAsync("api/shelters");
            string content = await response.Content.ReadAsStringAsync();

            // Assert
            string expected = ObjectToJson(shelters);

            context.ChangeTracker.Clear();

            response.EnsureSuccessStatusCode();
            Assert.Equal(expected, content);
        }

        [Fact]
        public async void GetShelter_ReturnsShelterFromId()
        {
            // Anrange
            Shelter shelter1 = new Shelter { CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, FosterProgram = false, Rank = 1, City = "Boulder", Name = "PetShelter1" };
            Shelter shelter2 = new Shelter { CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, FosterProgram = true, Rank = 2, City = "LongMount", Name = "PetShelter2" };

            List<Shelter> shelters = new() { shelter1, shelter2 };

            AdoptiverseApiContext context = GetDbContext();
            context.Shelters.AddRange(shelters);
            context.SaveChanges();

            HttpClient client = _factory.CreateClient();

            // Act
            HttpResponseMessage response = await client.GetAsync($"api/shelters/{shelter1.Id}");
            string content = await response.Content.ReadAsStringAsync();

            // Assert
            string expected = ObjectToJson(shelter1);

            context.ChangeTracker.Clear();

            response.EnsureSuccessStatusCode();
            Assert.Equal(expected, content);
        }

        [Fact]
        public async void PostShelter_CreatesShelterInDb()
        {
            // Create fresh database
            AdoptiverseApiContext context = GetDbContext();

            // Set up and send the request
            HttpClient client = _factory.CreateClient();
            var jsonString = "{\"CreatedAt\": \"2023-08-29T12:00:00.000Z\", \"UpdatedAt\": \"2023-08-29T12:05:00.000Z\", \"FosterProgram\": true, \"Rank\": 1, \"City\": \"San Francisco\", \"Name\": \"Happy Paws\"}";
            var requestContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/shelters", requestContent);

            var newShelter = context.Shelters.First();

            Assert.Equal("Created", response.StatusCode.ToString());
            Assert.Equal(201, (int)response.StatusCode);
            Assert.Equal("Happy Paws", newShelter.Name);
        }

        [Fact]
        public async void PutShelter_ShelterHasBeenUpdated()
        {
            Shelter shelter1 = new Shelter { CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, FosterProgram = false, Rank = 1, City = "Boulder", Name = "PetShelter1" };

            AdoptiverseApiContext context = GetDbContext();
            context.Shelters.Add(shelter1);
            context.SaveChanges();

            HttpClient client = _factory.CreateClient();
            var jsonString = "{\"CreatedAt\": \"2023-08-29T12:00:00.000Z\", \"UpdatedAt\": \"2023-08-29T12:05:00.000Z\", \"FosterProgram\": true, \"Rank\": 1, \"City\": \"San Francisco\", \"Name\": \"Happy Paws\"}";
            var requestContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"/api/shelters/{shelter1.Id}", requestContent);

            // Clear all previously tracked DB objects to get a new copy of the updated book
            context.ChangeTracker.Clear();

            Assert.Equal(204, (int)response.StatusCode);
            Assert.Equal("Happy Paws", context.Shelters.Find(shelter1.Id).Name);
        }
        // left off making test for delete
        [Fact]
        public async void DeleteShelter_ShelterHasBeenRemoved()
        {
            // Anrange
            Shelter shelter1 = new Shelter { CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, FosterProgram = false, Rank = 1, City = "Boulder", Name = "PetShelter1" };
            Shelter shelter2 = new Shelter { CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, FosterProgram = true, Rank = 2, City = "LongMount", Name = "PetShelter2" };

            List<Shelter> shelters = new() { shelter1, shelter2 };

            // Create fresh database
            AdoptiverseApiContext context = GetDbContext();

            context.Shelters.AddRange(shelters);
            context.SaveChanges();

            HttpClient client = _factory.CreateClient();

            // Act
            HttpResponseMessage response = await client.DeleteAsync($"api/shelters/{shelter1.Id}");
            string content = await response.Content.ReadAsStringAsync();

            // Assert
            string expected = ObjectToJson(shelter2);

            Assert.Equal(204, (int)response.StatusCode);
            Assert.DoesNotContain(expected, content);
        }

        private string ObjectToJson(object obj)
        {
            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            string json = JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                ContractResolver = contractResolver
            });

            return json;
        }

        private AdoptiverseApiContext GetDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<AdoptiverseApiContext>();
            optionsBuilder.UseInMemoryDatabase("TestDatabase");

            var context = new AdoptiverseApiContext(optionsBuilder.Options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            return context;
        }
    }
}