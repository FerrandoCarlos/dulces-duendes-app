using System.Security.Claims;
using DulcesDuendesApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DulcesDuendesApp.ViewComponents
{
    public class UserMenuViewComponent : ViewComponents
    {
        public IViewComponentResult Invoke()
        {
            var nombre = User.Identity?.Name ?? string.Empty;
            var email = User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
            var avatar = new AvatarViewModel(User.FindFirst("AvatarUrl")?.Value, ObtenerIniciales(nombre));

            return View(new UserMenuViewModel(nombre, email, avatar));
        }

        private static string ObtenerIniciales(string nombreCompleto)=>
            string.Concat(nombreCompleto
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Take(2)
            .Select(p=> char.ToUpper(p[0])));
    }
}
