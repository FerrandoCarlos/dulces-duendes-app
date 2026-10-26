using DulcesDuendesApp.Data;
using DulcesDuendesApp.Models;
using DulcesDuendesApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DulcesDuendesApp.Repositories.Implementations
{
    public class DetalleTicketRepository : IDetalleTicketRepository
    {
        private readonly ApplicationDbContext _context;

        public DetalleTicketRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IList<DetalleTicket>> ObtenerPorTicketId (int ticketId)
        {
            return await _context.DetalleTickets
                .AsNoTracking()
                .Include(d => d.Producto)
                .Where(d => d.TicketId == ticketId)
                .ToListAsync();
        }

        public async Task<DetalleTicket?> ObtenerPorId (int id)
        {
            return await _context.DetalleTickets
                .AsNoTracking()
                .Include(d => d.Producto)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<int> Alta(DetalleTicket detalleTicket)
        {
            await _context.DetalleTickets.AddAsync(detalleTicket);
            await _context.SaveChangesAsync();
            return detalleTicket.Id;
        }

        public async Task<int> Modificacion(int id, DetalleTicket detalleTicket)
        {
            var detalleExiste = await _context.DetalleTickets.FindAsync(id);
            if (detalleExiste == null) return 0;
            detalleExiste.Cantidad = detalleTicket.Cantidad;
            detalleExiste.PrecioUnitario = detalleTicket.PrecioUnitario;
            _context.DetalleTickets.Update(detalleExiste);
            return await _context.SaveChangesAsync();
        }
        
        public async Task<int> Baja (int id)
        {
            var detalleExiste = await _context.DetalleTickets.FindAsync(id);
            if (detalleExiste == null) return 0;
            _context.DetalleTickets.Remove(detalleExiste);
            return await _context.SaveChangesAsync();
        }
    }
}