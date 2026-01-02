# Section_04_26-Api-GetVillaById_UnifyErrorResponses-Result.md

## Summary

Successfully unified all error responses from the `GetVillaById` endpoint to use RFC 9110 Problem Details format for consistent, standardized error handling across all HTTP 4xx and 5xx responses.

## Implementation Details

### Files Created/Modified

#### 1. **Configuration/ProblemDetailsConfiguration.cs** (New)

Created a clean configuration class that encapsulates all Problem Details setup:

```csharp
public static class ProblemDetailsConfiguration
{
    public static IServiceCollection AddProblemDetailsSupport(this IServiceCollection services)
    {
        // Configure Problem Details middleware
        // Configure API behavior for validation errors
        return services;
    }
}
```

**Features:**

- Extension method for clean service registration
- Customizes Problem Details with trace IDs and instance paths
- Configures automatic validation error formatting
- Ensures all error responses follow RFC 9110 format

#### 2. **Program.cs** (Modified)

Added Problem Details support without cluttering the main code:

```csharp
// Add Problem Details support for unified error responses
builder.Services.AddProblemDetailsSupport();

// In middleware pipeline
app.UseExceptionHandler();
app.UseStatusCodePages();
```

**Changes:**

- Single line service registration using extension method
- Added exception handler middleware
- Added status code pages middleware

#### 3. **Helpers/ProblemDetailsHelper.cs** (New)

Created a static helper class with methods for creating RFC 9110 compliant `ProblemDetails` objects:

```csharp
public static class ProblemDetailsHelper
{
    public static ProblemDetails CreateBadRequest(string detail, string? instance = null)
    {
        return new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Bad Request",
            Detail = detail,
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            Instance = instance
        };
    }

    public static ProblemDetails CreateNotFound(string detail, string? instance = null) { ... }
    public static ProblemDetails CreateInternalServerError(string detail = "...", string? instance = null) { ... }
    public static ProblemDetails CreateUnauthorized(string detail = "...", string? instance = null) { ... }
    public static ProblemDetails CreateForbidden(string detail = "...", string? instance = null) { ... }
    public static ProblemDetails CreateConflict(string detail, string? instance = null) { ... }
    public static ProblemDetails CreateCustom(int statusCode, string title, string detail, string type, string? instance = null) { ... }
}
```

**Features:**

- Static methods that don't require controller inheritance
- Can be used anywhere in the application
- Pre-configured with RFC 9110 type URLs and standard titles
- Optional `instance` parameter for request path
- Includes common HTTP error codes (400, 401, 403, 404, 409, 500)
- Extensible for custom status codes

#### 4. **Controllers/BaseApiController.cs** (New)

Created a base controller class that uses the static helper methods:

```csharp
[ApiController]
public abstract class BaseApiController : ControllerBase
{
    protected ObjectResult ProblemBadRequest(string detail)
    {
        var problemDetails = ProblemDetailsHelper.CreateBadRequest(detail, Request.Path);
        return new ObjectResult(problemDetails)
        {
            StatusCode = problemDetails.Status
        };
    }

    protected ObjectResult ProblemNotFound(string detail)
    {
        var problemDetails = ProblemDetailsHelper.CreateNotFound(detail, Request.Path);
        return new ObjectResult(problemDetails)
        {
            StatusCode = problemDetails.Status
        };
    }

    protected ObjectResult ProblemInternalServerError(string detail = "An error occurred while processing your request.")
    {
        var problemDetails = ProblemDetailsHelper.CreateInternalServerError(detail, Request.Path);
        return new ObjectResult(problemDetails)
        {
            StatusCode = problemDetails.Status
        };
    }

    // Additional helpers for 401, 403, 409, and custom status codes
}
```

**Features:**

- Inherits from `ControllerBase` for controller functionality
- Uses static `ProblemDetailsHelper` for ProblemDetails creation
- Automatically includes request path in `instance` field
- Wraps in `ObjectResult` with proper status code
- Convenience layer over the static helpers

