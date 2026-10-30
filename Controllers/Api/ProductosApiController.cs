using DulcesDuendesApp.Common.Auth;
using DulcesDuendesApp.Common.Exceptions;
using DulcesDuendesApp.Common.Filters;
using DulcesDuendesApp.Common.Pagination;
using DulcesDuendesApp.Models.Dtos;
using DulcesDuendesApp.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DulcesDuendesApp.Controllers.Api
{
    [ApiController]
    [Route("api/productos")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ServiceFilter(typeof(AppExceptionFilter))]
    public class ProductosApiController : ControllerBase
    {
        private readonly IProductoService _service;

        public ProductosApiController(IProductoService service)
        {
            _service = service;
        }
        [HttpGet]
        public async Task<ActionResult<PagedResult<ProductoResponse>>> Listar([FromQuery] ProductoFiltro filtro, int page = 1, int pageSize = PageRequest.DefaultPageSize)
        {
            var resultado = await _service.ObtenerPaginado(filtro, new PageRequest(page, pageSize));
            return resultado.ToResponse();
        }
        [HttpGet("stock-critico")]
        public async Task<ActionResult<PagedResult<ProductoResponse>>> StockCritico(
           int page = 1, int pageSize = PageRequest.DefaultPageSize)
        {
            var filtro = new ProductoFiltro { Estado = EstadoStock.StockCritico };
            var resultado = await _service.ObtenerPaginado(filtro, new PageRequest(page, pageSize));
            return resultado.ToResponse();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductoResponse>> Obtener(int id)
        {
            var producto = await _service.ObtenerPorId(id);
            return producto is { Activo: true } ? producto.ToResponse() : NotFound();
        }

        [HttpGet("codigo/{codigoBarra}")]
        public async Task<ActionResult<ProductoResponse>> ObtenerPorCodigo(string codigoBarra)
        {
            var producto = await _service.ObtenerPorCodigoBarra(codigoBarra);
            return producto == null ? NotFound() : producto.ToResponse();
        }

        [HttpPost]
        [Authorize(Roles = NombresRol.Administrador, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<ProductoResponse>> Crear(ProductoRequest request)
        {
            var id = await _service.Alta(request.ToEntity());
            var creado = await _service.ObtenerPorId(id);
            return CreatedAtAction(nameof(Obtener), new { id }, creado!.ToResponse());
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = NombresRol.Administrador, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Modificar(int id, ProductoRequest request)
        {
            return await _service.Modificar(request.ToEntity(id)) ? NoContent() : NotFound();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = NombresRol.Administrador, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Baja(int id)
        {
            return await _service.Baja(id) ? NoContent() : NotFound();
        }
    }
}
