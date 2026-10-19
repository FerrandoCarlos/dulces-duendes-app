using DulcesDuendesApp.Common.Filters;
using DulcesDuendesApp.Common.Pagination;
using DulcesDuendesApp.Models;

namespace DulcesDuendesApp.Services.Interfaces
{
    public interface IProductoService
    {
        Task<PagedResult<Producto>> ObtenerPaginado(ProductoFiltro filtro, PageRequest page);
        Task<Producto?> ObtenerPorId(int id);
        Task<Producto?> ObtenerPorCodigoBarra(string codigoBarra);
        Task<int> Alta(Producto producto);
        Task<bool> Modificar(Producto producto);
        Task<bool> Baja(int id);

    }
}
