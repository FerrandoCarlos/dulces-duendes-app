using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using DulcesDuendesApp.Common.Configuration;
using DulcesDuendesApp.Models;
using DulcesDuendesApp.Models.Dtos;
using DulcesDuendesApp.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace DulcesDuendesApp.Services.Implementations
{
    public class TokenService : ITokenService
    {
        private readonly JwtOptions _opciones;

        public TokenService(IOptions<JwtOptions> opciones)
        {
            _opciones = opciones.Value;
        }
        public TokenResponse Generar(Usuario usuario)
        {
            var expira = DateTime.UtcNow.AddMinutes(_opciones.MinutosExpiracion);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new(ClaimTypes.Email, usuario.Email),
                new(ClaimTypes.Role, usuario.Rol?.Nombre ?? string.Empty),
            };

            var clave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opciones.Clave));
            var credenciales = new SigningCredentials(clave, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _opciones.Emisor,
                audience: _opciones.Audiencia,
                claims: claims,
                expires: expira,
                signingCredentials: credenciales
            );

            return new TokenResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiraUtc = expira
            };
        }
    }
}
