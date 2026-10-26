using System.ComponentModel.DataAnnotations;
using DulcesDuendesApp.Common.Validation;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DulcesDuendesApp.Models.ViewModels
{
    public class TipoProductoFormViewModel
    {
        [BindNever]
        public int Id { get; set; }

        [Display(Name = "Nombre")]
        [Required(ErrorMessage = MensajesValidacion.Requerido)]
        [StringLength(LimitesProducto.TipoMax, ErrorMessage = MensajesValidacion.LongitudMaxima)]
        public string Tipo { get; set; } = string.Empty;
    }
}
