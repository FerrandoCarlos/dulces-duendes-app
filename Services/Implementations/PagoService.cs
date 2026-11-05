using DulcesDuendesApp.Common.Pagination;
using DulcesDuendesApp.Models;
using DulcesDuendesApp.Repositories.Interfaces;
using DulcesDuendesApp.Services.Interfaces;

namespace DulcesDuendesApp.Services.Implementations
{
    public class PagoService : IPagoService
    {
        private readonly IPagoRepository _pagoRepository;

        public PagoService(IPagoRepository pagoRepository)
        {
            _pagoRepository = pagoRepository;
        }

        public async Task<IList<Pago>> ObtenerPorClienteId(int clienteId)
        {
            return await _pagoRepository.ObtenerPorClienteId(clienteId);
        }

        public async Task<PagedResult<Pago>> ObtenerPaginado(int clienteId, PageRequest pageRequest, string? fechaDesde = null, string? fechaHasta = null)
        {
            return await _pagoRepository.ObtenerPaginado(clienteId, pageRequest, fechaDesde, fechaHasta);
        }

        public async Task<Pago?> ObtenerPorTicketId(int ticketId)
        {
            return await _pagoRepository.ObtenerPorTicketId(ticketId);
        }

        public async Task RegistrarPago(int pagoId)
        {
            await _pagoRepository.RegistrarPago(pagoId);
        }

        public async Task CancelarPorTicketId(int ticketId)
        {
            await _pagoRepository.CancelarPorTicketId(ticketId);
        }
    }
}