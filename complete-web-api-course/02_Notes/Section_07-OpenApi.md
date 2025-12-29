# Section 07: OpenAPI Documentation and Versioning

## Lesson 07.57: Open API Support

### Adding OpenAPI Support to Your Web API

#### Step 1: Install Required NuGet Packages

Add the following packages to your project:

```xml
<PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="10.0.1" />
<PackageReference Include="Scalar.AspNetCore" Version="2.11.10" />
```

You can install them via Package Manager Console:

```bash
dotnet add package Microsoft.AspNetCore.OpenApi
dotnet add package Scalar.AspNetCore
```

#### Step 2: Add Using Statements

Add the following using statements to your `Program.cs`:

```csharp
using Microsoft.AspNetCore.OpenApi;
using Scalar.AspNetCore;
```

#### Step 3: Configure OpenAPI Services

Add OpenAPI services to the service collection with custom configuration:

```csharp
// Add OpenAPI support with Bearer authentication
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info = new()
        {
            Title = "WebApiDemo API",
            Version = "v1",
            Description = "A sample Web API with JWT authentication support"
        };

        return Task.CompletedTask;
    });
});
```

#### Step 4: Configure OpenAPI Endpoints and Scalar UI

Configure the OpenAPI endpoints and Scalar UI for development environment:

```csharp
// OpenAPI documentation with Scalar UI in development environment
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "WebApiDemo API";
        options.Theme = ScalarTheme.BluePlanet;
    });
}
```

#### Step 5: Accessing the API Documentation

Once configured, you can access your API documentation through:

1. **OpenAPI JSON Schema**: `https://localhost:5001/openapi/v1.json`
   - Raw OpenAPI specification in JSON format
   - Can be imported into tools like Postman, Insomnia, or other API clients

2. **Scalar UI Interface**: `https://localhost:5001/scalar/v1`
   - Beautiful, interactive API documentation
   - Built-in API testing capabilities
   - Multiple theme options (BluePlanet, Kepler, Mars, Moon, Saturn, etc.)
   - Code generation for various languages

### Using the Scalar UI Interface

#### Features

1. **Interactive Documentation**:
   - Browse all available endpoints
   - View request/response schemas
   - See example data

2. **Built-in API Testing**:
   - Execute API calls directly from the documentation
   - Input parameters and request bodies
   - View real-time responses
   - Support for authentication headers

3. **Code Generation**:
   - Generate client code in multiple languages
   - Copy curl commands
   - Export for various HTTP clients

4. **Schema Visualization**:
   - Clear model definitions
   - Data type specifications
   - Validation rules

#### Best Practices

1. **Environment-Specific**: Only enable in development to avoid exposing API structure in production
2. **Custom Themes**: Choose themes that match your brand or team preferences
3. **Comprehensive Documentation**: Ensure your controllers and models have proper XML documentation comments
4. **Authentication**: Configure JWT bearer token support for protected endpoints

### Security Considerations

- Always restrict OpenAPI documentation to development environments
- For production, consider hosting documentation separately
- Use proper authentication schemes in your OpenAPI configuration
- Avoid exposing sensitive endpoint details in production builds
