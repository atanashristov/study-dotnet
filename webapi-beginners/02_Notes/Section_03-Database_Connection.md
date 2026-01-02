# Section 03: Database Connection

## Lesson 03.20: Install NuGet Packages

**Add Nuget Pasckages:**

Execute the following commands:

```sh
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
```

What You Got:

- Core EF: Base Entity Framework functionality
- Migrations Support: Through Microsoft.EntityFrameworkCore.Tools
- PostgreSQL Provider: Full PostgreSQL database support via Npgsql
- CLI Tools: Ability to run dotnet ef commands for database operations

Note:

You do not need to install the standalone Npgsql package separately, as it's automatically included as a dependency of Npgsql.EntityFrameworkCore.PostgreSQL. Also, Microsoft.EntityFrameworkCore.Design was automatically included as a dependency of the Tools package.

Your project is now ready for Entity Framework Core with PostgreSQL support! You can proceed with creating your DbContext, models, and migrations.

**Add Docker Compose support:**

Docker and Docker Compose support has been added to the project. The setup includes:

- **PostgreSQL Service**: Latest Postgres version exposed on port 5432
- **Web API Service**: .NET 10 application with health check dependency on Postgres
- **Network**: Bridge network for service communication
- **Volume**: Persistent storage for database data

### Running the Docker Cluster

Navigate to the `01_Code` directory and start the services:

```bash
cd 01_Code
docker-compose up -d
```

The `-d` flag runs services in detached mode (background).

### Stopping the Docker Cluster

To stop services while preserving data:

```bash
docker-compose stop
```

To stop and remove containers (data is preserved in volumes):

```bash
docker-compose down
```

### Resetting the Docker Cluster

To completely reset (WARNING: This removes all database data):

```bash
docker-compose down -v
docker-compose up -d
```

### Running psql on the Docker Image

To connect to PostgreSQL from the host machine:

```bash
docker exec -it royalvilla_postgres psql -U postgres -d royalvilla
```

Or using docker-compose:

```bash
docker-compose exec postgres psql -U postgres -d royalvilla
```

Common psql commands:

- List databases: `\l`
- Connect to database: `\c royalvilla`
- List tables: `\dt`
- Describe table: `\d tablename`
- Exit psql: `\q`

For detailed documentation, see: `05_Copilot_Prompts/Section_03_20-Database_Connection-Add_Docker_Compose_Result.md`

**Install the NuGet packages:**

Add the necessary NuGet packages to support Entity Framework ad Postgres:

```sh
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
```

What You Got:

- Core EF: Base Entity Framework functionality
- Migrations Support: Through Microsoft.EntityFrameworkCore.Tools
- PostgreSQL Provider: Full PostgreSQL database support via Npgsql
- CLI Tools: Ability to run dotnet ef commands for database operations

Note:

You do not need to install the standalone Npgsql package separately, as it's automatically included as a dependency of Npgsql.EntityFrameworkCore.PostgreSQL. Also, Microsoft.EntityFrameworkCore.Design was automatically included as a dependency of the Tools package.

Your project is now ready for Entity Framework Core with PostgreSQL support! You can proceed with creating your DbContext, models, and migrations.

**Add connection string:**

We add _connection string_ "DefaultConnection" to the _appsettings.json_.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=RoyalVilla;Username=postgres;Password=postgres123"
  },
  "Logging": {
...
```

## Lesson 03.21: Db Context

Add file _Data/ApplicationDbContext.cs_.

```cs
using Microsoft.EntityFrameworkCore;

namespace RoyalVillaApi.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        // Define your DbSets here
        // public DbSet<YourEntity> YourEntities { get; set; }
    }
}
```

Add the Db Context to `builder.Services` in _Program.cs_:

```cs
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);
```

Then we create an empty "Initial" DB migration:

```sh
dotnet ef migrations add Initial
```

And run it:

```sh
dotnet ef database update
```

Since this is an empty migration, running it will create the migrations history table "__EFMigrationsHistory" and add one record to it:

```sql
royalvilla=# \d
                 List of relations
 Schema |         Name          | Type  |  Owner
--------+-----------------------+-------+----------
 public | __EFMigrationsHistory | table | postgres
(1 row)

royalvilla=# \d+ "__EFMigrationsHistory"
                                              Table "public.__EFMigrationsHistory"
     Column     |          Type          | Collation | Nullable | Default | Storage  | Compression | Stats target | Description
----------------+------------------------+-----------+----------+---------+----------+-------------+--------------+-------------
 MigrationId    | character varying(150) |           | not null |         | extended |             |              |
 ProductVersion | character varying(32)  |           | not null |         | extended |             |              |
Indexes:
    "PK___EFMigrationsHistory" PRIMARY KEY, btree ("MigrationId")
Not-null constraints:
    "__EFMigrationsHistory_MigrationId_not_null" NOT NULL "MigrationId"
    "__EFMigrationsHistory_ProductVersion_not_null" NOT NULL "ProductVersion"
Access method: heap

royalvilla=# select * from "__EFMigrationsHistory";
      MigrationId       | ProductVersion
------------------------+----------------
 20260102192101_Initial | 10.0.1
(1 row)

```

## Lesson 03.22: Create Villas Table

We add table model class `Villa` as file _Models/Villa.cs_.

Use data annotations as needed to control column definitions.

Then we change the `ApplicationDbContext` and define  `DbSet` and what is the model:

```cs
using Microsoft.EntityFrameworkCore;
using RoyalVillaApi.Models;

namespace RoyalVillaApi.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<Villa> Villas { get; set; } // Add villa table
    }
}
```

Use the following **conventions**:

| Layer            | Component        | Convention    | Example          |
| ---------------- | ---------------- | ------------- | ---------------- |
| Api Route        | Route name       | Plural        | api/Villas       |
| C# (Controller)  | Controller Class | Plural        | VillasController |
| C# (Object)      | Model Class      | Singular      | Villa            |
| C# (Data Access) | DbSet Property   | Plural        | Villas           |
| Database         | Table Name       | Plural        | Villas           |
| Database         | Primary Key      | Singular + Id | Id or VillaId    |

Then create migration "AddVillasToDb":

```sh
dotnet ef migrations add AddVillasToDb
```

And apply it to the database:

```sh
dotnet ef database update
```
