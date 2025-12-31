var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/", () => "Hello World!");

app.MapGet("/shirts", () =>
{
  var shirts = new[]
  {
        new { Id = 1, Color = "Red", Size = "M" },
        new { Id = 2, Color = "Blue", Size = "L" },
        new { Id = 3, Color = "Green", Size = "S" }
    };
  return Results.Ok(shirts);
});

app.MapGet("/shirts/{id}", (int id) =>
{
  var shirt = new { Id = id, Color = "Red", Size = "M" };
  return Results.Ok(shirt);
});

app.MapPost("/shirts", () =>
{
  // In a real application, you would save the shirt to a database
  // return Results.Created("/shirts/4", new { Id = 4, Color = "Yellow", Size = "L" });
  return Results.Created("/shirts/4", new { Id = 4 });
});

app.MapPut("/shirts/{id}", (int id) =>
{
  // In a real application, you would update the shirt in a database
  return Results.NoContent();
});

app.MapPatch("/shirts/{id}", (int id) =>
{
  // In a real application, you would partially update the shirt in a database
  return Results.NoContent();
});

app.MapDelete("/shirts/{id}", (int id) =>
{
  // In a real application, you would delete the shirt from a database
  return Results.NoContent();
});

app.Run();
