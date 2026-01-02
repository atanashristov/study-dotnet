using Microsoft.EntityFrameworkCore;
using RoyalVillaApi.Models;

namespace RoyalVillaApi.Data
{
  public class DatabaseSeeder
  {
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(ApplicationDbContext context, ILogger<DatabaseSeeder> logger)
    {
      _context = context;
      _logger = logger;
    }

    public async Task InitializeAsync()
    {
      _logger.LogInformation("🚀 Development environment detected - starting database setup and seeding...");

      var retryCount = 0;
      const int maxRetries = 10;
      bool databaseSetupSuccessful = false;

      while (retryCount < maxRetries && !databaseSetupSuccessful)
      {
        try
        {
          _logger.LogInformation("🔄 Attempting database migration (attempt {AttemptNumber}/{MaxRetries})...", retryCount + 1, maxRetries);

          // Apply any pending migrations
          await _context.Database.MigrateAsync();
          _logger.LogInformation("✅ Database migration completed successfully!");

          // Seed development data only if database is empty
          bool hasData = false;
          try
          {
            hasData = await _context.Villas.AnyAsync();
          }
          catch (Exception ex) when (ex.Message.Contains("does not exist") || ex.Message.Contains("pending changes"))
          {
            _logger.LogWarning("⚠️ Migration issue detected - this may be normal for the first startup. Retrying...");
            throw; // This will trigger a retry
          }

          if (!hasData)
          {
            await SeedVillasAsync();
          }
          else
          {
            _logger.LogInformation("ℹ️ Database already contains data - skipping seeding.");
          }

          databaseSetupSuccessful = true; // Success - exit retry loop
        }
        catch (Exception ex)
        {
          retryCount++;
          _logger.LogError("❌ Database connection attempt {AttemptNumber} failed: {ErrorMessage}", retryCount, ex.Message);

          // Clear the change tracker to avoid entity tracking conflicts on retry
          _context.ChangeTracker.Clear();

          if (retryCount < maxRetries)
          {
            _logger.LogInformation("⏳ Retrying in 2 seconds... ({AttemptNumber}/{MaxRetries})", retryCount, maxRetries);
            await Task.Delay(2000);
          }
          else
          {
            _logger.LogError(ex, "💥 All database connection attempts failed. Application will continue but database may not be ready.");
          }
        }
      }

      if (databaseSetupSuccessful)
      {
        _logger.LogInformation("🎉 Database setup completed successfully!");
      }
    }

    private async Task SeedVillasAsync()
    {
      _logger.LogInformation("🌱 Database is empty - seeding development data...");

      _context.Villas.AddRange(
          new Villa
          {
            Id = 1,
            Name = "Royal Villa",
            Details = "Luxurious villa with stunning ocean views and private beach access.",
            Rate = 500.0,
            Sqft = 2500,
            Occupancy = 6,
            ImageUrl = "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa1.jpg",
            CreatedDate = DateTime.SpecifyKind(new DateTime(2024, 1, 1), DateTimeKind.Utc),
            UpdatedDate = DateTime.SpecifyKind(new DateTime(2024, 1, 1), DateTimeKind.Utc)
          },
          new Villa
          {
            Id = 2,
            Name = "Diamond Villa",
            Details = "Elegant villa with marble interiors and panoramic mountain views.",
            Rate = 750.0,
            Sqft = 3200,
            Occupancy = 8,
            ImageUrl = "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa2.jpg",
            CreatedDate = DateTime.SpecifyKind(new DateTime(2024, 1, 15), DateTimeKind.Utc),
            UpdatedDate = DateTime.SpecifyKind(new DateTime(2024, 1, 15), DateTimeKind.Utc)
          },
          new Villa
          {
            Id = 3,
            Name = "Pool Villa",
            Details = "Modern villa featuring an infinity pool and outdoor entertainment area.",
            Rate = 350.0,
            Sqft = 1800,
            Occupancy = 4,
            ImageUrl = "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa3.jpg",
            CreatedDate = DateTime.SpecifyKind(new DateTime(2024, 2, 1), DateTimeKind.Utc),
            UpdatedDate = DateTime.SpecifyKind(new DateTime(2024, 2, 1), DateTimeKind.Utc)
          },
          new Villa
          {
            Id = 4,
            Name = "Luxury Villa",
            Details = "Premium villa with spa facilities and concierge services.",
            Rate = 900.0,
            Sqft = 4000,
            Occupancy = 10,
            ImageUrl = "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa4.jpg",
            CreatedDate = DateTime.SpecifyKind(new DateTime(2024, 2, 14), DateTimeKind.Utc),
            UpdatedDate = DateTime.SpecifyKind(new DateTime(2024, 2, 14), DateTimeKind.Utc)
          },
          new Villa
          {
            Id = 5,
            Name = "Garden Villa",
            Details = "Charming villa surrounded by tropical gardens and nature trails.",
            Rate = 275.0,
            Sqft = 1500,
            Occupancy = 3,
            ImageUrl = "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa5.jpg",
            CreatedDate = DateTime.SpecifyKind(new DateTime(2024, 3, 1), DateTimeKind.Utc),
            UpdatedDate = DateTime.SpecifyKind(new DateTime(2024, 3, 1), DateTimeKind.Utc)
          }
      );

      await _context.SaveChangesAsync();
      _logger.LogInformation("✅ Development data seeded successfully!");
    }
  }
}
