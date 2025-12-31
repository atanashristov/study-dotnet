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
            if (Authenticator.Authenticate(credentials.ClientId, credentials.ClientSecret))
            {
                var expiresAt = DateTime.UtcNow.AddMinutes(30);
                return Ok(new
                {
                    access_token = Authenticator.CreateToken(
                        credentials.ClientId,
                        expiresAt,
                        configuration["SecurityKey"] ?? string.Empty),
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

    }
}
