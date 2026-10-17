using System.Security.Claims;
using DulcesDuendesApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DulcesDuendesApp.ViewComponents
{
    public class UserMenuViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var nombre = UserClaimsPrincipal.Identity?.Name ?? string.Empty;
            var email = UserClaimsPrincipal.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
            var avatar = new AvatarViewModel(UserClaimsPrincipal.FindFirst("AvatarUrl")?.Value, ObtenerIniciales(nombre));

            return View(new UserMenuViewModel(nombre, email, avatar));
        }

        private static string ObtenerIniciales(string nombreCompleto)=>
            string.Concat(nombreCompleto
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Take(2)
            .Select(p=> char.ToUpper(p[0])));
    }
}
