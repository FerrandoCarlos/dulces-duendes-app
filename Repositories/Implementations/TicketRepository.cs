using DulcesDuendesApp.Common.Exceptions;
using DulcesDuendesApp.Common.Pagination;
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
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                await _context.Tickets.AddAsync(ticket);

                if (ticket.TipoPago == TipoPago.CuentaCorriente && ticket.ClienteId.HasValue && ticket.ClienteId.Value > 0)
                {
                    var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Id == ticket.ClienteId.Value && c.Activo);

                    if (cliente == null)
                    {
                        throw new AppException("El cliente seleccionado no existe o está inactivo.");
                    }

                    cliente.Monto += ticket.Total;
                    _context.Clientes.Update(cliente);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return ticket.Id;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
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
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var ticket = await _context.Tickets
                    .Include(t => t.Detalles)
                    .FirstOrDefaultAsync(t => t.Id == id && t.Activo);

                if (ticket == null)
                {
                    throw new AppException("El ticket no existe o ya ha sido anulado anteriormente.");
                }

                ticket.Activo = false;

                if (ticket.Detalles != null && ticket.Detalles.Any())
                {
                    foreach (var detalle in ticket.Detalles)
                    {
                        var producto = await _context.Productos.FirstOrDefaultAsync(p => p.Id == detalle.ProductoId);
                        if (producto != null)
                        {
                            producto.Stock += detalle.Cantidad;
                        }
                    }
                }

                if (ticket.TipoPago == TipoPago.CuentaCorriente && ticket.ClienteId.HasValue)
                {
                    var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Id == ticket.ClienteId.Value);
                    if (cliente != null)
                    {
                        cliente.Monto -= ticket.Total;
                        if (cliente.Monto < 0)
                        {
                            cliente.Monto = 0;
                        }
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return id;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
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

        public async Task<PagedResult<Ticket>> ObtenerPaginado(PageRequest pageRequest, string? fechaDesde = null, string? fechaHasta = null)
        {
            {
                var query = _context.Tickets
                    .Include(t => t.Cliente)
                    .Include(t => t.Usuario)
                    .Where(t => t.Activo)
                    .AsNoTracking();

                if (!string.IsNullOrEmpty(fechaDesde) && DateTime.TryParse(fechaDesde, out var desde))
                {
                    query = query.Where(t => t.Fecha >= desde.Date);
                }

                if (!string.IsNullOrEmpty(fechaHasta) && DateTime.TryParse(fechaHasta, out var hasta))
                {
                    var hastaFinDia = hasta.Date.AddDays(1).AddTicks(-1);
                    query = query.Where(t => t.Fecha <= hastaFinDia);
                }

                var totalCount = await query.CountAsync();

                var items = await query
                    .OrderByDescending(t => t.Fecha)
                    .Skip((pageRequest.Page - 1) * pageRequest.PageSize)
                    .Take(pageRequest.PageSize)
                    .ToListAsync();

                return new PagedResult<Ticket>
                {
                    Items = items,
                    TotalCount = totalCount,
                    Page = pageRequest.Page,
                    PageSize = pageRequest.PageSize
                };

            }
        }

        public async Task<IEnumerable<Ticket>> ObtenerTodosJson()
        {
            return await _context.Tickets
                .Include(t => t.Cliente)
                .Include(t => t.Usuario)
                .AsNoTracking()
                .OrderByDescending(t => t.Fecha)
                .ToListAsync();
        }
    }
}