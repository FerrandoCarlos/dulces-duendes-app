using System.ComponentModel.DataAnnotations;
using DulcesDuendesApp.Common.Validation;

namespace DulcesDuendesApp.Models.ViewModels
{
    public class PerfilViewModel
    {
        [Display(Name = "nombre")]
        [Required(ErrorMessage = MensajesValidacion.Requerido)]
        [StringLength(100, ErrorMessage = MensajesValidacion.LongitudMaxima)]
        [SoloLetras]
        public string Nombre { get; set; } = string.Empty;
        [Display(Name = "apellido")]
        [Required(ErrorMessage = MensajesValidacion.Requerido)]
        [StringLength(100, ErrorMessage = MensajesValidacion.LongitudMaxima)]
        [SoloLetras]
        public string Apellido { get; set; } = string.Empty;

        [Display(Name = "email")]
        [Required(ErrorMessage = MensajesValidacion.Requerido)]
        [StringLength(256, ErrorMessage = MensajesValidacion.LongitudMaxima)]
        [EmailAddress(ErrorMessage = MensajesValidacion.FormatoInvalido)]
        public string Email { get; set; } = string.Empty;
    }
}
