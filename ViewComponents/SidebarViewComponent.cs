using DulcesDuendesApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DulcesDuendesApp.ViewComponents
{
    public class SidebarViewComponent : ViewComponent
    {
        private static readonly List<SidebarItemViewModel> Items = new()
        {
            new("Clientes", "bi-people","Clientes","Index"),
            new("Ventas","bi-cart3","Home","Index"),
            new("Productos", "bi-box-seam","Productos", "Index"),
            new("Tickets", "bi-receipt","Tickets","Index"),
            new("Usuarios", "bi-person-gear", "Usuarios","Index",Rol:"Administrador"),
            new("Panel", "bi-speedometer2","Dashboard","Index"),
        };

        public IViewComponentResult Invoke()
        {
            var controllerActual = ViewContext.RouteData.Values["controller"]?.ToString();

            var visibles = Items
                .Where(i => i.Rol == null || User.IsInRole(i.Rol))
                .Select(i => i with { Activo = i.Controller == controllerActual });

            return View(visibles);
        }
    }
}
