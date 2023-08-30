using AdoptiverseAPI.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using AdoptiverseAPI.DataAccess;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Text;

namespace AdoptiverseEndpointTests
{
    public class PetsCrudEndPointTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public PetsCrudEndPointTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async void GetPets_ReturnsListOfPets()
        {
            // Arrange
            Shelter shelter = new Shelter { CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, FosterProgram = false, Rank = 1, City = "Boulder", Name = "PetShelter1" };
            var pet1 = new Pet
            {
                Id = 1, Name = "Fluffy", Breed = "German Shepherd", Age = 3, Adoptable = true, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, ShelterId = 1  // Assuming 1 is the ID of an existing shelter
            };

            var pet2 = new Pet
            {
                Id = 2, Name = "Spike", Breed = "Bulldog", Age = 2, Adoptable = false, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now, ShelterId = 1  // Assuming 1 is the ID of an existing shelter
            };

            var context = GetDbContext();
            context.Shelters.Add(shelter);
            context.Pets.AddRange(pet1, pet2);
            context.SaveChanges();

            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync($"api/shelters/{shelter.Id}/pets");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            var expected = ObjectToJson(new List<Pet> { pet1, pet2 });
            Assert.Equal(expected, content);
        }

        // Left off making bellow tests
        /*
        [Fact]
        public async void GetPets_ReturnsPetUsingId()
        {

        }


        [Fact]
        public async void PostPets_CreatesPetInDb()
        {

        }

        [Fact]
        public async void PutPet_PetHasBeenUpdated()
        {

        }

        [Fact]
        public async void DeletePet_PetHasBeenRemoved()
        {

        }
        */

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
