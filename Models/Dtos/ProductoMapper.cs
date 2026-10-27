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
