using DulcesDuendesApp.Common.Pagination;
using DulcesDuendesApp.Models;
using DulcesDuendesApp.Repositories.Interfaces;
using DulcesDuendesApp.Services.Interfaces;
using Microsoft.AspNetCore.Identity;


namespace DulcesDuendesApp.Services.Interfaces
{
    public interface IClienteService
    {
        Task<Cliente?> ObtenerPorId(int id);
        Task<int> Alta(Cliente cliente);
        Task<int> Modificacion(int id, Cliente cliente);
        Task<int> Baja(int id);
        //Task<IList<Cliente>> ObtenerClientes();

        Task<PagedResult<Cliente>> ObtenerPaginado(PageRequest pageRequest, string? busqueda);
    }
}