using DulcesDuendesApp.Common.Pagination;
using DulcesDuendesApp.Data;
using DulcesDuendesApp.Models;
using DulcesDuendesApp.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DulcesDuendesApp.Repositories.Implementations
{
    public class PagoRepository : IPagoRepository
    {
        private readonly ApplicationDbContext _context;

        public PagoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IList<Pago>> ObtenerPorClienteId(int clienteId)
        {
            return await _context.Pagos
                .Include(p => p.Cliente)
                .Include(p => p.Ticket)
                .Where(p => p.ClienteId == clienteId)
                .OrderByDescending(p => p.fechaVencimiento)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Pago?> ObtenerPorTicketId(int ticketId)
        {
            return await _context.Pagos.FirstOrDefaultAsync(p => p.TicketId == ticketId);
        }

        public async Task<PagedResult<Pago>> ObtenerPaginado(int clienteId, PageRequest pageRequest, string? fechaDesde = null, string? fechaHasta = null)
        {
            var query = _context.Pagos
                .Include(p => p.Cliente)
                .Include(p => p.Ticket)
                .Where(p => p.ClienteId == clienteId)
                .AsNoTracking();
            
            if (!string.IsNullOrEmpty(fechaDesde) && DateTime.TryParse(fechaDesde, out var desde))
            {
                query = query.Where(p => p.fechaVencimiento >= desde.Date);
            }

            if (!string.IsNullOrEmpty(fechaHasta) && DateTime.TryParse(fechaHasta, out var hasta))
            {
                var hastaFin = hasta.Date.AddDays(1).AddTicks(-1);
                query = query.Where(p => p.fechaVencimiento <= hastaFin);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(p => p.fechaVencimiento)
                .Skip((pageRequest.Page - 1) * pageRequest.PageSize)
                .Take(pageRequest.PageSize)
                .ToListAsync();
            
            return new PagedResult<Pago>
            {
                Items = items,
                TotalCount = totalCount,
                Page = pageRequest.Page,
                PageSize = pageRequest.PageSize,
            };
        }
        public async Task RegistrarPago(int pagoId)
        {
            var pago = await _context.Pagos
                .Include(p => p.Cliente)
                .FirstOrDefaultAsync(p => p.Id == pagoId);
            if (pago != null && pago.Estado == "Pendiente")
            {
                pago.Estado = "Pagado";
                pago.fechaPago = DateTime.Now;

                if (pago.Cliente != null)
                {
                    pago.Cliente.Monto -= pago.Monto;
                    if (pago.Cliente.Monto < 0) pago.Cliente.Monto = 0;
                }

                await _context.SaveChangesAsync();
            }
        }

        public async Task CancelarPorTicketId(int ticketId)
        {
            var pago = await _context.Pagos.FirstOrDefaultAsync(p => p.TicketId == ticketId);
            if (pago != null)
            {
                pago.Estado = "Cancelado";
                await _context.SaveChangesAsync();
            }
        }
    }
}