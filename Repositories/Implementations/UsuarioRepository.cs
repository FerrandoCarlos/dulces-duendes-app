using DulcesDuendesApp.Common.Auth;
using DulcesDuendesApp.Common.Pagination;
using DulcesDuendesApp.Data;
using DulcesDuendesApp.Models;
using DulcesDuendesApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DulcesDuendesApp.Repositories.Implementations
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly ApplicationDbContext _context;

        public UsuarioRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> ObtenerPorEmail(string email)
        {
            return await _context.Usuarios
             .Include(u => u.Rol)
             .FirstOrDefaultAsync(u => u.Email == email && u.Activo);
        }

        public async Task<int> Alta(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return usuario.Id;
        }

        public async Task<int> ObtenerCantidad()
        {
            return await _context.Usuarios.CountAsync();
        }

        public async Task<Usuario?> ObtenerPorId(int id, bool incluirInactivos = false)
        {
            return await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Id == id && (incluirInactivos || u.Activo));
        }

        public async Task<bool> ExisteEmail(string email, int idExcluido)
        {
            return await _context.Usuarios
                .AnyAsync(u => u.Email == email && u.Id != idExcluido);
        }

        public async Task Actualizar(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
        }
        public async Task<PagedResult<Usuario>> ObtenerPaginado(string? texto, PageRequest page)
        {
            var query = _context.Usuarios.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(texto))
            {
                var t = texto.Trim();
                query = query.Where(u => u.Nombre.Contains(t)
                    || u.Apellido.Contains(t)
                    || u.Email.Contains(t));
            }

            var total = await query.CountAsync();

            var items = await query
                .Include(u => u.Rol)
                .OrderBy(u => u.Apellido).ThenBy(u => u.Nombre).ThenBy(u => u.Id)
                .Skip(page.Skip)
                .Take(page.PageSize)
                .ToListAsync();

            return new PagedResult<Usuario>
            {
                Items = items,
                TotalCount = total,
                Page = page.Page,
                PageSize = page.PageSize
            };
        }
        public async Task<IReadOnlyList<Rol>> ObtenerRoles()
        {
            return await _context.Roles.AsNoTracking().OrderBy(r => r.Nombre).ToListAsync();
        }

        public async Task<int> ContarAdministradoresActivos()
        {
            return await _context.Usuarios
                .CountAsync(u => u.Activo && u.Rol!.Nombre == NombresRol.Administrador);
        }
    }
}
