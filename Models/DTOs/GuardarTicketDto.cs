namespace DulcesDuendesApp.Models.Dtos
{
    public class GuardarTicketDto
    {
        public int Id { get; set; }
        public TipoPago TipoPago { get; set; }
        public int? ClienteId { get; set; }
        public List<DetalleTicketDto> Detalles { get; set; } = new();
    }

    public class DetalleTicketDto
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}