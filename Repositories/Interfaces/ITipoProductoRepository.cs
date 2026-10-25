using DulcesDuendesApp.Models;

namespace DulcesDuendesApp.Repositories.Interfaces
{
    public interface ITipoProductoRepository
    {
        Task<TipoProducto?> ObtenerPorId(int id);
        Task<bool> Existe(int id);
        Task<IReadOnlyList<TipoProducto>> Buscar(string? texto, int max);
    }
}
