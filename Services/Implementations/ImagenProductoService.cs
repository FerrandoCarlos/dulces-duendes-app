using DulcesDuendesApp.Common.Exceptions;
using DulcesDuendesApp.Common.Validation;
using DulcesDuendesApp.Models;
using DulcesDuendesApp.Repositories.Interfaces;
using DulcesDuendesApp.Services.Interfaces;

namespace DulcesDuendesApp.Services.Implementations
{
    public class ImagenProductoService : IImagenProductoService
    {
        private const string Carpeta = "productos";

        private readonly IImagenProductoRepository _repo;
        private readonly IProductoRepository _productos;
        private readonly IAlmacenArchivos _almacen;

        public ImagenProductoService(
            IImagenProductoRepository repo,
            IProductoRepository productos,
            IAlmacenArchivos almacen)
        {
            _repo = repo;
            _productos = productos;
            _almacen = almacen;
        }

        public async Task<List<ImagenProducto>?> ObtenerPorProducto(int productoId)
        {
            if (!await ExisteActivo(productoId)) return null;
            return await _repo.ObtenerPorProducto(productoId);
        }

        public async Task<List<ImagenProducto>?> Agregar(int productoId, IReadOnlyList<Stream> archivos)
        {
            if (!await ExisteActivo(productoId)) return null;

            if (archivos.Count == 0)
                throw new AppException("Seleccioná al menos una imagen.");

            var existentes = await _repo.ContarPorProducto(productoId);
            if (existentes + archivos.Count > LimitesProducto.ImagenesMax)
                throw new AppException($"Un producto admite hasta {LimitesProducto.ImagenesMax} imágenes.");

            var guardadas = new List<string>();
            try
            {
                foreach (var archivo in archivos)
                    guardadas.Add(await _almacen.GuardarImagen(archivo, Carpeta));

                var nuevas = guardadas
                    .Select(url => new ImagenProducto { ProductoId = productoId, Url = url })
                    .ToList();

                await _repo.Alta(nuevas);
                return nuevas;
            }
            catch
            {
                foreach (var url in guardadas) _almacen.Eliminar(url);
                throw;
            }
        }

        public async Task<bool> Eliminar(int productoId, int imagenId)
        {
            var imagen = await _repo.ObtenerPorId(imagenId);
            if (imagen == null || imagen.ProductoId != productoId) return false;

            await _repo.Eliminar(imagen);
            _almacen.Eliminar(imagen.Url);
            return true;
        }

        private async Task<bool> ExisteActivo(int productoId)
        {
            var producto = await _productos.ObtenerPorId(productoId);
            return producto is { Activo: true };
        }
    }
}
