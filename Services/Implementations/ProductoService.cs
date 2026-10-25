using DulcesDuendesApp.Common.Exceptions;
using DulcesDuendesApp.Common.Filters;
using DulcesDuendesApp.Common.Pagination;
using DulcesDuendesApp.Models;
using DulcesDuendesApp.Repositories.Interfaces;
using DulcesDuendesApp.Services.Interfaces;
namespace DulcesDuendesApp.Services.Implementations
{
    public class ProductoService : IProductoService
    {
        private const string MsgCodigoDuplicado = "Ya existe un producto con ese código de barras.";
        private const string MsgTipoInexistente = "El tipo de producto seleccionado no existe.";

        private readonly IProductoRepository _repo;
        private readonly IPrecioCalculador _precios;
        private readonly ITipoProductoRepository _tipos;

        public ProductoService(IProductoRepository repo, IPrecioCalculador precios, ITipoProductoRepository tipos)
        {
            _repo = repo;
            _precios = precios;
            _tipos = tipos;
        }

        public Task<PagedResult<Producto>> ObtenerPaginado(ProductoFiltro filtro, PageRequest page) => _repo.ObtenerPaginado(filtro, page);
        public Task<Producto?> ObtenerPorId(int id) => _repo.ObtenerPorId(id);
        public async Task<Producto?> ObtenerPorCodigoBarra(string codigoBarra)
        {
            var producto = await _repo.ObtenerPorCodigoBarra(codigoBarra);
            return producto is { Activo: true } ? producto : null;
        }
        public async Task<int> Alta(Producto producto)
        {
            if (await _repo.ObtenerPorCodigoBarra(producto.CodigoBarra) != null) throw new AppException(MsgCodigoDuplicado);

            if (!await _tipos.Existe(producto.TipoProductoId)) throw new AppException(MsgTipoInexistente);

            CalcularPrecios(producto);
            producto.Activo = true;

            return await _repo.Alta(producto);
        }
        public async Task<bool> Modificar(Producto datos)
        {
            var producto = await _repo.ObtenerPorId(datos.Id);
            if (producto == null) return false;

            var conMismoCodigo = await _repo.ObtenerPorCodigoBarra(datos.CodigoBarra);
            if (conMismoCodigo != null && conMismoCodigo.Id != producto.Id)
                throw new AppException(MsgCodigoDuplicado);

            if (!await _tipos.Existe(datos.TipoProductoId)) throw new AppException(MsgTipoInexistente);

            producto.CodigoBarra = datos.CodigoBarra;
            producto.Descripcion = datos.Descripcion;
            producto.TipoProductoId = datos.TipoProductoId;
            producto.Stock = datos.Stock;
            producto.PrecioCosto = datos.PrecioCosto;
            producto.PorcentajeGanancia = datos.PorcentajeGanancia;

            CalcularPrecios(producto);
            await _repo.Modificar(producto);
            return true;

        }

        public async Task<bool> Baja(int id)
        {
            var producto = await _repo.ObtenerPorId(id);
            if (producto == null) return false;

            producto.Activo = false;
            await _repo.Modificar(producto);
            return true;
        }

        private void CalcularPrecios(Producto p)
        {
            p.PrecioVenta = _precios.PrecioVenta(p.PrecioCosto, p.PorcentajeGanancia);
            p.PrecioCuentaCorriente = _precios.PrecioCuentaCorriente(p.PrecioCosto, p.PorcentajeGanancia);
        }
    }
}
