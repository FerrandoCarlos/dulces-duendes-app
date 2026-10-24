using DulcesDuendesApp.Common.Auth;
using DulcesDuendesApp.Models.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace DulcesDuendesApp.ViewComponents
{
    public class UserMenuViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var nombre = UserClaimsPrincipal.Identity?.Name ?? string.Empty;
            var email = UserClaimsPrincipal.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;

            return View(new UserMenuViewModel(nombre, email, UserClaimsPrincipal.ObtenerAvatar()));
        }
    }
}
