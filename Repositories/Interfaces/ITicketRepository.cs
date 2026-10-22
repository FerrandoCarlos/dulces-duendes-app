using DulcesDuendesApp.Models;

namespace DulcesDuendesApp.Repositories.Interfaces
{
    public interface ITicketRepository
    {
        Task<Ticket?> ObtenerPorId (int id);
        Task<IList<Ticket>> ObtenerPorCliente (int clienteId);
        Task<int> Alta(Ticket ticket);
        Task<int> Modificacion(int id, Ticket ticket);
        Task<int> Baja (int id);
        Task<IList<Ticket>> ObtenerTickets();
    }
}