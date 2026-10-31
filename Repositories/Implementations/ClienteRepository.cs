using DulcesDuendesApp.Common.Pagination;
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

        public async Task<Cliente?> ObtenerPorId(int id)
        {
            return await _context.Clientes.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<int> Alta(Cliente cliente)
        {
            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();
            return cliente.Id;
        }

        public async Task<int> Modificacion(int id, Cliente cliente)
        {
            _context.Clientes.Update(cliente);
            await _context.SaveChangesAsync();
            return id;
        }

        public async Task<int> Baja(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente != null)
            {
                cliente.Activo = false;
                await _context.SaveChangesAsync();
            }
            return id;
        }
        /*public async Task<IList<Cliente>> ObtenerClientes(){
            return await _context.Clientes.Where(c => c.Activo).AsNoTracking().ToListAsync();
        }*/

        public async Task<PagedResult<Cliente>> ObtenerPaginado(PageRequest pageRequest, string? busqueda)
        {
            var query = _context.Clientes
                .AsNoTracking()
                .Where(c => c.Activo);

            if (!string.IsNullOrEmpty(busqueda))
            {
                string q = busqueda.Trim().ToLower();
                query = query.Where(c =>
                    c.Nombre.ToLower().Contains(q) ||
                    c.Apellido.ToLower().Contains(q));
            }

            int totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(c => c.Apellido)
                .ThenBy(c => c.Nombre)
                .Skip(pageRequest.Skip)
                .Take(pageRequest.PageSize)
                .ToListAsync();

            return new PagedResult<Cliente>
            {
                Items = items,
                TotalCount = totalCount,
                Page = pageRequest.Page,
                PageSize = pageRequest.PageSize
            };
        }
        public async Task<IList<Cliente>> BuscarPorNombre(string busqueda, int limite = 20)
        {
            if (string.IsNullOrWhiteSpace(busqueda))
            return new List<Cliente>();
            var termino = busqueda.Trim().ToLower();
            return await _context.Clientes
                .AsNoTracking()
                .Where(c => c.Activo && (c.Nombre.ToLower().Contains(termino) || c.Apellido.ToLower().Contains(termino)))
                .OrderBy(c => c.Nombre)
                .Take(limite)
                .ToListAsync();
        } 
    }
}