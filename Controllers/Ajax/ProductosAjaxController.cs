using DulcesDuendesApp.Common.Exceptions;
using DulcesDuendesApp.Common.Filters;
using DulcesDuendesApp.Common.Pagination;
using DulcesDuendesApp.Models.Dtos;
using DulcesDuendesApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DulcesDuendesApp.Controllers.Ajax
{
    [ApiController]
    [Route("ajax/productos")]
    [ServiceFilter(typeof(AppExceptionFilter))]
    public class ProductosAjaxController : ControllerBase
    {
        private readonly IProductoService _service;

        public ProductosAjaxController(IProductoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<ProductoResponse>>> Listar([FromQuery] ProductoFiltro filtro, int page = 1, int pageSize = PageRequest.DefaultPageSize)
        {
            var resultado = await _service.ObtenerPaginado(filtro, new PageRequest(page, pageSize));
            return resultado.ToResponse();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductoResponse>> Obtener(int id)
        {
            var producto = await _service.ObtenerPorId(id);
            return producto is { Activo: true } ? producto.ToResponse() : NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<ProductoResponse>> Crear(ProductoRequest request)
        {
            var id = await _service.Alta(request.ToEntity());
            var creado = await _service.ObtenerPorId(id);
            return CreatedAtAction(nameof(Obtener), new { id }, creado!.ToResponse());
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Modificar(int id, ProductoRequest request)
        {
            return await _service.Modificar(request.ToEntity(id)) ? NoContent() : NotFound();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Baja(int id)
        {
            return await _service.Baja(id) ? NoContent() : NotFound();
        }
    }
}
