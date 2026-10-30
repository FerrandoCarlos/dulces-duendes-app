using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DulcesDuendesApp.Models;
using DulcesDuendesApp.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using DulcesDuendesApp.Services.Interfaces;
using DulcesDuendesApp.Common.Exceptions;
using DulcesDuendesApp.Common.Pagination;

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

        public async Task<IActionResult> Index(string? busqueda, int paginaNro = 1)
        {
            var pageRequest = new PageRequest(page: paginaNro, pageSize: PageRequest.DefaultPageSize);
            PagedResult<Cliente> resultado = await _service.ObtenerPaginado(pageRequest, busqueda);

            ViewBag.Busqueda = busqueda;

            return View(resultado);

        }
        
        public async Task<IActionResult> ObtenerPaginadoJson(string? busqueda, int paginaNro = 1)
        {
            var pageRequest = new PageRequest(page: paginaNro, pageSize: PageRequest.DefaultPageSize);
            PagedResult<Cliente> resultado = await _service.ObtenerPaginado(pageRequest, busqueda);

            return Ok(resultado);
        }

        public async Task<IActionResult> BuscarClientesJson(string busqueda)
        {
            if (string.IsNullOrWhiteSpace(busqueda))
            {
                return Ok(new List<Cliente>());
            }
            try
            {
                var clientes = await _service.BuscarPorNombre(busqueda);
                return Ok(clientes);
            } catch (Exception ex) {
                return StatusCode(500, new { success = false, message = $"Error al buscar clientes: {ex.Message}"});
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
        public async Task<IActionResult> Guardar([FromBody] Cliente cliente)
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
                    var nuevoId = await _service.Alta(cliente);
                    return Ok(new { success = true, message = "Cliente creado correctamente", data = new { id = nuevoId} });
                }
                else
                {
                    await _service.Modificacion(cliente.Id, cliente);
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
        public async Task<IActionResult> Eliminar(int id) 
        {
            try 
            {
                await _service.Baja(id);
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