**Benefits of the Static Approach:**

- **Separation of Concerns**: Static helper separates ProblemDetails creation from controller logic
- **Reusability**: Can be used in middleware, filters, or any non-controller code
- **Testability**: Static methods are easier to unit test
- **Flexibility**: Controllers can use helpers directly or through BaseApiController
- **No Inheritance Required**: Code outside controllers can create ProblemDetails without inheritance

#### 5. **Controllers/VillasController.cs** (Modified)

Updated to inherit from `BaseApiController` and use simplified helper methods:

```csharp
[Route("api/villas")]
public class VillasController : BaseApiController
{
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

            return Ok(villa);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving villa with ID {VillaId}", id);
            return ProblemInternalServerError();
        }
    }
}
```

**Benefits:**

- Much cleaner and more readable code
- No need to specify status codes, titles, or type URLs repeatedly
- Consistent error responses across all controllers
- Easy to extend with additional helper methods
- Controllers can choose static helper directly or use base controller convenience methods

---

## 🏗️ Architecture: Static Helper + Instance Wrapper Pattern

The implementation uses a two-layer architecture for maximum flexibility:

### Layer 1: Static Helper (`ProblemDetailsHelper`)

Creates RFC 9110 compliant `ProblemDetails` objects with zero dependencies. Can be used anywhere in the codebase (middleware, filters, services, etc.).

### Layer 2: Instance Wrapper (`BaseApiController`)

Provides controller-friendly methods that:

1. Call the static helper to create `ProblemDetails`
2. Add the request path automatically using `Request.Path`
3. Wrap in `ObjectResult` for ASP.NET Core compatibility
4. Set proper status codes

### Why This Design?

**Static Helpers Benefits:**

- ✅ **Universal**: Use in middleware, filters, background services, etc.
- ✅ **No Dependencies**: No controller inheritance required
- ✅ **Testable**: Pure functions easy to unit test
- ✅ **Reusable**: Share across multiple projects

**Instance Wrapper Benefits:**

- ✅ **Convenience**: Controllers get cleaner syntax
- ✅ **Request Context**: Automatically includes request path
- ✅ **Framework Integration**: Returns proper ASP.NET Core `ActionResult`
- ✅ **Inheritance**: All controllers inherit for free

### Usage Options

**Option 1: Use BaseApiController (Recommended for Controllers)**

```csharp
public class VillasController : BaseApiController
{
    public ActionResult GetVilla(int id)
    {
        return ProblemBadRequest("Invalid ID");  // Cleanest option
    }
}
```

**Option 2: Use Static Helper Directly (For Non-Controller Code)**

```csharp
public class SomeMiddleware
{
    public Task InvokeAsync(HttpContext context)
    {
        var problemDetails = ProblemDetailsHelper.CreateBadRequest(
            "Invalid request",
            context.Request.Path
        );
        return context.Response.WriteAsJsonAsync(problemDetails);
    }
}
```

---

## RFC 9110 Problem Details Format

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

### Field Descriptions

| Field      | Description                                                      |
| ---------- | ---------------------------------------------------------------- |
| `type`     | URI reference to RFC 9110 section describing the error type      |
| `title`    | Short, human-readable summary of the problem type                |
| `status`   | HTTP status code                                                 |
| `detail`   | Detailed explanation specific to this occurrence of the problem  |
| `instance` | URI reference identifying the specific occurrence (request path) |
| `traceId`  | Correlation ID for debugging and log correlation                 |

## Example Responses

### 400 Bad Request

**Request:**

```http
GET /api/villas/-1
```

**Response:**

```http
HTTP/1.1 400 Bad Request
Content-Type: application/problem+json; charset=utf-8

{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "Bad Request",
  "status": 400,
  "detail": "Invalid villa ID. The ID must be greater than 0.",
  "instance": "/api/villas/-1",
  "traceId": "00-abc123..."
}
```

### 404 Not Found

**Request:**

```http
GET /api/villas/100
```

**Response:**

