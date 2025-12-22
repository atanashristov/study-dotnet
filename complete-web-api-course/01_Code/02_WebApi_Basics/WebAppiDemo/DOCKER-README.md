# Docker Setup for WebApiDemo

## Overview

This project includes Docker support with a PostgreSQL database. The setup includes:

- ASP.NET Core Web API container
- PostgreSQL database container
- Network configuration for inter-container communication
- Volume persistence for database data

## Quick Start

### Prerequisites

- Docker Desktop installed and running
- Docker Compose installed (included with Docker Desktop)

### Run the Application

1. **Build and start all services:**

   ```bash
   docker-compose up --build
   ```

2. **Run in detached mode (background):**

   ```bash
   docker-compose up -d --build
   ```

3. **Stop the services:**

   ```bash
   docker-compose down
   ```

4. **Stop and remove volumes (clean reset):**

   ```bash
   docker-compose down -v
   ```

### Individual Service Control

**Start/Stop specific services:**

```bash
# Start only PostgreSQL database
docker-compose up -d postgres

# Start only the Web API (requires database to be running)
docker-compose up -d webapi

# Stop specific service
docker-compose stop webapi
docker-compose stop postgres

# Restart specific service
docker-compose restart webapi
docker-compose restart postgres

# Start specific service that's stopped
docker-compose start webapi
docker-compose start postgres
```

**Rebuild specific service:**

```bash
# Rebuild only Web API container
docker-compose build webapi
docker-compose up -d webapi

# Rebuild and restart specific service
docker-compose up -d --build webapi
```

**View status of specific service:**

```bash
# Check specific service status
docker-compose ps webapi
docker-compose ps postgres

# View logs for specific service
docker-compose logs -f webapi
```

**Common Use Cases:**

```bash
# Restart API after code changes (keeps database running)
docker-compose restart webapi

# Rebuild API with new code changes
docker-compose build webapi && docker-compose up -d webapi

# Start database only (for local development)
docker-compose up -d postgres
```

## Services

### Web API Service

- **Container Name**: `webapi_webapi_1` (auto-generated)
- **Ports**:
  - HTTP: `localhost:5000` → container port `8080`
  - HTTPS: `localhost:5001` → container port `8081`
- **API Base URL**: `http://localhost:5000/api` or `https://localhost:5001/api`

### PostgreSQL Database Service

- **Container Name**: `postgres-webapi`
- **Port**: `localhost:5432` → container port `5432`
- **Database**: `WebApiDemo`
- **Username**: `postgres`
- **Password**: `postgres123`

## Database Connection

The Web API automatically connects to the PostgreSQL database using:

```
Host=postgres;Database=WebApiDemo;Username=postgres;Password=postgres123
```

### Connect from Host Machine

You can connect to the database directly from your host machine using:

```
Host=localhost;Port=5432;Database=WebApiDemo;Username=postgres;Password=postgres123
```

## Database and Shell Access

### PostgreSQL Database Access

#### 1. **Direct psql access to your database:**

```bash
docker exec -it postgres-webapi psql -U postgres -d WebApiDemo
```

#### 2. **psql access as postgres superuser:**

```bash
docker exec -it postgres-webapi psql -U postgres
```

#### 3. **Connect to specific database:**

```bash
docker exec -it postgres-webapi psql -U postgres -d postgres
```

### Shell Access

#### 4. **Bash shell in PostgreSQL container:**

```bash
docker exec -it postgres-webapi bash
```

#### 5. **Shell in Web API container:**

```bash
docker exec -it webapidemo-webapi-1 bash
```

*Note: Container name may vary. Use `docker ps` to see exact names.*

#### 6. **List running containers to find exact names:**

```bash
docker ps
```

### One-liner Commands

#### 7. **Run SQL commands directly:**

```bash
docker exec -it postgres-webapi psql -U postgres -d WebApiDemo -c "SELECT * FROM health_check;"
```

#### 8. **Check database list:**

```bash
docker exec -it postgres-webapi psql -U postgres -c "\l"
```

#### 9. **Check tables in WebApiDemo database:**

```bash
docker exec -it postgres-webapi psql -U postgres -d WebApiDemo -c "\dt"
```

### Useful psql Commands

Once you're in the psql prompt:

```sql
\l          -- List databases
\c database -- Connect to database
\dt         -- List tables
\d table    -- Describe table structure
\q          -- Quit psql
```

### Alternative: Docker Compose exec

If you prefer using docker-compose:

```bash
# PostgreSQL access
docker-compose exec postgres psql -U postgres -d WebApiDemo

# Shell access
docker-compose exec postgres bash
docker-compose exec webapi bash
```

**Most common command:** Direct SQL access to your WebApiDemo database:

```bash
docker exec -it postgres-webapi psql -U postgres -d WebApiDemo
```

## Development Workflow

### Local vs Docker Development

**Local Development (dotnet watch):**

