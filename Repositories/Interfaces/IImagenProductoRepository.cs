using DulcesDuendesApp.Models;

namespace DulcesDuendesApp.Repositories.Interfaces
{
    public interface IImagenProductoRepository
    {
        Task<List<ImagenProducto>> ObtenerPorProducto(int productoId);
        Task<ImagenProducto?> ObtenerPorId(int id);
        Task<int> ContarPorProducto(int productoId);
        Task Alta(IEnumerable<ImagenProducto> imagenes);
        Task Eliminar(ImagenProducto imagen);
    }
}
