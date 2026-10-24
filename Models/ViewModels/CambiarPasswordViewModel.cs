using System.ComponentModel.DataAnnotations;
using DulcesDuendesApp.Common.Validation;

namespace DulcesDuendesApp.Models.ViewModels
{
    public class CambiarPasswordViewModel
    {
        [Display(Name = "contraseña actual")]
        [Required(ErrorMessage = MensajesValidacion.Requerido)]
        [DataType(DataType.Password)]
        public string PasswordActual { get; set; } = string.Empty;

        [Display(Name = "nueva contraseña")]
        [Required(ErrorMessage = MensajesValidacion.Requerido)]
        [MinLength(LimitesUsuario.PasswordMinima, ErrorMessage = MensajesValidacion.LongitudMinima)]
        [StringLength(LimitesUsuario.PasswordMaxima, ErrorMessage = MensajesValidacion.LongitudMaxima)]
        [DataType(DataType.Password)]
        public string PasswordNueva { get; set; } = string.Empty;

        [Display(Name = "confirmación")]
        [Required(ErrorMessage = MensajesValidacion.Requerido)]
        [Compare(nameof(PasswordNueva), ErrorMessage = MensajesValidacion.ContrasenasNoCoinciden)]
        [DataType(DataType.Password)]
        public string ConfirmarPassword { get; set; } = string.Empty;
    }
}
