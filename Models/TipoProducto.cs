using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using DulcesDuendesApp.Common.Validation;

namespace DulcesDuendesApp.Models
{
    [Index(nameof(Tipo), IsUnique = true)]
    public class TipoProducto
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = MensajesValidacion.Requerido)]
        [StringLength(LimitesProducto.TipoMax, ErrorMessage = MensajesValidacion.LongitudMaxima)]
        [Display(Name = "Tipo de Producto")]
        public string Tipo { get; set; } = string.Empty;
    }
}
