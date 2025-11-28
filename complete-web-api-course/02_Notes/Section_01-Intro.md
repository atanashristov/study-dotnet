# Section 01: Intro

## Lesson 01.04: How Web API Works (Example with minimal API)

Create minimal API app with:

```sh
dotnet new web -n MinimalWebApiDemo -o MinimalWebApiDemo --framework net10.0
```

Run the app:

```sh
cd MinimalWebApiDemo
dotnet run
```

A minimal code to implement an API:

```cs
var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/", () => "Hello World!");

app.MapPut("/shirts/{id}", (int id) =>
{
  return Results.NoContent();
});

app.Run();
```

## Lesson 01.05: What is a Web Framework

They are the things that the API provides while running the handler for a method:

- Authentication and Authorization
- Routing
- Model binding: the input data gets provided
- Model validation: type, valid values, etc
- Execute the handler
- Exception handling
- Result formatting

## Lesson 01.06: ASP.NET Core Middleware

The ASP.NET Framework provides a middleware pipeline which consists of middleware components. Each component is responsible for a step in the middleware. It implements the  chain of responsibilities pattern:

```sh
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
...
```
