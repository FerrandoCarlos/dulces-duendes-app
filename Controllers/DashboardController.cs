using DulcesDuendesApp.Common.Auth;
using DulcesDuendesApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DulcesDuendesApp.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboard;

        public DashboardController(IDashboardService dashboard)
        {
            _dashboard = dashboard;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var vm = await _dashboard.Obtener(User.IsInRole(NombresRol.Administrador));
            return View(vm);
        }
    }
}
