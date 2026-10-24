using DulcesDuendesApp.Common.Auth;
using DulcesDuendesApp.Common.Exceptions;
using DulcesDuendesApp.Models;
using DulcesDuendesApp.Models.ViewModels;
using DulcesDuendesApp.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DulcesDuendesApp.Controllers
{
    [Authorize]
    public class PerfilController : Controller
    {
        private readonly IUsuarioService _usuarioService;

        public PerfilController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var usuario = await _usuarioService.ObtenerPorId(User.ObtenerId());
            if (usuario == null) return await CerrarSesion();

            return View(new PerfilViewModel
            {
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Email = usuario.Email
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(PerfilViewModel modelo)
        {
            if (!ModelState.IsValid) return View(modelo);

            try
            {
                var usuario = await _usuarioService.ActualizarPerfil(
                    User.ObtenerId(), modelo.Nombre, modelo.Apellido, modelo.Email);

                await ReemitirSesion(usuario);
                TempData["Exito"] = "Perfil actualizado.";
                return RedirectToAction(nameof(Index));
            }
            catch (AppException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(modelo);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarAvatar(IFormFile? avatar)
        {
            if (avatar == null || avatar.Length == 0)
            {
                TempData["Error"] = "Seleccioná una imagen.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await using var contenido = avatar.OpenReadStream();
                var usuario = await _usuarioService.CambiarAvatar(User.ObtenerId(), contenido);

                await ReemitirSesion(usuario);
                TempData["Exito"] = "Foto actualizada.";
            }
            catch (AppException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuitarAvatar()
        {
            try
            {
                var usuario = await _usuarioService.QuitarAvatar(User.ObtenerId());
                await ReemitirSesion(usuario);
                TempData["Exito"] = "Foto eliminada.";
            }
            catch (AppException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task ReemitirSesion(Usuario usuario)
        {
            var actual = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var propiedades = actual.Properties ?? new AuthenticationProperties();

            // Conserva IsPersistent (Recordarme) pero renueva las fechas de la cookie.
            propiedades.IssuedUtc = null;
            propiedades.ExpiresUtc = null;

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                UsuarioClaims.CrearPrincipal(usuario),
                propiedades);
        }

        private async Task<IActionResult> CerrarSesion()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

    }
}
