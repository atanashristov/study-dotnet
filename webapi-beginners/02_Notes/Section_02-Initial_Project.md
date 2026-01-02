# Section 02: Initial Project

## Lesson 02.11: Create Project

Create API project named _RoyalVillaApi_ and a solution:

```sh
dotnet new webapi -n RoyalVillaApi --use-controllers --framework net10.0
dotnet new sln -n RoyalVilla
dotnet sln ./RoyalVilla.sln add ./RoyalVillaApi/RoyalVillaApi.csproj
```

We can start the project:

```sh
cd RoyalVillaApi
dotnet run
```

, and browse to <http://localhost:5240/WeatherForecast/>.

## Lesson 02.12: Add Scalar

**Installing Scalar:**

While `Microsoft.AspNetCore.OpenApi` is installed by default, we need `Scalar.AspNetCore`.

You can install Scalar (and OpenApi if needed) via Package Manager Console:

```bash
# Installed by default
dotnet add package Microsoft.AspNetCore.OpenApi

# We need this package for Scalar
dotnet add package Scalar.AspNetCore

# Eventually clean rebuild
dotnet clean && dotnet build
```

Configure Scalar:

In _Program.cs_ we have by default OpenApi:

```cs
using Scalar.AspNetCore;

...

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
```

Now we can run the project and browse to <http://localhost:5240/scalar/v1>.

## Lesson 02.13: Make Scalar UI default url

Edit _launchSettings.json_ file and add make sure the profiles have:

```json
      "launchBrowser": true,
      "launchUrl": "scalar",
```

## Lesson 02.14: Code cleanup

Removed `WeatherForecastController`.

## Lesson 02.15: Add first API controller

Added `VillaController`.

It inherits from `ControllerBase`.

We decorate the controller with `ApiController` attribute.

## Lesson 02.16: First endpoint

Add controller method attribute it with _verb_, and a _route_.

## Lesson 02.17: Route Name

Set route name on the controller and use conventional routes for the endpoint based on action method name.

For the route on controller, prefer explicit path `[Route("api/villa")]` instead of controller name based `[Route("api/[controller]")]`

## Lesson 02.18: Route Parameters Binding

Provide e GET villa details method:

```cs
        [HttpGet("{id:int}")]
        public string GetVillaById(int id)
        {
            return "Villa Details of Id: " + id;
        }

```

## Lesson 02.19: Parameters of API

Provide action method with multiple path based parameters:

```cs
        [HttpGet("{id:int}/{name}")]
        public string GetVillaByIdAndName(int id, string name)
        {
            return "Villa Details of Id: " + id + " and Name: " + name;
        }
```

Note: "string" is the default type for name binding. If we specify the type, it will throw compiler error.

The _default route binding_ is based on route path (`FromRoute` attribute):

```cs
        [HttpGet("{id:int}/{name}")]
        public string GetVillaByIdAndName([FromRoute] int id, [FromRoute] string name)
        {
            return "Villa Details of Id: " + id + " and Name: " + name;
        }
```

We can bind parameters using the following attributes

- `FromRoute`: bind from route path segments
- `FromQuery`: bind from query string params
- `FromHeader`: bind from header params

Note: The path will have to stay unique.
