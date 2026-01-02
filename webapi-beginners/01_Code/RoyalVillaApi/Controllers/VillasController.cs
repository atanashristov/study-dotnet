using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoyalVillaApi.Data;
using RoyalVillaApi.Models;

namespace RoyalVillaApi.Controllers
{
    [Route("api/villas")]
    [ApiController]
    public class VillasController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public VillasController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Villa>>> GetVillas()
        {
            return Ok(await _db.Villas.ToListAsync());
        }

        [HttpGet("{id:int}")]
        public string GetVillaById(int id)
        {
            return "Villa Details of Id: " + id;
        }

        // [HttpGet("{id:int}/{name}")]
        // public string GetVillaByIdAndName([FromRoute] int id, [FromRoute] string name)
        // {
        //     return "Villa Details of Id: " + id + " and Name: " + name;
        // }

    }
}
