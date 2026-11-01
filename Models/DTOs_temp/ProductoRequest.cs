using System.ComponentModel.DataAnnotations;
using DulcesDuendesApp.Common.Validation;


namespace DulcesDuendesApp.Models.Dtos
{
    public class ProductoRequest
    {
        [Required(ErrorMessage = MensajesValidacion.Requerido)]
        [StringLength(LimitesProducto.CodigoBarraMax, ErrorMessage = MensajesValidacion.LongitudMaxima)]
        [CodigoBarra]
        [Display(Name = "Código de Barras")]
        public string CodigoBarra { get; set; } = string.Empty;

        [Required(ErrorMessage = MensajesValidacion.Requerido)]
        [StringLength(LimitesProducto.DescripcionMax, ErrorMessage = MensajesValidacion.LongitudMaxima)]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = MensajesValidacion.Seleccionar)]
        [Display(Name = "Tipo de Producto")]
        public int TipoProductoId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = MensajesValidacion.NoNegativo)]
        public int Stock { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = MensajesValidacion.NoNegativo)]
        [Display(Name = "Precio de Costo")]
        public decimal PrecioCosto { get; set; }

        [Range(0, 999.99, ErrorMessage = MensajesValidacion.FueraDeRango)]
        [Display(Name = "Porcentaje de Ganancia")]
        public decimal PorcentajeGanancia { get; set; }
    }
}
