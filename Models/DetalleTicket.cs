using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DulcesDuendesApp.Common.Validation;
using Humanizer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace DulcesDuendesApp.Models
{
    public class DetalleTicket
    {
        [Key]
        public int Id { get; set; }
        
        public int TicketId { get; set; }
        [ForeignKey(nameof(TicketId))]
        [BindNever]
        public Ticket? Ticket { get; set; }
        public int ProductoId { get; set; }
        [ForeignKey(nameof(ProductoId))]
        [BindNever]
        public Producto? Producto { get ; set; }
        public int Cantidad { get; set; }
        [Precision(18,2)]
        public decimal PrecioUnitario { get; set; }
        [Precision(18,2)]
        public decimal Subtotal { get; set; }
    }
}