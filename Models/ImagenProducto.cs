using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DulcesDuendesApp.Models
{
    public class ImagenProducto
    {
        [Key]
        public int Id { get; set; }

        public int ProductoId { get; set; }

        [ForeignKey(nameof(ProductoId))]
        [BindNever]
        public Producto? Producto { get; set; }

        [Required]
        [MaxLength(300)]
        public string Url { get; set; } = string.Empty;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }
}
