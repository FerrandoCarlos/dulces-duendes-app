using DulcesDuendesApp.Models;
using DulcesDuendesApp.Repositories.Interfaces;
using DulcesDuendesApp.Services.Interfaces;
using Microsoft.AspNetCore.Identity;


namespace DulcesDuendesApp.Services.Implementations
{
    public interface IClienteService
    {
        Task<Cliente?> ObtenerPorId(int id, CancellationToken cancellationToken = default);
        Task<int> Alta(Cliente cliente, CancellationToken cancellationToken = default);
        Task<int> Modificacion(int id, Cliente cliente, CancellationToken cancellationToken = default);
        Task<int> Baja(int id, CancellationToken cancellationToken = default);
        Task<IList<Cliente>> ObtenerClientes(CancellationToken cancellationToken = default);
    }
}