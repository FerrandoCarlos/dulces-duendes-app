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

        public async Task<Cliente?> ObtenerPorId(int id, CancellationToken cancellationToken = default)
        {
            return await _repositorio.ObtenerPorId(id, cancellationToken);
        }
        public async Task<int> Alta(Cliente cliente, CancellationToken cancellationToken = default)
        {
            return await _repositorio.Alta(cliente, cancellationToken);
        }
        public async Task<int> Modificacion(int id, Cliente cliente, CancellationToken cancellationToken = default){
            var c = await _repositorio.ObtenerPorId(id);
            if (c == null ) {
                return null;
            }
            return await _repositorio.Modificacion(id, cliente, cancellationToken);
        }
        public async Task<int> Baja(int id, CancellationToken cancellationToken = default)
        {
            var c = await _repositorio.ObtenerPorId(id);
            if (c == null) {
                return null;
            }
            return await _repositorio.Baja(id, cancellationToken);
        }
        public async Task<IList<Cliente>> ObtenerClientes(CancellationToken cancellationToken = default)
        {
            return await _repositorio.ObtenerClientes(cancellationToken);
        }
    }
}