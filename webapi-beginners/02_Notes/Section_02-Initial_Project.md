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
