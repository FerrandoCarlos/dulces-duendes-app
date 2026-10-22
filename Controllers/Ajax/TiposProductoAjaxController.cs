using DulcesDuendesApp.Common.Exceptions;
using DulcesDuendesApp.Models.Dtos;
using DulcesDuendesApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DulcesDuendesApp.Controllers.Ajax
{
    [ApiController]
    [Route("ajax/tipos-producto")]
    [ServiceFilter(typeof(AppExceptionFilter))]
    public class TiposProductoAjaxController : ControllerBase
    {
        private readonly ITipoProductoService _service;

        public TiposProductoAjaxController(ITipoProductoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TipoProductoResponse>>> Buscar([FromQuery] string? texto)
        {
            var tipos = await _service.Buscar(texto);
            return Ok(tipos.Select(t => t.ToResponse()));
        }
    }
}
