using DulcesDuendesApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DulcesDuendesApp.Controllers
{
    public class ProductosController : Controller
    {
        private readonly IProductoService _productoService;
        
        public ProductosController (IProductoService productoService)
        {
            _productoService = productoService;
        }
        public IActionResult Index() => View();

        public async Task<IActionResult> ObtenerPorCodigoJson(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return BadRequest(new { success = false, message = "El código no puede estar vacío." });

            var producto = await _productoService.ObtenerPorCodigoBarra(codigo);
            if (producto == null)
                return NotFound(new { success = false, message = "Producto no encontrado." });

            return Ok(producto);
        }

        [HttpGet]
        public async Task<IActionResult> BuscarPorDescripcionJson(string busqueda)
        {
            var productos = await _productoService.BuscarPorDescripcion(busqueda ?? string.Empty);
            return Ok(productos);
        }
        
    }
}

