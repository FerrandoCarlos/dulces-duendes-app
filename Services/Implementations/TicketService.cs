using DulcesDuendesApp.Common.Exceptions;
using DulcesDuendesApp.Data;
using DulcesDuendesApp.Models;
using DulcesDuendesApp.Repositories.Interfaces;
using DulcesDuendesApp.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace DulcesDuendesApp.Services.Implementations
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _repositorio;
        private readonly ApplicationDbContext _context;

        public TicketService(ITicketRepository repositorio, ApplicationDbContext context)
        {
            _repositorio = repositorio;
            _context = context;
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
            
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                foreach (var detalle in ticket.Detalles)
                {
                    var producto = await _context.Productos.FindAsync(detalle.ProductoId);
                    if (producto == null)
                    {
                        throw new AppException("El producto no existe.");
                    }

                    if (producto.Stock < detalle.Cantidad)
                    {
                        throw new AppException($"Stock insuficiente para el producto '{producto.Descripcion}'.");
                    }
                    producto.Stock -= detalle.Cantidad;
                    _context.Productos.Update(producto);
                }
                await _context.Tickets.AddAsync(ticket);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return ticket.Id;
            } catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
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