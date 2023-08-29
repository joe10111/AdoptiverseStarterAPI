using Microsoft.AspNetCore.Mvc;
using AdoptiverseAPI.Models;
using AdoptiverseAPI.DataAccess;
using static System.Reflection.Metadata.BlobBuilder;
using System.Xml.Linq;


namespace AdoptiverseAPI.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class SheltersController : ControllerBase
    {
        private readonly AdoptiverseApiContext _context;

        public SheltersController(AdoptiverseApiContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult GetShelters() 
        {
            var shelterList = _context.Shelters;

            Response.StatusCode = 200;

            return new JsonResult(shelterList);
        }

        [HttpGet("{shelterId}")]
        public ActionResult GetShelterFromId(int shelterId)
        {
            var shelter = _context.Shelters.Find(shelterId);

            if(shelter == null)
            {
                return NotFound();
            }

            Response.StatusCode = 200;

            return new JsonResult(shelter);
        }

        [HttpPost]
        public ActionResult CreateShelter(Shelter shelter)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            _context.Shelters.Add(shelter);
            _context.SaveChanges();

            Response.StatusCode = 201;

            return new JsonResult(shelter);
        }

        [HttpPut("{shelterId}")]
        public void UpdateShelter(int shelterId, Shelter shelter)
        {
            var shelterToFind = _context.Shelters.Find(shelterId);
            
            shelterToFind.Name = shelter.Name ;
             shelterToFind.City = shelter.City;
            shelterToFind.UpdatedAt = shelter.UpdatedAt;
            shelterToFind.CreatedAt = shelter.CreatedAt;
            shelterToFind.FosterProgram = shelter.FosterProgram;
             shelterToFind.Rank = shelter.Rank;

            _context.Shelters.Update(shelterToFind);
            _context.SaveChanges();

            Response.StatusCode = 204;

            return;
        }

        [HttpDelete("{shelterId}")]
        public ActionResult DeleteShelter(int shelterId)
        {
            var shelterToFind = _context.Shelters.Find(shelterId);

            if (shelterToFind == null)
            {
                return NotFound();
            }

            _context.Shelters.Remove(shelterToFind);
            _context.SaveChanges();

            Response.StatusCode = 204;

            return new JsonResult(shelterToFind);
        }
    }
}