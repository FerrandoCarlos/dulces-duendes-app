using DulcesDuendesApp.Common.Pagination;
using DulcesDuendesApp.Models;

namespace DulcesDuendesApp.Repositories.Interfaces
{
    public interface IPagoRepository
    {
        Task<IList<Pago>> ObtenerPorClienteId(int clienteId);
        Task<Pago?> ObtenerPorTicketId(int ticketId);
        Task<PagedResult<Pago>> ObtenerPaginado(int clienteId, PageRequest pageRequest, string? fechaDesde = null, string? fechaHasta = null);
        Task RegistrarPago(int pagoId);
        Task CancelarPorTicketId(int ticketId);
    }
}