using DulcesDuendesApp.Models;

namespace DulcesDuendesApp.Repositories.Interfaces
{
    public interface IPagoRepository
    {
        Task<IList<Pago>> ObtenerPorClienteId(int clienteId);
        Task<Pago?> ObtenerPorTicketId(int ticketId);
        Task RegistrarPago(int pagoId);
        Task CancelarPorTicketId(int ticketId);
    }
}