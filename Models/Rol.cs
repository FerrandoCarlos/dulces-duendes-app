using System.ComponentModel.DataAnnotations;

namespace DulcesDuendesApp.Models
{
    public class Rol
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;
    }
}
