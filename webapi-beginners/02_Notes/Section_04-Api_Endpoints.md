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

### Unified Error Responses with RFC 9110 Problem Details

To ensure consistent error responses across all endpoints, we implemented RFC 9110 Problem Details format for all HTTP 4xx and 5xx errors.

**Implementation:**

1. **Created ProblemDetailsConfiguration.cs** - A clean configuration class that encapsulates all Problem Details setup as an extension method, keeping Program.cs clean.

2. **Created ProblemDetailsHelper.cs** - A static helper class for creating RFC 9110 compliant ProblemDetails objects:
   ```cs
   public static class ProblemDetailsHelper
   {
       public static ProblemDetails CreateBadRequest(string detail, string? instance = null) { ... }
       public static ProblemDetails CreateNotFound(string detail, string? instance = null) { ... }
       public static ProblemDetails CreateInternalServerError(string detail, string? instance = null) { ... }
       public static ProblemDetails CreateUnauthorized(string detail, string? instance = null) { ... }
       public static ProblemDetails CreateForbidden(string detail, string? instance = null) { ... }
       public static ProblemDetails CreateConflict(string detail, string? instance = null) { ... }
       public static ProblemDetails CreateCustom(int statusCode, string title, string detail, string type, string? instance = null) { ... }
   }
   ```

3. **Created BaseApiController.cs** - A base controller class with convenience methods that wrap the static helpers:
   ```cs
   [ApiController]
   public abstract class BaseApiController : ControllerBase
   {
       protected ObjectResult ProblemBadRequest(string detail)
       {
           var problemDetails = ProblemDetailsHelper.CreateBadRequest(detail, Request.Path);
           return new ObjectResult(problemDetails) { StatusCode = problemDetails.Status };
       }
       // Similar methods for NotFound, InternalServerError, Unauthorized, Forbidden, Conflict, Custom
   }
   ```

4. **Updated Program.cs** - Added Problem Details support with a single line:
   ```cs
   builder.Services.AddProblemDetailsSupport();

   // In middleware pipeline
   app.UseExceptionHandler();
   app.UseStatusCodePages();
   ```

5. **Updated VillasController** - Inherits from `BaseApiController` and uses simplified helper methods:
   ```cs
   [Route("api/villas")]
   public class VillasController : BaseApiController
   {
       [HttpGet("{id:int}")]
       public async Task<ActionResult<Villa>> GetVillaById(int id)
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

           return Ok(villa);
       }
   }
   ```

**RFC 9110 Problem Details Response Format:**

All error responses now follow this standardized structure:

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5",
  "title": "Not Found",
  "status": 404,
  "detail": "Villa with ID 100 was not found.",
  "instance": "/api/villas/100",
  "traceId": "00-f7f0666ddf0139f797939f7434451d93-1d99fe78cd524abb-00"
}
```

**Benefits:**
- **Consistency**: All errors follow the same structure
- **Machine-Readable**: Clients can easily parse and handle errors
- **RFC Compliance**: Follows HTTP standards
- **Better Debugging**: Includes trace IDs and instance paths
- **Reduced Boilerplate**: Helper methods eliminate repetitive code
- **Reusability**: Static helpers work anywhere (middleware, filters, services); controllers can inherit from BaseApiController
- **Extensibility**: Easy to add custom fields or new helper methods
- **Flexibility**: Choose between static helpers (universal use) or base controller (convenience in controllers)
- **Testability**: Static helpers are pure functions, easy to unit test

**Architecture: Two-Layer Pattern**
- **Layer 1**: `ProblemDetailsHelper` - Static methods for creating ProblemDetails (no dependencies, use anywhere)
- **Layer 2**: `BaseApiController` - Instance methods wrapping static helpers with automatic Request.Path injection (controller convenience)

This design provides maximum flexibility: use static helpers directly in non-controller code, or inherit from BaseApiController for cleaner controller syntax.
