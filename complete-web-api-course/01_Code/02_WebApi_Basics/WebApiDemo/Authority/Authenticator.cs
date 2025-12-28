using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

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
                new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(securityKey)),
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
