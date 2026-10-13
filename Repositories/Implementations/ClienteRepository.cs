using DulcesDuendesApp.Data;
using DulcesDuendesApp.Models;
using DulcesDuendesApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DulcesDuendesApp.Repositories.Implementations
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly ApplicationDbContext _context;

        public ClienteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Cliente?> ObtenerPorId(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Clientes.AsNoTracking().FirstOrDefault(c => c.Id == id, cancellationToken);
        }

        public async Task<int> Alta(Cliente cliente, CancellationToken cancellationToken = default)
        {
            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync(cancellationToken);
            return cliente.Id;
        }

        public async Task<int> Modificacion(int id, Cliente cliente, CancellationToken cancellationToken = default)
        {
            _context.Clientes.Update(cliente);
            await _context.SaveChangesAsync(cancellationToken);
            return id;
        }

        public async Task<int> Baja(int id, CancellationToken cancellationToken = default)
        {
            var cliente = await _context.Clientes.FindAsync(new object[] { id }, cancellationToken);
            if (cliente != null)
            {
                cliente.Activo = false;
                await _context.SaveChangesAsync(cancellationToken);
            }
            return id;
        }
        public async Task<IList<Cliente>> ObtenerClientes(CancellationToken cancellationToken = default){
            return await _context.Clientes.Where(c => c.Activo).AsNoTracking().ToListAsync(cancellationToken);
        }
    }
}