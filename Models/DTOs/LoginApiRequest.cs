using System.ComponentModel.DataAnnotations;
using DulcesDuendesApp.Common.Validation;

namespace DulcesDuendesApp.Models.Dtos
{
    public class LoginApiRequest
    {
        [Display(Name = "Email")]
        [Required(ErrorMessage = MensajesValidacion.Requerido)]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Contraseña")]
        [Required(ErrorMessage = MensajesValidacion.Requerido)]
        public string Password { get; set; } = string.Empty;
    }
}
