using Microsoft.AspNetCore.Mvc.Filters;
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

            // 4. Validate the token
            if (!await Authenticator.ValidateTokenAsync(tokenString, secretKey))
            {
                context.Result = new Microsoft.AspNetCore.Mvc.UnauthorizedResult();
                return;
            }
        }
    }
}
