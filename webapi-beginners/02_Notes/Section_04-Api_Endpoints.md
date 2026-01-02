# Section 04: API Endpoints

## Lesson 04.24: Get Villas from DB

Get the villas from DB:

```cs
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
        public async Task<IEnumerable<Villa>> GetVillas()
        {
            return await _db.Villas.ToListAsync();
        }
```

## Lesson 04.25: Better Approach for Return Response

Change the action method to return `ActionResult`:

```cs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Villa>>> GetVillas()
        {
            return Ok(await _db.Villas.ToListAsync());
        }
```

## Lesson 04.26: Get Villa by Id

Change get villa by ID:

```cs
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

```
