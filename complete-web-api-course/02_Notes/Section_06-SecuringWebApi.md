# Section 06: Securing Web Api

## Lesson 06.51: Verify JWT Token

In this section we manually implement the verification.

Otherwise the easier way is to use the nuget package [Microsoft.AspNetCore.Authentication.JwtBearer](https://www.nuget.org/packages/Microsoft.AspNetCore.Authentication.JwtBearer) and middleware:

![jwt bearer auth middleware](../03_Resources/Diagrams/06_51-jwt_bearer_auth_middleware.png)

First we add Authentication service:

```cs
builder.Services.AddAuthentication("Bearer")
 .AddJwtBearer("Bearer",
 options =>
 {
  options.Authority = "https://localhost:5001"; // Identity Server URL
  options. Audience = "ShirtStoreManagementAPI'; // API resource name
 });
```

Then we add middleware:

```cs
app.UseAuthentication();
app.UseAuthorization());
```