- Uses `launchSettings.json` configuration
- Default ports: `http://localhost:5250` and `https://localhost:7298`
- Command: `dotnet watch` (uses first profile "http")
- Specify profile: `dotnet watch --launch-profile https`

**Docker Development:**

- Uses `docker-compose.yml` configuration
- Ports: `http://localhost:5000` and `https://localhost:5001`
- Command: `docker-compose up`
- Ignores `launchSettings.json`

### launchSettings.json Effects

The `launchSettings.json` file affects `dotnet watch` behavior:

**Profile Selection:**

```bash
dotnet watch                           # → Uses "http" profile (localhost:5250)
dotnet watch --launch-profile https   # → Uses "https" profile (localhost:7298)
```

**Profile Configuration:**

- **"http" profile**: `http://localhost:5250`, auto-launches browser
- **"https" profile**: `https://localhost:7298` + `http://localhost:5250`, auto-launches browser
- **Environment**: Both set `ASPNETCORE_ENVIRONMENT=Development`

**Note**: Docker containers ignore `launchSettings.json` and use `docker-compose.yml` port mappings instead.

### Initial Setup

```bash
# Clone/navigate to project directory
cd /path/to/WebAppiDemo

# Build and run
docker-compose up --build
```

### Making Changes

```bash
# After code changes, rebuild and restart
docker-compose up --build

# Or rebuild specific service
docker-compose build webapi
docker-compose up webapi
```

### Database Operations

#### Prerequisites for EF Core Migrations

**Install EF Core Global Tools:**

```bash
# Install globally (one-time setup)
dotnet tool install --global dotnet-ef

# Add to PATH for current session
export PATH="$PATH:/Users/$USER/.dotnet/tools"

# Add to PATH permanently (macOS/Linux)
echo 'export PATH="$PATH:/Users/$USER/.dotnet/tools"' >> ~/.zprofile
```

**Verify Installation:**

```bash
dotnet ef --version
```

#### Entity Framework Migrations

**Create a new migration:**

```bash
# Local development
dotnet ef migrations add Init

# With custom name
dotnet ef migrations add InitialCreate
dotnet ef migrations add AddNewFeature
```

**Apply migrations (up):**

```bash
# Local development - apply all pending migrations
dotnet ef database update

# Apply to specific migration
dotnet ef database update InitialCreate

# Docker container
docker-compose exec webapi dotnet ef database update
```

**Remove last migration (down):**

```bash
# Remove the last migration (must not be applied to database yet)
dotnet ef migrations remove

# Rollback to specific migration
dotnet ef database update PreviousMigrationName

# Rollback all migrations (empty database)
dotnet ef database update 0
```

**Migration Information:**

```bash
# List all migrations
dotnet ef migrations list

# Show migration details
dotnet ef migrations script

# Generate SQL script for specific migration
dotnet ef migrations script PreviousMigration TargetMigration
```

**Database Management:**

```bash
# Drop entire database
dotnet ef database drop

# Drop and recreate database
dotnet ef database drop --force
dotnet ef database update
```

#### Migration Best Practices: Development vs Production

**✅ Development Environment (Automatic Migrations):**

The application is configured to auto-migrate on startup in development:

```csharp
// Program.cs - Already implemented
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate(); // Auto-migrate in dev
}
```

**Benefits for Development:**

- ✅ Seamless Docker experience: `docker-compose up` handles migrations automatically
- ✅ Team consistency: All developers get the same database state
- ✅ No manual steps: No need to run `dotnet ef database update` manually
- ✅ Fresh setups: New developers don't need migration commands

**❌ Production Environment (Manual Control):**

**Why NOT to auto-migrate in production:**

1. **Data Loss Risk** - Schema changes can delete data
2. **Downtime** - Large migrations can lock tables
3. **No Rollback Plan** - Hard to undo if something breaks
4. **Security** - App needs elevated DB permissions
5. **No Review** - Changes applied without human verification

**✅ Recommended Production Strategy:**

```bash
# 1. Generate migration script
dotnet ef migrations script > migration.sql

# 2. Review SQL script manually
cat migration.sql

# 3. Test on staging environment first
psql -U postgres -d StagingDB -f migration.sql

# 4. Apply during planned maintenance window
psql -U postgres -d ProductionDB -f migration.sql

# 5. Monitor and have rollback plan ready
```

**Production Deployment Checklist:**

- [ ] Generate and review migration scripts
- [ ] Test changes on staging environment
- [ ] Plan maintenance window for deployment
- [ ] Backup database before applying migrations
- [ ] Monitor application after migration
- [ ] Have rollback plan and scripts ready

#### PostgreSQL Direct Access

```bash
# Access PostgreSQL container
docker exec -it postgres-webapi psql -U postgres -d WebApiDemo

# Run Entity Framework migrations (if using EF)
docker-compose exec webapi dotnet ef database update
```

### Logs

```bash
# View logs for all services
docker-compose logs

# View logs for specific service
docker-compose logs webapi
docker-compose logs postgres

# Follow logs in real-time
docker-compose logs -f webapi
```

## File Structure

