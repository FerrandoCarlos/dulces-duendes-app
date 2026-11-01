using DulcesDuendesApp.Common.Filters;
using DulcesDuendesApp.Common.Pagination;
using DulcesDuendesApp.Models.Dtos;
using DulcesDuendesApp.Models.ViewModels;
using DulcesDuendesApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DulcesDuendesApp.Controllers
{
    public class CatalogoController : Controller
    {
        private const int TarjetasPorPagina = 6;

        private readonly IProductoService _service;

        public CatalogoController(IProductoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? texto, int page = 1)
        {
            var filtro = new ProductoFiltro { Texto = texto };
            var resultado = await _service.ObtenerPaginado(filtro, new PageRequest(page, TarjetasPorPagina));

            if (!resultado.Items.Any() && resultado.TotalCount > 0)
                return RedirectToAction(nameof(Index), new { texto, page = resultado.TotalPages });

            return View(new CatalogoIndexViewModel { Texto = texto, Resultado = resultado.ToResponse() });
        }
    }
}
