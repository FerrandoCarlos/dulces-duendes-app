using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DulcesDuendesApp.Common.Validation;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace DulcesDuendesApp.Models
{
    [Index(nameof(CodigoBarra), IsUnique = true)]
    public class Producto
    {
        [Key]
        public int Id { get; set; }

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

        [ForeignKey(nameof(TipoProductoId))]
        [BindNever]
        public TipoProducto? TipoProducto { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = MensajesValidacion.NoNegativo)]
        public int Stock { get; set; }

        [Precision(18, 2)]
        [Range(0, double.MaxValue, ErrorMessage = MensajesValidacion.NoNegativo)]
        [Display(Name = "Precio de Costo")]
        public decimal PrecioCosto { get; set; }

        [Precision(5, 2)]
        [Range(0, 999.99, ErrorMessage = MensajesValidacion.FueraDeRango)]
        [Display(Name = "Porcentaje de Ganancia")]
        public decimal PorcentajeGanancia { get; set; }

        // Derivados: los escribe el servicio de precios
        [BindNever]
        [Precision(18, 2)]
        [Display(Name = "Precio de Venta")]
        public decimal PrecioVenta { get; set; }

        [BindNever]
        [Precision(18, 2)]
        [Display(Name = "Precio Cuenta Corriente")]
        public decimal PrecioCuentaCorriente { get; set; }

        [BindNever]
        public List<ImagenProducto> Imagenes { get; set; } = new();

        public bool Activo { get; set; } = true;
    }
}
