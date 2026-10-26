using DulcesDuendesApp.Common.Auth;
using DulcesDuendesApp.Common.Exceptions;
using DulcesDuendesApp.Common.Pagination;
using DulcesDuendesApp.Models;
using DulcesDuendesApp.Models.ViewModels;
using DulcesDuendesApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DulcesDuendesApp.Controllers
{
    [Authorize(Roles = NombresRol.Administrador)]
    public class UsuariosController : Controller
    {
        private readonly IUsuarioService _service;

        public UsuariosController(IUsuarioService service)
        {
            _service = service;
        }
        [HttpGet]
        public async Task<IActionResult> Index(string? texto, int page = 1)
        {
            var resultado = await _service.ObtenerPaginado(texto, new PageRequest(page));
            var idActual = User.ObtenerId();

            return View(new UsuariosIndexViewModel
            {
                Texto = texto,
                Resultado = new PagedResult<UsuarioFilaViewModel>
                {
                    Items = resultado.Items
                        .Select(u => new UsuarioFilaViewModel(
                            u.Id, $"{u.Apellido}, {u.Nombre}", u.Email,
                            u.Rol?.Nombre ?? string.Empty, u.Activo, u.Id == idActual))
                        .ToList(),
                    TotalCount = resultado.TotalCount,
                    Page = resultado.Page,
                    PageSize = resultado.PageSize
                }
            });
        }
        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            var vm = new UsuarioFormViewModel();
            await CargarRoles(vm);
            return View("Form", vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(UsuarioFormViewModel modelo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _service.Alta(new Usuario
                    {
                        Nombre = modelo.Nombre,
                        Apellido = modelo.Apellido,
                        Email = modelo.Email,
                        RolId = modelo.RolId
                    }, modelo.Password ?? string.Empty);
                    TempData["Exito"] = "Usuario creado.";
                    return RedirectToAction(nameof(Index));
                }
                catch (AppException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            await CargarRoles(modelo);
            return View("Form", modelo);
        }
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var usuario = await _service.ObtenerParaAdmin(id);
            if (usuario == null) return NotFound();

            var vm = new UsuarioFormViewModel
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Email = usuario.Email,
                RolId = usuario.RolId
            };
            await CargarRoles(vm);
            return View("Form", vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, UsuarioFormViewModel modelo)
        {
            modelo.Id = id;
            if (ModelState.IsValid)
            {
                try
                {
                    await _service.Modificar(id, modelo.Nombre, modelo.Apellido, modelo.Email, modelo.RolId, User.ObtenerId());

                    TempData["Exito"] = "Usuario actualizado.";
                    return RedirectToAction(nameof(Index));
                }
                catch (AppException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            await CargarRoles(modelo);
            return View("Form", modelo);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(int id, bool activo, string? texto, int page = 1)
        {
            try
            {
                await _service.CambiarEstado(id, activo, User.ObtenerId());
                TempData["Exito"] = activo ? "Usuario reactivado." : "Usuario desactivado.";
            }
            catch (AppException ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(Index), new { texto, page });
        }
        private async Task CargarRoles(UsuarioFormViewModel vm)
        {
            var roles = await _service.ObtenerRoles();
            vm.Roles = roles.Select(r => new SelectListItem(r.Nombre, r.Id.ToString())).ToList();
        }
    }
}
