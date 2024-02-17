# Advanced .NET Web API Security: Permission based auth & JWT

Contains code and notes from studying [Advanced .NET Web API Security: Permission based auth & JWT](https://www.udemy.com/course/advanced-net-web-api-security-permission-based-auth-jwt/).

Code is in directory [ABCHR](./ABCHR_Original/).

Original code is in directory [ABCHR_Original](./ABCHR_Original/).

## Section 3: Solution Design

### Lesson 3.5: Solution Architecture

We create _src_ folder.

Then inside _projects_:

- Domain - contains domain entities
- Application - business requirements and rules
- Infrastructure - external dependencies, ORM, Db context, Db connection, Service implementations
- WebApi - API. _Note_: we are not going to use the provided authentications, as we implement ours
- Common - Share between WebApi and later a Blazor app, use the security features in both

Also we _disable nullables_ and _enable implicit usings_ in all projects:

```html
  <PropertyGroup>
    <TargetFramework>net7.0</TargetFramework>
    <Nullable>disable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
```

### Lesson 3.6: Project References

Project refs:

- Application -> Domain, Common
- Infrastructure -> Application
- WebApi -> Infrastructure

### Lesson 3.7: Entity

We only have one entity `Employee` in _Domain_ project.

### Lesson 3.8: Nuget Packages

In _Infrastructure_ we add EF packages:

- Microsoft.EntityFrameworkCore
- (x) Microsoft.EntityFrameworkCore.SqlServer
- (in my case) Npgsql.EntityFrameworkCore.PostgreSQL
- (could use) Microsoft.EntityFrameworkCore.InMemory
- Microsoft.EntityFrameworkCore.Design
- Microsoft.EntityFrameworkCore.Tools
- Microsoft.AspNetCore.Identity.EntityFrameworkCore
- Microsoft.IdentityModel.Tokens
- Microsoft.IdentityModel.JsonWebTokens

### Lesson 3.9: Db Context - ORM

We add class _Infrastructure\Context\ApplicationDbContext.cs_ which is the identity DB context.

It defines what tables will be created with migration.
Some of provided classes we can inherit and extend, so that we customize and add table columns.

We add class _Infrastructure\ServiceCollectionExtensions.cs_ that is used to wire up DI.
Then we add the Db service in _WebApi\Program.cs_:

```csharp
builder.Services.AddControllers();
builder.Services.AddDatabase(builder.Configuration); // add Db service

```

### Lesson 3.13: Db Context - ORM
