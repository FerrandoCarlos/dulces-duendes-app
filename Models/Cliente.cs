using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using DulcesDuendesApp.Common.Validation;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace DulcesDuendesApp.Models
{
    [Index(nameof(Dni), IsUnique = true)]
    public class Cliente
    {
        [Key]
        public int Id { get; set; }


        [Required(ErrorMessage = MensajesValidacion.Requerido)]
        [StringLength(8, ErrorMessage = MensajesValidacion.LongitudMaxima)]
        public string Dni { get; set; } = string.Empty;

        [Required(ErrorMessage = MensajesValidacion.Requerido)]
        [StringLength(30, ErrorMessage = MensajesValidacion.LongitudMaxima)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = MensajesValidacion.Requerido)]
        [StringLength(30, ErrorMessage = MensajesValidacion.LongitudMaxima)]
        public string Apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = MensajesValidacion.Requerido)]
        [StringLength(30, ErrorMessage = MensajesValidacion.LongitudMaxima)]
        public string Telefono { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = MensajesValidacion.LongitudMaxima)]
        public string Direccion { get; set; } = string.Empty;

        [Precision(18, 2)]
        public decimal Monto { get; set; } = 0;

        public bool Activo { get; set; } = true;
    }
}