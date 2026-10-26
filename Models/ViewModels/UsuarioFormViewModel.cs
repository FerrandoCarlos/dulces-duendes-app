using System.ComponentModel.DataAnnotations;
using DulcesDuendesApp.Common.Validation;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DulcesDuendesApp.Models.ViewModels
{
    public class UsuarioFormViewModel
    {
        [BindNever]
        public int Id { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = MensajesValidacion.Requerido)]
        [StringLength(100, ErrorMessage = MensajesValidacion.LongitudMaxima)]
        [SoloLetras]
        public string Nombre { get; set; } = string.Empty;
        [Display(Name = "Apellido")]
        [Required(ErrorMessage = MensajesValidacion.Requerido)]
        [StringLength(100, ErrorMessage = MensajesValidacion.LongitudMaxima)]
        [SoloLetras]
        public string Apellido { get; set; } = string.Empty;
        [Display(Name = "Email")]
        [Required(ErrorMessage = MensajesValidacion.Requerido)]
        [StringLength(256, ErrorMessage = MensajesValidacion.LongitudMaxima)]
        [EmailAddress(ErrorMessage = MensajesValidacion.FormatoInvalido)]
        public string Email { get; set; } = string.Empty;
        [Display(Name = "Rol")]
        [Range(1, int.MaxValue, ErrorMessage = MensajesValidacion.Seleccionar)]
        public int RolId { get; set; }
        [Display(Name = "Contraseña inicial")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        [BindNever]
        public IReadOnlyList<SelectListItem> Roles { get; set; } = [];

    }
}
