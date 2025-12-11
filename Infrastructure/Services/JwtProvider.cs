using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using votesystembackend.Domain.Entities;

namespace votesystembackend.Infrastructure.Services
{
    public interface IJwtProvider
    {
        string GenerateToken(User user);
    }

    public class JwtProvider : IJwtProvider
    {
        private readonly IConfiguration _config;

        public JwtProvider(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(User user)
        {
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? "default_secret_key_please_change");
            var claims = new[]
            {
                new Claim("id", user.Id.ToString()),
                new Claim("email", user.Email)
            };

            var token = new JwtSecurityToken(
                expires: DateTime.UtcNow.AddDays(7),
                claims: claims,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256
                )
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
