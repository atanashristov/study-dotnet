using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Configure JSON serialization to use camelCase
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
      options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

// Configure ApiBehaviorOptions to use camelCase for validation errors
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
  options.InvalidModelStateResponseFactory = context =>
  {
    var problemDetails = new ValidationProblemDetails(context.ModelState);

    // Convert error keys to camelCase
    var camelCaseErrors = new Dictionary<string, string[]>();
    foreach (var error in problemDetails.Errors)
    {
      // Handle empty keys or convert to camelCase
      var camelCaseKey = string.IsNullOrEmpty(error.Key)
        ? error.Key
        : char.ToLowerInvariant(error.Key[0]) + error.Key.Substring(1);
      camelCaseErrors[camelCaseKey] = error.Value;
    }

    problemDetails.Errors.Clear();
    foreach (var error in camelCaseErrors)
    {
      problemDetails.Errors[error.Key] = error.Value;
    }

    return new BadRequestObjectResult(problemDetails);
  };
});

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/", () => "Hello World!");

app.MapControllers();

app.Run();
