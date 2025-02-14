using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ApiJWT.Models;
using Microsoft.IdentityModel.Tokens;

namespace ApiJWT.Custom
{
    public class Utilities(IConfiguration configuration)
    {
        private readonly IConfiguration _configuration = configuration;

        public string EncryptSha256(string key)
        {
            //Computar el hash
            byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(key));

            //Convertir el array de bytes en string
            StringBuilder builder = new();
            for (var i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }

            return builder.ToString();

        }

        public string GenerateToken(User model)
        {
            //Crear la información del usuario para el token
            var userClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, model.Name.ToString()),
                new Claim(ClaimTypes.Email, model.Email)
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:key"]!));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            //Crear detalle del token
            var jwtConfig = new JwtSecurityToken(
                claims : userClaims,
                expires : DateTime.UtcNow.AddMinutes(10),
                signingCredentials : credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(jwtConfig);
        }
    }
}