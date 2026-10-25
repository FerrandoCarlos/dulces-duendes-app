using DulcesDuendesApp.Data;
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
    }
}
