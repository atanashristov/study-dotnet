using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppiDemo.Data;
using WebAppiDemo.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
// Another way would be specify:
// options.UseNpgsql(builder.Configuration["ConnectionStrings:DefaultConnection"])

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

// Auto-create database in development environment only
if (app.Environment.IsDevelopment())
{
    Console.WriteLine("🚀 Development environment detected - starting database setup and seeding...");

    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // Wait for database to be ready with retry logic
    var retryCount = 0;
    const int maxRetries = 10;
    bool databaseSetupSuccessful = false;

    while (retryCount < maxRetries && !databaseSetupSuccessful)
    {
        try
        {
            Console.WriteLine($"🔄 Attempting database migration (attempt {retryCount + 1}/{maxRetries})...");

            // Apply any pending migrations
            await context.Database.MigrateAsync();
            Console.WriteLine("✅ Database migration completed successfully!");

            // Seed development data only if database is empty
            bool hasData = false;
            try
            {
                hasData = await context.Shirts.AnyAsync();
            }
            catch (Exception ex) when (ex.Message.Contains("does not exist") || ex.Message.Contains("pending changes"))
            {
                Console.WriteLine("⚠️ Migration issue detected - this may be normal for the first startup. Retrying...");
                throw; // This will trigger a retry
            }

            if (!hasData)
            {
                Console.WriteLine("🌱 Database is empty - seeding development data...");
                context.Shirts.AddRange(
                    new Shirt { Brand = "Nike", Color = "Red", Size = 10, Gender = "Male", Price = 29.99 },
                    new Shirt { Brand = "Adidas", Color = "Blue", Size = 12, Gender = "Male", Price = 34.99 },
                    new Shirt { Brand = "Puma", Color = "Green", Size = 8, Gender = "Female", Price = 24.99 }
                );
                await context.SaveChangesAsync();
                Console.WriteLine("✅ Development data seeded successfully!");
            }
            else
            {
                Console.WriteLine("ℹ️ Database already contains data - skipping seeding.");
            }

            databaseSetupSuccessful = true; // Success - exit retry loop
        }
        catch (Exception ex)
        {
            retryCount++;
            Console.WriteLine($"❌ Database connection attempt {retryCount} failed: {ex.Message}");

            if (retryCount < maxRetries)
            {
                Console.WriteLine($"⏳ Retrying in 2 seconds... ({retryCount}/{maxRetries})");
                await Task.Delay(2000);
            }
            else
            {
                Console.WriteLine("💥 All database connection attempts failed. Application will continue but database may not be ready.");
                Console.WriteLine($"🔍 Final error: {ex}");
            }
        }
    }

    if (databaseSetupSuccessful)
    {
        Console.WriteLine("🎉 Database setup completed successfully!");
    }
}
else
{
    Console.WriteLine($"🔒 Environment: {app.Environment.EnvironmentName} - Skipping automatic database setup.");
}

// Only use HTTPS redirection outside of Docker
if (!app.Environment.IsDevelopment() || Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") != "true")
{
    app.UseHttpsRedirection();
}
else
{
    Console.WriteLine("🐳 Running in Docker container - skipping HTTPS redirection.");
}

app.MapGet("/", () => "Hello World!");

app.MapControllers();

app.Run();
