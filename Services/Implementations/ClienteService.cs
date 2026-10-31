using DulcesDuendesApp.Common.Pagination;
using DulcesDuendesApp.Models;
using DulcesDuendesApp.Repositories.Interfaces;
using DulcesDuendesApp.Services.Interfaces;
using Microsoft.AspNetCore.Identity;


namespace DulcesDuendesApp.Services.Implementations
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _repositorio;
        
        public ClienteService(IClienteRepository repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<Cliente?> ObtenerPorId(int id)
        {
            return await _repositorio.ObtenerPorId(id);
        }
        public async Task<int> Alta(Cliente cliente)
        {
            return await _repositorio.Alta(cliente);
        }
        public async Task<int> Modificacion(int id, Cliente cliente){
            var c = await _repositorio.ObtenerPorId(id);
            if (c == null ) {
                return -1;
            }
            return await _repositorio.Modificacion(id, cliente);
        }
        public async Task<int> Baja(int id)
        {
            var c = await _repositorio.ObtenerPorId(id);
            if (c == null) {
                return -1;
            }
            return await _repositorio.Baja(id);
        }
        /*public async Task<IList<Cliente>> ObtenerClientes(CancellationToken cancellationToken = default)
        {
            return await _repositorio.ObtenerClientes(cancellationToken);
        }*/

        public async Task<PagedResult<Cliente>> ObtenerPaginado(PageRequest pageRequest, string? busqueda)
        {
            return await _repositorio.ObtenerPaginado(pageRequest, busqueda);
        }

        public async Task<IList<Cliente>> BuscarPorNombre(string busqueda)
        {
            if (string.IsNullOrWhiteSpace(busqueda))
                return new List<Cliente>();
            return await _repositorio.BuscarPorNombre(busqueda);
        }
    }
}