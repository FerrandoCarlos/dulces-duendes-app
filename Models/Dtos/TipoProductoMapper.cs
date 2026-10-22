namespace DulcesDuendesApp.Models.Dtos
{
    public static class TipoProductoMapper
    {
        public static TipoProductoResponse ToResponse(this TipoProducto t)
        {
            return new TipoProductoResponse { Id = t.Id, Tipo = t.Tipo };
        }

    }
}
