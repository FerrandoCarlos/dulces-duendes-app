using Microsoft.AspNetCore.Mvc;
using DulcesDuendesApp.Models;
using Microsoft.AspNetCore.Authorization;
using DulcesDuendesApp.Services.Interfaces;
using DulcesDuendesApp.Common.Exceptions;
using DulcesDuendesApp.Models.Dtos;
using System.Security.Claims;
using DulcesDuendesApp.Common.Pagination;
using System.Xml.XPath;

namespace DulcesDuendesApp.Controllers
{
    public class TicketsController : Controller
    {
        private readonly ITicketService _service;
        private readonly IDetalleTicketService _detalleService;
        private const int TamPaginaDefault = 10;
        public TicketsController(ITicketService service, IDetalleTicketService detalleService)
        {
            _service = service;
            _detalleService = detalleService;
        }

        // GET: /Tickets

        public async Task<IActionResult> Index(string? fechaDesde, string? fechaHasta, int paginaNro = 1)
        {
            try
            {
                var pageRequest = new PageRequest(page: paginaNro, pageSize: PageRequest.DefaultPageSize);
                PagedResult<Ticket> pagedTickets = await _service.ObtenerPaginado(pageRequest, fechaDesde, fechaHasta);
                return View(pagedTickets);
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = $"Ha ocurrido un erroral cargar las vistas de Tickets. {ex.Message}" });
            }
        }

        public async Task<IActionResult> ObtenerTodosJson(string? fechaDesde, string? fechaHasta, int paginaNro = 1)
        {
            var pageRequest = new PageRequest(page: paginaNro, pageSize: PageRequest.DefaultPageSize);
            PagedResult<Ticket> resultado = await _service.ObtenerPaginado(pageRequest, fechaDesde, fechaHasta);

            var response = new PagedResult<object>
            {
                Page = resultado.Page,
                PageSize = resultado.PageSize,
                TotalCount = resultado.TotalCount,
                Items = resultado.Items.Select(t => new
                {
                    id = t.Id,
                    fecha = t.Fecha,
                    usuarioNombre = t.Usuario != null ? $"{t.Usuario.Nombre} {t.Usuario.Apellido}" : "Sistema",
                    clienteNombre = t.Cliente != null ? $"{t.Cliente.Nombre} {t.Cliente.Apellido}" : "Consumidor Final",
                    tipoPago = t.TipoPago.ToString(),
                    total = t.Total,
                    activo = t.Activo
                }).ToList()
            };

            return Ok(response);
        }

        // GET: Tickets/ObtenerDetallesJson/ID

        public async Task<IActionResult> ObtenerDetallesJson(int ticketId)
        {
            try
            {
                var ticket = await _service.ObtenerPorId(ticketId);
                if (ticket == null)
                {
                    return NotFound(new { success = false, message = "Ticket no encontrado." });
                }

                var detalles = await _detalleService.ObtenerPorTicketId(ticketId);

                return Ok(new
                {
                    id = ticket.Id,
                    fecha = ticket.Fecha,
                    usuarioNombre = ticket.Usuario != null ? $"{ticket.Usuario.Nombre} {ticket.Usuario.Apellido}" : "Sistema",
                    clienteNombre = ticket.Cliente != null ? $"{ticket.Cliente.Nombre} {ticket.Cliente.Apellido}" : "Consumidor Final",
                    tipoPago = ticket.TipoPago.ToString(),
                    total = ticket.Total,
                    detalles = detalles
                });
            }
            catch (Exception ex)
            {
                var mensajeReal = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { success = false, message = $"Error al obtener el detalle: {mensajeReal}" });
            }
        }

        // GET : /Tickets/Create

        public async Task<IActionResult> Create()
        {
            return View();
        }

        // POST: /Tickets/Guardar

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Guardar([FromBody] GuardarTicketDto dto)
        {
            if (dto == null || dto.Detalles == null || !dto.Detalles.Any())
            {
                return BadRequest(new { success = false, message = "El ticket no contiene productos." });
            }

            if (dto.TipoPago == TipoPago.CuentaCorriente && (!dto.ClienteId.HasValue || dto.ClienteId.Value <= 0))
            {
                return BadRequest(new { success = false, message = "Debe seleccionar un cliente para cobros en Cuenta Corriente." });
            }

            try
            {
                var claimUsuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                int usuarioIdActual = int.TryParse(claimUsuarioId, out var id) ? id : 1;

                var ticket = new Ticket
                {
                    Id = dto.Id,
                    UsuarioId = usuarioIdActual,
                    ClienteId = dto.TipoPago == TipoPago.CuentaCorriente ? dto.ClienteId : null,
                    TipoPago = dto.TipoPago,
                    Fecha = DateTime.Now,
                    Activo = true,
                    Total = dto.Detalles.Sum(d => d.Cantidad * d.PrecioUnitario),
                    Detalles = dto.Detalles.Select(d => new DetalleTicket
                    {
                        ProductoId = d.ProductoId,
                        Cantidad = d.Cantidad,
                        PrecioUnitario = d.PrecioUnitario,
                        Subtotal = d.Cantidad * d.PrecioUnitario
                    }).ToList()
                };

                if (ticket.Id == 0)
                {
                    var nuevoId = await _service.Alta(ticket);
                    return Ok(new { success = true, message = "Venta registrada correctamente.", id = nuevoId });
                }

                await _service.Modificacion(ticket.Id, ticket);
                return Ok(new { success = true, message = "Ticket modificado correctamente." });
            }
            catch (AppException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                var mensajeReal = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, new { success = false, message = $"Error en base de datos: {mensajeReal}" });
            }
        }

        // POST : /Tickets/Anular/ID

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Anular(int id)
        {
            try
            {
                await _service.Baja(id);
                return Ok(new { success = true, message = "Ticket dado de baja correctamente." });
            }
            catch (AppException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Ocurró un error inesperado al eliminar el ticket. {ex.Message}" });
            }
        }

    }
}