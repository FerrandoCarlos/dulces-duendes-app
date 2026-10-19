namespace DulcesDuendesApp.Common.Filters
{
    public class ProductoFiltro
    {
        public string? Texto { get; set; }
        public int? TipoProductoId { get; set; }
        public EstadoStock Estado { get; set; } = EstadoStock.Todos;

    }
}
