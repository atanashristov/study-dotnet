# Section_03_23-Database_Connection-Seed_Villas_Table-Prompt.md

## Summary

Add data seeding for the "Villas" table

## Details

Add seeding for the "Villas" table using the Dot.Net 10 style.

The seeding only happens in Development mode.

We check if the DB is created.

Here is a sample code that we can use as an example:

```cs
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
                hasData = await context.Villas.AnyAsync();
            }
            catch (Exception ex) when (ex.Message.Contains("does not exist") || ex.Message.Contains("pending changes"))
            {
                Console.WriteLine("⚠️ Migration issue detected - this may be normal for the first startup. Retrying...");
                throw; // This will trigger a retry
            }

            if (!hasData)
            {
                Console.WriteLine("🌱 Database is empty - seeding development data...");
                context.Villas.AddRange(
                new Villa
                {
                    Id = 1,
                    Name = "Royal Villa",
                    Details = "Luxurious villa with stunning ocean views and private beach access.",
                    Rate = 500.0,
                    Sqft = 2500,
                    Occupancy = 6,
                    ImageUrl = "https://dotnetmasteryimages.blob.core.windows.net/bluevillaimages/villa1.jpg",
                    CreatedDate = new DateTime(2024, 1, 1),
                    UpdatedDate = new DateTime(2024, 1, 1)
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
                    CreatedDate = new DateTime(2024, 1, 15),
                    UpdatedDate = new DateTime(2024, 1, 15)
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
                    CreatedDate = new DateTime(2024, 2, 1),
                    UpdatedDate = new DateTime(2024, 2, 1)
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
                    CreatedDate = new DateTime(2024, 2, 14),
                    UpdatedDate = new DateTime(2024, 2, 14)
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
                    CreatedDate = new DateTime(2024, 3, 1),
                    UpdatedDate = new DateTime(2024, 3, 1)
                }

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

```

We don't want to drop that code in pollute the Program.cs. Instead we want to create a class that does the job and use that class inside Program.cs.

Run the instructions and create notes and instructions around the seeding into a file "Section_03_23-Database_Connection-Seed_Villas_Table-result.md" next to this file.

Summarize what we have done at the end of "02_Notes/Section_03-Database_Connection.md" file.
