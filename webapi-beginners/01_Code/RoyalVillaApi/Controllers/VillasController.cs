using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoyalVillaApi.Data;
using RoyalVillaApi.Models;

namespace RoyalVillaApi.Controllers
{
    [Route("api/villas")]
    public class VillasController : BaseApiController
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
                    return ProblemBadRequest("Invalid villa ID. The ID must be greater than 0.");
                }

                var villa = await _db.Villas.FirstOrDefaultAsync(v => v.Id == id);
                if (villa == null)
                {
                    return ProblemNotFound($"Villa with ID {id} was not found.");
                }

                // throw new Exception("Simulated exception for demonstration purposes.");
                return Ok(villa);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving villa with ID {VillaId}", id);
                return ProblemInternalServerError();
            }
        }

        // [HttpGet("{id:int}/{name}")]
        // public string GetVillaByIdAndName([FromRoute] int id, [FromRoute] string name)
        // {
        //     return "Villa Details of Id: " + id + " and Name: " + name;
        // }

    }
}
