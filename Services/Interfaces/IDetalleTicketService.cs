using DulcesDuendesApp.Models;

namespace DulcesDuendesApp.Services.Interfaces
{
    public interface IDetalleTicketService
    {
        Task<IList<DetalleTicket>> ObtenerPorTicketId (int ticketId);
        Task<DetalleTicket?> ObtenerPorId (int id);
        Task<int> Alta(DetalleTicket detalleTicket);
        Task<int> Modificacion(int id, DetalleTicket detalleTicket);
        Task<int> Baja (int id);
    }
}