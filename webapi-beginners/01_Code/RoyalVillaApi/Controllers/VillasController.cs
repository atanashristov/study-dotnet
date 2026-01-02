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
        private readonly ILogger<VillasController> _logger;

        public VillasController(ApplicationDbContext db, ILogger<VillasController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Villa>>> GetVillas()
        {
            return Ok(await _db.Villas.ToListAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Villa>> GetVillaById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid villa ID.");
                }

                var villa = await _db.Villas.FirstOrDefaultAsync(v => v.Id == id);
                if (villa == null)
                {
                    return NotFound();
                }

                return Ok(villa);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving villa with ID {VillaId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An error occurred while processing your request.");
            }
        }

        // [HttpGet("{id:int}/{name}")]
        // public string GetVillaByIdAndName([FromRoute] int id, [FromRoute] string name)
        // {
        //     return "Villa Details of Id: " + id + " and Name: " + name;
        // }

    }
}
