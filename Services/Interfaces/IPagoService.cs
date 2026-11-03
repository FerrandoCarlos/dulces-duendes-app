using DulcesDuendesApp.Models;

namespace DulcesDuendesApp.Services.Interfaces
{
    public interface IPagoService
    {
        Task<IList<Pago>> ObtenerPorClienteId(int clienteId);
        Task<Pago?> ObtenerPorTicketId(int ticketId);
        Task RegistrarPago(int pagoId);
        Task CancelarPorTicketId(int ticketId);
    }
}