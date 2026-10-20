using DulcesDuendesApp.Models;

namespace DulcesDuendesApp.Repositories.Interfaces
{
    public interface IClienteRepository
    {
        Task<Cliente?> ObtenerPorId(int id, CancellationToken cancellationToken = default);
        Task<int> Alta(Cliente cliente, CancellationToken cancellationToken = default);
        Task<int> Modificacion(int id, Cliente cliente, CancellationToken cancellationToken = default);
        Task<int> Baja(int id, CancellationToken cancellationToken = default);
        Task<IList<Cliente>> ObtenerClientes(CancellationToken cancellationToken = default);
    }
}