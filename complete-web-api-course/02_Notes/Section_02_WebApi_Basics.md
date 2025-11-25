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
