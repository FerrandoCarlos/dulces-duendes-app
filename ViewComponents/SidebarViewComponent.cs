using DulcesDuendesApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DulcesDuendesApp.ViewComponents
{
    public class SidebarViewComponent : ViewComponent
    {
        private static readonly List<SidebarItemViewModel> Items = new()
        {
            new("Perfil", "bi-person-circle", "Perfil", "Index"),
            new("Clientes", "bi-people","Clientes","Index"),
            new("Ventas","bi-cart3","Home","Index"),
            new("Productos", "bi-box-sean","Productos", "Index"),
            new("Tickets", "bi-receipt","Tickets","Index"),
            new("Usuarios", "bi-person-gear", "Usuarios","Index",Rol:"Administrador"),
        };

        public IViewComponentResult Invoke()
        {
            var controllerActual = ViewContext.RouteData.Values["controller"]?.ToString();

            var visibles = Items
                .Where(i => i.Rol == null || User.IsInRol(i.Rol))
                .Select(i => i with {Activo = i.Controller == controllerActual});

            return View(visibles);
        }
    }
}
