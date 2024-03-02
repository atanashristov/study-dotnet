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
dotnet add Infrastructure package Microsoft.AspNetCore.Authorization
dotnet add Infrastructure package Microsoft.AspNetCore.Identity

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

### Lesson 3.13-14: Database migration

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

### Lesson 3.15-21: Identity User Extended, Entity type configs, Schema names

First we create _Infrastructure\DbConfig_ folder and add EF configurations.

We create configuration for _schema names_:

- Infrastructure\DbConfig\SchemaNames.cs

We create configuration for "Identity" tables:

- DbConfig\IdentityEntitiesConfig.cs

We create configuration for "Identity" tables:

- Infrastructure\DbConfig\IdentityEntitiesConfig.cs

We create configuration for "Employee" table:

- Infrastructure\DbConfig\EmployeeEntityConfig.cs

These configurations are scanned by EF from `ApplicationDbContext.OnModelCreating()` in _Infrastructure\Context\ApplicationDbContext.cs_:

```csharp
        protected override void OnModelCreating(ModelBuilder builder)
        {
            foreach (var property in builder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetColumnType("decimal(18,2)");
            }

            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(GetType().Assembly); // <- here
        }
```

Then we delete the _Infrastructure\Migration_ folder.

Login into Postgres as postgres user. Then run the following to rename and create new the database:

```sql
alter database abchrdb_dev rename to abchrdb_asp_tbls;
create database abchrdb_dev;
alter database abchrdb_dev owner to abchruser;
```

Then run:

```sh
dotnet ef migrations add -c Infrastructure.Context.ApplicationDbContext -s WebApi -p Infrastructure InitialDb
```

You should see migration generated in:

- Infrastructure\Migrations\...InitialDb.cs

If you look at the generated migration, it creates tables with the new names and schema names.

Then we want to **apply migrations** to the DB.

```sh
dotnet ef database update -c Infrastructure.Context.ApplicationDbContext -s WebApi -p Infrastructure InitialDb
```

You can login into Postgres and check the database.

List the schemas: `\dn` or `\dnS+`.

List the tables from all schemas: `\dt *.*`.

List the tables from schema "Security": `\dt "Security".*`.

List the tables from schema "HR": `\dt "HR".*`.

To view the table definition, escape both schema and table names, because of the capitalization of names:

```sh
abchrdb_dev=> \dt "HR"."Employees"
           List of relations
 Schema |   Name    | Type  |   Owner
--------+-----------+-------+-----------
 HR     | Employees | table | abchruser
(1 row)
```

### Lesson 3.22-23: Understanding migrations, schema, creation and db update

Let's say we want to modify data type sizes.

We could directly edit the migration files.

Or we can use code first _data annotations_ (System.ComponentModel.DataAnnotations).

If we change the `Employee` class with annotations:

```csharp
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(128)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(128)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(256)]
        public string Email { get; set; }

        public decimal Salary { get; set; }
    }
}
```

Then we can run `remove-migration` from the VS Studio, or run:

```sh
dotnet ef migrations remove --force -c Infrastructure.Context.ApplicationDbContext -s WebApi -p Infrastructure
```

Note: `--force` reverts the migration from the DB as well.

Then recreate the migration:

```sh
dotnet ef migrations add -c Infrastructure.Context.ApplicationDbContext -s WebApi -p Infrastructure InitialDb
```

, and apply the migration to the DB:

```sh
dotnet ef database update -c Infrastructure.Context.ApplicationDbContext -s WebApi -p Infrastructure InitialDb
```

Here is now how the table got created in the DB:

```sh
abchrdb_dev=> \d "HR"."Employees"
                                     Table "HR.Employees"
  Column   |          Type          | Collation | Nullable |             Default
-----------+------------------------+-----------+----------+----------------------------------
 Id        | integer                |           | not null | generated by default as identity
 FirstName | character varying(128) |           | not null |
 LastName  | character varying(128) |           | not null |
 Email     | character varying(256) |           | not null |
 Salary    | numeric(18,2)          |           | not null |
Indexes:
    "PK_Employees" PRIMARY KEY, btree ("Id")
    "IX_Employees_FirstName" btree ("FirstName")
    "IX_Employees_LastName" btree ("LastName")
```

, or:

```sh
abchrdb_dev=> \d "Security"."Users"
                               Table "Security.Users"
         Column         |           Type           | Collation | Nullable | Default
------------------------+--------------------------+-----------+----------+---------
 Id                     | text                     |           | not null |
 FirstName              | character varying(128)   |           |          |
 LastName               | character varying(128)   |           |          | ...
```

### Change all PKs to Guid

There is a rolled back commit with converted the PKs to Guid.

By the way, this is how in Postgres one can generate UUID:

```sql
abchrdb_dev=> select * from gen_random_uuid();
           gen_random_uuid
--------------------------------------
 44f7c6f2-df18-4893-aa83-2e736f246028
(1 row)
```

## Section 4: Authentication Constants

Every user is assign a role.
Every role is assigned permissions.
Every permission is on an application feature:

- We perform actions on these features.
- To this specific feature, you are permitted to apply these actions.

We add the **features** in `Common\Authorization\AppFeature.cs` such as "Employees", "Users", "Roles", etc.

Then the **actions** in `Common\Authorization\AppAction.cs` such as "Create", "Read", "Update", "Delete".

Then the **claims** in `Common\Authorization\AppClaim.cs` such as "permission" and "exp" (for the JSON web token expiration time). We can add user full name, phone number etc, as needed...

We add **role group** in `Common\Authorization\AppRoleGroup.cs` such as "SystemAccess", "ManagementHierarchy".

We add **roles** in `Common\Authorization\AppRoles.cs` such as "Admin", "Basic". These are the _default roles_ that are _created with the application_. We _seed these_ to the DB.

We add **default credentials** in `Common\Authorization\AppCredentials.cs`. We could also use configuration instead of hardcoding.

We add **permissions** in `Common\Authorization\AppPermission.cs` such as "Create Users", "Update Users", etc. The permission consists of feature and action.

Basic permission is where:

- When we create an user it gets only the basic permissions
- Any application role can read the employees, that's why this is a basic permission

## Section 5: Seeding the Db

We add users, roles and grand permissions to these roles.

We add in Db seeder class in `Infrastructure\Context\ApplicationDbSeeeder.cs`.

It associates the ASP.Net identity models with the application models:

```csharp
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
```

We add `\Infrastructure\ServiceCollectionExtensions.cs` for initialization of service providers and running the DB seeder.

Then call the DB seeder from `Program.cs`:

```csharp
var app = builder.Build();
app.SeedDatabase();
```

## Section 6: Permissions Resources

We setup requirement.
Then we add authorization handler.
And there we handle/verify if the user can access specific resource.
For that we will need a permission policy provider.

As the permission requirements grow, at run time we keep adding them to the list that we created here `Common\Authorization\AppPermission.cs`.

We add the following 3 classes:

- \Infrastructure\Permissions\PermissionRequirement.cs
- \Infrastructure\Permissions\PermissionAuthorizationHandler.cs
- \Infrastructure\Permissions\PermissionPolicyProvider.cs

Register these in DI (`\Infrastructure\ServiceCollectionExtensions.cs`).
