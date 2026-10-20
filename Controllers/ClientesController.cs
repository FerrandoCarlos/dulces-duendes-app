using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DulcesDuendesApp.Models;
using DulcesDuendesApp.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using DulcesDuendesApp.Services.Interfaces;
using DulcesDuendesApp.Common.Exceptions;

namespace DulcesDuendesApp.Controllers
{
    public class ClientesController : Controller
    {

        private readonly IClienteService _service;
        private const int TamPaginaDefault = 10;

        public ClientesController(IClienteService service)
        {
            _service = service;
        }

        // GET: /Clientes

        public async Task<IActionResult> Index(int paginaNro = 1, CancellationToken cancellationToken = default)
        {
            try {
                var lista = await _service.ObtenerClientes(cancellationToken);
                return View(lista);
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = $"Ha ocurrido un error al cargar las vistas de Clientes. {ex.Message}"});
            }
        }

        // GET : /Clientes/Create

        public IActionResult Create()
        {
            return View();
        }

        // POST: /Clientes/Guardar

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Guardar([FromBody] Cliente cliente, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                var errores = ModelState.Where(kvp => kvp.Value?.Errors.Count > 0).SelectMany(kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage));
                return BadRequest(new { success = false, message = string.Join(" ", errores)});
            }
            try
            { 
                if (cliente.Id == 0)
                {
                    var nuevoId = await _service.Alta(cliente, cancellationToken);
                    return Ok(new { success = true, message = "Cliente creado correctamente", data = new { id = nuevoId} });
                }
                else
                {
                    await _service.Modificacion(cliente.Id, cliente, cancellationToken);
                    return Ok(new { success = true, message = "Cliente modificado con éxito"});
                }
            } catch (AppException ex) 
            {
                return BadRequest(new { success = false, message = ex.Message});
            } catch (Exception ex) 
            {
                return StatusCode(500, new { success = false, message = $"Ocurrió un error inesperado al guardar el cliente. {ex.Message}"});
            }
        }

        // POST : /Clientes/Eliminar/ID
    
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id, CancellationToken cancellationToken = default) 
        {
            try 
            {
                await _service.Baja(id, cancellationToken);
                return Ok(new { success = true, message = "Cliente dado de baja correctamente"});
            }
            catch (AppException ex) 
            {
                return BadRequest(new { success = false, message = ex.Message});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Ocurrió un error inesperado al eliminar el cliente. {ex.Message}"});
            }
        }
    }
}
