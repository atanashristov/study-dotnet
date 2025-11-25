# Section 02: Web API Basics

## Lesson 02.08: Web API Controller

Controller organizes the methods in classes.

## Lesson 02.09: Routing

Firsts, in the main method we add controllers to DI container.

Then we apply `app.MapControllers()`.

```cs
var builder = WebApplication.CreateBuilder(args);

// add controllers to DI container
builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
```

Secondly we apply `Http` and `Route` attributes to the controller methods:

```cs
namespace WebAppiDemo.Controllers
{
    [ApiController]
    public class ShirtsController : ControllerBase
    {
        [HttpGet]
        [Route("api/shirts")]
        public IActionResult GetShirts()
        {
            var shirts = new[]
            {
                new { Id = 1, Color = "Red", Size = "M" },
                new { Id = 2, Color = "Blue", Size = "L" },
                new { Id = 3, Color = "Green", Size = "S" }
            };
            return Ok(shirts);
        }
```

We can also apply the `Route` attribute on the class:

```cs
    [ApiController]
    [// Route("api/[controller]")]
    Route("api/[controller]")]
    public class ShirtsController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetShirts()
        {
            var shirts = new[]
            {
                new { Id = 1, Color = "Red", Size = "M" },
                new { Id = 2, Color = "Blue", Size = "L" },
                new { Id = 3, Color = "Green", Size = "S" }
            };
            return Ok(shirts);
        }

        // [HttpGet]
        // [Route("{id}")]
        [HttpGet("{id}")]
        public IActionResult GetShirtById(int id)
        {
            var shirt = new { Id = id, Color = "Red", Size = "M" };
            return Ok(shirt);
        }
...
```

## Lesson 02.10: Model Binding

they are multiple ways for model binding.

- Binding from **route**

```cs
 [HttpGet("{id}/{color}")]
 public IActionResult GetShirtById(int id, string color)
```

We can also explicitly specify that the variable value comes `FromRoute`:

```cs
 [HttpGet("{id}/{color}")]
 public IActionResult GetShirtById(int id, [FromRoute] string color)
```

If the route is missing it, then it will throw and exception. We can specify the parameter as optional: `FromRoute] string? color`

- Binding from **query** params

```cs
 [HttpGet("{id}")]
 public IActionResult GetShirtById(int id, [FromQuery] string color)
```

If the query params is missing it, then it will throw and exception. We can specify the parameter as optional: `FromQuery] string? color`

- Binding from **header**

We specify the key of the header key/value pair:

```cs
 [HttpGet("{id}")]
 public IActionResult GetShirtById(int id, [FromHeader(Name = "color")] string color)
```

If the header with the key is missing, then it will throw an exception, We can specify the parameter as optional: `FromHeader(Name = "color")] string? color`

- Model binding from **body**

This receives a json serialized object:

```cs
 [HttpPost]
 public IActionResult CreateShirt([FromBody] Shirt shirt)
```

, and the request looks like this:

```js
POST {{WebApiDemo_BaseUrl}}/api/shirts
Content-Type: application/json

{
  "size": 43,
  "color": "Blue",
  "price": 19.99
}
```

- Model binding from **form**

This receives a form body data:

```cs
 [HttpPost]
 public IActionResult CreateShirt([FromForm] Shirt shirt)
```

, and the request looks like this:

```js
POST {{WebApiDemo_BaseUrl}}/api/shirts
Content-Type: application/x-www-form-urlencoded

size=43&color=Blue&price=19.99
```

## Lesson 02.11: Model Validation with DataAnnotations

The model binding runs before the controller method.

The model validation is the next step of the pipeline also before the controller method.

They are couple of ways to provide model validation. One way is to use **DataAnnotations**. In the below example, nullable properties that are not marked `[Required]` are optional.

```cs
using System.ComponentModel.DataAnnotations;

namespace WebAppiDemo.Models
{
    public class Shirt
    {
        public int ShirtId { get; set; }

        [Required]
        public string? Brand { get; set; }

        [Required]
        public string? Color { get; set; }

        public int? Size { get; set; }

        [Required]
        public string? Gender { get; set; }

        public double? Price { get; set; }
    }
}
```

When we run request and the validation fails, we get an error message:

