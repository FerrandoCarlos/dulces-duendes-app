using DulcesDuendesApp.Common.Exceptions;
using DulcesDuendesApp.Common.Pagination;
using DulcesDuendesApp.Common.Validation;
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
        public Task<PagedResult<TipoProducto>> ObtenerPaginado(string? texto, PageRequest page) => _repo.ObtenerPaginado(texto, page);

        public Task<TipoProducto?> ObtenerPorId(int id) => _repo.ObtenerPorId(id);

        public async Task<int> Alta(string tipo)
        {
            tipo = await ValidarNombre(tipo, 0);
            return await _repo.Alta(new TipoProducto { Tipo = tipo });
        }

        public async Task<bool> Modificar(int id, string tipo)
        {
            var existente = await _repo.ObtenerPorId(id);
            if (existente == null) return false;

            existente.Tipo = await ValidarNombre(tipo, id);
            await _repo.Actualizar(existente);
            return true;
        }

        public async Task<bool> Eliminar(int id)
        {
            var existente = await _repo.ObtenerPorId(id);
            if (existente == null) return false;

            if (await _repo.TieneProductos(id))
                throw new AppException("No se puede eliminar: hay productos (activos o dados de baja) con este tipo.");

            await _repo.Eliminar(existente);
            return true;
        }

        private async Task<string> ValidarNombre(string? tipo, int idExcluido)
        {
            tipo = tipo?.Trim() ?? "";

            if (tipo.Length == 0)
                throw new AppException("El nombre del tipo es obligatorio.");
            if (tipo.Length > LimitesProducto.TipoMax)
                throw new AppException($"El nombre no puede superar los {LimitesProducto.TipoMax} caracteres.");
            if (await _repo.ExisteNombre(tipo, idExcluido))
                throw new AppException("Ya existe un tipo con ese nombre.");

            return tipo;
        }
    }
}
