# Section_03_23-Database_Connection-Seed_Villas_Table-Result.md

## Summary

Database seeding has been successfully implemented using .NET 10 style with a clean, class-based approach. The seeding only runs in Development mode and includes retry logic for database readiness.

## Implementation Details

### Files Created/Modified

#### 1. **Data/DatabaseSeeder.cs** (New)

A dedicated class responsible for database initialization and seeding logic:

```csharp
public class DatabaseSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DatabaseSeeder> _logger;

    public async Task InitializeAsync()
    {
        // Handles migration and seeding with retry logic
    }

    private async Task SeedVillasAsync()
    {
        // Seeds 5 villa records
    }
}
```

**Key Features:**

- **Automatic Migrations**: Applies pending migrations automatically using `MigrateAsync()`
- **Retry Logic**: Attempts up to 10 times with 2-second delays between retries
- **Smart Seeding**: Only seeds data if the database is empty
- **Detailed Logging**: Emoji-enhanced logs for easy status tracking
- **Exception Handling**: Gracefully handles connection issues and migration conflicts

#### 2. **Program.cs** (Modified)

Updated to use the DatabaseSeeder in development mode:

```csharp
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<DatabaseSeeder>>();
    var seeder = new DatabaseSeeder(context, logger);
    await seeder.InitializeAsync();
}
```

### Seeded Data

The following 5 villa records are seeded:

| Id  | Name          | Rate | Sqft | Occupancy | Created Date |
| --- | ------------- | ---- | ---- | --------- | ------------ |
| 1   | Royal Villa   | $500 | 2500 | 6         | 2024-01-01   |
| 2   | Diamond Villa | $750 | 3200 | 8         | 2024-01-15   |
| 3   | Pool Villa    | $350 | 1800 | 4         | 2024-02-01   |
| 4   | Luxury Villa  | $900 | 4000 | 10        | 2024-02-14   |
| 5   | Garden Villa  | $275 | 1500 | 3         | 2024-03-01   |

## How It Works

### Development Mode

When running in Development mode:

1. Application starts and detects Development environment
2. DatabaseSeeder is instantiated with dependency injection
3. Seeder checks database connection (with retry logic)
4. Applies any pending migrations automatically
5. Checks if Villas table has data
6. If empty, seeds the 5 sample villas
7. Application continues normal startup

### Production Mode

In Production or other environments:

- Seeding is **completely skipped**
- A log message indicates the environment
- Migrations must be applied manually or via deployment pipeline

## Running the Application

You have two options for running the application:

### Option 1: Using Docker Compose (Recommended)

This runs both PostgreSQL **and** the Web API in containers:

```bash
cd 01_Code
docker-compose up -d
```

**What happens:**

- PostgreSQL container starts and waits for health check
- Web API container starts (depends on postgres being healthy)
- Application detects Development environment
- **Seeding runs automatically** inside the container
- API is available at <http://localhost:5050>

**View the logs to see seeding:**

```bash
docker-compose logs -f webapi
```

**Expected Console Output:**

```
🚀 Development environment detected - starting database setup and seeding...
🔄 Attempting database migration (attempt 1/10)...
✅ Database migration completed successfully!
🌱 Database is empty - seeding development data...
✅ Development data seeded successfully!
🎉 Database setup completed successfully!
```

### Option 2: Running Locally (Development Only)

This runs the Web API on your host machine (useful for debugging):

1. **Start only PostgreSQL in Docker:**

   ```bash
   cd 01_Code
   docker-compose up -d postgres
   ```

2. **Run the application locally:**

   ```bash
   cd RoyalVillaApi
   dotnet run
   ```

   The app uses the connection string from `appsettings.Development.json` (localhost:5432) and is available at <http://localhost:5000>

3. **Expected Console Output:**

   ```
   🚀 Development environment detected - starting database setup and seeding...
   🔄 Attempting database migration (attempt 1/10)...
   ✅ Database migration completed successfully!
   🌱 Database is empty - seeding development data...
   ✅ Development data seeded successfully!
   🎉 Database setup completed successfully!
   ```

### Subsequent Runs (Either Option)

On subsequent runs, you'll see:

```
🚀 Development environment detected - starting database setup and seeding...
🔄 Attempting database migration (attempt 1/10)...
✅ Database migration completed successfully!
ℹ️ Database already contains data - skipping seeding.
🎉 Database setup completed successfully!
```

## Verifying the Seeded Data

### Using psql

Connect to the database:

```bash
docker exec -it royalvilla_postgres psql -U postgres -d royalvilla
```

Query the data:

```sql
SELECT * FROM "Villas";
```

Exit psql:

```
\q
```

### Using API Endpoints

Once the API is running, you can access the villas via your controller endpoints (e.g., `GET /api/villas`).

## Advantages of This Approach

1. **Clean Separation**: Database logic is separated from Program.cs
2. **Reusable**: DatabaseSeeder can be easily extended for other entities
3. **Environment-Aware**: Only runs in Development
4. **Resilient**: Retry logic handles timing issues with Docker containers
5. **Maintainable**: Easy to modify seed data in one place
6. **Testable**: Can be unit tested independently
7. **Logging**: Clear visibility into what's happening

