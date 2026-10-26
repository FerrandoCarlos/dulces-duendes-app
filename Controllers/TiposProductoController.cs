using DulcesDuendesApp.Common.Exceptions;
using DulcesDuendesApp.Common.Pagination;
using DulcesDuendesApp.Models.ViewModels;
using DulcesDuendesApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DulcesDuendesApp.Controllers
{
    public class TiposProductoController : Controller
    {
        private readonly ITipoProductoService _service;

        public TiposProductoController(ITipoProductoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? texto, int page = 1)
        {
            var resultado = await _service.ObtenerPaginado(texto, new PageRequest(page));

            // Si la página pedida quedó vacía (por ejemplo tras borrar el último ítem), vuelve a la última.
            if (!resultado.Items.Any() && resultado.TotalCount > 0)
                return RedirectToAction(nameof(Index), new { texto, page = resultado.TotalPages });

            return View(new TiposProductoIndexViewModel { Texto = texto, Resultado = resultado });
        }

        [HttpGet]
        public IActionResult Crear() => View("Form", new TipoProductoFormViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(TipoProductoFormViewModel modelo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _service.Alta(modelo.Tipo);
                    TempData["Exito"] = "Tipo creado.";
                    return RedirectToAction(nameof(Index));
                }
                catch (AppException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            return View("Form", modelo);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var tipo = await _service.ObtenerPorId(id);
            if (tipo == null) return NotFound();

            return View("Form", new TipoProductoFormViewModel { Id = tipo.Id, Tipo = tipo.Tipo });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, TipoProductoFormViewModel modelo)
        {
            modelo.Id = id;

            if (ModelState.IsValid)
            {
                try
                {
                    if (!await _service.Modificar(id, modelo.Tipo)) return NotFound();

                    TempData["Exito"] = "Tipo actualizado.";
                    return RedirectToAction(nameof(Index));
                }
                catch (AppException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            return View("Form", modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id, string? texto, int page = 1)
        {
            try
            {
                var eliminado = await _service.Eliminar(id);
                TempData["Exito"] = eliminado ? "Tipo eliminado." : "El tipo ya no existe.";
            }
            catch (AppException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new { texto, page });
        }
    }
}
