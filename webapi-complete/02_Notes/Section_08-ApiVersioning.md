# Section 08: API Versioning

## API Versioning Implementation

### Step 1: Install API Versioning Packages

Add the following NuGet packages for .NET 10:

```xml
<PackageReference Include="Asp.Versioning.Mvc" Version="8.1.0" />
<PackageReference Include="Asp.Versioning.Mvc.ApiExplorer" Version="8.1.0" />
```

Install via Package Manager Console:

```bash
dotnet add package Asp.Versioning.Mvc
dotnet add package Asp.Versioning.Mvc.ApiExplorer
```

### Step 2: Configure API Versioning Services

Add using statements and configure versioning in `Program.cs`:

```csharp
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;

// Add API Versioning
builder.Services.AddApiVersioning(options =>
{
    // Default version
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;

    // Version reading strategies
    options.ApiVersionReader = ApiVersionReader.Combine(
        new HeaderApiVersionReader("x-api-version"),
        new QueryStringApiVersionReader("api-version"),
        new UrlSegmentApiVersionReader()
    );
}).AddMvc().AddApiExplorer(options =>
{
    // Group name format for OpenAPI
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});
```

### Step 3: Configure OpenAPI with Simplified Setup

Add a simple OpenAPI configuration that works with all API versions:

```csharp
// Add OpenAPI support
builder.Services.AddOpenApi();
```

### Step 4: Configure OpenAPI Endpoint and Scalar UI

Set up a single unified OpenAPI document that includes all versions:

```csharp
// OpenAPI documentation with Scalar UI in development environment
if (app.Environment.IsDevelopment())
{
    // Map a single OpenAPI document
    app.MapOpenApi();

    // Configure Scalar UI
    app.MapScalarApiReference(options =>
    {
        options.Title = "WebApiDemo API";
        options.Theme = ScalarTheme.BluePlanet;
        options.ShowSidebar = true;
    });
}
```

**Note**: This simplified approach provides a unified API documentation showing all endpoints from all versions in one place, making it easier for consumers to discover and test different API versions.

### Step 5: Version Your Controllers

Update existing controllers and create new versions using a single route pattern:

#### V1 Controller (Existing - Default Version)

```csharp
[ApiController]
[Route("api/[controller]")]
[ApiVersion("1.0")]
public class ShirtsController : ControllerBase
{
    // Existing implementation...
}
```

#### V2 Controller (New Version)

```csharp
[ApiController]
[Route("api/Shirts")]
[ApiVersion("2.0")]
public class ShirtsV2Controller : ControllerBase
{
    // Enhanced V2 implementation with additional features...
}
```

**Note**: Both controllers use the same base route path (`api/Shirts`). The API version is specified through the `[ApiVersion]` attribute. Version selection is handled via headers or query parameters, not URL segments, resulting in cleaner API documentation.

### Version Request Methods

Clients can specify the API version using these methods:

1. **Header**: `x-api-version: 2.0`

   ```bash
   curl -H "x-api-version: 2.0" http://localhost:5250/api/shirts
   ```

2. **Query Parameter**: `?api-version=2.0`

   ```bash
   curl http://localhost:5250/api/shirts?api-version=2.0
   ```

**Note**: URL segment versioning (`/api/v2/shirts`) is not used in this implementation to maintain cleaner API documentation. Version selection is handled exclusively through headers and query parameters.

### Accessing API Documentation

- **OpenAPI Specification**: `http://localhost:5250/openapi/v1.json`
- **Scalar UI**: `http://localhost:5250/scalar`

The Scalar UI provides a unified view of all API endpoints across all versions. Use the header or query parameter methods shown above to test different versions of the same endpoint.

### API Version Features Comparison

#### V1 Features (Default)

- Basic CRUD operations
- Standard HTTP responses
- Simple data structures
- Compatible with existing clients

#### V2 Features (Enhanced)

- All V1 functionality
- Response metadata (timestamps, statistics)
- Enhanced error details
- New statistical endpoints
- Improved response structures
- Audit trail support

### Best Practices for API Versioning

1. **Version Strategy**: Use semantic versioning (Major.Minor)
2. **Backward Compatibility**: Maintain V1 for existing clients
3. **Default Version**: Always specify a default version
4. **Documentation**: Document changes between versions in a unified view
5. **Deprecation**: Plan deprecation strategy for old versions
6. **Testing**: Test all version endpoints thoroughly using headers/query parameters
7. **Client Communication**: Notify clients of new versions and deprecation timelines
8. **Clean Routes**: Use single route paths with version selection via headers/query parameters for cleaner API documentation
9. **OpenAPI Integration**: Leverage unified OpenAPI documentation to show all versions together

### Version Migration Guidelines

1. **Non-Breaking Changes**: Can be added to existing version
2. **Breaking Changes**: Require new version
3. **Data Model Changes**: Usually require new version
4. **New Endpoints**: Can be added to existing version
5. **Response Format Changes**: Require new version
