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