```json
HTTP/1.1 400 Bad Request
Connection: close
Content-Type: application/problem+json; charset=utf-8
Date: Tue, 25 Nov 2025 05:29:11 GMT
Server: Kestrel
Transfer-Encoding: chunked

{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Brand": [
      "The Brand field is required."
    ],
    "Gender": [
      "The Gender field is required."
    ]
  },
  "traceId": "00-31b9a1519cf7117415e717631c7ca7f9-71a4c09ae46e9711-00"
}
```

They are many more validation attributes. Tak a look here:

[DataAnnotations documentation](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations?view=net-10.0)

## Lesson 02.12: Model Validation with custom Validation attribute

Sometimes we need to evaluate more complex logic. For example we need to compare 2 or more of the properties of the model.

One option to add these classes is in `Models/Validations`. We prefix these with the name of the model. Example: `Shirt_EnsureCorrectSizingAttribute`.

```cs
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace WebAppiDemo.Models.Validations
{
    public class Shirt_EnsureCorrectSizingAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var shirt = validationContext.ObjectInstance as Shirt;
            if (shirt != null && !string.IsNullOrWhiteSpace(shirt.Gender) && shirt.Size.HasValue)
            {
                if (shirt.Gender.Equals("Male", StringComparison.OrdinalIgnoreCase) && shirt.Size < 8)
                {
                    return new ValidationResult("Invalid size for male shirt. Minimum size is 8.");
                }
                else if (shirt.Gender.Equals("Female", StringComparison.OrdinalIgnoreCase) && shirt.Size < 6)
                {
                    return new ValidationResult("Invalid size for female shirt. Minimum size is 6.");
                }
            }
            return ValidationResult.Success;
        }
    }
}
```

And if we call the end point with size that is too small:

```js
POST {{WebApiDemo_BaseUrl}}/api/shirts
Content-Type: application/json

{
  "size": 5,
  "color": "Blue",
  "price": 19.99,
  "brand": "Nike",
  "gender": "female"
}
```

, the response is:

```json
HTTP/1.1 400 Bad Request
Connection: close
Content-Type: application/problem+json; charset=utf-8
Date: Tue, 25 Nov 2025 06:26:11 GMT
Server: Kestrel
Transfer-Encoding: chunked

{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Size": [
      "Invalid size for female shirt. Minimum size is 6."
    ]
  },
  "traceId": "00-a65a9d84feca6e409586b9d3588265b6-abc29d042f2ea54a-00"
}
```

## Lesson 02.12-Remarks: Validation vs DB

Keep using validation attributes for:

- Required fields
- String length, range validation
- Format validation (email, phone)
- Business rules based on the object's own properties (like your size/gender rule)

For DB validation, use these approaches instead of ValidationAttribute.

### Action Filter (for cross-cutting concerns)

```cs
public class ValidateShirtExistsAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var id = context.ActionArguments["id"] as int?;
        var dbContext = context.HttpContext.RequestServices
            .GetRequiredService<AppDbContext>();

        if (!await dbContext.Shirts.AnyAsync(s => s.Id == id))
        {
            context.Result = new NotFoundResult();
            return;
        }
        await next();
    }
}
```

### In the Controller Action (most common)

```cs
[HttpPut("{id}")]
public async Task<IActionResult> UpdateShirt(int id, [FromBody] Shirt shirt)
{
    var existingShirt = await _dbContext.Shirts.FindAsync(id);
    if (existingShirt == null)
        return NotFound();

    // Update logic...
}
```

, and not found returns:

```json
HTTP/1.1 404 Not Found
Connection: close
Content-Type: application/problem+json; charset=utf-8
Date: Tue, 25 Nov 2025 15:32:37 GMT
Server: Kestrel
Transfer-Encoding: chunked

{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5",
  "title": "Not Found",
  "status": 404,
  "traceId": "00-ab4f4aa3914b87d0113cbc8a439f6dae-91c7703e4abe12eb-00"
}
```

### Service/Business Layer (best for complex logic)

```cs
public class ShirtService
{
    public async Task<Result> CreateShirtAsync(Shirt shirt)
    {
        if (await _dbContext.Shirts.AnyAsync(s => s.Sku == shirt.Sku))
            return Result.Fail("SKU already exists");

        // Create logic...
    }
}
```

