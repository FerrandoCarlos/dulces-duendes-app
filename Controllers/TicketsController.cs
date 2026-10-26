using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DulcesDuendesApp.Models;
using DulcesDuendesApp.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using DulcesDuendesApp.Services.Interfaces;
using DulcesDuendesApp.Common.Exceptions;

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

        public async Task<IActionResult> Index (int paginaNro = 1)
        {
            try
            {
                var lista = await _service.ObtenerTickets();
                return View(lista);
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = $"Ha ocurrido un erroral cargar las vistas de Tickets. {ex.Message}"});
            }
        }

        // GET: Tickets/ObtenerDetallesJson/ID

        public async Task<IActionResult> ObtenerDetallesJson(int ticketId)
        {
            try
            {
                var detalles = await _detalleService.ObtenerPorTicketId(ticketId);
                return Ok(detalles);
            } catch (Exception ex)
            {
                return BadRequest(new { success = false, message = $"Ha ocurrido un error al cargar las vistas de Detalles. {ex.Message}"});
            }
        }

        // GET : /Tickets/Create

        public async Task<IActionResult> Create ()
        {
            return View();
        }

        // POST: /Tickets/Guardar

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Guardar([FromBody] Ticket ticket)
        {
            if (!ModelState.IsValid)
            {
                var errores = ModelState.Where(kvp => kvp.Value?.Errors.Count > 0).SelectMany(kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage));
                return BadRequest(new { success = false, message = string.Join(" ", errores)});
            }
            try
            {
                if (ticket.Id == 0)
                {
                    var nuevoId = await _service.Alta(ticket);
                    return Ok(new { success = true, message = "Ticket creado correctamente", data = new { id = nuevoId}});
                }
                else
                {
                    await _service.Modificacion(ticket.Id, ticket);
                    return Ok(new { success = true, message = "Ticket modificado con éxito."});
                }
            } catch (AppException ex)
            {
                return BadRequest(new { success = false, message = ex.Message});
            } catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Ocurrió un error inesperado al guardar el ticket. {ex.Message}"});
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
                return Ok(new { success = true, message = "Ticket dado de baja correctamente."});
            }
            catch (AppException ex)
            {
                return BadRequest(new { success = false, message = ex.Message});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Ocurró un error inesperado al eliminar el ticket. {ex.Message}"});
            }
        }

    }
}