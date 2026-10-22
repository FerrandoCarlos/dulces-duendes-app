using DulcesDuendesApp.Data;
using DulcesDuendesApp.Models;
using DulcesDuendesApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DulcesDuendesApp.Repositories.Implementations
{
    public class TicketRepository : ITicketRepository
    {
        private readonly ApplicationDbContext _context;

        public TicketRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Ticket?> ObtenerPorId(int id)
        {
            return await _context.Tickets
                .AsNoTracking()
                .Include(t => t.Usuario)
                .Include(t => t.Cliente)
                .FirstOrDefaultAsync(t => t.Id == id && t.Activo);
        }

        public async Task<IList<Ticket>> ObtenerPorCliente(int clienteId)
        {
            return await _context.Tickets
                .AsNoTracking()
                .Include(t => t.Usuario)
                .Include(t => t.Cliente)
                .Where(t => t.ClienteId == clienteId && t.Activo)
                .OrderByDescending(t => t.Fecha)
                .ToListAsync();
        }

        public async Task<int> Alta(Ticket ticket)
        {
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();
            return ticket.Id;
        }

        public async Task<int> Modificacion(int id, Ticket ticket)
        {
            var ticketExiste = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == id && t.Activo);
            if (ticketExiste != null)
            {
                ticketExiste.ClienteId = ticket.ClienteId;
                ticketExiste.UsuarioId = ticket.UsuarioId;
                ticketExiste.TipoPago = ticket.TipoPago;
                ticketExiste.Total = ticket.Total;
                ticketExiste.Fecha = ticket.Fecha;
                await _context.SaveChangesAsync();
            }
            return id;
        }

        public async Task<int> Baja(int id)
        {
            var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == id && t.Activo);
            if (ticket != null)
            {
                ticket.Activo = false;
                await _context.SaveChangesAsync();
            }
            return id;
        }
        public async Task<IList<Ticket>> ObtenerTickets()
        {
            return await _context.Tickets
            .AsNoTracking()
            .Include(t => t.Usuario)
            .Include(t => t.Cliente)
            .Where(t => t.Activo)
            .OrderByDescending(t => t.Fecha)
            .ToListAsync();
        }
    }
}