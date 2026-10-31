using DulcesDuendesApp.Models;
using DulcesDuendesApp.Common.Pagination;
using DulcesDuendesApp.Common.Filters;

namespace DulcesDuendesApp.Repositories.Interfaces
{
    public interface IProductoRepository
    {
        Task<Producto?> ObtenerPorId(int id);
        Task<PagedResult<Producto>> ObtenerPaginado(ProductoFiltro filtro, PageRequest page);
        Task<Producto?> ObtenerPorCodigoBarra(string codigoBarra);
        Task<int> Alta(Producto producto);
        Task Modificar(Producto producto);
        Task<IList<Producto>> BuscarPorDescripcion(string busqueda, int limite = 20);
    }
}
