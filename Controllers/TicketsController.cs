using Microsoft.AspNetCore.Mvc;
using DulcesDuendesApp.Models;
using Microsoft.AspNetCore.Authorization;
using DulcesDuendesApp.Services.Interfaces;
using DulcesDuendesApp.Common.Exceptions;
using DulcesDuendesApp.Models.Dtos;
using System.Security.Claims;

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

        public async Task<IActionResult> Index(int paginaNro = 1)
        {
            try
            {
                var lista = await _service.ObtenerTickets();
                return View(lista);
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = $"Ha ocurrido un erroral cargar las vistas de Tickets. {ex.Message}" });
            }
        }

        // GET: Tickets/ObtenerDetallesJson/ID

        public async Task<IActionResult> ObtenerDetallesJson(int ticketId)
        {
            try
            {
                var detalles = await _detalleService.ObtenerPorTicketId(ticketId);
                return Ok(detalles);
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = $"Ha ocurrido un error al cargar las vistas de Detalles. {ex.Message}" });
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

        // POST : /Tickets/Eliminar/ID

        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
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