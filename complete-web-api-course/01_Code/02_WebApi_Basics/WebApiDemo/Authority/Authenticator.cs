using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace WebApiDemo.Authority
{
    public class Authenticator
    {
        public static bool Authenticate(string clientId, string clientSecret)
        {
            var app = AppRepository.GetApplicationByClientId(clientId);
            if (app == null)
            {
                return false;
            }

            return app.ClientId == clientId
                && app.ClientSecret == clientSecret;

        }

        public static string CreateToken(string clientId, DateTime expiresAt, string securityKey)
        {
            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey)),
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
                // (*) Audience and Issuer can be set here if needed. In this example, we skip them.
            };

            var tokenHandler = new JsonWebTokenHandler();
            return tokenHandler.CreateToken(tokenDescriptor);
        }

        public static async Task<bool> ValidateTokenAsync(string token, string secretKey)
        {
            try
            {
                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(secretKey))
                {
                    return false;
                }

                var keyBytes = Encoding.UTF8.GetBytes(secretKey);
                var tokenHandler = new JsonWebTokenHandler();

                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false, // (*) these are not set in our tokens
                    ValidateAudience = false, // (*) these are not set in our tokens
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                    ClockSkew = TimeSpan.Zero, // No clock skew. For distributed systems, consider a small clock skew.
                };


                var validationResult = await tokenHandler.ValidateTokenAsync(token, tokenValidationParameters);

                return validationResult.IsValid;
            }
            catch (SecurityTokenException)
            {
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
