using DulcesDuendesApp.Common.Pagination;
namespace DulcesDuendesApp.Models.Dtos
{
    public static class ProductoMapper
    {
        public static ProductoResponse ToResponse(this Producto p)
        {
            return new ProductoResponse
            {
                Id = p.Id,
                CodigoBarra = p.CodigoBarra,
                Descripcion = p.Descripcion,
                TipoProductoId = p.TipoProductoId,
                TipoProductoNombre = p.TipoProducto?.Tipo ?? string.Empty,
                Stock = p.Stock,
                PrecioCosto = p.PrecioCosto,
                PorcentajeGanancia = p.PorcentajeGanancia,
                PrecioVenta = p.PrecioVenta,
                PrecioCuentaCorriente = p.PrecioCuentaCorriente,
                ImagenesUrl = p.Imagenes.Select(i => i.Url).ToList()
            };
        }

        public static PagedResult<ProductoResponse> ToResponse(this PagedResult<Producto> r)
        {
            return new PagedResult<ProductoResponse>
            {
                Items = r.Items.Select(p => p.ToResponse()).ToList(),
                TotalCount = r.TotalCount,
                Page = r.Page,
                PageSize = r.PageSize
            };
        }
        public static Producto ToEntity(this ProductoRequest r, int id = 0)
        {
            return new Producto
            {
                Id = id,
                CodigoBarra = r.CodigoBarra,
                Descripcion = r.Descripcion,
                TipoProductoId = r.TipoProductoId,
                Stock = r.Stock,
                PrecioCosto = r.PrecioCosto,
                PorcentajeGanancia = r.PorcentajeGanancia
            };
        }
    }
}
