using Microsoft.AspNetCore.Mvc.Filters;
using WebApiDemo.Attributes;
using WebApiDemo.Authority;
using WebApiDemo.Utilities;

namespace WebApiDemo.Filters.AuthFilters
{
    public class JwtTokenAuthFilterAttribute : Attribute, IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // 1. Get the JWT token from the Authorization header
            // var authHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            // if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
            // {
            //     // No token provided
            //     throw new UnauthorizedAccessException("No JWT token provided.");
            // }

            if (!context.HttpContext.Request.Headers.TryGetValue("Authorization", out var authHeaderValues))
            {
                context.Result = new Microsoft.AspNetCore.Mvc.UnauthorizedResult();
                return;
            }

            var tokenString = authHeaderValues.ToString();

            // 2. Get rid of "Bearer" prefix

            if (tokenString.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                tokenString = tokenString.Substring("Bearer ".Length).Trim();
            }
            else
            {
                context.Result = new Microsoft.AspNetCore.Mvc.UnauthorizedResult();
                return;
            }

            // 3. Get configuration and SecretKey (SecurityKey)
            var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var secretKey = configuration["SecurityKey"] ?? string.Empty;

            // 4. Validate the token and exact claims
            // if (!await Authenticator.ValidateTokenAsync(tokenString, secretKey))
            // {
            //     context.Result = new Microsoft.AspNetCore.Mvc.UnauthorizedResult();
            //     return;
            // }
            var claims = await Authenticator.ValidateTokenAsync(tokenString, secretKey);
            if (claims == null)
            {
                context.Result = new Microsoft.AspNetCore.Mvc.UnauthorizedResult();
                return;
            }

            // Get the claims required
            var requiredClaims = context.ActionDescriptor.EndpointMetadata
                .OfType<RequiredClaimAttribute>()
                .ToList();

            if (!requiredClaims.All(rc => claims.Any(c =>
                c.Type.Equals(rc.ClaimType, StringComparison.OrdinalIgnoreCase)
                    && c.Value.Equals(rc.ClaimValue, StringComparison.OrdinalIgnoreCase))))
            {
                // Not all required claims are present
                context.Result = new Microsoft.AspNetCore.Mvc.StatusCodeResult(403);
                return;
            }

        }
    }
}
