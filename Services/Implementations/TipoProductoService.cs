using DulcesDuendesApp.Models;
using DulcesDuendesApp.Repositories.Interfaces;
using DulcesDuendesApp.Services.Interfaces;

namespace DulcesDuendesApp.Services.Implementations
{
    public class TipoProductoService : ITipoProductoService
    {
        private readonly ITipoProductoRepository _repo;
        private const int MaxResultados = 10;

        public TipoProductoService(ITipoProductoRepository repo)
        {
            _repo = repo;
        }

        public Task<IReadOnlyList<TipoProducto>> Buscar(string? texto)
        {
            return _repo.Buscar(texto, MaxResultados);
        }
    }
}
