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
        private const int TamPaginaDefault = 10;
        public TicketsController(ITicketService service)
        {
            _service = service;
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

    }
}