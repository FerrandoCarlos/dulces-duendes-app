using DulcesDuendesApp.Common.Configuration;
using DulcesDuendesApp.Common.Filters;
using DulcesDuendesApp.Common.Pagination;
using DulcesDuendesApp.Data;
using DulcesDuendesApp.Models;
using DulcesDuendesApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DulcesDuendesApp.Repositories.Implementations
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly int _stockBajo;

        public ProductoRepository(ApplicationDbContext context, IOptionsSnapshot<NegocioOptions> negocio)
        {
            _context = context;
            _stockBajo = negocio.Value.StockBajo;
        }

        public async Task<Producto?> ObtenerPorId(int id)
        {
            return await _context.Productos
                .Include(p => p.TipoProducto)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<PagedResult<Producto>> ObtenerPaginado(ProductoFiltro filtro, PageRequest page)
        {
            var query = AplicarFiltro(_context.Productos.AsNoTracking(), filtro);

            var total = await query.CountAsync();

            var items = await query
                .Include(p => p.TipoProducto)
                .OrderBy(p => p.Descripcion)
                .ThenBy(p => p.Id)
                .Skip(page.Skip)
                .Take(page.PageSize)
                .ToListAsync();
            return new PagedResult<Producto>
            {
                Items = items,
                TotalCount = total,
                Page = page.Page,
                PageSize = page.PageSize
            };
        }

        private IQueryable<Producto> AplicarFiltro(IQueryable<Producto> query, ProductoFiltro filtro)
        {
            query = query.Where(p => p.Activo);

            if (!string.IsNullOrWhiteSpace(filtro.Texto))
            {
                var texto = filtro.Texto.Trim();
                query = query.Where(p => p.CodigoBarra.StartsWith(texto) || p.Descripcion.Contains(texto));
            }

            if (filtro.TipoProductoId.HasValue)
            {
                query = query.Where(p => p.TipoProductoId == filtro.TipoProductoId.Value);
            }

            return filtro.Estado switch
            {
                EstadoStock.ConStock => query.Where(p => p.Stock > 0),
                EstadoStock.StockBajo => query.Where(p => p.Stock > 0 && p.Stock <= _stockBajo),
                EstadoStock.SinStock => query.Where(p => p.Stock == 0),
                _ => query
            };
        }
    }
}
