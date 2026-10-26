using DulcesDuendesApp.Data;
using DulcesDuendesApp.Models;
using DulcesDuendesApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DulcesDuendesApp.Repositories.Implementations
{
    public class ImagenProductoRepository : IImagenProductoRepository
    {
        private readonly ApplicationDbContext _context;

        public ImagenProductoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ImagenProducto>> ObtenerPorProducto(int productoId)
        {
            return await _context.ImagenesProducto
                .AsNoTracking()
                .Where(i => i.ProductoId == productoId)
                .OrderBy(i => i.Id)
                .ToListAsync();
        }

        public async Task<ImagenProducto?> ObtenerPorId(int id)
        {
            return await _context.ImagenesProducto.FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<int> ContarPorProducto(int productoId)
        {
            return await _context.ImagenesProducto.CountAsync(i => i.ProductoId == productoId);
        }

        public async Task Alta(IEnumerable<ImagenProducto> imagenes)
        {
            _context.ImagenesProducto.AddRange(imagenes);
            await _context.SaveChangesAsync();
        }

        public async Task Eliminar(ImagenProducto imagen)
        {
            _context.ImagenesProducto.Remove(imagen);
            await _context.SaveChangesAsync();
        }
    }
}
