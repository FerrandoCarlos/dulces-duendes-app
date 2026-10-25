namespace DulcesDuendesApp.Models.Dtos
{
    public class ProductoResponse
    {
        public int Id { get; set; }
        public string CodigoBarra { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public int TipoProductoId { get; set; }
        public string TipoProductoNombre { get; set; } = string.Empty;
        public int Stock { get; set; }
        public decimal PrecioCosto { get; set; }
        public decimal PorcentajeGanancia { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal PrecioCuentaCorriente { get; set; }
    }
}
