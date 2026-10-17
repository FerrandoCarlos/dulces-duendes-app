using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace DulcesDuendesApp.Models
{
    [Index(nameof(Tipo), IsUnique = true)]
    public class TipoProducto
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "Nombre")]
        public string Tipo { get; set; } = string.Empty;
    }
}
