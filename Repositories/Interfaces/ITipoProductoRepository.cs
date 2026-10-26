using DulcesDuendesApp.Models;
using DulcesDuendesApp.Common.Pagination;

namespace DulcesDuendesApp.Repositories.Interfaces
{
    public interface ITipoProductoRepository
    {
        Task<TipoProducto?> ObtenerPorId(int id);
        Task<bool> Existe(int id);
        Task<IReadOnlyList<TipoProducto>> Buscar(string? texto, int max);
        Task<PagedResult<TipoProducto>> ObtenerPaginado(string? texto, PageRequest page);
        Task<bool> ExisteNombre(string tipo, int idExcluido);
        Task<bool> TieneProductos(int id);
        Task<int> Alta(TipoProducto tipo);
        Task Actualizar(TipoProducto tipo);
        Task Eliminar(TipoProducto tipo);
    }
}
