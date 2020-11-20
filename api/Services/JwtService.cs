// David Wahid
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using api.Models.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace api.Services
{
    public interface IJwtService
    {
        public string GenerateToken(string userName, string role);
        public bool ValidateToken(string token);
        public string GetClaim(string token);
    }

    public class JwtService : IJwtService
    {
        private readonly IOptionsSnapshot<JwtOptions> mOptions;

        public JwtService(IOptionsSnapshot<JwtOptions> options)
        {
            mOptions = options;
        }

        public string GenerateToken(string userName, string role)
        {
            var secret = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(mOptions.Value.SecretKey));

            var authClaims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userName),
                    new Claim("role", role)
                };

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(authClaims),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = mOptions.Value.Issuer,
                Audience = mOptions.Value.Audience,
                SigningCredentials = new SigningCredentials(secret, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public bool ValidateToken(string token)
        {
            var secret = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(mOptions.Value.SecretKey));
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var result = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = secret
                }, out SecurityToken validatedToken);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public string GetClaim(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
            return securityToken.Claims.FirstOrDefault().Value;
        }
    }
}
