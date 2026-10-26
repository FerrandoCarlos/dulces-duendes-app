using DulcesDuendesApp.Models;
using DulcesDuendesApp.Common.Pagination;

namespace DulcesDuendesApp.Services.Interfaces
{
    public interface ITipoProductoService
    {
        Task<IReadOnlyList<TipoProducto>> Buscar(string? texto);
        Task<PagedResult<TipoProducto>> ObtenerPaginado(string? texto, PageRequest page);
        Task<TipoProducto?> ObtenerPorId(int id);
        Task<int> Alta(string tipo);
        Task<bool> Modificar(int id, string tipo);
        Task<bool> Eliminar(int id);
    }
}