## Customizing Seed Data

To modify or add seed data, edit the `SeedVillasAsync()` method in [Data/DatabaseSeeder.cs](../01_Code/RoyalVillaApi/Data/DatabaseSeeder.cs):

```csharp
private async Task SeedVillasAsync()
{
    _context.Villas.AddRange(
        new Villa
        {
            Id = 6,
            Name = "Your New Villa",
            Details = "Description...",
            Rate = 600.0,
            // ... other properties
        }
    );
    await _context.SaveChangesAsync();
}
```

**Important**: If you add villas with specific IDs, you may need to reset the database first:

```bash
dotnet ef database drop --force
dotnet run
```

## Disabling Auto-Seeding

If you want to disable automatic seeding temporarily:

1. Comment out the seeding block in Program.cs
2. Or set environment variable: `ASPNETCORE_ENVIRONMENT=Production`
3. Or use conditional compilation: `#if DEBUG`

## Alternative: Traditional Migration-Based Seeding

The traditional approach uses `ModelBuilder.Entity<Villa>().HasData()` in `ApplicationDbContext.OnModelCreating()`. This approach:

- Creates seed data as part of migrations
- Tracks seed data changes in migration files
- Works in all environments

The new approach (used here):

- Seeds data at application startup
- Only in Development mode
- Easier to modify without creating new migrations
- Better for dynamic seed data

## Troubleshooting

### Seeding Not Running After Code Changes

**Problem:** You added the DatabaseSeeder code but seeding doesn't run when you do `docker-compose up -d`.

**Cause:** Docker is using a cached/stale image built before the DatabaseSeeder was added.

**Solution:**

```bash
# Stop containers and rebuild the webapi image
cd 01_Code
docker-compose down
docker-compose build --no-cache webapi
docker-compose up -d
```

**Important:** Always rebuild the Docker image after making code changes!

### DateTime Kind Error (PostgreSQL)

**Problem:** You see this error:

```
Cannot write DateTime with Kind=Unspecified to PostgreSQL type 'timestamp with time zone',
only UTC is supported.
```

**Cause:** PostgreSQL requires DateTime values to have `DateTimeKind.Utc`. Using `new DateTime(2024, 1, 1)` creates an unspecified DateTime.

**Solution:** Use `DateTime.SpecifyKind()` to set the kind to UTC:

```csharp
// ❌ Wrong - Unspecified kind
CreatedDate = new DateTime(2024, 1, 1)

// ✅ Correct - UTC kind
CreatedDate = DateTime.SpecifyKind(new DateTime(2024, 1, 1), DateTimeKind.Utc)
```

### Entity Tracking Conflicts

**Problem:** You see this error during retries:

```
The instance of entity type 'Villa' cannot be tracked because another instance
with the same key value for {'Id'} is already being tracked.
```

**Cause:** The retry logic attempts to track the same entities multiple times after failed attempts.

**Solution:** Clear the change tracker in the catch block:

```csharp
catch (Exception ex)
{
    retryCount++;
    _logger.LogError("❌ Database connection attempt {AttemptNumber} failed", retryCount);

    // Clear tracked entities before retry
    _context.ChangeTracker.Clear();

    if (retryCount < maxRetries)
    {
        await Task.Delay(2000);
    }
}
```

### Database Won't Connect

If you see retry messages:

```
❌ Database connection attempt 1 failed: ...
⏳ Retrying in 2 seconds... (1/10)
```

**Solutions:**

- Wait for Docker PostgreSQL container to fully start
- Check Docker container status: `docker-compose ps`
- Verify connection string in appsettings.Development.json
- Check PostgreSQL logs: `docker-compose logs postgres`

### Duplicate Key Errors

If you see "duplicate key value violates unique constraint":

**Solution (Local):**

```bash
# Drop and recreate the database
dotnet ef database drop --force
dotnet run
```

**Solution (Docker):**

```bash
# Completely reset Docker environment including database volume
cd 01_Code
docker-compose down -v
docker-compose up -d
```

### Migration Issues

If migrations fail:

```bash
# Check migration status
dotnet ef migrations list

# Remove last migration if needed
dotnet ef migrations remove

# Create fresh migration
dotnet ef migrations add Initial
```

## Best Practices

1. **Never use auto-seeding in Production** - Always use migration scripts
2. **Keep seed data minimal** - Only what's needed for development
3. **Use realistic data** - Helps with testing and demos
4. **Version control seed data** - Track changes in Git
5. **Document seed data** - Explain what each record represents
6. **Reset when needed** - Don't be afraid to drop and recreate dev databases

## Next Steps

With seeded data in place, you can now:

1. Create API endpoints to retrieve villas
2. Implement CRUD operations
3. Add filtering and pagination
4. Test with realistic data
5. Build front-end integrations

The database is now ready for development work!
