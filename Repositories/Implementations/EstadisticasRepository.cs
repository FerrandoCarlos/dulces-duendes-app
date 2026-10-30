using DulcesDuendesApp.Common.Auth;
using DulcesDuendesApp.Data;
using DulcesDuendesApp.Models;
using DulcesDuendesApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DulcesDuendesApp.Repositories.Implementations
{
    public class EstadisticasRepository : IEstadisticasRepository
    {
        private readonly ApplicationDbContext _context;

        public EstadisticasRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        private IQueryable<Producto> Activos => _context.Productos.AsNoTracking().Where(p => p.Activo);
        public Task<int> ContarProductosActivos() => Activos.CountAsync();
        public Task<int> ContarStockBajo(int umbral) => Activos.CountAsync(p => p.Stock > 0 && p.Stock <= umbral);
        public Task<int> ContarSinStock() => Activos.CountAsync(p => p.Stock == 0);
        public async Task<IReadOnlyList<Producto>> ObtenerAReponer(int umbral, int max)
        {
            return await Activos
                .Where(p => p.Stock <= umbral)
                .OrderBy(p => p.Stock).ThenBy(p => p.Descripcion).ThenBy(p => p.Id)
                .Take(max)
                .ToListAsync();
        }
        public Task<decimal> ValorInventario() => Activos.SumAsync(p => p.PrecioCosto * p.Stock);
        public Task<decimal> GananciaEstimada() => Activos.SumAsync(p => (p.PrecioVenta - p.PrecioCosto) * p.Stock);
        public Task<int> ContarUsuarios(bool activos) => _context.Usuarios.CountAsync(u => u.Activo == activos);
        public Task<int> ContarAdministradoresActivos() => _context.Usuarios.CountAsync(u => u.Activo && u.Rol!.Nombre == NombresRol.Administrador);
    }
}
