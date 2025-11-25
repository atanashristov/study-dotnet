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