```
WebAppiDemo/
├── Dockerfile              # Web API container definition
├── docker-compose.yml      # Multi-container application definition
├── .dockerignore           # Files to exclude from Docker build
├── init.sql                # PostgreSQL initialization script
└── DOCKER-README.md        # This file
```

### Database Setup Strategy

This project uses a **hybrid approach** for database initialization:

#### 🔧 **Entity Framework Core (EF Core)**
- **Purpose**: Manages application data tables
- **Tables**: `Shirts` table (and other business entities)
- **Method**: Auto-creation via `EnsureCreated()` in Program.cs
- **Data**: Automatic seeding of business data on first startup

#### 🩺 **init.sql Script**
- **Purpose**: Creates utility and monitoring tables
- **Tables**: `health_check`, `database_info`
- **Method**: PostgreSQL initialization script (runs once on first container startup)
- **Data**: System metadata and health monitoring setup

#### 💡 **Why This Separation?**

**✅ Benefits:**
- **No conflicts**: EF Core and init.sql manage different tables
- **Health monitoring**: Database connectivity and status tracking
- **Troubleshooting**: System info helps with debugging
- **Separation of concerns**: App data vs. operational data

**🚫 Avoiding Conflicts:**
- EF Core manages business entities (`Shirts`, future models)
- init.sql manages system utilities (`health_check`, `database_info`)
- No overlap = no conflicts between auto-migration and SQL scripts

#### 📊 **Testing Database Health**

**Check health monitoring:**
```bash
# View health check records
docker exec -it postgres-webapi psql -U postgres -d WebApiDemo -c "SELECT * FROM health_check;"

# View database info
docker exec -it postgres-webapi psql -U postgres -d WebApiDemo -c "SELECT * FROM database_info;"

# Add custom health check
docker exec -it postgres-webapi psql -U postgres -d WebApiDemo -c "SELECT update_health_check('api_test', 'success', 'Manual test');"
```

**Check EF Core tables:**
```bash
# View business data (managed by EF Core)
docker exec -it postgres-webapi psql -U postgres -d WebApiDemo -c "SELECT * FROM \"Shirts\";"

# List all tables
docker exec -it postgres-webapi psql -U postgres -d WebApiDemo -c "\dt"
```

#### 🔄 **Startup Sequence**

1. **PostgreSQL container starts** → Runs `init.sql` (creates utility tables)
2. **Web API container starts** → EF Core runs `EnsureCreated()` (creates app tables)
3. **Data seeding** → Program.cs seeds initial shirt data
4. **Health check** → `health_check` table tracks initialization success

**Important**: If you need a fresh start, use:
```bash
docker-compose down && docker volume rm webappidemo_postgres_data
```
This recreates both init.sql tables and EF Core tables with fresh data.

## Configuration Details

### Environment Variables

- `ASPNETCORE_ENVIRONMENT=Development`
- `ASPNETCORE_URLS=http://+:8080;https://+:8081`
- `ConnectionStrings__DefaultConnection` - Auto-configured for PostgreSQL

### Network

- **Name**: `webapi-network`
- **Type**: Bridge network for internal container communication

### Data Persistence

- **Volume**: `postgres_data`
- **Mount**: `/var/lib/postgresql/data` in PostgreSQL container
- Data persists between container restarts

## Troubleshooting

### Common Issues

1. **Port conflicts**: Ensure ports 5000, 5001, and 5432 are not in use
2. **Permission errors**: Ensure Docker has proper permissions
3. **Build fails**: Check Dockerfile and ensure all dependencies are installed

### Reset Everything

**Quick Clean Reset (Recommended):**

```bash
# Stop containers and remove the database volume for a fresh start
docker-compose down && docker volume rm webappidemo_postgres_data
```

**What this command does:**

- `docker-compose down` - Stops and removes all containers and networks
- `docker volume rm webappidemo_postgres_data` - Deletes the PostgreSQL data volume

**When to use this:**

- 🔄 **Database schema changes**: When EF Core model changes conflict with existing data
- 🐛 **Database corruption**: When database gets into an inconsistent state
- 🧪 **Fresh testing environment**: When you need clean seed data for testing
- 🚀 **New team member setup**: To ensure everyone starts with the same clean database
- 🔧 **Migration conflicts**: When auto-migration fails due to existing incompatible data

**What happens after running this:**

- Next `docker-compose up --build` will create a completely fresh database
- All data will be lost and replaced with fresh seed data
- Tables will be recreated from the current EF Core model
- No migration conflicts or existing data issues

**Complete Nuclear Reset (if needed):**

```bash
# Stop containers and remove everything
docker-compose down -v
docker system prune -f

# Rebuild from scratch
docker-compose up --build
```

### Health Check

Visit `http://localhost:5000/api` to verify the API is running.

## Production Considerations

- Change default PostgreSQL password
- Use environment variables for sensitive configuration
- Add SSL certificates for HTTPS
- Configure proper logging and monitoring
- Use multi-stage builds for smaller image sizes
- Consider using managed database services
