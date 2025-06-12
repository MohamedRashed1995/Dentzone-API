using Dragza.Domain.DTO;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Infrastructure.Services
{
    public class JWTService
    {
        private readonly IConfiguration _configuration;

        public JWTService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<JWTTokenDTO> GenerateJwtToken(string userId)
        {

            var jwtSettings = _configuration.GetSection("JwtSettings");

            var claims = new[]
            {
                new Claim(ClaimTypes.Role, "client"),
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
              issuer: jwtSettings["Issuer"],
              audience: jwtSettings["Audience"],
              claims: claims,
              expires: DateTime.Now.AddHours(24),
              signingCredentials: credentials);

            var refreshToken = GenerateRefreshToken();
            var result = new JWTTokenDTO
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = refreshToken,
                UserId = userId
            };
            return result;
        }
        public JWTTokenDTO GenerateJwtToken(string userId, string NameIdentifier, string role)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");

            var claims = new[]
            {
                new Claim(ClaimTypes.Role, role),
                new Claim(ClaimTypes.NameIdentifier, NameIdentifier),
                new Claim("userid", userId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
              issuer: jwtSettings["Issuer"],
              audience: jwtSettings["Audience"],
              claims: claims,
              expires: DateTime.Now.AddHours(24),
              signingCredentials: credentials);

            var refreshToken = GenerateRefreshToken();
            var result = new JWTTokenDTO
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = refreshToken,
                UserId = userId,
                Role = role
            };
            return result;
        }

        public async Task<JWTTokenDTO> GenerateInventoryJwtToken(Guid doctorId)
        {

            var jwtSettings = _configuration.GetSection("JwtSettings");

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, doctorId.ToString()),
            new Claim(ClaimTypes.Role, "doctor"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
              issuer: jwtSettings["Issuer"],
              audience: jwtSettings["Audience"],
              claims: claims,
              expires: DateTime.Now.AddHours(24),
              signingCredentials: credentials);

            var refreshToken = GenerateRefreshToken();
            var result = new JWTTokenDTO
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = refreshToken,
                UserId = doctorId.ToString()
            };
            return result;
        }
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }

            return Convert.ToBase64String(randomNumber);
        }

        public string GeneratePasswordResetToken(string email)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var token = new JwtSecurityToken(
                jwtSettings["Issuer"],
                jwtSettings["Audience"],
                claims,
                expires: DateTime.Now.AddHours(3), // Set token expiration time
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtSettings = _configuration.GetSection("JwtSettings");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]));



            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    ClockSkew = TimeSpan.Zero // Remove default buffer for token expiration
                }, out SecurityToken validatedToken);

                return principal;
            }
            catch
            {
                // Token validation failed
                return null;
            }
        }
    }

}
