using System.Security.Claims;
using DulcesDuendesApp.Models;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace DulcesDuendesApp.Common.Auth
{
    public static class UsuarioClaims
    {
        public const string AvatarUrl = "AvatarUrl";

        public static ClaimsPrincipal CrearPrincipal(Usuario u)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, u.Id.ToString()),
                new(ClaimTypes.Name, $"{u.Nombre} {u.Apellido}"),
                new(ClaimTypes.Email, u.Email),
                new(ClaimTypes.Role, u.Rol?.Nombre ?? string.Empty),
            };

            if (!string.IsNullOrEmpty(u.AvatarUrl)) claims.Add(new Claim(AvatarUrl, u.AvatarUrl));

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            return new ClaimsPrincipal(identity);
        }

        public static int ObtenerId(this ClaimsPrincipal user) => int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}
