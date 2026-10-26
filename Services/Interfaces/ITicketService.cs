using DulcesDuendesApp.Models;
using DulcesDuendesApp.Repositories.Interfaces;
using DulcesDuendesApp.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace DulcesDuendesApp.Services.Interfaces
{
    public interface ITicketService
    {
        Task<Ticket?> ObtenerPorId(int id);
        Task<IList<Ticket>> ObtenerPorCliente(int clienteId);
        Task<int> Alta(Ticket ticket);
        Task<int> Modificacion(int id, Ticket ticket);
        Task<int> Baja(int id);
        Task<IList<Ticket>> ObtenerTickets();
    }
}