```http
HTTP/1.1 404 Not Found
Content-Type: application/problem+json; charset=utf-8

{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5",
  "title": "Not Found",
  "status": 404,
  "detail": "Villa with ID 100 was not found.",
  "instance": "/api/villas/100",
  "traceId": "00-def456..."
}
```

### 500 Internal Server Error

**Request:**

```http
GET /api/villas/1
```

**Response:**

```http
HTTP/1.1 500 Internal Server Error
Content-Type: application/problem+json; charset=utf-8

{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.6.1",
  "title": "Internal Server Error",
  "status": 500,
  "detail": "An error occurred while processing your request.",
  "instance": "/api/villas/1",
  "traceId": "00-ghi789..."
}
```

## Benefits

### 1. **Consistency**

All error responses follow the same structure, making client-side error handling predictable and easier to implement.

### 2. **Machine-Readable**

Clients can parse the standardized JSON format to provide appropriate error handling and user feedback.

### 3. **RFC Compliance**

Follows the RFC 9110 (HTTP Semantics) standard, making the API interoperable with tools and libraries that expect this format.

### 4. **Better Debugging**

- `traceId` allows correlation between client errors and server logs
- `instance` shows the exact endpoint that failed
- Structured logging in controller helps diagnose issues

### 5. **Enhanced Detail**

The `detail` field provides context-specific error information without exposing sensitive internal details.

### 6. **Extensibility**

Problem Details format allows adding custom fields if needed in the future.

### 7. **Reduced Boilerplate**

The static helper and base controller eliminate repetitive code. Compare:

**Before (verbose):**

```csharp
return Problem(
    statusCode: StatusCodes.Status404NotFound,
    title: "Not Found",
    detail: $"Villa with ID {id} was not found.",
    type: "https://tools.ietf.org/html/rfc9110#section-15.5.5"
);
```

**After with BaseApiController (concise):**

```csharp
return ProblemNotFound($"Villa with ID {id} was not found.");
```

**After with Static Helper (explicit):**

```csharp
var problemDetails = ProblemDetailsHelper.CreateNotFound(
    $"Villa with ID {id} was not found.",
    Request.Path
);
return new ObjectResult(problemDetails) { StatusCode = 404 };
```

### 8. **Reusability**

- **Controllers**: Inherit from `BaseApiController` for convenience methods
- **Non-Controllers**: Use `ProblemDetailsHelper` static methods directly
- **Middleware**: Can create Problem Details without controller dependencies
- **Cross-Project**: Static helpers can be shared as a library

## Testing

To test the unified error responses:

```bash
# Test 400 Bad Request
curl -i http://localhost:5050/api/villas/-1

# Test 404 Not Found
curl -i http://localhost:5050/api/villas/999

# Test 500 Internal Server Error (uncomment throw line in controller)
curl -i http://localhost:5050/api/villas/1
```

Or use the RoyalVillaApi.http file:

```http
### Get Villa by ID - Bad Request (ID <= 0)
GET {{RoyalVillaApi_HostAddress}}/api/villas/-1

### Get Villa by ID - Not Found
GET {{RoyalVillaApi_HostAddress}}/api/villas/999

### Get Villa by ID - Success
GET {{RoyalVillaApi_HostAddress}}/api/villas/1
```

## Additional Notes

### Validation Errors

The configuration also handles automatic validation errors. For example, if you add model validation:

```csharp
public class CreateVillaDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; }
}
```

