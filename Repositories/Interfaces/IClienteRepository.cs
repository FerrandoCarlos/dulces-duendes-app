using DulcesDuendesApp.Common.Pagination;
using DulcesDuendesApp.Models;

namespace DulcesDuendesApp.Repositories.Interfaces
{
    public interface IClienteRepository
    {
        Task<Cliente?> ObtenerPorId(int id);
        Task<int> Alta(Cliente cliente);
        Task<int> Modificacion(int id, Cliente cliente);
        Task<int> Baja(int id);
        //Task<IList<Cliente>> ObtenerClientes();
        Task<PagedResult<Cliente>> ObtenerPaginado(PageRequest pageRequest, string? busqueda);
        Task<IList<Cliente>> BuscarPorNombre(string busqueda, int limite = 20);
    }
}