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

### Lesson 3.9-10: Db Context, ORM, Identity Db Context

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