Validation failures will automatically return:

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Name": ["The Name field is required."]
  },
  "instance": "/api/villas",
  "traceId": "00-..."
}
```

### Exception Handler Middleware

The `app.UseExceptionHandler()` middleware catches unhandled exceptions and automatically converts them to Problem Details responses, providing a safety net for unexpected errors.

### Status Code Pages Middleware

The `app.UseStatusCodePages()` middleware ensures that any status code response without a body (like a 404 from routing) also gets formatted as Problem Details.

## Best Practices

### 1. **Choose the Right Abstraction Level**

**Use BaseApiController** for controllers (recommended):

```csharp
public class VillasController : BaseApiController
{
    public ActionResult GetVilla(int id)
    {
        return ProblemNotFound($"Villa {id} not found.");  // Cleanest
    }
}
```

**Use Static Helper** for non-controller code:

```csharp
public class CustomMiddleware
{
    public async Task InvokeAsync(HttpContext context)
    {
        var problem = ProblemDetailsHelper.CreateUnauthorized("Token expired", context.Request.Path);
        await context.Response.WriteAsJsonAsync(problem);
    }
}
```

### 2. **Provide Meaningful Details**

Include context-specific information without exposing sensitive data or stack traces.

### 3. **Log Before Returning Errors**

Always log exceptions and important error conditions for debugging.

### 4. **Be Consistent**

Apply this pattern to all controller actions that can return errors.

### 5. **Extend When Needed**

Use `ProblemCustom()` or `CreateCustom()` for non-standard HTTP status codes.

### 6. **Test Error Responses**

Write unit tests for both static helpers (easy to test) and controller error paths.

## Using BaseApiController in Other Controllers

To add error handling to other controllers:

```csharp
[Route("api/bookings")]
public class BookingsController : BaseApiController
{
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Booking>> GetBooking(int id)
    {
        if (id <= 0)
        {
            return ProblemBadRequest("Invalid booking ID.");
        }

        var booking = await _db.Bookings.FindAsync(id);
        if (booking == null)
        {
            return ProblemNotFound($"Booking with ID {id} was not found.");
        }

        return Ok(booking);
    }
}
```

## Extending the Error Handling System

The two-layer architecture makes extension straightforward. Add methods to both layers for consistency.

### Add New Error Type

**1. Add to Static Helper (`ProblemDetailsHelper.cs`):**

```csharp
public static ProblemDetails CreatePaymentRequired(string detail, string? instance = null)
{
    return new ProblemDetails
    {
        Type = "https://tools.ietf.org/html/rfc9110#section-15.5.3",
        Title = "Payment Required",
        Status = StatusCodes.Status402PaymentRequired,
        Detail = detail,
        Instance = instance
    };
}
```

**2. Add Wrapper to BaseApiController:**

```csharp
protected ObjectResult ProblemPaymentRequired(string detail = "Payment is required.")
{
    var problemDetails = ProblemDetailsHelper.CreatePaymentRequired(detail, Request.Path);
    return new ObjectResult(problemDetails)
    {
        StatusCode = problemDetails.Status
    };
}
```

### Use Custom Method for Ad-Hoc Errors

For one-off status codes, use the `Custom` methods without extending the classes:

**With BaseApiController:**

```csharp
return ProblemCustom(
    statusCode: 418,
    title: "I'm a teapot",
    detail: "The server refuses to brew coffee.",
    type: "https://tools.ietf.org/html/rfc2324#section-2.3.2"
);
```

**With Static Helper:**

```csharp
var problem = ProblemDetailsHelper.CreateCustom(
    statusCode: 418,
    title: "I'm a teapot",
    detail: "The server refuses to brew coffee.",
    type: "https://tools.ietf.org/html/rfc2324#section-2.3.2",
    instance: Request.Path
);
return new ObjectResult(problem) { StatusCode = 418 };
```

## Future Enhancements

Consider implementing:

1. **Custom Problem Details Extensions**: Add API-specific metadata to error responses
2. **Localization**: Translate error messages based on `Accept-Language` header
3. **Error Codes**: Add custom error codes for specific business rule violations
4. **Development vs Production**: Show more details in development, less in production
5. **Unit Tests**: Test static helpers (pure functions) and controller error paths

## References

- [RFC 9110 - HTTP Semantics](https://tools.ietf.org/html/rfc9110)
- [RFC 7807 - Problem Details for HTTP APIs](https://tools.ietf.org/html/rfc7807)
- [ASP.NET Core Problem Details Documentation](https://learn.microsoft.com/en-us/aspnet/core/web-api/handle-errors)
