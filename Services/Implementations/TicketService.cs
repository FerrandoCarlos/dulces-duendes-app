using DulcesDuendesApp.Common.Exceptions;
using DulcesDuendesApp.Models;
using DulcesDuendesApp.Repositories.Interfaces;
using DulcesDuendesApp.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace DulcesDuendesApp.Services.Implementations
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _repositorio;

        public TicketService(ITicketRepository repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<Ticket?> ObtenerPorId(int id)
        {
            return await _repositorio.ObtenerPorId(id);
        }

        public async Task<IList<Ticket>> ObtenerPorCliente(int clienteId)
        {
            return await _repositorio.ObtenerPorCliente(clienteId);
        }

        public async Task<int> Alta(Ticket ticket)
        {
            if (ticket.Detalles == null || !ticket.Detalles.Any())
            {
                throw new AppException("El ticket debe contener al menos un producto");
            }
            return await _repositorio.Alta(ticket);
        }

        public async Task<int> Modificacion(int id, Ticket ticket)
        {
            var t = await _repositorio.ObtenerPorId(id);
            if (t == null)
            {
                return -1;
            }
            return await _repositorio.Modificacion(id, ticket);
        }

        public async Task<int> Baja(int id)
        {
            var t = await _repositorio.ObtenerPorId(id);
            if (t == null)
            {
                return -1;
            }
            return await _repositorio.Baja(id);
        }

        public async Task<IList<Ticket>> ObtenerTickets()
        {
            return await _repositorio.ObtenerTickets();
        }
    }
}