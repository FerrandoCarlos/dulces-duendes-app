using DulcesDuendesApp.Models;

namespace DulcesDuendesApp.Services.Interfaces
{
    public interface IImagenProductoService
    {
        Task<List<ImagenProducto>?> ObtenerPorProducto(int productoId);
        Task<List<ImagenProducto>?> Agregar(int productoId, IReadOnlyList<Stream> archivos);
        Task<bool> Eliminar(int productoId, int imagenId);
    }
}
