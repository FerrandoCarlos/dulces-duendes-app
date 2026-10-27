using DulcesDuendesApp.Common.Exceptions;
using DulcesDuendesApp.Models.Dtos;
using DulcesDuendesApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DulcesDuendesApp.Controllers.Ajax
{
    [ApiController]
    [Route("ajax/productos/{productoId:int}/imagenes")]
    [ServiceFilter(typeof(AppExceptionFilter))]
    public class ImagenesProductoAjaxController : ControllerBase
    {
        private readonly IImagenProductoService _service;

        public ImagenesProductoAjaxController(IImagenProductoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ImagenProductoResponse>>> Listar(int productoId)
        {
            var imagenes = await _service.ObtenerPorProducto(productoId);
            return imagenes == null ? NotFound() : Ok(imagenes.Select(i => i.ToResponse()));
        }

        [HttpPost]
        [RequestSizeLimit(12 * 1024 * 1024)]
        public async Task<ActionResult<IEnumerable<ImagenProductoResponse>>> Subir(
            int productoId, [FromForm] List<IFormFile>? archivos)
        {
            var streams = (archivos ?? new List<IFormFile>())
                .Select(a => a.OpenReadStream())
                .ToList();
            try
            {
                var nuevas = await _service.Agregar(productoId, streams);
                return nuevas == null ? NotFound() : Ok(nuevas.Select(i => i.ToResponse()));
            }
            finally
            {
                foreach (var s in streams) await s.DisposeAsync();
            }
        }

        [HttpDelete("{imagenId:int}")]
        public async Task<IActionResult> Eliminar(int productoId, int imagenId)
        {
            return await _service.Eliminar(productoId, imagenId) ? NoContent() : NotFound();
        }
    }
}
