using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DulcesDuendesApp.Common.Validation;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DulcesDuendesApp.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = MensajesValidacion.Requerido)]
        [EmailAddress(ErrorMessage = MensajesValidacion.FormatoInvalido)]
        [StringLength(256, ErrorMessage = MensajesValidacion.LongitudMaxima)]
        public string Email { get; set; } = string.Empty;

        [BindNever]
        [MaxLength(300)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required(ErrorMessage = MensajesValidacion.Requerido)]
        [StringLength(100, ErrorMessage = MensajesValidacion.LongitudMaxima)]
        [SoloLetras]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = MensajesValidacion.Requerido)]
        [StringLength(100, ErrorMessage = MensajesValidacion.LongitudMaxima)]
        [SoloLetras]
        public string Apellido { get; set; } = string.Empty;

        public string? AvatarUrl { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = MensajesValidacion.Seleccionar)]
        [Display(Name = "Rol")]
        public int RolId { get; set; }

        [ForeignKey(nameof(RolId))]
        [BindNever]
        public Rol? Rol { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [NotMapped]
        public string? Password { get; set; }
    }
}
