using System.Security.Claims;
using DulcesDuendesApp.Common.Auth;
using DulcesDuendesApp.Models.Dtos;
using DulcesDuendesApp.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DulcesDuendesApp.Controllers.Api
{
    [ApiController]
    [Route("api/auth")]
    public class AuthApiController : ControllerBase
    {
        private readonly IUsuarioService _usuarios;
        private readonly ITokenService _tokens;

        public AuthApiController(IUsuarioService usuarios, ITokenService tokens)
        {
            _usuarios = usuarios;
            _tokens = tokens;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<TokenResponse>> Login(LoginApiRequest request)
        {
            var usuario = await _usuarios.ValidarCredenciales(request.Email.Trim(), request.Password);

            // Mismo mensaje para email inexistente y contraseña errónea: no revela qué cuentas existen.
            if (usuario == null)
                return Unauthorized(new { mensaje = "Email o contraseña incorrectos." });

            return _tokens.Generar(usuario);
        }

        [HttpGet("yo")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult Yo()
        {
            return Ok(new
            {
                id = User.ObtenerId(),
                email = User.FindFirstValue(ClaimTypes.Email),
                rol = User.FindFirstValue(ClaimTypes.Role)
            });
        }
    }
}
