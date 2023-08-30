using Microsoft.AspNetCore.Mvc;
using AdoptiverseAPI.Models;
using AdoptiverseAPI.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace AdoptiverseAPI.Controllers
{
    [Route("api/shelters/{shelterId}/[controller]")]
    [ApiController]
    public class PetsController : Controller
    {
        private readonly AdoptiverseApiContext _context;

        public PetsController(AdoptiverseApiContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        public ActionResult GetPets(int shelterId)
        {
            var shelterWithPets = _context.Shelters.Include(s => s.Pets).FirstOrDefault(s => s.Id == shelterId);

            if (shelterWithPets == null)
            {
                return NotFound("Shelter not found");
            }

            return Ok(shelterWithPets.Pets);
        }

        [HttpGet("{petId}")]
        public ActionResult GetPetsFromId(int shelterId, int petId)
        {
            var shelterWithPets = _context.Shelters.Include(s => s.Pets).FirstOrDefault(s => s.Id == shelterId);
            if (shelterWithPets == null)
            {
                return NotFound("Shelter not found");
            }

            var petInShelter = shelterWithPets.Pets.FirstOrDefault(p => p.Id == petId);
            if (petInShelter == null)
            {
                return NotFound("Pet not found in the specified shelter");
            }

            return Ok(petInShelter);
        }

        [HttpPut("{petId}")]
        public ActionResult UpdatePet(int shelterId, int petId, Pet pet)
        {
            var shelterWithPets = _context.Shelters.Include(s => s.Pets).FirstOrDefault(s => s.Id == shelterId);
            if (shelterWithPets == null)
            {
                return NotFound("Shelter not found");
            }

            var petInShelter = shelterWithPets.Pets.FirstOrDefault(p => p.Id == petId);
            if (petInShelter == null)
            {
                return NotFound("Pet not found in the specified shelter");
            }

            petInShelter.Name = pet.Name;
            petInShelter.UpdatedAt = pet.UpdatedAt;
            petInShelter.CreatedAt = pet.CreatedAt;
            petInShelter.ShelterId = shelterId;
            petInShelter.Breed = pet.Breed;
            petInShelter.Age = pet.Age;
            petInShelter.Adoptable = pet.Adoptable;

            _context.Pets.Update(petInShelter);
            _context.SaveChanges();

            return Ok(petInShelter);
        }

        [HttpDelete("{petId}")]
        public ActionResult DeletePet(int shelterId, int petId)
        {
            var shelterWithPets = _context.Shelters.Include(s => s.Pets).FirstOrDefault(s => s.Id == shelterId);
            if (shelterWithPets == null)
            {
                return NotFound("Shelter not found");
            }

            var petInShelter = shelterWithPets.Pets.FirstOrDefault(p => p.Id == petId);
            if (petInShelter == null)
            {
                return NotFound("Pet not found in the specified shelter");
            }

            _context.Pets.Remove(petInShelter);
            _context.SaveChanges();

            return Ok(petInShelter);
        }
    }
}
