using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using WebApiDemo.Authority;

namespace WebApiDemo.Controllers
{
    [ApiController]
    public class AuthorityController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public AuthorityController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpPost("auth")]
        public IActionResult Authenticate(AppCredentials credentials)
        {
            if (AppRepository.Authenticate(credentials.ClientId, credentials.ClientSecret))
            {
                var expiresAt = DateTime.UtcNow.AddMinutes(10);
                return Ok(new
                {
                    access_token = CreateToken(credentials.ClientId, expiresAt),
                    expires_at = expiresAt,
                });
            }
            else
            {
                // Using Problem() is the cleanest for this authentication scenario
                // since it creates proper Problem Details(RFC 7807)
                // without the complexity of validation errors,
                // which are more appropriate for input validation
                // rather than authentication failures.
                return Problem(
                    detail: "Invalid client credentials.",
                    statusCode: (int)HttpStatusCode.Unauthorized,
                    title: "Authentication Failed");
            }
        }

        private string CreateToken(string clientId, DateTime expiresAt)
        {
            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(configuration["SecurityKey"]
                    ?? string.Empty)),
                SecurityAlgorithms.HmacSha256Signature);

            var app = AppRepository.GetApplicationByClientId(clientId);
            if (app == null)
            {
                throw new InvalidOperationException("Application not found for the given clientId.");
            }

            var claimsDictionary = new Dictionary<string, object>
            {
                // { "ClientId", app.ClientId ?? string.Empty },
                { "AppName", app.ApplicationName ?? string.Empty },
                { "Read", app.Scopes != null && app.Scopes.Contains("read") },
                { "Write", app.Scopes != null && app.Scopes.Contains("write") },
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                SigningCredentials = signingCredentials,
                Claims = claimsDictionary,
                Expires = expiresAt,
                NotBefore = DateTime.UtcNow,
            };

            var tokenHandler = new JsonWebTokenHandler();
            return tokenHandler.CreateToken(tokenDescriptor);
        }
    }
}
