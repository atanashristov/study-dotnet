using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
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
            };

            var scopes = app.Scopes != null ? string.Join(',', app.Scopes) : string.Empty;
            if (!string.IsNullOrWhiteSpace(scopes))
            {
                foreach (var scope in scopes.Split(','))
                {
                    claimsDictionary.Add(scope.Trim().ToLowerInvariant(), "true");
                }
            }

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

        public static async Task<IEnumerable<Claim>?> ValidateTokenAsync(string tokenString, string secretKey)
        {
            try
            {
                if (string.IsNullOrEmpty(tokenString) || string.IsNullOrEmpty(secretKey))
                {
                    return null;
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


                var validationResult = await tokenHandler.ValidateTokenAsync(tokenString, tokenValidationParameters);
                if (validationResult.SecurityToken == null || !validationResult.IsValid)
                {
                    return null;
                }

                var tokenObject = tokenHandler.ReadJsonWebToken(tokenString);

                return tokenObject.Claims ?? Enumerable.Empty<Claim>();
            }
            catch (SecurityTokenException)
            {
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
