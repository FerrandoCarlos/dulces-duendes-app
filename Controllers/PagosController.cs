using DulcesDuendesApp.Models;
using DulcesDuendesApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DulcesDuendesApp.Controllers
{
    public class PagosController : Controller
    {
        private readonly IPagoService _pagoService;
        private readonly IClienteService _clienteService;

        public PagosController(IPagoService pagoService, IClienteService clienteService)
        {
            _pagoService = pagoService;
            _clienteService = clienteService;
        }

        // GET : /Pagos/Cliente/ID

        public async Task<IActionResult> Index(int clienteId)
        {
            var cliente = await _clienteService.ObtenerPorId(clienteId);
            if (cliente == null) return NotFound();
            return View();
        }

        // GET : /Pagos/ObtenerCuentaCorrienteJson

        public async Task<IActionResult> ObtenerDatosCuentaCorrienteJson(int clienteId)
        {
            try
            {
                var cliente = await _clienteService.ObtenerPorId(clienteId);
                if (cliente == null)
                {
                    return NotFound(new { success = false, message = "Cliente no encontrado." });
                }
                var pagos = await _pagoService.ObtenerPorClienteId(clienteId);

                return Ok(new
                {
                    cliente = new
                    {
                        id = cliente.Id,
                        nombreCompleto = $"{cliente.Nombre} {cliente.Apellido}",
                        dni = cliente.Dni,
                        monto = cliente.Monto
                    },
                    pagos = pagos.Select(p => new
                    {
                        id = p.Id,
                        ticketId = p.TicketId,
                        monto = p.Monto,
                        fechaVencimiento = p.fechaVencimiento.ToString("dd/MM/yyyy"),
                        fechaPago = p.fechaPago != DateTime.MinValue ? p.fechaPago.ToString("dd/MM/yyyy") : "-",
                        estado = p.Estado
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // POST : /Pagos/RegistrarPago/ID
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrarPago(int pagoId)
        {
            try
            {
                await _pagoService.RegistrarPago(pagoId);
                return Ok(new { success = true, message = "Pago registrado correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}