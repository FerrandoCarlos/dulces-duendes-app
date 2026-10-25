using DulcesDuendesApp.Models;
using DulcesDuendesApp.Repositories.Interfaces;
using DulcesDuendesApp.Services.Interfaces;

namespace DulcesDuendesApp.Services.Implementations
{
    public class DetalleTicketService : IDetalleTicketService
    {
        private readonly IDetalleTicketRepository _repositorio;

        public DetalleTicketService(IDetalleTicketRepository repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<IList<DetalleTicket>> ObtenerPorTicketId(int ticketId)
        {
            return await _repositorio.ObtenerPorTicketId(ticketId);
        }
        public async Task<DetalleTicket?> ObtenerPorId(int id)
        {
            return await _repositorio.ObtenerPorId(id);
        }
        public async Task<int> Alta(DetalleTicket detalleTicket)
        {
            return await _repositorio.Alta(detalleTicket);
        }
        public async Task<int> Modificacion(int id, DetalleTicket detalleTicket)
        {
            var d = await _repositorio.ObtenerPorId(id);
            if (d == null)
            {
                return -1;
            }
            return await _repositorio.Modificacion(id, detalleTicket);
        }
        public async Task<int> Baja (int id)
        {
            var d = await _repositorio.ObtenerPorId(id);
            if (d == null)
            {
                return -1;
            }
            return await _repositorio.Baja(id);
        }
    }
}