## Lesson 02.12-Remarks: How client should handle the ApI errors

Based on the standard .NET Web API responses shown in your notes, here are the proper ways to check for errors:

### HTTP Status Codes (Primary Method)

The most reliable way is to check the HTTP status code:

```js
// In your HTTP client or JavaScript
if (response.status >= 400) {
    // Error occurred
    console.log('Error:', response.status);
}

// More specific checks
if (response.status === 400) {
    // Bad Request - validation errors
} else if (response.status === 404) {
    // Not Found
} else if (response.status >= 500) {
    // Server error
}
```

### Response Body Structure

For validation errors (400 Bad Request), the response follows the Problem Details format:

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Brand": ["The Brand field is required."],
    "Gender": ["The Gender field is required."]
  },
  "traceId": "00-31b9a1519cf7117415e717631c7ca7f9-71a4c09ae46e9711-00"
}
```

For Not Found errors (404), it's simpler:

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5",
  "title": "Not Found",
  "status": 404,
  "traceId": "00-ab4f4aa3914b87d0113cbc8a439f6dae-91c7703e4abe12eb-00"
}
```

### Practical Error Handling Examples

In HTTP Files (VS Code REST Client):

```js
### Test with error handling
POST {{WebApiDemo_BaseUrl}}/api/shirts
Content-Type: application/json

{
  "color": "Blue",
  "price": 19.99
}

> {%
client.test("Request executed successfully", function() {
    client.assert(response.status !== 500, "Server error occurred");
    if (response.status === 400) {
        client.log("Validation errors: " + JSON.stringify(response.body.errors));
    }
});
%}
```

In JavaScript/TypeScript:

```js
async function createShirt(shirtData) {
    try {
        const response = await fetch('/api/shirts', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(shirtData)
        });

        if (!response.ok) {
            const errorData = await response.json();

            if (response.status === 400) {
                // Handle validation errors
                console.log('Validation errors:', errorData.errors);
                return { success: false, validationErrors: errorData.errors };
            } else if (response.status === 404) {
                console.log('Resource not found');
                return { success: false, error: 'Not found' };
            } else {
                console.log('Unexpected error:', errorData);
                return { success: false, error: errorData.title };
            }
        }

        const data = await response.json();
        return { success: true, data };
    } catch (error) {
        console.error('Network error:', error);
        return { success: false, error: 'Network error' };
    }
}
```

In C# (HttpClient):

```cs
public async Task<ApiResult<Shirt>> CreateShirtAsync(Shirt shirt)
{
    var response = await _httpClient.PostAsJsonAsync("/api/shirts", shirt);

    if (response.IsSuccessStatusCode)
    {
        var createdShirt = await response.Content.ReadFromJsonAsync<Shirt>();
        return ApiResult<Shirt>.Success(createdShirt);
    }
    else if (response.StatusCode == HttpStatusCode.BadRequest)
    {
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        return ApiResult<Shirt>.ValidationError(problemDetails.Errors);
    }
    else if (response.StatusCode == HttpStatusCode.NotFound)
    {
        return ApiResult<Shirt>.NotFound();
    }
    else
    {
        return ApiResult<Shirt>.Error($"Unexpected error: {response.StatusCode}");
    }
}
```

### Key Points for Error Detection

Key Points for Error Detection:

1. Always check HTTP status code first - it's the most reliable indicator
1. Status codes 400-499 indicate client errors (bad request, validation, not found, etc.)
1. Status codes 500-599 indicate server errors
Problem Details format is the standard for .NET error responses
1. Validation errors will have an errors object with field-specific messages
1. The `traceId` can be useful for debugging server-side issues

The HTTP status code is your primary indicator of success vs. error, and the response body provides the details about what went wrong.

## Lesson 02.13: Return types

Typically we return `IActionResult` from controller methods:

```cs
[HttpGet("{id}")]
public IActionResult GetShirtById(int id)
{
 if (id <= 0)
 {
  return BadRequest("Invalid shirt ID.");
 }

 var shirt = shirts.FirstOrDefault(s => s.ShirtId == id);
 if (shirt == null)
 {
  return NotFound();
 }
 return Ok(shirt);
}
```
