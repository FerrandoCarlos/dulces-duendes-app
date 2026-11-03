using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace DulcesDuendesApp.Models
{
    public class Pago
    {
        [Key]
        public int Id { get; set; }
        public int ClienteId { get; set; }
        [ForeignKey(nameof(ClienteId))]
        [BindNever]
        public Cliente? Cliente { get; set; }
        public int TicketId { get; set; }
        [ForeignKey(nameof(TicketId))]
        [BindNever]
        public Ticket? Ticket { get; set; }
        [Precision(18,2)]
        public decimal Monto { get; set; }
        public DateTime fechaVencimiento { get; set; }
        public DateTime fechaPago { get; set; }
        public string Estado { get; set; } = string.Empty;
    }
}