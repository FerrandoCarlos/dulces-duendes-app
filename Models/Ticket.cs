using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using DulcesDuendesApp.Common.Validation;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace DulcesDuendesApp.Models
{
    public enum TipoPago
    {
        Efectivo = 1,
        CuentaCorriente = 2
    }
    public class Ticket
    {
        [Key]
        public int Id { get; set; }
        
        public int UsuarioId { get; set; }
        [ForeignKey(nameof(UsuarioId))]
        [BindNever]
        public Usuario? Usuario { get; set; }
        public int? ClienteId { get; set; }
        [ForeignKey(nameof(ClienteId))]
        [BindNever]
        public Cliente? Cliente { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public TipoPago TipoPago { get; set; }
        [Precision(18,2)]
        [Range(0, double.MaxValue, ErrorMessage = MensajesValidacion.NoNegativo)]
        public decimal Total { get; set; }

        public bool Activo { get; set; } = true;
        public ICollection<DetalleTicket> Detalles { get; set; } = new List<DetalleTicket>();
    }    
}