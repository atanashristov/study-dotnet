# Advanced .NET Web API Security: Permission based auth & JWT

Contains code and notes from studying [Advanced .NET Web API Security: Permission based auth & JWT](https://www.udemy.com/course/advanced-net-web-api-security-permission-based-auth-jwt/).

Code is in directory [ABCHR_Project](./ABCHR_Project/).

Original code is in directory [ABCHR_Original](./ABCHR_Original/).

## Section 3: Solution Design

### Lesson 3.5: Solution Architecture

Create projects:

```sh
dotnet new classlib -n Domain -o Domain                   # Contains domain entities
dotnet new classlib -n Application -o Application         # Business requirements and rules
dotnet new classlib -n Infrastructure -o Infrastructure   # External dependencies, ORM, Db context, Db connection, Service implementations
dotnet new classlib -n Common -o Common                   # Share between WebApi and later a Blazor app, use the security features in both
dotnet new webapi -n WebApi -o WebApi                     # API

```

Add project references:

```sh
dotnet add Application reference Domain
dotnet add Application reference Common
dotnet add Infrastructure reference Application
dotnet add WebApi reference Infrastructure
```

Add nuget packages:

```sh
dotnet add Infrastructure package Microsoft.EntityFrameworkCore
dotnet add Infrastructure package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add Infrastructure package Microsoft.EntityFrameworkCore.Design
dotnet add Infrastructure package Microsoft.EntityFrameworkCore.Tools
dotnet add Infrastructure package Microsoft.AspNetCore.Identity.EntityFrameworkCore
dotnet add Infrastructure package Microsoft.IdentityModel.Tokens
dotnet add Infrastructure package Microsoft.IdentityModel.JsonWebTokens

dotnet add WebApi package Microsoft.EntityFrameworkCore.Design
```

Notes:

- One could use `Microsoft.EntityFrameworkCore.InMemory` instead of `Npgsql` during development
- For MS SQL Server use `Microsoft.EntityFrameworkCore.SqlServer`

Create solution and add projects:

```sh
dotnet new sln -n ABCHR
dotnet sln add Domain
dotnet sln add Application
dotnet sln add Infrastructure
dotnet sln add Common
dotnet sln add WebApi
```

Make sure it builds: `dotnet build`

Open the solution in Visual Studio, then for each project:

- disable nullables
- enable implicit

```html
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>disable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
```

Right click on "WebApi" in solution explorer and make it a startup project.

### Lesson 3.9-12: Db Context, ORM, Identity Db Context, Db Context Extensions

Add domain entity:

- Domain\Employee.cs

Add 3 classes that override Identity classes and extend these with more properties.
These will be saved as additional columns in the database tables.

- Infrastructure\Models\ApplicationRole.cs - Inherit from `IdentityRole`
- Infrastructure\Models\ApplicationRoleClaim.cs - Inherit from `IdentityRoleClaim`
- Infrastructure\Models\ApplicationUser.cs - Inherit from `IdentityUser`

Add an application Db Context class:

- Infrastructure\Context\ApplicationDbContext.cs

It inherits from `IdentityDbContext` and the key types are defined as _string_ and will hold _Guid_ values.

It adds the tables as DbSets, specifically the `Employees` DbSet.

It adds default precision for decimal type as `decimal(18,2)`.

Add Db Context extensions for the DI:

- Infrastructure\ServiceCollectionExtensions.cs

We register the Db connection:

- WebApi\Program.cs

```csharp
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddDatabase(builder.Configuration); // <-- here
```

### Lesson 3.13: Database migration

Next notes are on Postgres. We want to **create user and database**.

Login into Postgres as postgres user. Then run the following to create user, database and to set ownership:

```sql
create user abchruser with encrypted password 'abchrpass';
create database abchrdb_dev;
alter database abchrdb_dev owner to abchruser;
-- not needed:
-- grant all privileges on database abchrdb_dev to abchruser;
```

I am running Postgres in _WSL_, this is now you _find the IP address_ of the Ubuntu from PowerShell:

```sh
wsl hostname -I
192.168.185.137
```

Setup DB connection string in:

- WebApi\appsettings.json
- WebApi\appsettings.Development.json

This thereof is the connection string for development:

```json
  "ConnectionStrings": {
    "ABCHRDbConnection": "Host=192.168.185.137;Username=abchruser;Password=abchrpass;Database=abchrdb_dev"
  },

```

We want to **generate migration**.

One way is to use the VS Studio.
In VS Studio click on "Tools -> Nuget Package Manager -> Package Manager Console".
Select as "Default Project" the one where the Application DB context was defined, which is _Infrastructure_.
Then run `Add-Migration InitialDb`.

For command line, first make sure you updated the tooling: `dotnet tool update --global dotnet-ef`.

Then run:

```sh
dotnet ef migrations add -c Infrastructure.Context.ApplicationDbContext -s WebApi -p Infrastructure InitialDb
```

You should see migration generated in:

- Infrastructure\Migrations\...InitialDb.cs

If you look at the generated migration, it creates tables as they come from _AspNet Identity_.

Then we want to **apply migrations** to the DB.

We can run in VS Studio `Update-Database`.

Or we can run:

```sh
dotnet ef database update -c Infrastructure.Context.ApplicationDbContext -s WebApi -p Infrastructure InitialDb
```

You can login into Postgres and check the database.
Note: Postgres `psql` needs escaping for capital letters, so use `\d "Employees"`

```sh
psql abchrdb_dev abchruser -h 127.0.0.1

abchrdb_dev=> \d
                    List of relations
 Schema |          Name           |   Type   |   Owner
--------+-------------------------+----------+-----------
 public | AspNetRoleClaims        | table    | abchruser
 public | AspNetRoleClaims_Id_seq | sequence | abchruser
 public | AspNetRoles             | table    | abchruser
 public | AspNetUserClaims        | table    | abchruser
 public | AspNetUserClaims_Id_seq | sequence | abchruser
 public | AspNetUserLogins        | table    | abchruser
 public | AspNetUserRoles         | table    | abchruser
 public | AspNetUserTokens        | table    | abchruser
 public | AspNetUsers             | table    | abchruser
 public | Employees               | table    | abchruser
 public | Employees_Id_seq        | sequence | abchruser
 public | __EFMigrationsHistory   | table    | abchruser
(12 rows)

abchrdb_dev=> \d "Employees"
                              Table "public.Employees"
  Column   |     Type      | Collation | Nullable |             Default
-----------+---------------+-----------+----------+----------------------------------
 Id        | integer       |           | not null | generated by default as identity
 FirstName | text          |           |          |
 LastName  | text          |           |          |
 Email     | text          |           |          |
 Salary    | numeric(18,2) |           | not null |
Indexes:
    "PK_Employees" PRIMARY KEY, btree ("Id")

```

We will drop this database and re-create in the next lesson, because we want to name the "AspNet*" tables differently.

We also want to have them in a separate schema, not in the default (dbo or public).

We also want to extend some of these tables with additional columns.
