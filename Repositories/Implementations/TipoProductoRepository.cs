using DulcesDuendesApp.Data;
using DulcesDuendesApp.Common.Pagination;
using DulcesDuendesApp.Models;
using DulcesDuendesApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DulcesDuendesApp.Repositories.Implementations
{
    public class TipoProductoRepository : ITipoProductoRepository
    {
        private readonly ApplicationDbContext _context;

        public TipoProductoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TipoProducto?> ObtenerPorId(int id)
        {
            return await _context.TiposProducto.FindAsync(id);
        }

        public async Task<bool> Existe(int id)
        {
            return await _context.TiposProducto.AnyAsync(t => t.Id == id);
        }

        public async Task<IReadOnlyList<TipoProducto>> Buscar(string? texto, int max)
        {
            var query = _context.TiposProducto.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(texto))
            {
                var t = texto.Trim();
                query = query.Where(x => x.Tipo.Contains(t));
            }
            return await query
                .OrderBy(x => x.Tipo)
                .Take(max)
                .ToListAsync();
        }
        public async Task<PagedResult<TipoProducto>> ObtenerPaginado(string? texto, PageRequest page)
        {
            var query = _context.TiposProducto.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(texto))
            {
                var t = texto.Trim();
                query = query.Where(x => x.Tipo.Contains(t));
            }

            var total = await query.CountAsync();

            var items = await query
                .OrderBy(x => x.Tipo).ThenBy(x => x.Id)
                .Skip(page.Skip)
                .Take(page.PageSize)
                .ToListAsync();

            return new PagedResult<TipoProducto>
            {
                Items = items,
                TotalCount = total,
                Page = page.Page,
                PageSize = page.PageSize
            };
        }

        public async Task<bool> ExisteNombre(string tipo, int idExcluido)
        {
            return await _context.TiposProducto.AnyAsync(t => t.Tipo == tipo && t.Id != idExcluido);
        }

        public async Task<bool> TieneProductos(int id)
        {
            // Sin filtrar por Activo: los productos dados de baja también bloquean el borrado.
            return await _context.Productos.AnyAsync(p => p.TipoProductoId == id);
        }

        public async Task<int> Alta(TipoProducto tipo)
        {
            _context.TiposProducto.Add(tipo);
            await _context.SaveChangesAsync();
            return tipo.Id;
        }

        public async Task Actualizar(TipoProducto tipo)
        {
            await _context.SaveChangesAsync();
        }

        public async Task Eliminar(TipoProducto tipo)
        {
            _context.TiposProducto.Remove(tipo);
            await _context.SaveChangesAsync();
        }
    }
}
