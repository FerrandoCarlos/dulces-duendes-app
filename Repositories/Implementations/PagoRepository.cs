using DulcesDuendesApp.Data;
using DulcesDuendesApp.Models;
using DulcesDuendesApp.Repositories.Interfaces;
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